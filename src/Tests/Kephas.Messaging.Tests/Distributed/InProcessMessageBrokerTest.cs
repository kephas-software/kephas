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
        public async Task DispatchAsync_Ping_success()
        {
            var container = this.CreateContainer();
            var messageBroker = container.GetExport<IMessageBroker>();

            var pingBack = await messageBroker.DispatchAsync(new BrokeredMessage { Content = new PingMessage() });

            Assert.IsInstanceOf<PingBackMessage>(pingBack);
        }

        [Test]
        public async Task ProcessAsync_Ping_success()
        {
            var container = this.CreateContainer();
            var messageBroker = container.GetExport<IMessageBroker>();

            var pingBack = await messageBroker.ProcessAsync(new BrokeredMessage { Content = new PingMessage() });

            Assert.IsInstanceOf<PingBackMessage>(pingBack);
        }

        [Test]
        public async Task PublishAsync_event()
        {
            var container = this.CreateContainer(parts: new[] { typeof(TestEventHandler) });
            var messageBroker = container.GetExport<IMessageBroker>();

            var message = new TestEvent();
            var eventBack = await messageBroker.ProcessAsync(new BrokeredMessage { Content = message });

            Assert.IsNull(eventBack);
            Assert.AreEqual("ok", message["received"]);
        }

        [Test]
        public async Task PublishAsync_event_one_way()
        {
            var container = this.CreateContainer(parts: new[] { typeof(TestEventHandler) });
            var messageBroker = container.GetExport<IMessageBroker>();

            var message = new TestEvent();
            var eventBack = await messageBroker.ProcessAsync(new BrokeredMessage { Content = message, IsOneWay = true });

            Assert.IsInstanceOf<EmptyMessage>(eventBack);
            Assert.IsNull(message["received"]);
        }

        public class TestEvent : Expando, IEvent { }

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
                await Task.Factory.StartNew(
                    () =>
                        {
                            Thread.Sleep(100);
                            message["received"] = "ok";
                        });

                return null;
            }
        }
    }
}