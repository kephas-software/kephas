// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisConnectionFactoryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Redis.Tests.Connectivity;

using Kephas.Application;
using Kephas.Cryptography;
using Kephas.Interaction;
using Kephas.Logging;
using Kephas.Redis.Connectivity;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class RedisConnectionFactoryTest
{
    [TestCase("redis://redis:6789/?password=123&ssl=True", "redis:6789,password=123,ssl=True")]
    [TestCase("redis://redis:6789/", "redis:6789")]
    public void GetConnectionString(string uri, string connectionString)
    {
        var actual = new TestRedisConnectionFactory().PublicGetConnectionString(new Uri(uri));

        Assert.AreEqual(connectionString, actual);
    }

    public class TestRedisConnectionFactory : RedisConnectionFactory
    {
        public TestRedisConnectionFactory(
            IAppRuntime? appRuntime = null,
            IEventHub? eventHub = null,
            IEncryptionService? encryptionService = null,
            ILogManager? logManager = null)
            : base(
                appRuntime ?? Substitute.For<IAppRuntime>(),
                eventHub ?? Substitute.For<IEventHub>(),
                encryptionService ?? Substitute.For<IEncryptionService>(),
                logManager)
        {
        }

        public string PublicGetConnectionString(Uri host) => this.GetConnectionString(host);
    }
}