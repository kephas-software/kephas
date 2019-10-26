﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InProcessMessageBrokerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in process message broker test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Autofac.Distributed
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Messaging.Behaviors;
    using Kephas.Messaging.Behaviors.Composition;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Distributed.Routing.Composition;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.Messages;
    using Kephas.Serialization;
    using Kephas.Serialization.Json;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class AutofacDefaultMessageBrokerTest : AutofacMessagingTestBase
    {
        public async Task<IMessageBroker> GetMessageBrokerAsync(ICompositionContext compositionContext)
        {
            var messageBroker = compositionContext.GetExport<IMessageBroker>();
            await ServiceHelper.InitializeAsync(messageBroker, new Context(compositionContext));

            return messageBroker;
        }

        [Test]
        public void DefaultMessageBroker_Composition_success()
        {
            var container = CreateContainer();
            var messageBroker = container.GetExport<IMessageBroker>();
            Assert.IsInstanceOf<DefaultMessageBroker>(messageBroker);
        }

        [Test]
        public async Task InitializeAsync_ignore_router_error()
        {
            var container = CreateContainer(parts: new[] { typeof(OptionalMessageRouter) });
            var messageBroker = container.GetExport<IMessageBroker>();
            await (messageBroker as IAsyncInitializable).InitializeAsync(new Context(container));
        }

        [Test]
        public void InitializeAsync_throw_router_error()
        {
            var container = CreateContainer(parts: new[] { typeof(RequiredMessageRouter) });
            var messageBroker = container.GetExport<IMessageBroker>();
            Assert.ThrowsAsync<NotImplementedException>(() => (messageBroker as IAsyncInitializable).InitializeAsync(new Context(container)));
        }

        [Test]
        public async Task DispatchAsync_timeout()
        {
            var container = CreateContainer();
            var messageBroker = await GetMessageBrokerAsync(container);
            var handlerRegistry = container.GetExport<IMessageHandlerRegistry>();
            handlerRegistry.RegisterHandler<TimeoutMessage>((msg, ctx) =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                return Substitute.For<IMessage>();
            });

            var brokeredMessage = new BrokeredMessage
            {
                Content = new TimeoutMessage(),
                Timeout = TimeSpan.FromMilliseconds(30),
            };
            Assert.That(() => messageBroker.DispatchAsync(brokeredMessage, Substitute.For<IContext>(), default), Throws.InstanceOf<TimeoutException>());
        }

        [Test]
        public async Task DispatchAsync_timeout_logging()
        {
            var sb = new StringBuilder();
            var logger = GetLogger<IMessageBroker>(sb);

            var container = CreateContainer(parts: new[] { typeof(LoggableMessageBroker) });
            var messageBroker = await GetMessageBrokerAsync(container);
            ((LoggableMessageBroker)messageBroker).SetLogger(logger);

            var handlerRegistry = container.GetExport<IMessageHandlerRegistry>();
            handlerRegistry.RegisterHandler<TimeoutMessage>((msg, ctx) =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                return Substitute.For<IMessage>();
            });

            var brokeredMessage = new BrokeredMessage
            {
                Content = new TimeoutMessage(),
                Timeout = TimeSpan.FromMilliseconds(30),
            };

            try
            {
                await messageBroker.DispatchAsync(brokeredMessage, new Context(container)).PreserveThreadContext();
                throw new InvalidOperationException("Should have timed out.");
            }
            catch (TimeoutException tex)
            {
            }

            var log = sb.ToString();
            Assert.IsTrue(log.Contains("Warning"));
            Assert.IsTrue(log.Contains("Timeout"));
        }

        [Test]
        public async Task DispatchAsync_Ping_success_with_timeout()
        {
            var container = CreateContainer();
            var messageBroker = await GetMessageBrokerAsync(container);

            var pingBack = await messageBroker.DispatchAsync(new BrokeredMessage
            {
                Content = new PingMessage(),
                Timeout = TimeSpan.FromSeconds(100)
            }, new Context(container));

            Assert.IsInstanceOf<PingBackMessage>(pingBack);
        }

        [Test]
        public async Task DispatchAsync_dispose_created_context()
        {
            var container = CreateContainer(parts: new[] { typeof(TestMessageProcessor) });
            var messageBroker = await GetMessageBrokerAsync(container);
            var messageProcessor = (TestMessageProcessor)container.GetExport<IMessageProcessor>();
            var disposable = Substitute.For<IDisposable>();
            var added = false;
            messageProcessor.ProcessingContextConfigurator = (msg, ctx) =>
                {
                    if (!added)
                    {
                        ctx.AddResource(disposable);
                    }

                    added = true;
                };

            var pingBack = await messageBroker.DispatchAsync(
                new BrokeredMessage
                {
                    Content = new PingMessage(),
                    Timeout = TimeSpan.FromSeconds(100),
                },
                new Context(container));

            disposable.Received(1).Dispose();
        }

        [Test]
        public async Task ProcessAsync_Ping_over_serialization_success()
        {
            var container = CreateContainer(assemblies: new[] { typeof(IJsonSerializerSettingsProvider).Assembly }, parts: new[] { typeof(RemoteMessageBroker) });
            var messageBroker = await GetMessageBrokerAsync(container);

            var pingBack = await messageBroker.ProcessAsync(new PingMessage(), new Context(container));

            Assert.IsInstanceOf<PingBackMessage>(pingBack);
        }

        [Test]
        public async Task ProcessAsync_Ping_success()
        {
            var container = CreateContainer();
            var messageBroker = await GetMessageBrokerAsync(container);

            var pingBack = await messageBroker.ProcessAsync(new PingMessage(), new Context(container));

            Assert.IsInstanceOf<PingBackMessage>(pingBack);
        }

        [Test]
        public async Task ProcessAsync_Ping_exception()
        {
            var container = CreateContainer(parts: new[] { typeof(ExceptionEventHandler) });
            var messageBroker = await GetMessageBrokerAsync(container);

            Assert.That(() => messageBroker.ProcessAsync(new PingMessage(), new Context(container)), Throws.InstanceOf<MessagingException>());
        }

        [Test]
        public async Task ProcessAsync_null_response_event_no_handlers()
        {
            var container = CreateContainer();
            var messageBroker = await GetMessageBrokerAsync(container);

            var nullResponse = await messageBroker.ProcessAsync(new TestEvent(), new Context(container));

            Assert.IsNull(nullResponse);
        }

        [Test]
        public async Task PublishAsync_event()
        {
            var container = CreateContainer(parts: new[] { typeof(TestEventHandler) });
            var messageBroker = await GetMessageBrokerAsync(container);

            var message = new TestEvent { TaskCompletionSource = new TaskCompletionSource<(string, IBrokeredMessage)>() };
            await messageBroker.PublishAsync(message, new Context(container));

            var response = await message.TaskCompletionSource.Task;
            Assert.AreEqual("ok", response.response);
        }

        [Test]
        public async Task ProcessOneWayAsync_event()
        {
            var container = CreateContainer(parts: new[] { typeof(TestEventHandler) });
            var messageBroker = await GetMessageBrokerAsync(container);

            var message = new TestEvent { TaskCompletionSource = new TaskCompletionSource<(string, IBrokeredMessage)>() };
            await messageBroker.ProcessOneWayAsync(message, new Context(container));

            var response = await message.TaskCompletionSource.Task;
            Assert.AreEqual("ok", response.response);
            Assert.AreSame(message, response.brokeredMessage.Content);
        }

        private ILogger<T> GetLogger<T>(StringBuilder sb)
        {
            var logger = Substitute.For<ILogger<T>>();
            logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
            logger.WhenForAnyArgs(l => l.Log(LogLevel.Debug, null, null, new object[0])).Do(
                ci => { sb.Append($"{ci.Arg<LogLevel>()} {ci.Arg<string>()} {ci.Arg<Exception>()?.GetType().Name}"); });
            return logger;
        }

        public class TestEvent : Expando, IEvent
        {
            public TaskCompletionSource<(string response, IBrokeredMessage brokeredMessage)> TaskCompletionSource { get; set; }

            public IMessage Response { get; set; }
        }

        public class TestEventHandler : MessageHandlerBase<TestEvent, IMessage>
        {
            public override async Task<IMessage> ProcessAsync(TestEvent message, IMessagingContext context, CancellationToken token)
            {
                message.TaskCompletionSource?.SetResult(("ok", context.GetBrokeredMessage()));

                return message.Response;
            }
        }

        [OverridePriority(Priority.High)]
        public class ExceptionEventHandler : MessageHandlerBase<PingMessage, PingBackMessage>
        {
            public override async Task<PingBackMessage> ProcessAsync(PingMessage message, IMessagingContext context, CancellationToken token)
            {
                throw new ArgumentException();
            }
        }

        public class TimeoutMessage : IMessage { }

        [OverridePriority(Priority.Normal)]
        public class RemoteMessageBroker : DefaultMessageBroker
        {
            private readonly ISerializationService serializationService;

            public RemoteMessageBroker(
                ICollection<Lazy<IMessageRouter, MessageRouterMetadata>> routerFactories,
                IExportFactory<IBrokeredMessageBuilder> builderFactory,
                ISerializationService serializationService)
                : base(routerFactories, builderFactory)
            {
                this.serializationService = serializationService;
            }

            protected override async Task RouterDispatchAsync(
                IBrokeredMessage brokeredMessage,
                IContext context,
                CancellationToken cancellationToken)
            {
                var serializedMessage = await serializationService.JsonSerializeAsync(
                                            brokeredMessage,
                                            cancellationToken: cancellationToken);
                var deserializedMessage = await serializationService.JsonDeserializeAsync(
                                              serializedMessage,
                                              cancellationToken: cancellationToken);
                await base.RouterDispatchAsync((IBrokeredMessage)deserializedMessage, context, cancellationToken);
            }
        }

        [OverridePriority(Priority.High)]
        public class LoggableMessageBroker : DefaultMessageBroker
        {
            public LoggableMessageBroker(
                ICollection<Lazy<IMessageRouter, MessageRouterMetadata>> routerFactories,
                IExportFactory<IBrokeredMessageBuilder> builderFactory)
                : base(routerFactories, builderFactory)
            {
            }

            public void SetLogger(ILogger logger)
            {
                Logger = logger;
            }
        }

        [OverridePriority(Priority.High)]
        public class TestMessageProcessor : DefaultMessageProcessor
        {
            public TestMessageProcessor(IContextFactory contextFactory, IMessageHandlerRegistry handlerRegistry, IMessageMatchService messageMatchService, IList<IExportFactory<IMessagingBehavior, MessagingBehaviorMetadata>> behaviorFactories)
                : base(contextFactory, handlerRegistry, messageMatchService, behaviorFactories)
            {
            }

            public Action<IMessage, IMessagingContext> ProcessingContextConfigurator { get; set; }

            protected override Task ApplyBeforeProcessBehaviorsAsync(
                IEnumerable<IMessagingBehavior> behaviors,
                IMessagingContext context,
                CancellationToken token)
            {
                ProcessingContextConfigurator?.Invoke(context.Message, context);
                return base.ApplyBeforeProcessBehaviorsAsync(behaviors, context, token);
            }
        }

        [MessageRouter(IsOptional = true)]
        public class OptionalMessageRouter : IMessageRouter, IAsyncInitializable
        {
            public event EventHandler<ReplyReceivedEventArgs> ReplyReceived;

            public Task InitializeAsync(IContext context = null, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task<(RoutingInstruction action, IMessage reply)> DispatchAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        [MessageRouter(IsOptional = false)]
        public class RequiredMessageRouter : IMessageRouter, IAsyncInitializable
        {
            public event EventHandler<ReplyReceivedEventArgs> ReplyReceived;

            public Task InitializeAsync(IContext context = null, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task<(RoutingInstruction action, IMessage reply)> DispatchAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}