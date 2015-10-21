// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingResponsehandlerTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="PingResponsehandler" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Kephas.RequestProcessing.Ping;
using Kephas.RequestProcessing.Server;
using NUnit.Framework;
using Telerik.JustMock;

namespace Kephas.RequestProcessing.Tests.Ping
{
    /// <summary>
    /// Test class for <see cref="PingRequestHandler"/>
    /// </summary>
    [TestFixture]
    public class PingResponsehandlerTest
    {
        [Test]
        public void ProcessAsync()
        {
            var instance = new PingRequestHandler();
            var request = Mock.Create<PingRequest>();
            var context = Mock.Create<ProcessingContext>();
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var before = DateTimeOffset.Now;
            var result = instance.ProcessAsync(request, context, token).Result.ServerTime;
            var after = DateTimeOffset.Now;
            Assert.LessOrEqual(before, result);
            Assert.GreaterOrEqual(after, result);
        }
    }
}