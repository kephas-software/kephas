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
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration.Providers;
    using Kephas.Diagnostics.Logging;
    using Kephas.Injection;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Queues;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Messages;
    using Kephas.Messaging.Redis.Configuration;
    using Kephas.Messaging.Redis.Routing;
    using Kephas.Services;
    using NUnit.Framework;

    using AppContext = Kephas.Application.AppContext;

    [TestFixture]
    public class RedisAppMessageRouterTest : RedisMessagingTestBase
    {
        public override IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>(base.GetDefaultParts())
            {
                typeof(RedisSettingsProvider),
                typeof(InProcessStaticMessageQueueStore),
            };
        }

        [Test]
        public void Injection()
        {
            var container = this.CreateInjector();
            var router = container.ResolveMany<IMessageRouter>().OfType<RedisAppMessageRouter>().SingleOrDefault();

            Assert.IsNotNull(router);
        }

        [Test]
        public async Task DispatchAsync_pair_request_response()
        {
            var masterId = $"Master-{Guid.NewGuid():N}";
            var masterInstanceId = $"{masterId}-{Guid.NewGuid():N}";
            var masterContainer = this.CreateInjector(
                this.CreateAmbientServices()
                    .WithStaticAppRuntime(appId: masterId, appInstanceId: masterInstanceId));
            var masterRuntime = masterContainer.Resolve<IAppRuntime>();

            var slaveId = $"Slave-{Guid.NewGuid():N}";
            var slaveInstanceId = $"{slaveId}-{Guid.NewGuid():N}";
            var slaveContainer = this.CreateInjector(
                this.CreateAmbientServices()
                    .WithStaticAppRuntime(appId: slaveId, appInstanceId: slaveInstanceId));
            var slaveRuntime = slaveContainer.Resolve<IAppRuntime>();

            await this.InitializeAppAsync(masterContainer);
            await this.InitializeAppAsync(slaveContainer);

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
                    .WithDebugLogManager(sbMaster)
                    .WithStaticAppRuntime(appId: masterId, appInstanceId: masterInstanceId, assemblyFilter: this.IsNotTestAssembly));
            var masterRuntime = masterContainer.Resolve<IAppRuntime>();

            var sbSlave = new StringBuilder();
            var slaveId = $"Slave-{Guid.NewGuid():N}";
            var slaveInstanceId = $"{slaveId}-{Guid.NewGuid():N}";
            var slaveContainer = this.CreateInjector(
                this.CreateAmbientServices()
                    .WithDebugLogManager(sbSlave)
                    .WithStaticAppRuntime(appId: slaveId, appInstanceId: slaveInstanceId, assemblyFilter: this.IsNotTestAssembly));
            var slaveRuntime = slaveContainer.Resolve<IAppRuntime>();

            await this.InitializeAppAsync(masterContainer);
            await this.InitializeAppAsync(slaveContainer);

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
        public async Task DispatchAsync_pair_request_response_redis_unavailable()
        {
            var masterId = $"Master-{Guid.NewGuid():N}";
            var masterInstanceId = $"{masterId}-{Guid.NewGuid():N}";
            var masterContainer = this.CreateInjector(
                this.CreateAmbientServices()
                    .WithStaticAppRuntime(appId: masterId, appInstanceId: masterInstanceId));
            var masterRuntime = masterContainer.Resolve<IAppRuntime>();

            var slaveId = $"Slave-{Guid.NewGuid():N}";
            var slaveInstanceId = $"{slaveId}-{Guid.NewGuid():N}";
            var slaveContainer = this.CreateInjector(
                this.CreateAmbientServices()
                    .WithStaticAppRuntime(appId: slaveId, appInstanceId: slaveInstanceId));
            var slaveRuntime = slaveContainer.Resolve<IAppRuntime>();

            await this.InitializeAppAsync(masterContainer);
            await this.InitializeAppAsync(slaveContainer);

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
        public async Task DispatchAsync_pair_request_response_redis_unavailable_multiple()
        {
            var masterId = $"Master-{Guid.NewGuid():N}";
            var masterInstanceId = $"{masterId}-{Guid.NewGuid():N}";
            var masterContainer = this.CreateInjector(
                this.CreateAmbientServices()
                    .WithStaticAppRuntime(appId: masterId, appInstanceId: masterInstanceId));
            var masterRuntime = masterContainer.Resolve<IAppRuntime>();

            var slaveId = $"Slave-{Guid.NewGuid():N}";
            var slaveInstanceId = $"{slaveId}-{Guid.NewGuid():N}";
            var slaveContainer = this.CreateInjector(
                this.CreateAmbientServices()
                    .WithStaticAppRuntime(appId: slaveId, appInstanceId: slaveInstanceId));
            var slaveRuntime = slaveContainer.Resolve<IAppRuntime>();

            await this.InitializeAppAsync(masterContainer);
            await this.InitializeAppAsync(slaveContainer);

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

        private async Task InitializeAppAsync(IInjector container)
        {
            var appManager = container.Resolve<IAppManager>();
            await appManager.InitializeAsync(
                new AppContext(container.Resolve<IAmbientServices>(), container.Resolve<IAppRuntime>()));
        }

        private async Task FinalizeAppAsync(IInjector container)
        {
            var appManager = container.Resolve<IAppManager>();
            await appManager.FinalizeAsync(
                new AppContext(container.Resolve<IAmbientServices>(), container.Resolve<IAppRuntime>()));
        }

        public class RedisSettingsProvider : ISettingsProvider
        {
            public object? GetSettings(Type settingsType, IContext? context)
            {
                if (settingsType == typeof(RedisRoutingSettings))
                {
                    return new RedisRoutingSettings
                    {
                        Namespace = "unit-test",
                        ConnectionUri = "redis://localhost",
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

            public InProcessStaticMessageQueueStore(IContextFactory contextFactory)
                : base(contextFactory)
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
