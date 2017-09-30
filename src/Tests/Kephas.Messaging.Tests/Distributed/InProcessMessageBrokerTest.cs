// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InProcessMessageBrokerTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the in process message broker test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Distributed
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Dynamic;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Messages;
    using Kephas.Messaging.Ping;
    using Kephas.Testing.Composition.Mef;

    using NUnit.Framework;

    [TestFixture]
    public class InProcessMessageBrokerTest : CompositionTestBase
    {
        public override ICompositionContext CreateContainer(IEnumerable<Assembly> assemblies = null, IEnumerable<Type> parts = null, Action<MefCompositionContainerBuilder> config = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? new Assembly[0]);
            assemblyList.Add(typeof(IMessageProcessor).GetTypeInfo().Assembly); /* Kephas.Messaging */
            return base.CreateContainer(assemblyList, parts, config);
        }

        [Test]
        public void InProcessMessageBroker_Composition_success()
        {
            var container = this.CreateContainer();
            var messageBroker = container.GetExport<IMessageBroker>();
            Assert.IsInstanceOf<InProcessMessageBroker>(messageBroker);
        }

        [Test]
        public async Task DispatchAsync_Ping_timeout()
        {
            var container = this.CreateContainer(parts: new[] { typeof(TimeoutMessageHandler) });
            var messageBroker = container.GetExport<IMessageBroker>();

            var brokeredMessage = new BrokeredMessage
                                      {
                                          Content = new TimeoutMessage(),
                                          Timeout = TimeSpan.FromSeconds(0)
                                      };
            Assert.That(() => messageBroker.DispatchAsync(brokeredMessage), Throws.InstanceOf<TimeoutException>());
        }

        [Test]
        public async Task DispatchAsync_Ping_success()
        {
            var container = this.CreateContainer();
            var messageBroker = container.GetExport<IMessageBroker>();

            var pingBack = await messageBroker.DispatchAsync(new BrokeredMessage
                                                                 {
                                                                     Content = new PingMessage(),
                                                                     Timeout = TimeSpan.FromSeconds(10)
                                                                 });

            Assert.IsInstanceOf<PingBackMessage>(pingBack);
        }

        [Test]
        public async Task ProcessAsync_Ping_success()
        {
            var container = this.CreateContainer();
            var messageBroker = container.GetExport<IMessageBroker>();

            var pingBack = await messageBroker.ProcessAsync(new PingMessage());

            Assert.IsInstanceOf<PingBackMessage>(pingBack);
        }

        [Test]
        public async Task ProcessAsync_null_response()
        {
            var container = this.CreateContainer();
            var messageBroker = container.GetExport<IMessageBroker>();

            var nullResponse = await messageBroker.ProcessAsync(new TestEvent());

            Assert.IsNull(nullResponse);
        }

        [Test]
        public async Task PublishAsync_event()
        {
            var container = this.CreateContainer(parts: new[] { typeof(TestEventHandler) });
            var messageBroker = container.GetExport<IMessageBroker>();

            var message = new TestEvent { TaskCompletionSource = new TaskCompletionSource<string>() };
            await messageBroker.PublishAsync(message);

            var response = await message.TaskCompletionSource.Task;
            Assert.AreEqual("ok", response);
        }

        [Test]
        public async Task PublishAsync_event_one_way()
        {
            var container = this.CreateContainer(parts: new[] { typeof(TestEventHandler) });
            var messageBroker = container.GetExport<IMessageBroker>();

            var message = new TestEvent { TaskCompletionSource = new TaskCompletionSource<string>() };
            await messageBroker.PublishAsync(message);

            var response = await message.TaskCompletionSource.Task;
            Assert.AreEqual("ok", response);
        }

        public class TestEvent : Expando, IEvent
        {
            public TaskCompletionSource<string> TaskCompletionSource { get; set; }

            public IMessage Response { get; set; }
        }

        public class TestEventHandler : MessageHandlerBase<TestEvent, IMessage>
        {
            /// <summary>
            /// Processes the provided message asynchronously and returns a response promise.
            /// </summary>
            /// <param name="message">The message to be handled.</param>
            /// <param name="context">The processing context.</param>
            /// <param name="token">The cancellation token.</param>
            /// <returns>
            /// The response promise.
            /// </returns>
            public override async Task<IMessage> ProcessAsync(TestEvent message, IMessageProcessingContext context, CancellationToken token)
            {
                message.TaskCompletionSource?.SetResult("ok");

                return message.Response;
            }
        }

        public class TimeoutMessage : IMessage { }

        public class TimeoutMessageHandler : MessageHandlerBase<TimeoutMessage, IMessage>
        {
            /// <summary>
            /// Processes the provided message asynchronously and returns a response promise.
            /// </summary>
            /// <param name="message">The message to be handled.</param>
            /// <param name="context">The processing context.</param>
            /// <param name="token">The cancellation token.</param>
            /// <returns>
            /// The response promise.
            /// </returns>
            public override async Task<IMessage> ProcessAsync(TimeoutMessage message, IMessageProcessingContext context, CancellationToken token)
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));

                return new EmptyMessage();
            }
        }
    }
}