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
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration.Providers;
    using Kephas.Diagnostics.Logging;
    using Kephas.Interaction;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Queues;
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
        public override IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>(base.GetDefaultParts())
            {
                typeof(PipesSettingsProvider),
                typeof(InProcessStaticMessageQueueStore),
            };
        }

        [Test]
        public void Injection()
        {
            var container = this.BuildServiceProvider();
            var router = container.ResolveMany<IMessageRouter>().OfType<PipesAppMessageRouter>().SingleOrDefault();

            Assert.IsNotNull(router);
        }

        [Test]
        public async Task DispatchAsync_pair_request_response()
        {
            var masterId = $"Master-{Guid.NewGuid():N}";
            var masterInstanceId = $"{masterId}-{Guid.NewGuid():N}";
            var masterContainer = this.BuildServiceProvider(
                this.CreateAmbientServices(),
                appRuntime: new StaticAppRuntime(
                    isRoot: true,
                    appId: masterId,
                    appInstanceId: masterInstanceId));
            var masterRuntime = masterContainer.Resolve<IAppRuntime>();

            var slaveArgs = new AppArgs { [AppArgs.RootArgName] = masterInstanceId };
            var slaveId = $"Slave-{Guid.NewGuid():N}";
            var slaveInstanceId = $"{slaveId}-{Guid.NewGuid():N}";
            var slaveContainer = this.BuildServiceProvider(
                this.CreateAmbientServices()
                    .AddAppArgs(slaveArgs),
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
            var masterContainer = this.BuildServiceProvider(
                this.CreateAmbientServices()
                    .WithDebugLogManager(sbMaster),
                appRuntime: new StaticAppRuntime(
                    isRoot: true,
                    appId: masterId,
                    appInstanceId: masterInstanceId)
                        .OnIsAppAssembly(this.IsNotTestAssembly));
            var masterRuntime = masterContainer.Resolve<IAppRuntime>();

            var slaveArgs = new AppArgs { [AppArgs.RootArgName] = masterInstanceId };
            var sbSlave = new StringBuilder();
            var slaveId = $"Slave-{Guid.NewGuid():N}";
            var slaveInstanceId = $"{slaveId}-{Guid.NewGuid():N}";
            var slaveContainer = this.BuildServiceProvider(
                this.CreateAmbientServices()
                    .WithDebugLogManager(sbSlave)
                    .AddAppArgs(slaveArgs),
                appRuntime: new StaticAppRuntime(
                    isRoot: false,
                    appId: slaveId,
                    appInstanceId: slaveInstanceId)
                        .OnIsAppAssembly(this.IsNotTestAssembly));
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
            var masterContainer = this.BuildServiceProvider(
                this.CreateAmbientServices(),
                appRuntime: new StaticAppRuntime(
                    isRoot: true,
                    appId: masterId,
                    appInstanceId: masterInstanceId));
            var masterRuntime = masterContainer.Resolve<IAppRuntime>();

            var slaveArgs = new AppArgs { [AppArgs.RootArgName] = masterInstanceId };
            var slaveId = $"Slave-{Guid.NewGuid():N}";
            var slaveInstanceId = $"{slaveId}-{Guid.NewGuid():N}";
            var slaveContainer = this.BuildServiceProvider(
                this.CreateAmbientServices()
                    .AddAppArgs(slaveArgs),
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
            var masterContainer = this.BuildServiceProvider(
                this.CreateAmbientServices(),
                appRuntime: new StaticAppRuntime(
                    isRoot: true,
                    appId: masterId,
                    appInstanceId: masterInstanceId));
            var masterRuntime = masterContainer.Resolve<IAppRuntime>();

            var slaveArgs = new AppArgs { [AppArgs.RootArgName] = masterInstanceId };
            var slaveId = $"Slave-{Guid.NewGuid():N}";
            var slaveInstanceId = $"{slaveId}-{Guid.NewGuid():N}";
            var slaveContainer = this.BuildServiceProvider(
                this.CreateAmbientServices()
                    .AddAppArgs(slaveArgs),
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

        private async Task InitializeAppAsync(IServiceProvider container, IAppArgs? appArgs = null, IRuntimeAppInfo? slaveAppInfo = null)
        {
            var appManager = container.Resolve<IAppManager>();
            var appContext = new AppContext(
                container.Resolve<IAmbientServices>(),
                appArgs);
            await appManager.InitializeAsync(appContext);

            if (slaveAppInfo != null)
            {
                var eventHub = container.Resolve<IEventHub>();
                await eventHub.PublishAsync(new AppStartingEvent { AppInfo = slaveAppInfo }, appContext);
            }
        }

        private async Task FinalizeAppAsync(IServiceProvider container)
        {
            var appManager = container.Resolve<IAppManager>();
            await appManager.FinalizeAsync(
                new AppContext(container.Resolve<IAmbientServices>()));
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

        [OverridePriority(Priority.High)]
        public class InProcessStaticMessageQueueStore : InProcessMessageQueueStore
        {
            private static readonly ConcurrentDictionary<string, IMessageQueue> Channels = new ();

            public InProcessStaticMessageQueueStore(IInjectableFactory injectableFactory)
                : base(injectableFactory)
            {
            }

            /// <summary>
            /// Gets the message queue with the provided name.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <returns>
            /// The message queue.
            /// </returns>
            public override IMessageQueue GetMessageQueue(string name)
            {
                return Channels.GetOrAdd(name, _ => this.CreateMessageQueue(name));
            }
        }
    }
}
