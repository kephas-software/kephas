// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipesAppMessageRouterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the pipes application message router test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Pipes.Tests.Routing
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Commands;
    using Kephas.Configuration.Providers;
    using Kephas.Diagnostics.Logging;
    using Kephas.Injection;
    using Kephas.Interaction;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Messages;
    using Kephas.Messaging.Pipes.Configuration;
    using Kephas.Messaging.Pipes.Routing;
    using Kephas.Orchestration;
    using Kephas.Orchestration.Interaction;
    using Kephas.Services;
    using NUnit.Framework;

    using AppContext = Kephas.Application.AppContext;

    [TestFixture]
    public class PipesAppMessageRouterTest : PipesMessagingTestBase
    {
        [Test]
        public void Injection()
        {
            var container = this.CreateInjector();
            var router = container.ResolveMany<IMessageRouter>().OfType<PipesAppMessageRouter>().SingleOrDefault();

            Assert.IsNotNull(router);
        }

        [Test]
        public async Task DispatchAsync_pair_request_response()
        {
            var masterId = $"Master-{Guid.NewGuid():N}";
            var masterInstanceId = $"{masterId}-{Guid.NewGuid():N}";
            var masterContainer = this.CreateInjector(
                this.CreateAmbientServices(),
                parts: new[] { typeof(PipesSettingsProvider) },
                appRuntime: new StaticAppRuntime(
                    isRoot: true,
                    appId: masterId,
                    appInstanceId: masterInstanceId));
            var masterRuntime = masterContainer.Resolve<IAppRuntime>();

            var slaveArgs = new AppArgs { [AppArgs.RootArgName] = masterInstanceId };
            var slaveId = $"Slave-{Guid.NewGuid():N}";
            var slaveInstanceId = $"{slaveId}-{Guid.NewGuid():N}";
            var slaveContainer = this.CreateInjector(
                this.CreateAmbientServices()
                    .RegisterAppArgs(slaveArgs),
                parts: new[] { typeof(PipesSettingsProvider) },
                appRuntime: new StaticAppRuntime(
                    isRoot: false,
                    appId: slaveId,
                    appInstanceId: slaveInstanceId));
            var slaveRuntime = slaveContainer.Resolve<IAppRuntime>();

            await this.InitializeAppAsync(masterContainer, slaveAppInfo: new RuntimeAppInfo { AppId = slaveId, AppInstanceId = slaveInstanceId });
            await this.InitializeAppAsync(slaveContainer, slaveArgs);

            var masterMessageBroker = masterContainer.Resolve<IMessageBroker>();

            try
            {
                var pingBack = await masterMessageBroker.DispatchAsync(
                    new PingMessage(),
                    ctx => ctx.To((IEndpoint)new Endpoint(appInstanceId: slaveRuntime.GetAppInstanceId()))
                              .Timeout(TimeSpan.FromSeconds(2)));
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
            var masterContainer = this.CreateInjector(
                this.CreateAmbientServices()
                    .WithDebugLogManager(sbMaster),
                parts: new[] { typeof(PipesSettingsProvider) },
                appRuntime: new StaticAppRuntime(
                    isRoot: true,
                    appId: masterId,
                    appInstanceId: masterInstanceId,
                    defaultAssemblyFilter: this.IsNotTestAssembly));
            var masterRuntime = masterContainer.Resolve<IAppRuntime>();

            var slaveArgs = new AppArgs { [AppArgs.RootArgName] = masterInstanceId };
            var sbSlave = new StringBuilder();
            var slaveId = $"Slave-{Guid.NewGuid():N}";
            var slaveInstanceId = $"{slaveId}-{Guid.NewGuid():N}";
            var slaveContainer = this.CreateInjector(
                this.CreateAmbientServices()
                    .WithDebugLogManager(sbSlave)
                    .RegisterAppArgs(slaveArgs),
                parts: new[] { typeof(PipesSettingsProvider) },
                appRuntime: new StaticAppRuntime(
                    isRoot: false,
                    appId: slaveId,
                    appInstanceId: slaveInstanceId,
                    defaultAssemblyFilter: this.IsNotTestAssembly));
            var slaveRuntime = slaveContainer.Resolve<IAppRuntime>();

            await this.InitializeAppAsync(masterContainer, slaveAppInfo: new RuntimeAppInfo { AppId = slaveId, AppInstanceId = slaveInstanceId });
            await this.InitializeAppAsync(slaveContainer, slaveArgs);

            var masterMessageBroker = masterContainer.Resolve<IMessageBroker>();

            try
            {
                var pingBackTasks = new int[3]
                    .Select(i =>
                        masterMessageBroker.DispatchAsync(
                            new PingMessage(),
                            ctx => ctx.To((IEndpoint)new Endpoint(appInstanceId: slaveRuntime.GetAppInstanceId()))
                                      .Timeout(TimeSpan.FromSeconds(2))));

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
        public async Task DispatchAsync_pair_request_response_pipes_unavailable()
        {
            var masterId = $"Master-{Guid.NewGuid():N}";
            var masterInstanceId = $"{masterId}-{Guid.NewGuid():N}";
            var masterContainer = this.CreateInjector(
                this.CreateAmbientServices(),
                parts: new[] { typeof(PipesSettingsProvider) },
                appRuntime: new StaticAppRuntime(
                    isRoot: true,
                    appId: masterId,
                    appInstanceId: masterInstanceId));
            var masterRuntime = masterContainer.Resolve<IAppRuntime>();

            var slaveArgs = new AppArgs { [AppArgs.RootArgName] = masterInstanceId };
            var slaveId = $"Slave-{Guid.NewGuid():N}";
            var slaveInstanceId = $"{slaveId}-{Guid.NewGuid():N}";
            var slaveContainer = this.CreateInjector(
                this.CreateAmbientServices()
                    .RegisterAppArgs(slaveArgs),
                parts: new[] { typeof(PipesSettingsProvider) },
                appRuntime: new StaticAppRuntime(
                    isRoot: false,
                    appId: slaveId,
                    appInstanceId: slaveInstanceId));
            var slaveRuntime = slaveContainer.Resolve<IAppRuntime>();

            await this.InitializeAppAsync(masterContainer, slaveAppInfo: new RuntimeAppInfo { AppId = slaveId, AppInstanceId = slaveInstanceId });
            await this.InitializeAppAsync(slaveContainer, slaveArgs);

            var masterMessageBroker = masterContainer.Resolve<IMessageBroker>();

            try
            {
                var pingBack = await masterMessageBroker.DispatchAsync(
                    new PingMessage(),
                    ctx => ctx.To((IEndpoint)new Endpoint(appInstanceId: slaveRuntime.GetAppInstanceId()))
                              .Timeout(TimeSpan.FromSeconds(2)));
                Assert.IsInstanceOf<PingBackMessage>(pingBack);
            }
            finally
            {
                await this.FinalizeAppAsync(slaveContainer);
                await this.FinalizeAppAsync(masterContainer);
            }
        }

        [Test]
        public async Task DispatchAsync_pair_request_response_pipes_unavailable_multiple()
        {
            var masterId = $"Master-{Guid.NewGuid():N}";
            var masterInstanceId = $"{masterId}-{Guid.NewGuid():N}";
            var masterContainer = this.CreateInjector(
                this.CreateAmbientServices(),
                parts: new[] { typeof(PipesSettingsProvider) },
                appRuntime: new StaticAppRuntime(
                    isRoot: true,
                    appId: masterId,
                    appInstanceId: masterInstanceId));
            var masterRuntime = masterContainer.Resolve<IAppRuntime>();

            var slaveArgs = new AppArgs { [AppArgs.RootArgName] = masterInstanceId };
            var slaveId = $"Slave-{Guid.NewGuid():N}";
            var slaveInstanceId = $"{slaveId}-{Guid.NewGuid():N}";
            var slaveContainer = this.CreateInjector(
                this.CreateAmbientServices()
                    .RegisterAppArgs(slaveArgs),
                parts: new[] { typeof(PipesSettingsProvider) },
                appRuntime: new StaticAppRuntime(
                    isRoot: false,
                    appId: slaveId,
                    appInstanceId: slaveInstanceId));
            var slaveRuntime = slaveContainer.Resolve<IAppRuntime>();

            await this.InitializeAppAsync(masterContainer, slaveAppInfo: new RuntimeAppInfo { AppId = slaveId, AppInstanceId = slaveInstanceId });
            await this.InitializeAppAsync(slaveContainer, slaveArgs);

            var masterMessageBroker = masterContainer.Resolve<IMessageBroker>();

            try
            {
                var pingBackTasks = new int[3]
                    .Select(i =>
                        masterMessageBroker.DispatchAsync(
                            new PingMessage(),
                            ctx => ctx.To((IEndpoint)new Endpoint(appInstanceId: slaveRuntime.GetAppInstanceId()))
                                      .Timeout(TimeSpan.FromSeconds(2))));

                var pingBacks = await Task.WhenAll(pingBackTasks);
                CollectionAssert.AllItemsAreInstancesOfType(pingBacks, typeof(PingBackMessage));
            }
            finally
            {
                await this.FinalizeAppAsync(slaveContainer);
                await this.FinalizeAppAsync(masterContainer);
            }
        }

        private async Task InitializeAppAsync(IInjector container, IAppArgs? appArgs = null, IRuntimeAppInfo? slaveAppInfo = null)
        {
            var appManager = container.Resolve<IAppManager>();
            var appContext = new AppContext(
                container.Resolve<IAmbientServices>(),
                container.Resolve<IAppRuntime>(),
                appArgs);
            await appManager.InitializeAsync(appContext);

            if (slaveAppInfo != null)
            {
                var eventHub = container.Resolve<IEventHub>();
                await eventHub.PublishAsync(new AppStartingEvent { AppInfo = slaveAppInfo }, appContext);
            }
        }

        private async Task FinalizeAppAsync(IInjector container)
        {
            var appManager = container.Resolve<IAppManager>();
            await appManager.FinalizeAsync(
                new AppContext(container.Resolve<IAmbientServices>(), container.Resolve<IAppRuntime>()));
        }

        public class PipesSettingsProvider : ISettingsProvider
        {
            public object? GetSettings(Type settingsType, IContext? context)
            {
                if (settingsType == typeof(PipesSettingsProvider))
                {
                    return new PipesSettings
                    {
                        Namespace = "unit-test",
                        ServerName = "localhost",
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
