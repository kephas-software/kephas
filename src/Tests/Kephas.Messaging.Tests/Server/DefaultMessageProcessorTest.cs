﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageProcessorTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="DefaultMessageProcessor" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Server
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
    using Kephas.Messaging.Ping;
    using Kephas.Messaging.Server;
    using Kephas.Messaging.Server.Composition;
    using Kephas.Services;
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
        public async Task ProcessAsync_result()
        {
            var compositionContainer = Substitute.For<ICompositionContext>();
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            handler.ProcessAsync(Arg.Any<IMessage>(), Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));
            compositionContainer.TryGetExport(Arg.Any<Type>(), Arg.Any<string>())
                .Returns(new ExportFactory<IMessageHandler>(() => handler));
            var processor = this.CreateRequestProcessor(compositionContainer);
            var result = await processor.ProcessAsync(Substitute.For<IMessage>(), null, default(CancellationToken));

            Assert.AreSame(expectedResponse, result);
        }

        [Test]
        public void ProcessAsync_missing_handler_exception()
        {
            var compositionContainer = Substitute.For<ICompositionContext>();
            compositionContainer.TryGetExport(Arg.Any<Type>(), Arg.Any<string>())
                .Returns(null);
            var processor = this.CreateRequestProcessor(compositionContainer);
            Assert.That(() => processor.ProcessAsync(Substitute.For<IMessage>(), null, default(CancellationToken)), Throws.InstanceOf<MissingHandlerException>());
        }

        [Test]
        public void ProcessAsync_exception()
        {
            var compositionContainer = Substitute.For<ICompositionContext>();
            var handler = Substitute.For<IMessageHandler>();

            handler.ProcessAsync(Arg.Any<IMessage>(), Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Throws(new InvalidOperationException());
            compositionContainer.TryGetExport(Arg.Any<Type>(), Arg.Any<string>())
                .Returns(new ExportFactory<IMessageHandler>(() => handler));
            var processor = this.CreateRequestProcessor(compositionContainer);
            Assert.That(() => processor.ProcessAsync(Substitute.For<IMessage>(), null, default(CancellationToken)), Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public async Task ProcessAsync_disposed_handler()
        {
            var compositionContainer = Substitute.For<ICompositionContext>();
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            handler.ProcessAsync(Arg.Any<IMessage>(), Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));

            compositionContainer.TryGetExport(Arg.Any<Type>(), Arg.Any<string>())
                .Returns(new ExportFactory<IMessageHandler>(() => handler));

            var processor = this.CreateRequestProcessor(compositionContainer);
            var result = await processor.ProcessAsync(Substitute.For<IMessage>(), null, default(CancellationToken));

            Assert.LessOrEqual(1, handler.ReceivedCalls().Count());
        }

        [Test]
        public async Task ProcessAsync_ordered_filter()
        {
            var compositionContainer = Substitute.For<ICompositionContext>();
            var handler = Substitute.For<IMessageHandler>();
            var expectedResponse = Substitute.For<IMessage>();

            handler.ProcessAsync(Arg.Any<IMessage>(), Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));
            compositionContainer.TryGetExport(Arg.Any<Type>(), Arg.Any<string>())
                .Returns(new ExportFactory<IMessageHandler>(() => handler));

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
            var result = await processor.ProcessAsync(Substitute.For<IMessage>(), null, default(CancellationToken));

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

            handler.ProcessAsync(Arg.Any<IMessage>(), Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));
            compositionContainer.TryGetExport(Arg.Any<Type>(), Arg.Any<string>())
                .Returns(new ExportFactory<IMessageHandler>(() => handler));

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
            var result = await processor.ProcessAsync(Substitute.For<IMessage>(), null, default(CancellationToken));

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

            handler.ProcessAsync(Arg.Any<IMessage>(), Arg.Any<IMessageProcessingContext>(), Arg.Any<CancellationToken>())
                .Throws(new InvalidOperationException());
            compositionContainer.TryGetExport(Arg.Any<Type>(), Arg.Any<string>())
                .Returns(new ExportFactory<IMessageHandler>(() => handler));

            var beforelist = new List<Exception>();
            var afterlist = new List<Exception>();
            var f1 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(c.Exception); return TaskHelper.CompletedTask; }, 
                (c, t) => { afterlist.Add(c.Exception); return TaskHelper.CompletedTask; });

            var processor = this.CreateRequestProcessor(compositionContainer, new[] { f1 });
            InvalidOperationException thrownException = null;
            try
            {
                var result = await processor.ProcessAsync(Substitute.For<IMessage>(), null, default(CancellationToken));
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

        private DefaultMessageProcessor CreateRequestProcessor(ICompositionContext compositionContainer, IList<IExportFactory<IMessageProcessingFilter, MessageProcessingFilterMetadata>> filterFactories = null)
        {
            filterFactories = filterFactories
                              ?? new List<IExportFactory<IMessageProcessingFilter, MessageProcessingFilterMetadata>>();
            return new DefaultMessageProcessor(Substitute.For<IAmbientServices>(), compositionContainer, filterFactories);
        }
    }
}