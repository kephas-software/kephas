// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisAppMessageRouterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the redis application message router test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Redis.Tests.Routing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Configuration.Providers;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Messages;
    using Kephas.Messaging.Redis.Routing;
    using Kephas.Redis.Configuration;
    using NUnit.Framework;
    using AppContext = Kephas.Application.AppContext;

    [TestFixture]
    public class RedisAppMessageRouterTest : RedisMessagingTestBase
    {
        [Test]
        public void Composition()
        {
            var container = this.CreateContainer();
            var router = container.GetExports<IMessageRouter>().OfType<RedisAppMessageRouter>().SingleOrDefault();

            Assert.IsNotNull(router);
        }

        [Test]
        public async Task DispatchAsync_pair_request_response()
        {
            var masterContainer = this.CreateContainer(parts: new[] { typeof(RedisSettingsProvider) });
            var masterRuntime = masterContainer.GetExport<IAppRuntime>();
            var masterId = $"Master-{Guid.NewGuid():N}";
            masterRuntime[AppRuntimeBase.AppIdKey] = masterId;
            masterRuntime[AppRuntimeBase.AppInstanceIdKey] = $"{masterId}-1";

            var slaveContainer = this.CreateContainer(parts: new[] { typeof(RedisSettingsProvider) });
            var slaveRuntime = slaveContainer.GetExport<IAppRuntime>();
            var slaveId = $"Slave-{Guid.NewGuid():N}";
            slaveRuntime[AppRuntimeBase.AppIdKey] = slaveId;
            slaveRuntime[AppRuntimeBase.AppInstanceIdKey] = $"{slaveId}-1";

            await this.InitializeAppAsync(masterContainer);
            await this.InitializeAppAsync(slaveContainer);

            var masterMessageBroker = masterContainer.GetExport<IMessageBroker>();

            try
            {
                var pingBack = await masterMessageBroker.DispatchAsync(
                    new PingMessage(),
                    ctx => ctx.To((IEndpoint)new Endpoint(appInstanceId: slaveRuntime.GetAppInstanceId()))
                              .Timeout(TimeSpan.FromSeconds(500)));
                Assert.IsInstanceOf<PingBackMessage>(pingBack);
            }
            finally
            {
                await this.FinalizeAppAsync(slaveContainer);
                await this.FinalizeAppAsync(masterContainer);
            }
        }

        [Test]
        public async Task DispatchAsync_pair_request_response_redis_unavailable()
        {
            var masterContainer = this.CreateContainer();
            var masterRuntime = masterContainer.GetExport<IAppRuntime>();
            masterRuntime[AppRuntimeBase.AppIdKey] = "Master";
            masterRuntime[AppRuntimeBase.AppInstanceIdKey] = "Master-1";

            var slaveContainer = this.CreateContainer();
            var slaveRuntime = slaveContainer.GetExport<IAppRuntime>();
            slaveRuntime[AppRuntimeBase.AppIdKey] = "Slave";
            slaveRuntime[AppRuntimeBase.AppInstanceIdKey] = "Slave-1";

            await this.InitializeAppAsync(masterContainer);
            await this.InitializeAppAsync(slaveContainer);

            var masterMessageBroker = masterContainer.GetExport<IMessageBroker>();

            try
            {
                var pingBack = await masterMessageBroker.DispatchAsync(
                    new PingMessage(),
                    ctx => ctx.To((IEndpoint)new Endpoint(appInstanceId: slaveRuntime.GetAppInstanceId()))
                              .Timeout(TimeSpan.FromSeconds(5)));
                Assert.IsInstanceOf<PingBackMessage>(pingBack);
            }
            finally
            {
                await this.FinalizeAppAsync(slaveContainer);
                await this.FinalizeAppAsync(masterContainer);
            }
        }

        private async Task InitializeAppAsync(ICompositionContext container)
        {
            var appManager = container.GetExport<IAppManager>();
            await appManager.InitializeAppAsync(
                new AppContext(container.GetExport<IAmbientServices>(), container.GetExport<IAppRuntime>()));
        }

        private async Task FinalizeAppAsync(ICompositionContext container)
        {
            var appManager = container.GetExport<IAppManager>();
            await appManager.FinalizeAppAsync(
                new AppContext(container.GetExport<IAmbientServices>(), container.GetExport<IAppRuntime>()));
        }

        public class RedisSettingsProvider : ISettingsProvider
        {
            public object GetSettings(Type settingsType)
            {
                if (settingsType == typeof(RedisClientSettings))
                {
                    return new RedisClientSettings
                    {
                        Namespace = "unit-test",
                        ConnectionString = "kephas-test.redis.cache.windows.net:6380,password=9PuI+WoNAfRaVx4BgCzaAvTmZCGJN3AmM35kg13vdoY=,ssl=True,abortConnect=False",
                    };
                }

                return null;
            }
        }
    }
}
