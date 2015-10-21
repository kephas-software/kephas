// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestHandlerBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="RequestHandlerBase" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Kephas.Extensions;
using Kephas.RequestProcessing.Ping;
using Kephas.RequestProcessing.Server;
using NUnit.Framework;
using Telerik.JustMock;

namespace Kephas.RequestProcessing.Tests.Server
{
    /// <summary>
    /// Test class for <see cref="RequestHandlerBase"/>
    /// </summary>
    [TestFixture]
    public class RequestHandlerBaseTest
    {
        /*[Test]
        public void ProcessAsync()
        {
            var request = Mock.Create<IRequest>();
            var context = Mock.Create<IProcessingContext>();
            var token = new CancellationToken();
            var instance = Mock.Create<RequestHandlerBase<IRequest, IResponse>>();
            var before = DateTimeOffset.Now;
            var response = instance.ProcessAsync(request, context, token);
            var result = response.Result;
            var after = DateTimeOffset.Now;
            Assert.LessOrEqual(before, result);
        }*/

        [Test]
        public async Task ProcessAsync_request()
        {
            var instance = Mock.Create<RequestHandlerBase<IRequest, IResponse>>();
            await instance.ProcessAsync().ConfigureAwait(false);
        }
    }
}
