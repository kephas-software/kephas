// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessingContextTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="ProcessingContext" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;
using Kephas.RequestProcessing.Ping;
using Kephas.RequestProcessing.Server;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.AutoMock.Ninject.Activation;

namespace Kephas.RequestProcessing.Tests.Server
{
    /// <summary>
    /// Test class for <see cref="ProcessingContext"/>
    /// </summary>
    [TestFixture]
    public class ProcessingContextTest
    {
        [Test]
        public void ProcessingContext_constructor()
        {
            var request = Mock.Create<IRequest>();
            var handler = Mock.Create<IRequestHandler>();
            var context = new ProcessingContext(request, handler);
            Assert.AreEqual(request, context.Request);
            Assert.AreEqual(handler, context.Handler);
        }

        [Test]
        public void Response_get_set()
        {
            var response = Mock.Create<IResponse>();
            var request = Mock.Create<IRequest>();
            var handler = Mock.Create<IRequestHandler>();
            var context = new ProcessingContext(request, handler) {Response = response};
            Assert.AreEqual(context.Response, response);
        }

        [Test]
        public void Exception_get_set()
        {
            var exception = Mock.Create<Exception>();
            var request = Mock.Create<IRequest>();
            var handler = Mock.Create<IRequestHandler>();
            var context = new ProcessingContext(request, handler) {Exception = exception};
            Assert.AreEqual(context.Exception, exception);
        }
    }
}