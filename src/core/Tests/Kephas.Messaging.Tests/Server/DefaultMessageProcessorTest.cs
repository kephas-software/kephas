// --------------------------------------------------------------------------------------------------------------------
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
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Composition.Mef;
    using Kephas.Messaging.Ping;
    using Kephas.Messaging.Server;
    using Kephas.Messaging.Server.Composition;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    using NUnit.Framework;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    /// <summary>
    /// Test class for <see cref="DefaultMessageProcessor"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class DefaultMessageProcessorTest : CompositionTestBase
    {
        public override ICompositionContext CreateContainer(IEnumerable<Assembly> assemblies)
        {
            var assemblyList = new List<Assembly>(assemblies ?? new Assembly[0]);
            assemblyList.Add(typeof(IMessageProcessor).Assembly); /* Kephas.Messaging */
            return base.CreateContainer(assemblyList);
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
            var compositionContainer = Mock.Create<ICompositionContext>();
            var handler = Mock.Create<IMessageHandler>();
            var expectedResponse = Mock.Create<IMessage>();

            handler.Arrange(h => h.ProcessAsync(Arg.IsAny<IMessage>(), Arg.IsAny<IMessageProcessingContext>(), Arg.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expectedResponse));
            compositionContainer.Arrange(c => c.GetExport(Arg.IsAny<Type>(), Arg.IsAny<string>()))
                .Returns(handler);
            var processor = this.CreateRequestProcessor(compositionContainer);
            var result = await processor.ProcessAsync(Mock.Create<IMessage>(), null, default(CancellationToken));

            Assert.AreSame(expectedResponse, result);
        }

        [Test]
        public async Task ProcessAsync_exception()
        {
            var compositionContainer = Mock.Create<ICompositionContext>();
            var handler = Mock.Create<IMessageHandler>();

            handler.Arrange(h => h.ProcessAsync(Arg.IsAny<IMessage>(), Arg.IsAny<IMessageProcessingContext>(), Arg.IsAny<CancellationToken>()))
                .Throws(new InvalidOperationException());
            compositionContainer.Arrange(c => c.GetExport(Arg.IsAny<Type>(), Arg.IsAny<string>()))
                .Returns(handler);
            var processor = this.CreateRequestProcessor(compositionContainer);
            Assert.Throws<InvalidOperationException>(() => processor.ProcessAsync(Mock.Create<IMessage>(), null, default(CancellationToken)));
        }

        [Test]
        public async Task ProcessAsync_disposed_handler()
        {
            var compositionContainer = Mock.Create<ICompositionContext>();
            var handler = Mock.Create<IMessageHandler>();
            var expectedResponse = Mock.Create<IMessage>();

            handler.Arrange(h => h.ProcessAsync(Arg.IsAny<IMessage>(), Arg.IsAny<IMessageProcessingContext>(), Arg.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expectedResponse));
            handler.Arrange(h => h.Dispose()).MustBeCalled();

            compositionContainer.Arrange(c => c.GetExport(Arg.IsAny<Type>(), Arg.IsAny<string>()))
                .Returns(handler);

            var processor = this.CreateRequestProcessor(compositionContainer);
            var result = await processor.ProcessAsync(Mock.Create<IMessage>(), null, default(CancellationToken));

            Mock.Assert(handler);
        }

        [Test]
        public async Task ProcessAsync_ordered_filter()
        {
            var compositionContainer = Mock.Create<ICompositionContext>();
            var handler = Mock.Create<IMessageHandler>();
            var expectedResponse = Mock.Create<IMessage>();

            handler.Arrange(h => h.ProcessAsync(Arg.IsAny<IMessage>(), Arg.IsAny<IMessageProcessingContext>(), Arg.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expectedResponse));
            compositionContainer.Arrange(c => c.GetExport(Arg.IsAny<Type>(), Arg.IsAny<string>()))
                .Returns(handler);

            var beforelist = new List<int>();
            var afterlist = new List<int>();
            var f1 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(1); return CompletedTask.Value; }, 
                (c, t) => { afterlist.Add(1); return CompletedTask.Value; }, 
                processingPriority: 2);
            var f2 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(2); return CompletedTask.Value; }, 
                (c, t) => { afterlist.Add(2); return CompletedTask.Value; }, 
                processingPriority: 1);

            var processor = this.CreateRequestProcessor(compositionContainer, new[] { f1, f2 });
            var result = await processor.ProcessAsync(Mock.Create<IMessage>(), null, default(CancellationToken));

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
            var compositionContainer = Mock.Create<ICompositionContext>();
            var handler = Mock.Create<IMessageHandler>();
            var expectedResponse = Mock.Create<IMessage>();

            handler.Arrange(h => h.ProcessAsync(Arg.IsAny<IMessage>(), Arg.IsAny<IMessageProcessingContext>(), Arg.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expectedResponse));
            compositionContainer.Arrange(c => c.GetExport(Arg.IsAny<Type>(), Arg.IsAny<string>()))
                .Returns(handler);

            var beforelist = new List<int>();
            var afterlist = new List<int>();
            var f1 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(1); return CompletedTask.Value; }, 
                (c, t) => { afterlist.Add(1); return CompletedTask.Value; }, 
                requestType: typeof(PingMessage));
            var f2 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(2); return CompletedTask.Value; }, 
                (c, t) => { afterlist.Add(2); return CompletedTask.Value; });

            var processor = this.CreateRequestProcessor(compositionContainer, new[] { f1, f2 });
            var result = await processor.ProcessAsync(Mock.Create<IMessage>(), null, default(CancellationToken));

            Assert.AreEqual(1, beforelist.Count);
            Assert.AreEqual(2, beforelist[0]);

            Assert.AreEqual(1, afterlist.Count);
            Assert.AreEqual(2, afterlist[0]);
        }

        [Test]
        public async Task ProcessAsync_exception_with_filter()
        {
            var compositionContainer = Mock.Create<ICompositionContext>();
            var handler = Mock.Create<IMessageHandler>();

            handler.Arrange(h => h.ProcessAsync(Arg.IsAny<IMessage>(), Arg.IsAny<IMessageProcessingContext>(), Arg.IsAny<CancellationToken>()))
                .Throws(new InvalidOperationException());
            compositionContainer.Arrange(c => c.GetExport(Arg.IsAny<Type>(), Arg.IsAny<string>()))
                .Returns(handler);

            var beforelist = new List<Exception>();
            var afterlist = new List<Exception>();
            var f1 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(c.Exception); return CompletedTask.Value; }, 
                (c, t) => { afterlist.Add(c.Exception); return CompletedTask.Value; });

            var processor = this.CreateRequestProcessor(compositionContainer, new[] { f1 });
            InvalidOperationException thrownException = null;
            try
            {
                var result = await processor.ProcessAsync(Mock.Create<IMessage>(), null, default(CancellationToken));
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
            beforeFunc = beforeFunc ?? ((c, t) => CompletedTask.Value);
            afterFunc = afterFunc ?? ((c, t) => CompletedTask.Value);
            var filter = Mock.Create<IMessageProcessingFilter>();
            filter.Arrange(f => f.BeforeProcessAsync(Arg.IsAny<IMessageProcessingContext>(), Arg.IsAny<CancellationToken>()))
                .Returns(beforeFunc);
            filter.Arrange(f => f.AfterProcessAsync(Arg.IsAny<IMessageProcessingContext>(), Arg.IsAny<CancellationToken>()))
                .Returns(afterFunc);
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
            return new DefaultMessageProcessor(compositionContainer, filterFactories);
        }
    }
}