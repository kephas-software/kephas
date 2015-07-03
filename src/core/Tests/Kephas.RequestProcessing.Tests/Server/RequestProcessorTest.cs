// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestProcessorTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="RequestProcessor" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing.Tests.Server
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Composition.Mef;
    using Kephas.Composition.Mef.Conventions;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Configuration;
    using Kephas.Logging;
    using Kephas.RequestProcessing;
    using Kephas.RequestProcessing.Ping;
    using Kephas.RequestProcessing.Server;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    /// <summary>
    /// Test class for <see cref="RequestProcessor"/>
    /// </summary>
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class RequestProcessorTest
    {
        [TestMethod]
        public void Composition_success()
        {
            var logManager = Mock.Create<ILogManager>();
            var configManager = Mock.Create<IConfigurationManager>();
            var platformManager = Mock.Create<IPlatformManager>();

            var containerBuilder = new CompositionContainerBuilder(logManager, configManager, platformManager);
            containerBuilder.WithAssemblies(
                new[]
                    {
                        typeof(ILogger).Assembly,                           /* Kephas.Core */
                        typeof(IMefConventionBuilderProvider).Assembly,     /* Kephas.Composition.Mef */
                        typeof(IRequestProcessor).Assembly,                 /* Kephas.RequestProcessing */
                    });

            var container = containerBuilder.CreateContainer();
            var requestProcessor = container.GetExport<IRequestProcessor>();
            Assert.IsInstanceOfType(requestProcessor, typeof(RequestProcessor));

            var typedRequestprocessor = (RequestProcessor)requestProcessor;
            Assert.IsNotNull(typedRequestprocessor.Logger);
        }

        [TestMethod]
        public async Task ProcessAsync_result()
        {
            var compositionContainer = Mock.Create<ICompositionContainer>();
            var handler = Mock.Create<IRequestHandler>();
            var expectedResponse = Mock.Create<IResponse>();

            handler.Arrange(h => h.ProcessAsync(Arg.IsAny<IRequest>(), Arg.IsAny<IProcessingContext>(), Arg.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expectedResponse));
            compositionContainer.Arrange(c => c.GetExport(Arg.IsAny<Type>(), Arg.IsAny<string>()))
                .Returns(handler);
            var processor = this.CreateRequestProcessor(compositionContainer, handler);
            var result = await processor.ProcessAsync(Mock.Create<IRequest>(), default(CancellationToken));

            Assert.AreSame(expectedResponse, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ProcessAsync_exception()
        {
            var compositionContainer = Mock.Create<ICompositionContainer>();
            var handler = Mock.Create<IRequestHandler>();

            handler.Arrange(h => h.ProcessAsync(Arg.IsAny<IRequest>(), Arg.IsAny<IProcessingContext>(), Arg.IsAny<CancellationToken>()))
                .Throws(new InvalidOperationException());
            compositionContainer.Arrange(c => c.GetExport(Arg.IsAny<Type>(), Arg.IsAny<string>()))
                .Returns(handler);
            var processor = this.CreateRequestProcessor(compositionContainer, handler);
            var result = await processor.ProcessAsync(Mock.Create<IRequest>(), default(CancellationToken));
        }

        [TestMethod]
        public async Task ProcessAsync_disposed_handler()
        {
            var compositionContainer = Mock.Create<ICompositionContainer>();
            var handler = Mock.Create<IRequestHandler>();
            var expectedResponse = Mock.Create<IResponse>();

            handler.Arrange(h => h.ProcessAsync(Arg.IsAny<IRequest>(), Arg.IsAny<IProcessingContext>(), Arg.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expectedResponse));
            handler.Arrange(h => h.Dispose()).MustBeCalled();

            compositionContainer.Arrange(c => c.GetExport(Arg.IsAny<Type>(), Arg.IsAny<string>()))
                .Returns(handler);

            var processor = this.CreateRequestProcessor(compositionContainer, handler);
            var result = await processor.ProcessAsync(Mock.Create<IRequest>(), default(CancellationToken));

            Mock.Assert(handler);
        }

        [TestMethod]
        public async Task ProcessAsync_ordered_filter()
        {
            var compositionContainer = Mock.Create<ICompositionContainer>();
            var handler = Mock.Create<IRequestHandler>();
            var expectedResponse = Mock.Create<IResponse>();

            handler.Arrange(h => h.ProcessAsync(Arg.IsAny<IRequest>(), Arg.IsAny<IProcessingContext>(), Arg.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expectedResponse));
            compositionContainer.Arrange(c => c.GetExport(Arg.IsAny<Type>(), Arg.IsAny<string>()))
                .Returns(handler);

            var beforelist = new List<int>();
            var afterlist = new List<int>();
            var f1 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(1); return Empty<bool>.Task; },
                (c, t) => { afterlist.Add(1); return Empty<bool>.Task; },
                processingPriority: 2);
            var f2 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(2); return Empty<bool>.Task; },
                (c, t) => { afterlist.Add(2); return Empty<bool>.Task; },
                processingPriority: 1);

            var processor = this.CreateRequestProcessor(compositionContainer, handler, new[] { f1, f2 });
            var result = await processor.ProcessAsync(Mock.Create<IRequest>(), default(CancellationToken));

            Assert.AreEqual(2, beforelist.Count);
            Assert.AreEqual(2, beforelist[0]);
            Assert.AreEqual(1, beforelist[1]);

            Assert.AreEqual(2, afterlist.Count);
            Assert.AreEqual(1, afterlist[0]);
            Assert.AreEqual(2, afterlist[1]);
        }

        [TestMethod]
        public async Task ProcessAsync_matching_filter()
        {
            var compositionContainer = Mock.Create<ICompositionContainer>();
            var handler = Mock.Create<IRequestHandler>();
            var expectedResponse = Mock.Create<IResponse>();

            handler.Arrange(h => h.ProcessAsync(Arg.IsAny<IRequest>(), Arg.IsAny<IProcessingContext>(), Arg.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expectedResponse));
            compositionContainer.Arrange(c => c.GetExport(Arg.IsAny<Type>(), Arg.IsAny<string>()))
                .Returns(handler);

            var beforelist = new List<int>();
            var afterlist = new List<int>();
            var f1 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(1); return Empty<bool>.Task; },
                (c, t) => { afterlist.Add(1); return Empty<bool>.Task; },
                requestType: typeof(PingRequest));
            var f2 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(2); return Empty<bool>.Task; },
                (c, t) => { afterlist.Add(2); return Empty<bool>.Task; });

            var processor = this.CreateRequestProcessor(compositionContainer, handler, new[] { f1, f2 });
            var result = await processor.ProcessAsync(Mock.Create<IRequest>(), default(CancellationToken));

            Assert.AreEqual(1, beforelist.Count);
            Assert.AreEqual(2, beforelist[0]);

            Assert.AreEqual(1, afterlist.Count);
            Assert.AreEqual(2, afterlist[0]);
        }

        [TestMethod]
        public async Task ProcessAsync_exception_with_filter()
        {
            var compositionContainer = Mock.Create<ICompositionContainer>();
            var handler = Mock.Create<IRequestHandler>();

            handler.Arrange(h => h.ProcessAsync(Arg.IsAny<IRequest>(), Arg.IsAny<IProcessingContext>(), Arg.IsAny<CancellationToken>()))
                .Throws(new InvalidOperationException());
            compositionContainer.Arrange(c => c.GetExport(Arg.IsAny<Type>(), Arg.IsAny<string>()))
                .Returns(handler);

            var beforelist = new List<Exception>();
            var afterlist = new List<Exception>();
            var f1 = this.CreateFilterFactory(
                (c, t) => { beforelist.Add(c.Exception); return Empty<bool>.Task; },
                (c, t) => { afterlist.Add(c.Exception); return Empty<bool>.Task; });

            var processor = this.CreateRequestProcessor(compositionContainer, handler, new[] { f1 });
            InvalidOperationException thrownException = null;
            try
            {
                var result = await processor.ProcessAsync(Mock.Create<IRequest>(), default(CancellationToken));
            }
            catch (InvalidOperationException ex)
            {
                thrownException = ex;
            }

            Assert.IsInstanceOfType(thrownException, typeof(InvalidOperationException));

            Assert.AreEqual(1, beforelist.Count);
            Assert.IsNull(beforelist[0]);

            Assert.AreEqual(1, afterlist.Count);
            Assert.IsInstanceOfType(afterlist[0], typeof(InvalidOperationException));
        }

        private IExportFactory<IRequestProcessingFilter, RequestProcessingFilterMetadata> CreateFilterFactory(
            Func<IProcessingContext, CancellationToken, Task> beforeFunc = null,
            Func<IProcessingContext, CancellationToken, Task> afterFunc = null,
            Type requestType = null, 
            int processingPriority = 0, 
            Priority overridePriority = Priority.Normal)
        {
            requestType = requestType ?? typeof(IRequest);
            beforeFunc = beforeFunc ?? ((c, t) => Empty<bool>.Task);
            afterFunc = afterFunc ?? ((c, t) => Empty<bool>.Task);
            var filter = Mock.Create<IRequestProcessingFilter>();
            filter.Arrange(f => f.BeforeProcessAsync(Arg.IsAny<IProcessingContext>(), Arg.IsAny<CancellationToken>()))
                .Returns(beforeFunc);
            filter.Arrange(f => f.AfterProcessAsync(Arg.IsAny<IProcessingContext>(), Arg.IsAny<CancellationToken>()))
                .Returns(afterFunc);
            var factory =
                new ExportFactoryAdapter<IRequestProcessingFilter, RequestProcessingFilterMetadata>(
                    () => Tuple.Create(filter, (Action)(() => { })),
                    new RequestProcessingFilterMetadata(requestType, processingPriority, overridePriority));
            return factory;
        } 

        private RequestProcessor CreateRequestProcessor(ICompositionContainer compositionContainer, IRequestHandler handler, IList<IExportFactory<IRequestProcessingFilter, RequestProcessingFilterMetadata>> filterFactories = null)
        {
            filterFactories = filterFactories
                              ?? new List<IExportFactory<IRequestProcessingFilter, RequestProcessingFilterMetadata>>();
            var requestType = Mock.Create<IRequest>().GetType();
            var handlerFactories = new List<IExportFactory<IRequestHandler, RequestHandlerMetadata>>
                                       {
                                           new ExportFactoryAdapter<IRequestHandler, RequestHandlerMetadata>(() => Tuple.Create(handler, (Action)(() => {})), new RequestHandlerMetadata(requestType))
                                       };
            return new RequestProcessor(compositionContainer, handlerFactories, filterFactories);
        }
    }
}