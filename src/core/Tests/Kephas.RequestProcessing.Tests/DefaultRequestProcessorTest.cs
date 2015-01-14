// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRequestProcessorTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="DefaultRequestProcessor" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.RequestProcessing;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    /// <summary>
    /// Test class for <see cref="DefaultRequestProcessor"/>
    /// </summary>
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class DefaultRequestProcessorTest
    {
        [TestMethod]
        public async Task ProcessAsync_result()
        {
            var compositionContainer = Mock.Create<ICompositionContainer>();
            var handler = Mock.Create<IRequestHandler>();
            var expectedResponse = Mock.Create<IResponse>();

            handler.Arrange(h => h.ProcessAsync(Arg.IsAny<IRequest>()))
                .Returns(Task.FromResult(expectedResponse));
            compositionContainer.Arrange(c => c.GetExport(Arg.IsAny<Type>(), Arg.IsAny<string>()))
                .Returns(handler);
            var processor = new DefaultRequestProcessor(compositionContainer);
            var result = await processor.ProcessAsync(Mock.Create<IRequest>());

            Assert.AreSame(expectedResponse, result);
        }

        [TestMethod]
        public async Task ProcessAsync_disposed_handler()
        {
            var compositionContainer = Mock.Create<ICompositionContainer>();
            var handler = Mock.Create<IRequestHandler>();
            var expectedResponse = Mock.Create<IResponse>();

            handler.Arrange(h => h.ProcessAsync(Arg.IsAny<IRequest>()))
                .Returns(Task.FromResult(expectedResponse));
            handler.Arrange(h => h.Dispose()).MustBeCalled();

            compositionContainer.Arrange(c => c.GetExport(Arg.IsAny<Type>(), Arg.IsAny<string>()))
                .Returns(handler);
            var processor = new DefaultRequestProcessor(compositionContainer);
            var result = await processor.ProcessAsync(Mock.Create<IRequest>());

            Mock.Assert(handler);
        }
    }
}