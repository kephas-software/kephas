﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefDefaultMessageProcessorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="DefaultMessageProcessor" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.Autofac.Hosting;
    using Kephas.Composition.ExportFactories;
    using Kephas.Composition.Mef;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Messaging.Behaviors;
    using Kephas.Messaging.Behaviors.Composition;
    using Kephas.Messaging.Composition;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.HandlerProviders;
    using Kephas.Messaging.Messages;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Testing.Composition;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using NUnit.Framework;

    using TaskHelper = Kephas.Threading.Tasks.TaskHelper;

    /// <summary>
    /// Test class for <see cref="DefaultMessageProcessor"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AutofacDefaultMessageProcessorTest : AutofacCompositionTestBase
    {
        public override ICompositionContext CreateContainer(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<AutofacCompositionContainerBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? new Assembly[0])
            {
                typeof(IMessageProcessor).GetTypeInfo().Assembly, /* Kephas.Messaging */
            };

            return base.CreateContainer(ambientServices, assemblyList, parts, config, logManager, appRuntime);
        }

        [Test]
        public void DefaultMessageProcessor_Composition_success()
        {
            var container = this.CreateContainer();
            var requestProcessor = container.GetExport<IMessageProcessor>();
            Assert.IsInstanceOf<DefaultMessageProcessor>(requestProcessor);

            var typedRequestprocessor = (DefaultMessageProcessor)requestProcessor;
            Assert.IsNotNull(typedRequestprocessor.Logger);
        }

        [Test]
        public async Task ProcessAsync_Composition_success()
        {
            var container = this.CreateContainer();
            var requestProcessor = container.GetExport<IMessageProcessor>();
            Assert.IsInstanceOf<DefaultMessageProcessor>(requestProcessor);

            var result = await requestProcessor.ProcessAsync(new PingMessage(), null, CancellationToken.None);
            Assert.IsInstanceOf<PingBackMessage>(result);
        }

        [Test]
        public async Task ProcessAsync_Composition_non_message_success()
        {
            var container = this.CreateContainer();
            var handlerRegistry = container.GetExport<IMessageHandlerRegistry>();
            handlerRegistry.RegisterHandler<string>((s, c, token) => Task.FromResult<IMessage>(new ResponseMessage { Message = s + " handled" }));
            var requestProcessor = container.GetExport<IMessageProcessor>();
            Assert.IsInstanceOf<DefaultMessageProcessor>(requestProcessor);

            var result = await requestProcessor.ProcessAsync("hello", null, CancellationToken.None);
            Assert.IsInstanceOf<ResponseMessage>(result);
            Assert.AreEqual("hello handled", ((ResponseMessage)result).Message);
        }

        [Test]
        public async Task ProcessAsync_Composition_non_message_sync_success()
        {
            var container = this.CreateContainer();
            var handlerRegistry = container.GetExport<IMessageHandlerRegistry>();
            handlerRegistry.RegisterHandler<string>((s, c) => new ResponseMessage { Message = s + " handled" });
            var requestProcessor = container.GetExport<IMessageProcessor>();
            Assert.IsInstanceOf<DefaultMessageProcessor>(requestProcessor);

            var result = await requestProcessor.ProcessAsync("hello", null, CancellationToken.None);
            Assert.IsInstanceOf<ResponseMessage>(result);
            Assert.AreEqual("hello handled", ((ResponseMessage)result).Message);
        }

        [Test]
        public async Task ProcessAsync_dispose_created_contexts()
        {
            var handler = Substitute.For<IMessageHandler>();
            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => Substitute.For<IMessage>());
            var processor = (TestMessageProcessor) this.CreateMessageProcessor(handler, message);
            IMessagingContext context = null;
            processor.CreateProcessingContextFunc = (msg, ctx) =>
                    {
                        context = Substitute.For<IMessagingContext>();
                        context.MessageProcessor.Returns(processor);
                        context.Message.Returns(msg);
                        return context;
                    };

            var result = await processor.ProcessAsync(message);

            context.Received(1).Dispose();
        }

        [Test]
        public async Task ProcessAsync_result()
        {
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));
            var processor = this.CreateMessageProcessor(handler, message);
            var result = await processor.ProcessAsync(message);

            Assert.AreSame(expectedResponse, result);
        }

        [Test]
        public async Task ProcessAsync_override_handler()
        {
            var handler1 = Substitute.For<IMessageHandler>();
            var handler2 = Substitute.For<IMessageHandler>();
            var expectedResponse1 = Substitute.For<IMessage>();
            var expectedResponse2 = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler1.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse1));
            handler2.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse2));
            var processor = this.CreateMessageProcessor(new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                                            {
                                                                new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler1, new MessageHandlerMetadata(message.GetType(), overridePriority: (int)Priority.Low)),
                                                                new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler2, new MessageHandlerMetadata(message.GetType(), overridePriority: (int)Priority.High))
                                                            });
            var result = await processor.ProcessAsync(message, null, default);

            Assert.AreSame(expectedResponse2, result);
        }

        [Test]
        public async Task ProcessAsync_override_handler_with_message_ID()
        {
            var handler1 = Substitute.For<IMessageHandler>();
            var handler2 = Substitute.For<IMessageHandler>();
            var expectedResponse1 = Substitute.For<IMessage>();
            var expectedResponse2 = Substitute.For<IMessage>();

            var message = new IdentifiedMessage();
            message.Id = "hi";
            handler1.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse1));
            handler2.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse2));
            var processor = this.CreateMessageProcessor(new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                                            {
                                                                new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler1, new MessageHandlerMetadata(message.GetType(), messageId: message.Id, overridePriority: (int)Priority.Low)),
                                                                new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler2, new MessageHandlerMetadata(message.GetType(), messageId: message.Id, overridePriority: (int)Priority.High))
                                                            });
            var result = await processor.ProcessAsync(message, null, default);

            Assert.AreSame(expectedResponse2, result);
        }

        [Test]
        public async Task ProcessAsync_pseudo_override_handler_with_different_message_ID()
        {
            var handler1 = Substitute.For<IMessageHandler>();
            var handler2 = Substitute.For<IMessageHandler>();
            var expectedResponse1 = Substitute.For<IMessage>();
            var expectedResponse2 = Substitute.For<IMessage>();

            var message = new ExpandoMessage();
            (message as dynamic).MessageId = "hi";
            handler1.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse1));
            handler2.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse2));
            var processor = this.CreateMessageProcessor(new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                                            {
                                                                new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler1, new MessageHandlerMetadata(message.GetType(), messageId: "hi", messageIdMatching: MessageIdMatching.Id, overridePriority: (int)Priority.Low)),
                                                                new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler2, new MessageHandlerMetadata(message.GetType(), messageId: "not-hi", messageIdMatching: MessageIdMatching.Id, overridePriority: (int)Priority.High))
                                                            });
            var result = await processor.ProcessAsync(message, null, default);

            Assert.AreSame(expectedResponse1, result);
        }

        [Test]
        public async Task ProcessAsync_ambiguous_handler()
        {
            var handler1 = Substitute.For<IMessageHandler>();
            var handler2 = Substitute.For<IMessageHandler>();
            var expectedResponse1 = Substitute.For<IMessage>();
            var expectedResponse2 = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler1.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse1));
            handler2.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse2));
            var processor = this.CreateMessageProcessor(new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                                            {
                                                                new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler1, new MessageHandlerMetadata(message.GetType(), overridePriority: (int)Priority.Low)),
                                                                new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler2, new MessageHandlerMetadata(message.GetType(), overridePriority: (int)Priority.Low))
                                                            });

            Assert.That(() => processor.ProcessAsync(message, null, default), Throws.InstanceOf<AmbiguousMatchException>());
        }

        [Test]
        public void ProcessAsync_missing_handler_exception()
        {
            var message = Substitute.For<IMessage>();
            var processor = this.CreateMessageProcessor(new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                                            {
                                                            });
            Assert.That(() => processor.ProcessAsync(message, null, default), Throws.InstanceOf<MissingHandlerException>());
        }

        [Test]
        public void ProcessAsync_exception()
        {
            var handler = Substitute.For<IMessageHandler>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Throws(new InvalidOperationException());
            var processor = this.CreateMessageProcessor(handler, message);
            Assert.That(() => processor.ProcessAsync(message, null, default), Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public async Task ProcessAsync_disposed_handler()
        {
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));

            var processor = this.CreateMessageProcessor(handler, message);
            var result = await processor.ProcessAsync(message, null, default);

            Assert.LessOrEqual(1, handler.ReceivedCalls().Count());
        }

        [Test]
        public async Task ProcessAsync_test_behavior()
        {
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            var message = new PingMessage();
            handler.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));

            var f = this.CreateTestBehaviorFactory(messageType: typeof(PingMessage));

            var processor = (TestMessageProcessor)this.CreateMessageProcessor(new[] { f }, handler, message);
            var processingContext = new MessagingContext(Substitute.For<ICompositionContext>(), processor, message);
            processor.CreateProcessingContextFunc = (msg, ctx) => processingContext;
            var result = await processor.ProcessAsync(message, null, default);

            Assert.AreEqual(true, processingContext["Before TestBehavior"]);
            Assert.AreEqual(true, processingContext["After TestBehavior"]);
        }

        [Test]
        public async Task ProcessAsync_ordered_behavior()
        {
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));

            var beforelist = new List<int>();
            var afterlist = new List<int>();
            var f1 = this.CreateBehaviorFactory(
                (c, t) => { beforelist.Add(1); return Task.CompletedTask; },
                (c, t) => { afterlist.Add(1); return Task.CompletedTask; },
                processingPriority: 2);
            var f2 = this.CreateBehaviorFactory(
                (c, t) => { beforelist.Add(2); return Task.CompletedTask; },
                (c, t) => { afterlist.Add(2); return Task.CompletedTask; },
                processingPriority: 1);

            var processor = this.CreateMessageProcessor(new[] { f1, f2 }, handler, message);
            var result = await processor.ProcessAsync(message, null, default);

            Assert.AreEqual(2, beforelist.Count);
            Assert.AreEqual(2, beforelist[0]);
            Assert.AreEqual(1, beforelist[1]);

            Assert.AreEqual(2, afterlist.Count);
            Assert.AreEqual(1, afterlist[0]);
            Assert.AreEqual(2, afterlist[1]);
        }

        [Test]
        public async Task ProcessAsync_matching_behavior()
        {
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));

            var beforelist = new List<int>();
            var afterlist = new List<int>();
            var f1 = this.CreateBehaviorFactory(
                (c, t) => { beforelist.Add(1); return Task.CompletedTask; },
                (c, t) => { afterlist.Add(1); return Task.CompletedTask; },
                messageType: typeof(PingMessage));
            var f2 = this.CreateBehaviorFactory(
                (c, t) => { beforelist.Add(2); return Task.CompletedTask; },
                (c, t) => { afterlist.Add(2); return Task.CompletedTask; });

            var processor = this.CreateMessageProcessor(new[] { f1, f2 }, handler, message);
            var result = await processor.ProcessAsync(message, null, default);

            Assert.AreEqual(1, beforelist.Count);
            Assert.AreEqual(2, beforelist[0]);

            Assert.AreEqual(1, afterlist.Count);
            Assert.AreEqual(2, afterlist[0]);
        }

        [Test]
        public async Task ProcessAsync_matching_behavior_with_ID()
        {
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            var message = new IdentifiedMessage2 { MessageId = "hello" };
            handler.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));

            var beforelist = new List<int>();
            var afterlist = new List<int>();
            var f1 = this.CreateBehaviorFactory(
                (c, t) => { beforelist.Add(1); return Task.CompletedTask; },
                (c, t) => { afterlist.Add(1); return Task.CompletedTask; },
                messageType: typeof(IdentifiedMessage2),
                messageId: "hi");
            var f2 = this.CreateBehaviorFactory(
                (c, t) => { beforelist.Add(2); return Task.CompletedTask; },
                (c, t) => { afterlist.Add(2); return Task.CompletedTask; },
                messageType: typeof(IdentifiedMessage2),
                messageId: "hello");
            var f3 = this.CreateBehaviorFactory(
                (c, t) => { beforelist.Add(3); return Task.CompletedTask; },
                (c, t) => { afterlist.Add(3); return Task.CompletedTask; },
                messageType: typeof(IdentifiedMessage2),
                messageIdMatching: MessageIdMatching.All);
            var f4 = this.CreateBehaviorFactory(
                (c, t) => { beforelist.Add(4); return Task.CompletedTask; },
                (c, t) => { afterlist.Add(4); return Task.CompletedTask; },
                messageType: typeof(IMessage),
                messageTypeMatching: MessageTypeMatching.TypeOrHierarchy,
                messageIdMatching: MessageIdMatching.All);

            var processor = this.CreateMessageProcessor(new[] { f1, f2, f3, f4 }, 
                new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler, new MessageHandlerMetadata(message.GetType(), messageId: "hello")));
            var result = await processor.ProcessAsync(message, null, default);

            Assert.AreEqual(3, beforelist.Count);
            Assert.AreEqual(2, beforelist[0]);
            Assert.AreEqual(3, beforelist[1]);
            Assert.AreEqual(4, beforelist[2]);

            Assert.AreEqual(3, afterlist.Count);
            Assert.AreEqual(4, afterlist[0]);
            Assert.AreEqual(3, afterlist[1]);
            Assert.AreEqual(2, afterlist[2]);
        }

        [Test]
        public async Task ProcessAsync_exception_with_behavior()
        {
            var handler = Substitute.For<IMessageHandler>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Throws(new InvalidOperationException());

            var beforelist = new List<Exception>();
            var afterlist = new List<Exception>();
            var f1 = this.CreateBehaviorFactory(
                (c, t) => { beforelist.Add(c.Exception); return Task.CompletedTask; },
                (c, t) => { afterlist.Add(c.Exception); return Task.CompletedTask; });

            var processor = this.CreateMessageProcessor(new[] { f1 }, handler, message);
            InvalidOperationException thrownException = null;
            try
            {
                var result = await processor.ProcessAsync(message, null, default);
            }
            catch (InvalidOperationException ex)
            {
                thrownException = ex;
            }

            Assert.IsInstanceOf<InvalidOperationException>(thrownException);

            Assert.AreEqual(1, beforelist.Count);
            Assert.IsNull(beforelist[0]);

            Assert.AreEqual(1, afterlist.Count);
            Assert.IsInstanceOf<InvalidOperationException>(afterlist[0]);
        }

        [Test]
        public async Task ProcessAsync_multi_cast_event()
        {
            var mms = new DefaultMessageMatchService();
            var handler1 = Substitute.For<IMessageHandler>();
            var handler2 = Substitute.For<IMessageHandler>();

            var message = Substitute.For<IEvent>();
            var handlerFactories = new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                       {
                                           new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler1, new MessageHandlerMetadata(message.GetType(), overridePriority: (int)Priority.Low)),
                                           new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler2, new MessageHandlerMetadata(message.GetType(), overridePriority: (int)Priority.Low))
                                       };
            var processor = this.CreateMessageProcessor(
                handlerProviderFactories: new List<IExportFactory<IMessageHandlerProvider, AppServiceMetadata>>
                                              {
                                                  new ExportFactory<IMessageHandlerProvider, AppServiceMetadata>(() => new EventMessageHandlerProvider(mms), new AppServiceMetadata())
                                              },
                handlerFactories: handlerFactories);
            await processor.ProcessAsync(message, null, default);

            handler1.Received(1).ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>());
            handler2.Received(1).ProcessAsync(message, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task ProcessAsync_selector_works_properly()
        {
            var mms = new DefaultMessageMatchService();
            var eventMessage = Substitute.For<IEvent>();
            var eventHandler = Substitute.For<IMessageHandler>();
            var plainMessage = Substitute.For<IMessage>();
            var plainHandler = Substitute.For<IMessageHandler>();

            var handlerFactories = new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                       {
                                           new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => eventHandler, new MessageHandlerMetadata(eventMessage.GetType())),
                                           new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => plainHandler, new MessageHandlerMetadata(plainMessage.GetType()))
                                       };

            var processor = this.CreateMessageProcessor(
                handlerProviderFactories: new List<IExportFactory<IMessageHandlerProvider, AppServiceMetadata>>
                                              {
                                                  new ExportFactory<IMessageHandlerProvider, AppServiceMetadata>(() => new DefaultMessageHandlerProvider(mms), new AppServiceMetadata(processingPriority: (int)Priority.Low)),
                                                  new ExportFactory<IMessageHandlerProvider, AppServiceMetadata>(() => new EventMessageHandlerProvider(mms), new AppServiceMetadata(processingPriority: (int)Priority.High))
                                              },
                handlerFactories: handlerFactories);

            await processor.ProcessAsync(eventMessage, null, default);
            eventHandler.Received(1).ProcessAsync(eventMessage, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>());
            plainHandler.Received(0).ProcessAsync(eventMessage, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>());

            await processor.ProcessAsync(plainMessage, null, default);
            eventHandler.Received(0).ProcessAsync(plainMessage, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>());
            plainHandler.Received(1).ProcessAsync(plainMessage, Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>());
        }

        private IExportFactory<IMessagingBehavior, MessagingBehaviorMetadata> CreateBehaviorFactory(
            Func<IMessagingContext, CancellationToken, Task> beforeFunc = null,
            Func<IMessagingContext, CancellationToken, Task> afterFunc = null,
            Type messageType = null,
            MessageTypeMatching messageTypeMatching = MessageTypeMatching.TypeOrHierarchy,
            object messageId = null,
            MessageIdMatching messageIdMatching = MessageIdMatching.Id,
            int processingPriority = 0,
            Priority overridePriority = Priority.Normal)
        {
            messageType = messageType ?? typeof(IMessage);
            beforeFunc = beforeFunc ?? ((c, t) => Task.CompletedTask);
            afterFunc = afterFunc ?? ((c, t) => Task.CompletedTask);
            var behavior = Substitute.For<IMessagingBehavior>();
            behavior.BeforeProcessAsync(Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => beforeFunc(ci.Arg<IMessagingContext>(), ci.Arg<CancellationToken>()));
            behavior.AfterProcessAsync(Arg.Any<IMessagingContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => afterFunc(ci.Arg<IMessagingContext>(), ci.Arg<CancellationToken>()));
            var factory =
                new ExportFactoryAdapter<IMessagingBehavior, MessagingBehaviorMetadata>(
                    () => Tuple.Create(behavior, (Action)(() => { })),
                    new MessagingBehaviorMetadata(messageType, messageTypeMatching: messageTypeMatching, messageId: messageId, messageIdMatching: messageIdMatching, processingPriority: processingPriority, overridePriority: (int)overridePriority));
            return factory;
        }

        private IExportFactory<IMessagingBehavior, MessagingBehaviorMetadata> CreateTestBehaviorFactory(
            Type messageType = null,
            object messageId = null,
            int processingPriority = 0,
            Priority overridePriority = Priority.Normal)
        {
            messageType ??= typeof(IMessage);
            var behavior = new TestBehavior();
            var factory =
                new ExportFactoryAdapter<IMessagingBehavior, MessagingBehaviorMetadata>(
                    () => Tuple.Create((IMessagingBehavior)behavior, (Action)(() => { })),
                    new MessagingBehaviorMetadata(messageType, messageId: messageId, processingPriority: processingPriority, overridePriority: (int)overridePriority));
            return factory;
        }

        private DefaultMessageProcessor CreateMessageProcessor(
            IList<IExportFactory<IMessagingBehavior, MessagingBehaviorMetadata>> behaviorFactories,
            ExportFactory<IMessageHandler, MessageHandlerMetadata> handlerFactory)
        {
            return this.CreateMessageProcessor(
                behaviorFactories, null, new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                           {
                                               handlerFactory
                                           });
        }

        private TestMessageProcessor CreateMessageProcessor(
            IList<IExportFactory<IMessagingBehavior, MessagingBehaviorMetadata>> behaviorFactories,
            IMessageHandler messageHandler,
            IMessage message)
        {
            return this.CreateMessageProcessor(
                behaviorFactories, null, new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                {
                                    new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => messageHandler, new MessageHandlerMetadata(message.GetType()))
                                });
        }

        private TestMessageProcessor CreateMessageProcessor(IMessageHandler messageHandler, IMessage message)
        {
            return this.CreateMessageProcessor(
                null,
                null,
                new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                    {
                        new ExportFactory<IMessageHandler, MessageHandlerMetadata>(
                            () => messageHandler,
                            new MessageHandlerMetadata(message.GetType()))
                    });
        }

        private TestMessageProcessor CreateMessageProcessor(IList<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerFactories = null)
        {
            return this.CreateMessageProcessor(null, null, handlerFactories);
        }

        private TestMessageProcessor CreateMessageProcessor(
            IList<IExportFactory<IMessagingBehavior, MessagingBehaviorMetadata>> behaviorFactories = null,
            IList<IExportFactory<IMessageHandlerProvider, AppServiceMetadata>> handlerProviderFactories = null,
            IList<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerFactories = null)
        {
            var mms = new DefaultMessageMatchService();
            behaviorFactories = behaviorFactories
                              ?? new List<IExportFactory<IMessagingBehavior, MessagingBehaviorMetadata>>();

            handlerProviderFactories = handlerProviderFactories
                                       ?? new List<IExportFactory<IMessageHandlerProvider, AppServiceMetadata>>
                                              {
                                                  new ExportFactory<IMessageHandlerProvider, AppServiceMetadata>(() => new DefaultMessageHandlerProvider(mms), new AppServiceMetadata()),
                                              };
            handlerFactories = handlerFactories
                                       ?? new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                              {
                                                  // new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => new DefaultMessageHandlerProvider(mms), new AppServiceMetadata()),
                                              };

            var contextFactory = Substitute.For<IContextFactory>();
            var processor = new TestMessageProcessor(
                contextFactory,
                mms,
                new DefaultMessageHandlerRegistry(mms, handlerProviderFactories, handlerFactories),
                behaviorFactories);

            contextFactory.CreateContext<MessagingContext>(Arg.Any<object[]>())
                .Returns(ci => new MessagingContext(Substitute.For<ICompositionContext>(), processor));

            return processor;
        }
    }

    public class ExpandoMessage : Expando, IMessage
    {
    }

    public class IdentifiedMessage : IMessage
    {
        public string Id { get; set; }
    }

    public class IdentifiedMessage2 : IMessage
    {
        public string MessageId { get; set; }
    }

    public class TestBehavior : MessagingBehaviorBase<PingMessage>
    {
        public override Task BeforeProcessAsync(PingMessage message, IMessagingContext context, CancellationToken token)
        {
            context["Before TestBehavior"] = true;
            return base.BeforeProcessAsync(message, context, token);
        }

        public override Task AfterProcessAsync(PingMessage message, IMessagingContext context, CancellationToken token)
        {
            context["After TestBehavior"] = true;
            return base.AfterProcessAsync(message, context, token);
        }
    }

    public class TestMessageProcessor : DefaultMessageProcessor
    {
        public Func<IMessage, Action<IMessagingContext>, IMessagingContext> CreateProcessingContextFunc { get; set; }

        public TestMessageProcessor(IContextFactory contextFactory, IMessageMatchService messageMatchService, IMessageHandlerRegistry handlerRegistry, IList<IExportFactory<IMessagingBehavior, MessagingBehaviorMetadata>> behaviorFactories)
            : base(contextFactory, handlerRegistry, messageMatchService, behaviorFactories)
        {
        }

        protected override IMessagingContext CreateProcessingContext(IMessage message, Action<IMessagingContext> optionsConfig)
        {
            return
                this.CreateProcessingContextFunc?.Invoke(message, optionsConfig)
                ?? base.CreateProcessingContext(message, optionsConfig);
        }
    }
}