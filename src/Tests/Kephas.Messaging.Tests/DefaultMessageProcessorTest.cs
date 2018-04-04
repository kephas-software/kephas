// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageProcessorTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="DefaultMessageProcessor" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Composition.Mef;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Dynamic;
    using Kephas.Messaging.Composition;
    using Kephas.Messaging.HandlerSelectors;
    using Kephas.Messaging.Ping;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Testing.Composition.Mef;

    using NSubstitute;
    using NSubstitute.ExceptionExtensions;

    using NUnit.Framework;

    using TaskHelper = Kephas.Threading.Tasks.TaskHelper;

    /// <summary>
    /// Test class for <see cref="DefaultMessageProcessor"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class DefaultMessageProcessorTest : CompositionTestBase
    {
        public override ICompositionContext CreateContainer(IEnumerable<Assembly> assemblies, IEnumerable<Type> parts = null, Action<MefCompositionContainerBuilder> config = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? new Assembly[0]);
            assemblyList.Add(typeof(IMessageProcessor).GetTypeInfo().Assembly); /* Kephas.Messaging */
            return base.CreateContainer(assemblyList, parts, config);
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
        public async Task ProcessAsync_context_handler_and_message_preserved()
        {
            var handler = Substitute.For<IMessageHandler>();
            var contextHandler = Substitute.For<IMessageHandler>();
            var contextMessage = Substitute.For<IMessage>();

            var expectedResponse = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(
                    ci =>
                        {
                            var ctx = ci.Arg<IMessageProcessingContext>();
                            ctx.Message = Substitute.For<IMessage>();
                            ctx.Handler = Substitute.For<IMessageHandler>();
                            return Task.FromResult(expectedResponse);
                        });
            var processor = this.CreateRequestProcessor(handler, message);

            var context = new MessageProcessingContext(processor, contextMessage, contextHandler);

            var result = await processor.ProcessAsync(message, context);

            Assert.AreSame(contextMessage, context.Message);
            Assert.AreSame(contextHandler, context.Handler);
        }

        [Test]
        public async Task ProcessAsync_result()
        {
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));
            var processor = this.CreateRequestProcessor(handler, message);
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
            handler1.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse1));
            handler2.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse2));
            var processor = this.CreateRequestProcessor(new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                                            {
                                                                new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler1, new MessageHandlerMetadata(message.GetType(), overridePriority: (int)Priority.Low)),
                                                                new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler2, new MessageHandlerMetadata(message.GetType(), overridePriority: (int)Priority.High))
                                                            });
            var result = await processor.ProcessAsync(message, null, default);

            Assert.AreSame(expectedResponse2, result);
        }

        [Test]
        public async Task ProcessAsync_override_handler_with_message_name()
        {
            var handler1 = Substitute.For<IMessageHandler>();
            var handler2 = Substitute.For<IMessageHandler>();
            var expectedResponse1 = Substitute.For<IMessage>();
            var expectedResponse2 = Substitute.For<IMessage>();

            var message = new NamedMessage();
            message.MessageName = "hi";
            handler1.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse1));
            handler2.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse2));
            var processor = this.CreateRequestProcessor(new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                                            {
                                                                new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler1, new MessageHandlerMetadata(message.GetType(), message.MessageName, overridePriority: (int)Priority.Low)),
                                                                new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler2, new MessageHandlerMetadata(message.GetType(), message.MessageName, overridePriority: (int)Priority.High))
                                                            });
            var result = await processor.ProcessAsync(message, null, default);

            Assert.AreSame(expectedResponse2, result);
        }

        [Test]
        public async Task ProcessAsync_pseudo_override_handler_with_different_message_name()
        {
            var handler1 = Substitute.For<IMessageHandler>();
            var handler2 = Substitute.For<IMessageHandler>();
            var expectedResponse1 = Substitute.For<IMessage>();
            var expectedResponse2 = Substitute.For<IMessage>();

            var message = new ExpandoMessage();
            (message as dynamic).MessageName = "hi";
            handler1.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse1));
            handler2.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse2));
            var processor = this.CreateRequestProcessor(new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                                            {
                                                                new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler1, new MessageHandlerMetadata(message.GetType(), "hi", overridePriority: (int)Priority.Low)),
                                                                new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler2, new MessageHandlerMetadata(message.GetType(), "not-hi", overridePriority: (int)Priority.High))
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
            handler1.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse1));
            handler2.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse2));
            var processor = this.CreateRequestProcessor(new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
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
            var processor = this.CreateRequestProcessor(new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                                            {
                                                            });
            Assert.That(() => processor.ProcessAsync(message, null, default), Throws.InstanceOf<MissingHandlerException>());
        }

        [Test]
        public void ProcessAsync_exception()
        {
            var handler = Substitute.For<IMessageHandler>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Throws(new InvalidOperationException());
            var processor = this.CreateRequestProcessor(handler, message);
            Assert.That(() => processor.ProcessAsync(message, null, default), Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public async Task ProcessAsync_disposed_handler()
        {
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));

            var processor = this.CreateRequestProcessor(handler, message);
            var result = await processor.ProcessAsync(message, null, default);

            Assert.LessOrEqual(1, handler.ReceivedCalls().Count());
        }

        [Test]
        public async Task ProcessAsync_test_filter()
        {
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            var message = new PingMessage();
            handler.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));

            var f = this.CreateTestFilterFactory(messageType: typeof(PingMessage));

            var processor = this.CreateRequestProcessor(new[] { f }, handler, message);
            var processingContext = new MessageProcessingContext(processor, message, Substitute.For<IMessageHandler>());
            var result = await processor.ProcessAsync(message, processingContext, default);

            Assert.AreEqual(true, processingContext["Before TestFilter"]);
            Assert.AreEqual(true, processingContext["After TestFilter"]);
        }

        [Test]
        public async Task ProcessAsync_ordered_filter()
        {
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));

            var beforelist = new List<int>();
            var afterlist = new List<int>();
            var f1 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(1); return TaskHelper.CompletedTask; },
                (c, t) => { afterlist.Add(1); return TaskHelper.CompletedTask; },
                processingPriority: 2);
            var f2 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(2); return TaskHelper.CompletedTask; },
                (c, t) => { afterlist.Add(2); return TaskHelper.CompletedTask; },
                processingPriority: 1);

            var processor = this.CreateRequestProcessor(new[] { f1, f2 }, handler, message);
            var result = await processor.ProcessAsync(message, null, default);

            Assert.AreEqual(2, beforelist.Count);
            Assert.AreEqual(2, beforelist[0]);
            Assert.AreEqual(1, beforelist[1]);

            Assert.AreEqual(2, afterlist.Count);
            Assert.AreEqual(1, afterlist[0]);
            Assert.AreEqual(2, afterlist[1]);
        }

        [Test]
        public async Task ProcessAsync_matching_filter()
        {
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));

            var beforelist = new List<int>();
            var afterlist = new List<int>();
            var f1 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(1); return TaskHelper.CompletedTask; },
                (c, t) => { afterlist.Add(1); return TaskHelper.CompletedTask; },
                messageType: typeof(PingMessage));
            var f2 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(2); return TaskHelper.CompletedTask; },
                (c, t) => { afterlist.Add(2); return TaskHelper.CompletedTask; });

            var processor = this.CreateRequestProcessor(new[] { f1, f2 }, handler, message);
            var result = await processor.ProcessAsync(message, null, default);

            Assert.AreEqual(1, beforelist.Count);
            Assert.AreEqual(2, beforelist[0]);

            Assert.AreEqual(1, afterlist.Count);
            Assert.AreEqual(2, afterlist[0]);
        }

        [Test]
        public async Task ProcessAsync_matching_filter_with_name()
        {
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            var message = new NamedMessage { MessageName = "hello" };
            handler.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));

            var beforelist = new List<int>();
            var afterlist = new List<int>();
            var f1 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(1); return TaskHelper.CompletedTask; },
                (c, t) => { afterlist.Add(1); return TaskHelper.CompletedTask; },
                messageType: typeof(NamedMessage),
                messageName: "hi");
            var f2 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(2); return TaskHelper.CompletedTask; },
                (c, t) => { afterlist.Add(2); return TaskHelper.CompletedTask; },
                messageType: typeof(NamedMessage),
                messageName: "hello");
            var f3 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(3); return TaskHelper.CompletedTask; },
                (c, t) => { afterlist.Add(3); return TaskHelper.CompletedTask; },
                messageType: typeof(NamedMessage),
                messageName: null);

            var processor = this.CreateRequestProcessor(new[] { f1, f2, f3 }, 
                new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler, new MessageHandlerMetadata(message.GetType(), "hello")));
            var result = await processor.ProcessAsync(message, null, default);

            Assert.AreEqual(2, beforelist.Count);
            Assert.AreEqual(2, beforelist[0]);
            Assert.AreEqual(3, beforelist[1]);

            Assert.AreEqual(2, afterlist.Count);
            Assert.AreEqual(3, afterlist[0]);
            Assert.AreEqual(2, afterlist[1]);
        }

        [Test]
        public async Task ProcessAsync_exception_with_filter()
        {
            var handler = Substitute.For<IMessageHandler>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Throws(new InvalidOperationException());

            var beforelist = new List<Exception>();
            var afterlist = new List<Exception>();
            var f1 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(c.Exception); return TaskHelper.CompletedTask; },
                (c, t) => { afterlist.Add(c.Exception); return TaskHelper.CompletedTask; });

            var processor = this.CreateRequestProcessor(new[] { f1 }, handler, message);
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
            var handler1 = Substitute.For<IMessageHandler>();
            var handler2 = Substitute.For<IMessageHandler>();

            var message = Substitute.For<IEvent>();
            var handlerFactories = new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                       {
                                           new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler1, new MessageHandlerMetadata(message.GetType(), overridePriority: (int)Priority.Low)),
                                           new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler2, new MessageHandlerMetadata(message.GetType(), overridePriority: (int)Priority.Low))
                                       };
            var processor = this.CreateRequestProcessor(
                handlerSelectorFactories: new List<IExportFactory<IMessageHandlerSelector, AppServiceMetadata>>
                                              {
                                                  new ExportFactory<IMessageHandlerSelector, AppServiceMetadata>(() => new EventMessageHandlerSelector(handlerFactories), new AppServiceMetadata())
                                              });
            await processor.ProcessAsync(message, null, default);

            handler1.Received(1).ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>());
            handler2.Received(1).ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task ProcessAsync_selector_works_properly()
        {
            var eventMessage = Substitute.For<IEvent>();
            var eventHandler = Substitute.For<IMessageHandler>();
            var plainMessage = Substitute.For<IMessage>();
            var plainHandler = Substitute.For<IMessageHandler>();

            var handlerFactories = new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                       {
                                           new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => eventHandler, new MessageHandlerMetadata(eventMessage.GetType())),
                                           new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => plainHandler, new MessageHandlerMetadata(plainMessage.GetType()))
                                       };

            var processor = this.CreateRequestProcessor(
                handlerSelectorFactories: new List<IExportFactory<IMessageHandlerSelector, AppServiceMetadata>>
                                              {
                                                  new ExportFactory<IMessageHandlerSelector, AppServiceMetadata>(() => new DefaultMessageHandlerSelector(handlerFactories), new AppServiceMetadata(processingPriority: (int)Priority.Low)),
                                                  new ExportFactory<IMessageHandlerSelector, AppServiceMetadata>(() => new EventMessageHandlerSelector(handlerFactories), new AppServiceMetadata(processingPriority: (int)Priority.High))
                                              });

            await processor.ProcessAsync(eventMessage, null, default);
            eventHandler.Received(1).ProcessAsync(eventMessage, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>());
            plainHandler.Received(0).ProcessAsync(eventMessage, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>());

            await processor.ProcessAsync(plainMessage, null, default);
            eventHandler.Received(0).ProcessAsync(plainMessage, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>());
            plainHandler.Received(1).ProcessAsync(plainMessage, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>());
        }

        private IExportFactory<IMessageProcessingFilter, MessageProcessingFilterMetadata> CreateFilterFactory(
            Func<IMessageProcessingContext, CancellationToken, Task> beforeFunc = null,
            Func<IMessageProcessingContext, CancellationToken, Task> afterFunc = null,
            Type messageType = null,
            string messageName = null,
            int processingPriority = 0,
            Priority overridePriority = Priority.Normal)
        {
            messageType = messageType ?? typeof(IMessage);
            beforeFunc = beforeFunc ?? ((c, t) => TaskHelper.CompletedTask);
            afterFunc = afterFunc ?? ((c, t) => TaskHelper.CompletedTask);
            var filter = Substitute.For<IMessageProcessingFilter>();
            filter.BeforeProcessAsync(Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => beforeFunc(ci.Arg<IMessageProcessingContext>(), ci.Arg<CancellationToken>()));
            filter.AfterProcessAsync(Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => afterFunc(ci.Arg<IMessageProcessingContext>(), ci.Arg<CancellationToken>()));
            var factory =
                new ExportFactoryAdapter<IMessageProcessingFilter, MessageProcessingFilterMetadata>(
                    () => Tuple.Create(filter, (Action)(() => { })),
                    new MessageProcessingFilterMetadata(messageType, messageName, processingPriority: processingPriority, overridePriority: (int)overridePriority));
            return factory;
        }

        private IExportFactory<IMessageProcessingFilter, MessageProcessingFilterMetadata> CreateTestFilterFactory(
            Type messageType = null,
            string messageName = null,
            int processingPriority = 0,
            Priority overridePriority = Priority.Normal)
        {
            messageType = messageType ?? typeof(IMessage);
            var filter = new TestFilter();
            var factory =
                new ExportFactoryAdapter<IMessageProcessingFilter, MessageProcessingFilterMetadata>(
                    () => Tuple.Create((IMessageProcessingFilter)filter, (Action)(() => { })),
                    new MessageProcessingFilterMetadata(messageType, messageName, processingPriority: processingPriority, overridePriority: (int)overridePriority));
            return factory;
        }

        private DefaultMessageProcessor CreateRequestProcessor(
            IList<IExportFactory<IMessageProcessingFilter, MessageProcessingFilterMetadata>> filterFactories,
            ExportFactory<IMessageHandler, MessageHandlerMetadata> handlerFactory)
        {
            return this.CreateRequestProcessor(
                filterFactories, null, new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                           {
                                               handlerFactory
                                           });
        }

        private DefaultMessageProcessor CreateRequestProcessor(
            IList<IExportFactory<IMessageProcessingFilter, MessageProcessingFilterMetadata>> filterFactories,
            IMessageHandler messageHandler, IMessage message)
        {
            return this.CreateRequestProcessor(
                filterFactories, null, new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                {
                                    new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => messageHandler, new MessageHandlerMetadata(message.GetType()))
                                });
        }

        private DefaultMessageProcessor CreateRequestProcessor(IMessageHandler messageHandler, IMessage message)
        {
            return this.CreateRequestProcessor(
                null, null, new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                                                                     {
                                                                                         new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => messageHandler, new MessageHandlerMetadata(message.GetType()))
                                                                                     });
        }

        private DefaultMessageProcessor CreateRequestProcessor(IList<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerFactories = null)
        {
            return this.CreateRequestProcessor(null, null, handlerFactories);
        }

        private DefaultMessageProcessor CreateRequestProcessor(
            IList<IExportFactory<IMessageProcessingFilter, MessageProcessingFilterMetadata>> filterFactories = null,
            IList<IExportFactory<IMessageHandlerSelector, AppServiceMetadata>> handlerSelectorFactories = null,
            IList<IExportFactory<IMessageHandler, MessageHandlerMetadata>> handlerFactories = null)
        {
            filterFactories = filterFactories
                              ?? new List<IExportFactory<IMessageProcessingFilter, MessageProcessingFilterMetadata>>();

            handlerSelectorFactories = handlerSelectorFactories
                                       ?? new List<IExportFactory<IMessageHandlerSelector, AppServiceMetadata>>
                                              {
                                                  new ExportFactory<IMessageHandlerSelector, AppServiceMetadata>(() => new DefaultMessageHandlerSelector(handlerFactories ?? new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>()), new AppServiceMetadata())
                                              };

            return new DefaultMessageProcessor(Substitute.For<IAmbientServices>(), handlerSelectorFactories, filterFactories);
        }
    }

    public class ExpandoMessage : Expando, IMessage
    {
    }

    public class NamedMessage : IMessage
    {
        public string MessageName { get; set; }
    }

    public class TestFilter : MessageProcessingFilterBase<PingMessage>
    {
        public override Task BeforeProcessAsync(PingMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            context["Before TestFilter"] = true;
            return base.BeforeProcessAsync(message, context, token);
        }

        public override Task AfterProcessAsync(PingMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            context["After TestFilter"] = true;
            return base.AfterProcessAsync(message, context, token);
        }
    }
}