// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageBrokerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in process message broker test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Distributed
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Behaviors;
    using Kephas.Diagnostics.Logging;
    using Kephas.Dynamic;
    using Kephas.Injection;
    using Kephas.Logging;
    using Kephas.Messaging.Behaviors;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Queues;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.Messages;
    using Kephas.Model.AttributedModel;
    using Kephas.Serialization;
    using Kephas.Serialization.Json;
    using Kephas.Services;
    using Kephas.Services.Behaviors;
    using Kephas.Threading.Tasks;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultMessageBrokerTest : MessagingTestBase
    {
        public async Task<IMessageBroker> GetMessageBrokerAsync(IInjector injector)
        {
            var messageBroker = injector.Resolve<IMessageBroker>();
            await ServiceHelper.InitializeAsync(messageBroker, new Context(injector));

            return messageBroker;
        }

        [Test]
        public void DefaultMessageBroker_Injection_success()
        {
            var container = this.CreateInjector();
            var messageBroker = container.Resolve<IMessageBroker>();
            Assert.IsInstanceOf<DefaultMessageBroker>(messageBroker);
        }

        [Test]
        public async Task InitializeAsync_ignore_router_error()
        {
            var container = this.CreateInjector(parts: new[] { typeof(OptionalMessageRouter) });
            var messageBroker = container.Resolve<IMessageBroker>();
            await (messageBroker as IAsyncInitializable).InitializeAsync(new Context(container));
        }

        [Test]
        public async Task InitializeAsync_override_router()
        {
            var container = this.CreateInjector(parts: new[] { typeof(OverrideMessageRouter) });
            var messageBroker = (DefaultMessageBroker)container.Resolve<IMessageBroker>();
            
            // TODO ensure that the InProcessMessageRouter is overwritten.
            await (messageBroker as IAsyncInitializable).InitializeAsync(new Context(container));
        }

        [Test]
        public void InitializeAsync_throw_router_error()
        {
            var container = this.CreateInjector(parts: new[] { typeof(RequiredMessageRouter) });
            var messageBroker = container.Resolve<IMessageBroker>();
            Assert.ThrowsAsync<NotImplementedException>(() => (messageBroker as IAsyncInitializable).InitializeAsync(new Context(container)));
        }

        [Test]
        public async Task InitializeAsync_with_match_provider()
        {
            var container = this.CreateInjector(parts: new[] { typeof(WithProviderMessageRouter) });
            var messageBroker = container.Resolve<IMessageBroker>();
            await (messageBroker as IAsyncInitializable).InitializeAsync(new Context(container));
        }

        [Test]
        public void InitializeAsync_with_bad_match_provider()
        {
            var container = this.CreateInjector(parts: new[] { typeof(WithBadProviderMessageRouter) });
            var messageBroker = container.Resolve<IMessageBroker>();
            Assert.ThrowsAsync<InvalidOperationException>(() => (messageBroker as IAsyncInitializable).InitializeAsync(new Context(container)));
        }

        [Test]
        public async Task DispatchAsync_timeout()
        {
            var container = this.CreateInjector();
            var appRuntime = container.Resolve<IAppRuntime>();
            var handlerRegistry = container.Resolve<IMessageHandlerRegistry>();
            handlerRegistry.RegisterHandler<TimeoutMessage>(async (msg, ctx, token) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(10), token);
                return Substitute.For<IMessage>();
            });

            var messageBroker = await this.GetMessageBrokerAsync(container);

            Assert.That(
                () => messageBroker.DispatchAsync(
                    new TimeoutMessage(),
                    ctx => ctx.To(Endpoint.CreateAppInstanceEndpoint(appRuntime))
                    .Timeout(TimeSpan.FromMilliseconds(30))),
                Throws.InstanceOf<TimeoutException>());
        }

        [Test]
        public async Task DispatchAsync_timeout_logging()
        {
            var sb = new StringBuilder();
            var logger = this.GetLogger<IMessageBroker>(sb);

            var container = this.CreateInjector(parts: new[] { typeof(LoggableMessageBroker) });
            var appRuntime = container.Resolve<IAppRuntime>();
            var messageBroker = await this.GetMessageBrokerAsync(container);
            ((LoggableMessageBroker)messageBroker).SetLogger(logger);

            var handlerRegistry = container.Resolve<IMessageHandlerRegistry>();
            handlerRegistry.RegisterHandler<TimeoutMessage>(async (msg, ctx, token) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(10), token);
                return Substitute.For<IMessage>();
            });

            var brokeredMessage = new BrokeredMessage(new TimeoutMessage())
            {
                Timeout = TimeSpan.FromMilliseconds(30),
                Recipients = new[] { Endpoint.CreateAppInstanceEndpoint(appRuntime) },
            };

            try
            {
                await messageBroker.DispatchAsync(brokeredMessage, null).PreserveThreadContext();
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
        public async Task DispatchAsync_Ping_argument_missing_recipients()
        {
            var container = this.CreateInjector();
            var messageBroker = await this.GetMessageBrokerAsync(container);

            Assert.ThrowsAsync<ArgumentException>(() => messageBroker.DispatchAsync(new PingMessage()));
        }

        [Test]
        public async Task DispatchAsync_Ping_success_with_timeout()
        {
            var container = this.CreateInjector();
            var appRuntime = container.Resolve<IAppRuntime>();
            var messageBroker = await this.GetMessageBrokerAsync(container);

            var pingBack = await messageBroker.DispatchAsync(
                new BrokeredMessage
                {
                    Content = new PingMessage(),
                    Timeout = TimeSpan.FromSeconds(100),
                    Recipients = new[] { Endpoint.CreateAppInstanceEndpoint(appRuntime) },
                });

            Assert.IsInstanceOf<PingBackMessage>(pingBack);
        }

        [Test]
        public async Task DispatchAsync_Ping_with_enabled_switch()
        {
            var container = this.CreateInjector(parts: new[]
            {
                typeof(CanDisableMessageRouterEnabledRule),
                typeof(CanDisableMessageRouter),
            });
            var appRuntime = container.Resolve<IAppRuntime>();
            var messageBroker = await this.GetMessageBrokerAsync(container);

            var pingBack1 = (PingBackMessage?)await messageBroker.DispatchAsync(
                new BrokeredMessage
                {
                    Content = new PingMessage(),
                    Recipients = new[] { Endpoint.CreateAppInstanceEndpoint(appRuntime) },
                });

            var pingBack2 = (PingBackMessage?)await messageBroker.DispatchAsync(
                new BrokeredMessage
                {
                    Content = new PingMessage(),
                    Recipients = new[] { Endpoint.CreateAppInstanceEndpoint(appRuntime) },
                });

            var longMessage = pingBack1?.Message.Length > pingBack2.Message.Length
                                        ? pingBack1?.Message
                                        : pingBack2?.Message;
            var shortMessage = pingBack1?.Message.Length < pingBack2.Message.Length
                ? pingBack1?.Message
                : pingBack2?.Message;
            Assert.AreEqual("Hello from app App, instance App.", shortMessage);
            if (shortMessage != longMessage)
            {
                Assert.AreEqual("CanDisable " + shortMessage, longMessage);
            }
        }

        [Test]
        public async Task DispatchAsync_dispose_created_context()
        {
            var container = this.CreateInjector(parts: new[] { typeof(TestMessageProcessor) });
            var appRuntime = container.Resolve<IAppRuntime>();
            var messageBroker = await this.GetMessageBrokerAsync(container);
            var messageProcessor = (TestMessageProcessor)container.Resolve<IMessageProcessor>();
            var disposable = Substitute.For<IDisposable>();
            var calls = 0;
            messageProcessor.ProcessingContextConfigurator = (msg, ctx) =>
            {
                Interlocked.Increment(ref calls);
                if (calls == 1)
                {
                    ctx.AddResource(disposable);
                }
            };

            var pingBack = await messageBroker.DispatchAsync(
                new BrokeredMessage
                {
                    Content = new PingMessage(),
                    Timeout = TimeSpan.FromSeconds(100),
                    Recipients = new[] { Endpoint.CreateAppInstanceEndpoint(appRuntime) },
                });

            disposable.Received(1).Dispose();
        }

        [Test]
        public async Task ProcessAsync_Ping_over_serialization_success()
        {
            var container = this.CreateInjector(parts: new[] { typeof(RemoteMessageBroker) });
            var appRuntime = container.Resolve<IAppRuntime>();
            var messageBroker = await this.GetMessageBrokerAsync(container);

            var pingBack = await messageBroker.DispatchAsync(new PingMessage(), ctx => ctx.To(Endpoint.CreateAppInstanceEndpoint(appRuntime)));

            Assert.IsInstanceOf<PingBackMessage>(pingBack);
        }

        [Test]
        public async Task ProcessAsync_Ping_success()
        {
            var container = this.CreateInjector();
            var appRuntime = container.Resolve<IAppRuntime>();
            var messageBroker = await this.GetMessageBrokerAsync(container);

            var pingBack = await messageBroker.DispatchAsync(new PingMessage(), ctx => ctx.To(Endpoint.CreateAppInstanceEndpoint(appRuntime)));

            Assert.IsInstanceOf<PingBackMessage>(pingBack);
        }

        [Test]
        public async Task ProcessAsync_Ping_exception()
        {
            var container = this.CreateInjector(parts: new[] { typeof(ExceptionEventHandler) });
            var appRuntime = container.Resolve<IAppRuntime>();
            var messageBroker = await this.GetMessageBrokerAsync(container);

            Assert.That(() => messageBroker.DispatchAsync(new PingMessage(), ctx => ctx.To(Endpoint.CreateAppInstanceEndpoint(appRuntime))), Throws.InstanceOf<MessagingException>());
        }

        [Test]
        public async Task ProcessAsync_null_response_event_no_handlers()
        {
            var container = this.CreateInjector();
            var messageBroker = await this.GetMessageBrokerAsync(container);

            var nullResponse = await messageBroker.DispatchAsync(new TestEvent());

            Assert.IsNull(nullResponse);
        }

        [Test]
        public async Task PublishAsync_event()
        {
            var container = this.CreateInjector(parts: new[] { typeof(TestEventHandler) });
            var messageBroker = await this.GetMessageBrokerAsync(container);

            var message = new TestEvent { TaskCompletionSource = new TaskCompletionSource<(string, IBrokeredMessage)>() };
            await messageBroker.PublishAsync(message);

            var response = await message.TaskCompletionSource.Task;
            Assert.AreEqual("ok", response.response);
        }

        [Test]
        public async Task ProcessOneWayAsync_event()
        {
            var container = this.CreateInjector(parts: new[] { typeof(TestEventHandler) });
            var messageBroker = await this.GetMessageBrokerAsync(container);

            var message = new TestEvent { TaskCompletionSource = new TaskCompletionSource<(string, IBrokeredMessage)>() };
            await messageBroker.ProcessOneWayAsync(message);

            var response = await message.TaskCompletionSource.Task;
            Assert.AreEqual("ok", response.response);
            Assert.AreSame(message, response.brokeredMessage.Content);
        }

        private ILogger GetLogger<T>(StringBuilder sb)
        {
            return new DebugLogManager(sb).GetLogger<T>();
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
                IInjectableFactory injectableFactory,
                IAppRuntime appRuntime,
                IEnabledLazyEnumerable<IMessageRouter, MessageRouterMetadata> routerFactories,
                ISerializationService serializationService)
                : base(injectableFactory, appRuntime, routerFactories)
            {
                this.serializationService = serializationService;
            }

            protected override async Task RouterDispatchAsync(
                IBrokeredMessage brokeredMessage,
                IDispatchingContext context,
                CancellationToken cancellationToken)
            {
                var serializedMessage = await this.serializationService.JsonSerializeAsync(
                                            brokeredMessage,
                                            cancellationToken: cancellationToken);
                var deserializedMessage = await this.serializationService.JsonDeserializeAsync(
                                              serializedMessage,
                                              cancellationToken: cancellationToken);
                await base.RouterDispatchAsync((IBrokeredMessage)deserializedMessage, context, cancellationToken);
            }
        }

        [OverridePriority(Priority.High)]
        public class LoggableMessageBroker : DefaultMessageBroker
        {
            public LoggableMessageBroker(
                IInjectableFactory injectableFactory,
                IAppRuntime appRuntime,
                IEnabledLazyEnumerable<IMessageRouter, MessageRouterMetadata> routerFactories)
                : base(injectableFactory, appRuntime, routerFactories)
            {
            }

            public void SetLogger(ILogger logger)
            {
                this.Logger = logger;
            }
        }

        [OverridePriority(Priority.High)]
        public class TestMessageProcessor : DefaultMessageProcessor
        {
            public TestMessageProcessor(IInjectableFactory injectableFactory, IMessageHandlerRegistry handlerRegistry,
                IMessageMatchService messageMatchService,
                IList<IExportFactory<IMessagingBehavior, MessagingBehaviorMetadata>> behaviorFactories)
                : base(injectableFactory, handlerRegistry, messageMatchService, behaviorFactories)
            {
            }

            public Action<IMessage, IMessagingContext> ProcessingContextConfigurator { get; set; }

            protected override Task ApplyBeforeProcessBehaviorsAsync(
                IEnumerable<IMessagingBehavior> behaviors,
                IMessagingContext context,
                CancellationToken token)
            {
                this.ProcessingContextConfigurator?.Invoke(context.Message, context);
                return base.ApplyBeforeProcessBehaviorsAsync(behaviors, context, token);
            }
        }

        [ProcessingPriority(Priority.High)]
        [MessageRouter(ReceiverMatch = ChannelType + ":.*", IsFallback = true)]
        public class CanDisableMessageRouter : DefaultAppMessageRouter
        {
            private object sync = new object();

            public CanDisableMessageRouter(IInjectableFactory injectableFactory, IAppRuntime appRuntime, IMessageProcessor messageProcessor)
                : base(injectableFactory, appRuntime, messageProcessor, new InProcessMessageQueueStore(injectableFactory))
            {
            }

            public bool Enabled { get; private set; } = false;

            protected override async Task<IMessage> ProcessAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
            {
                var message = await base.ProcessAsync(brokeredMessage, context, cancellationToken);
                if (message is PingBackMessage pingBack)
                {
                    pingBack.Message = $"CanDisable " + pingBack.Message;
                    lock (this.sync)
                    {
                        this.Enabled = !this.Enabled;
                    }
                }

                return message;
            }
        }

        public class CanDisableMessageRouterEnabledRule : EnabledServiceBehaviorRuleBase<IMessageRouter, AppServiceMetadata, CanDisableMessageRouter>
        {
            public override IBehaviorValue<bool> GetValue(IServiceBehaviorContext<IMessageRouter, AppServiceMetadata> context)
            {
                return (context.Service as CanDisableMessageRouter).Enabled ? BehaviorValue.True : BehaviorValue.False;
            }
        }

        [MessageRouter(IsOptional = true)]
        [Override]
        public class OverrideMessageRouter : DefaultAppMessageRouter
        {
            public OverrideMessageRouter(IInjectableFactory injectableFactory, IAppRuntime appRuntime, IMessageProcessor messageProcessor)
                : base(injectableFactory, appRuntime, messageProcessor, new InProcessMessageQueueStore(injectableFactory))
            {
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

            public Task<(RoutingInstruction action, IMessage reply)> DispatchAsync(IBrokeredMessage brokeredMessage, IDispatchingContext context, CancellationToken cancellationToken)
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

            public Task<(RoutingInstruction action, IMessage reply)> DispatchAsync(IBrokeredMessage brokeredMessage, IDispatchingContext context, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        [MessageRouter(ReceiverMatchProviderType = typeof(string))]
        public class WithBadProviderMessageRouter : IMessageRouter
        {
            public event EventHandler<ReplyReceivedEventArgs> ReplyReceived;

            public Task<(RoutingInstruction action, IMessage reply)> DispatchAsync(IBrokeredMessage brokeredMessage, IDispatchingContext context, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        [MessageRouter(ReceiverMatchProviderType = typeof(TestReceiverMatchProvider))]
        public class WithProviderMessageRouter : IMessageRouter
        {
            public event EventHandler<ReplyReceivedEventArgs> ReplyReceived;

            public Task<(RoutingInstruction action, IMessage reply)> DispatchAsync(IBrokeredMessage brokeredMessage, IDispatchingContext context, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        public class TestReceiverMatchProvider : IReceiverMatchProvider
        {
            public TestReceiverMatchProvider(IAppRuntime appRuntime)
            {
            }

            public string GetReceiverMatch(IContext context = null) => "test";
        }
    }
}