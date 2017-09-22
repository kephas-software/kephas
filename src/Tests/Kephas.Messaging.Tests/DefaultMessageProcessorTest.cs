// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageProcessorTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using Kephas.Composition.ExportFactoryImporters;
    using Kephas.Composition.Mef;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Messaging.Composition;
    using Kephas.Messaging.HandlerSelectors;
    using Kephas.Messaging.Ping;
    using Kephas.Reflection;
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
            var compositionContainer = Substitute.For<ICompositionContext>();
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
            this.ConfigureHandlersForMessage(compositionContainer, message, handler);
            var processor = this.CreateRequestProcessor(compositionContainer);

            var context = new MessageProcessingContext(processor, contextMessage, contextHandler);

            var result = await processor.ProcessAsync(message, context, default);

            Assert.AreSame(contextMessage, context.Message);
            Assert.AreSame(contextHandler, context.Handler);
        }

        [Test]
        public async Task ProcessAsync_result()
        {
            var compositionContainer = Substitute.For<ICompositionContext>();
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));
            this.ConfigureHandlersForMessage(compositionContainer, message, handler);
            var processor = this.CreateRequestProcessor(compositionContainer);
            var result = await processor.ProcessAsync(message, null, default);

            Assert.AreSame(expectedResponse, result);
        }

        [Test]
        public async Task ProcessAsync_override_handler()
        {
            var compositionContainer = Substitute.For<ICompositionContext>();
            var handler1 = Substitute.For<IMessageHandler>();
            var handler2 = Substitute.For<IMessageHandler>();
            var expectedResponse1 = Substitute.For<IMessage>();
            var expectedResponse2 = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler1.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse1));
            handler2.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse2));
            this.ConfigureHandlersForMessage(
                compositionContainer,
                message, 
                new ExportFactory<IMessageHandler, AppServiceMetadata>(() => handler1, new AppServiceMetadata(overridePriority: (int)Priority.Low)),
                new ExportFactory<IMessageHandler, AppServiceMetadata>(() => handler2, new AppServiceMetadata(overridePriority: (int)Priority.High)));
            var processor = this.CreateRequestProcessor(compositionContainer);
            var result = await processor.ProcessAsync(message, null, default);

            Assert.AreSame(expectedResponse2, result);
        }

        [Test]
        public async Task ProcessAsync_ambiguous_handler()
        {
            var compositionContainer = Substitute.For<ICompositionContext>();
            var handler1 = Substitute.For<IMessageHandler>();
            var handler2 = Substitute.For<IMessageHandler>();
            var expectedResponse1 = Substitute.For<IMessage>();
            var expectedResponse2 = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler1.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse1));
            handler2.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse2));
            this.ConfigureHandlersForMessage(
                compositionContainer,
                message,
                new ExportFactory<IMessageHandler, AppServiceMetadata>(() => handler1, new AppServiceMetadata(overridePriority: (int)Priority.Low)),
                new ExportFactory<IMessageHandler, AppServiceMetadata>(() => handler2, new AppServiceMetadata(overridePriority: (int)Priority.Low)));
            var processor = this.CreateRequestProcessor(compositionContainer);

            Assert.That(() => processor.ProcessAsync(message, null, default), Throws.InstanceOf<AmbiguousMatchException>());
        }

        [Test]
        public void ProcessAsync_missing_handler_exception()
        {
            var compositionContainer = Substitute.For<ICompositionContext>();
            var message = Substitute.For<IMessage>();
            this.ConfigureHandlersForMessage(compositionContainer, message,  new IMessageHandler[0]);
            var processor = this.CreateRequestProcessor(compositionContainer);
            Assert.That(() => processor.ProcessAsync(message, null, default), Throws.InstanceOf<MissingHandlerException>());
        }

        [Test]
        public void ProcessAsync_exception()
        {
            var compositionContainer = Substitute.For<ICompositionContext>();
            var handler = Substitute.For<IMessageHandler>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Throws(new InvalidOperationException());
            this.ConfigureHandlersForMessage(compositionContainer, message, handler);
            var processor = this.CreateRequestProcessor(compositionContainer);
            Assert.That(() => processor.ProcessAsync(message, null, default), Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public async Task ProcessAsync_disposed_handler()
        {
            var compositionContainer = Substitute.For<ICompositionContext>();
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));

            this.ConfigureHandlersForMessage(compositionContainer, message, handler);

            var processor = this.CreateRequestProcessor(compositionContainer);
            var result = await processor.ProcessAsync(message, null, default);

            Assert.LessOrEqual(1, handler.ReceivedCalls().Count());
        }

        [Test]
        public async Task ProcessAsync_ordered_filter()
        {
            var compositionContainer = Substitute.For<ICompositionContext>();
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));
            this.ConfigureHandlersForMessage(compositionContainer, message, handler);

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

            var processor = this.CreateRequestProcessor(compositionContainer, new[] { f1, f2 });
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
            var compositionContainer = Substitute.For<ICompositionContext>();
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));
            this.ConfigureHandlersForMessage(compositionContainer, message, handler);

            var beforelist = new List<int>();
            var afterlist = new List<int>();
            var f1 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(1); return TaskHelper.CompletedTask; }, 
                (c, t) => { afterlist.Add(1); return TaskHelper.CompletedTask; }, 
                requestType: typeof(PingMessage));
            var f2 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(2); return TaskHelper.CompletedTask; }, 
                (c, t) => { afterlist.Add(2); return TaskHelper.CompletedTask; });

            var processor = this.CreateRequestProcessor(compositionContainer, new[] { f1, f2 });
            var result = await processor.ProcessAsync(message, null, default);

            Assert.AreEqual(1, beforelist.Count);
            Assert.AreEqual(2, beforelist[0]);

            Assert.AreEqual(1, afterlist.Count);
            Assert.AreEqual(2, afterlist[0]);
        }

        [Test]
        public async Task ProcessAsync_exception_with_filter()
        {
            var compositionContainer = Substitute.For<ICompositionContext>();
            var handler = Substitute.For<IMessageHandler>();

            var message = Substitute.For<IMessage>();
            handler.ProcessAsync(message, Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Throws(new InvalidOperationException());
            this.ConfigureHandlersForMessage(compositionContainer, message, handler);

            var beforelist = new List<Exception>();
            var afterlist = new List<Exception>();
            var f1 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(c.Exception); return TaskHelper.CompletedTask; }, 
                (c, t) => { afterlist.Add(c.Exception); return TaskHelper.CompletedTask; });

            var processor = this.CreateRequestProcessor(compositionContainer, new[] { f1 });
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
            var compositionContainer = Substitute.For<ICompositionContext>();
            var handler1 = Substitute.For<IMessageHandler>();
            var handler2 = Substitute.For<IMessageHandler>();

            var message = Substitute.For<IEvent>();
            var handlerFactories = new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>
                                       {
                                           new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler1, new MessageHandlerMetadata(message.GetType(), overridePriority: (int)Priority.Low)),
                                           new ExportFactory<IMessageHandler, MessageHandlerMetadata>(() => handler2, new MessageHandlerMetadata(message.GetType(), overridePriority: (int)Priority.Low))
                                       };
            var processor = this.CreateRequestProcessor(
                compositionContainer,
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
            var compositionContainer = Substitute.For<ICompositionContext>();

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
                compositionContainer,
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
            Type requestType = null, 
            int processingPriority = 0, 
            Priority overridePriority = Priority.Normal)
        {
            requestType = requestType ?? typeof(IMessage);
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
                    new MessageProcessingFilterMetadata(requestType, processingPriority, (int)overridePriority));
            return factory;
        } 

        private DefaultMessageProcessor CreateRequestProcessor(ICompositionContext compositionContainer,
            IList<IExportFactory<IMessageProcessingFilter, MessageProcessingFilterMetadata>> filterFactories = null,
            IList<IExportFactory<IMessageHandlerSelector, AppServiceMetadata>> handlerSelectorFactories = null)
        {
            filterFactories = filterFactories
                              ?? new List<IExportFactory<IMessageProcessingFilter, MessageProcessingFilterMetadata>>();

            handlerSelectorFactories = handlerSelectorFactories
                                       ?? new List<IExportFactory<IMessageHandlerSelector, AppServiceMetadata>>
                                              {
                                                  new ExportFactory<IMessageHandlerSelector, AppServiceMetadata>(() => new DefaultMessageHandlerSelector(compositionContainer), new AppServiceMetadata())
                                              };

            return new DefaultMessageProcessor(Substitute.For<IAmbientServices>(), handlerSelectorFactories, filterFactories);
        }
    }
}