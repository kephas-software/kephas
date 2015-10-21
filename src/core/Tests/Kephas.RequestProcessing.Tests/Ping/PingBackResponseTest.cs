// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingBackResponseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="RuntimeDynamicType" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Kephas.RequestProcessing.Ping;
using NUnit.Framework;

namespace Kephas.RequestProcessing.Tests.Ping
{
    /// <summary>
    /// Test class for <see cref="PingBackResponse"/>
    /// </summary>
    [TestFixture]
    public class PingBackResponseTest
    {
        [Test]
        public void ServerTime_get_set()
        {
            var instance = new PingBackResponse();
            var dateTimeOffset = DateTimeOffset.Now;
            instance.ServerTime = dateTimeOffset;
            var response = instance.ServerTime;
            Assert.AreEqual(response, dateTimeOffset);
        }
    }
}