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
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Configuration.Providers;
    using Kephas.Diagnostics.Logging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Messages;
    using Kephas.Messaging.Redis.Routing;
    using Kephas.Redis.Configuration;
    using Kephas.Services;
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
            var masterId = $"Master-{Guid.NewGuid():N}";
            var masterInstanceId = $"{masterId}-{Guid.NewGuid():N}";
            var masterContainer = this.CreateContainer(
                new AmbientServices()
                    .WithStaticAppRuntime(appId: masterId, appInstanceId: masterInstanceId),
                parts: new[] { typeof(RedisSettingsProvider) });
            var masterRuntime = masterContainer.GetExport<IAppRuntime>();

            var slaveId = $"Slave-{Guid.NewGuid():N}";
            var slaveInstanceId = $"{slaveId}-{Guid.NewGuid():N}";
            var slaveContainer = this.CreateContainer(
                new AmbientServices()
                    .WithStaticAppRuntime(appId: slaveId, appInstanceId: slaveInstanceId),
                parts: new[] { typeof(RedisSettingsProvider) });
            var slaveRuntime = slaveContainer.GetExport<IAppRuntime>();

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

        [Test]
        public async Task DispatchAsync_pair_request_response_multiple()
        {
            var sbMaster = new StringBuilder();
            var masterId = $"Master-{Guid.NewGuid():N}";
            var masterInstanceId = $"{masterId}-{Guid.NewGuid():N}";
            var masterContainer = this.CreateContainer(
                new AmbientServices()
                    .WithDebugLogManager(sbMaster)
                    .WithStaticAppRuntime(appId: masterId, appInstanceId: masterInstanceId, assemblyFilter: this.IsNotTestAssembly),
                parts: new[] { typeof(RedisSettingsProvider) });
            var masterRuntime = masterContainer.GetExport<IAppRuntime>();

            var sbSlave = new StringBuilder();
            var slaveId = $"Slave-{Guid.NewGuid():N}";
            var slaveInstanceId = $"{slaveId}-{Guid.NewGuid():N}";
            var slaveContainer = this.CreateContainer(
                new AmbientServices()
                    .WithDebugLogManager(sbSlave)
                    .WithStaticAppRuntime(appId: slaveId, appInstanceId: slaveInstanceId, assemblyFilter: this.IsNotTestAssembly),
                parts: new[] { typeof(RedisSettingsProvider) });
            var slaveRuntime = slaveContainer.GetExport<IAppRuntime>();

            await this.InitializeAppAsync(masterContainer);
            await this.InitializeAppAsync(slaveContainer);

            var masterMessageBroker = masterContainer.GetExport<IMessageBroker>();

            try
            {
                var pingBackTasks = new int[3]
                    .Select(i =>
                        masterMessageBroker.DispatchAsync(
                            new PingMessage(),
                            ctx => ctx.To((IEndpoint)new Endpoint(appInstanceId: slaveRuntime.GetAppInstanceId()))
                                      .Timeout(TimeSpan.FromSeconds(5))));

                var pingBacks = await Task.WhenAll(pingBackTasks);
                CollectionAssert.AllItemsAreInstancesOfType(pingBacks, typeof(PingBackMessage));
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
            var masterId = $"Master-{Guid.NewGuid():N}";
            var masterInstanceId = $"{masterId}-{Guid.NewGuid():N}";
            var masterContainer = this.CreateContainer(
                new AmbientServices()
                    .WithStaticAppRuntime(appId: masterId, appInstanceId: masterInstanceId),
                parts: new[] { typeof(RedisSettingsProvider) });
            var masterRuntime = masterContainer.GetExport<IAppRuntime>();

            var slaveId = $"Slave-{Guid.NewGuid():N}";
            var slaveInstanceId = $"{slaveId}-{Guid.NewGuid():N}";
            var slaveContainer = this.CreateContainer(
                new AmbientServices()
                    .WithStaticAppRuntime(appId: slaveId, appInstanceId: slaveInstanceId),
                parts: new[] { typeof(RedisSettingsProvider) });
            var slaveRuntime = slaveContainer.GetExport<IAppRuntime>();

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

        [Test]
        public async Task DispatchAsync_pair_request_response_redis_unavailable_multiple()
        {
            var masterId = $"Master-{Guid.NewGuid():N}";
            var masterInstanceId = $"{masterId}-{Guid.NewGuid():N}";
            var masterContainer = this.CreateContainer(
                new AmbientServices()
                    .WithStaticAppRuntime(appId: masterId, appInstanceId: masterInstanceId),
                parts: new[] { typeof(RedisSettingsProvider) });
            var masterRuntime = masterContainer.GetExport<IAppRuntime>();

            var slaveId = $"Slave-{Guid.NewGuid():N}";
            var slaveInstanceId = $"{slaveId}-{Guid.NewGuid():N}";
            var slaveContainer = this.CreateContainer(
                new AmbientServices()
                    .WithStaticAppRuntime(appId: slaveId, appInstanceId: slaveInstanceId),
                parts: new[] { typeof(RedisSettingsProvider) });
            var slaveRuntime = slaveContainer.GetExport<IAppRuntime>();

            await this.InitializeAppAsync(masterContainer);
            await this.InitializeAppAsync(slaveContainer);

            var masterMessageBroker = masterContainer.GetExport<IMessageBroker>();

            try
            {
                var pingBackTasks = new int[3]
                    .Select(i =>
                        masterMessageBroker.DispatchAsync(
                            new PingMessage(),
                            ctx => ctx.To((IEndpoint)new Endpoint(appInstanceId: slaveRuntime.GetAppInstanceId()))
                                      .Timeout(TimeSpan.FromSeconds(5))));

                var pingBacks = await Task.WhenAll(pingBackTasks);
                CollectionAssert.AllItemsAreInstancesOfType(pingBacks, typeof(PingBackMessage));
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
            public object? GetSettings(Type settingsType, IContext? context)
            {
                if (settingsType == typeof(RedisClientSettings))
                {
                    return new RedisClientSettings
                    {
                        Namespace = "unit-test",
                        ConnectionString = "localhost",
                    };
                }

                return null;
            }

            public async Task UpdateSettingsAsync(object settings, IContext? context, CancellationToken cancellationToken = default)
            {
                await Task.Yield();
            }
        }
    }
}
