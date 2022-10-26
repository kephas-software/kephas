// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultOrchestrationManagerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default orchestration manager test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Orchestration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Services;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Orchestration;
    using Kephas.Orchestration.Configuration;
    using Kephas.Orchestration.Diagnostics;
    using Kephas.Orchestration.Interaction;
    using Kephas.Services;
    using Kephas.Services.Builder;
    using NSubstitute;
    using NUnit.Framework;

    using AppContext = Kephas.Application.AppContext;

    [TestFixture]
    public class DefaultOrchestrationManagerTest : OrchestrationTestBase
    {
        [Test]
        public void Injection()
        {
            var container = this.CreateServicesBuilder(this.CreateAmbientServices())
                .BuildWithAutofac();

            var manager = container.Resolve<IOrchestrationManager>();

            Assert.IsInstanceOf<DefaultOrchestrationManager>(manager);
        }

        [Test]
        public async Task InitializeAsync_Heartbeat_integration()
        {
            var ambientServices = this.CreateAmbientServices();
            var container = this.CreateServicesBuilder(ambientServices)
                .BuildWithAutofac();

            var appManager = container.Resolve<IAppManager>();
            var orchManager = (DefaultOrchestrationManager)container.Resolve<IOrchestrationManager>();
            orchManager.HeartbeatDueTime = TimeSpan.FromMilliseconds(100);
            orchManager.HeartbeatInterval = TimeSpan.FromMilliseconds(100);
            var registry = container.Resolve<IMessageHandlerRegistry>();

            AppHeartbeatEvent? heartbeat = null;
            registry.RegisterHandler<AppHeartbeatEvent>((e, ctx) => (heartbeat = e).ToEvent());

            await appManager.InitializeAsync(new AppContext(ambientServices));
            await Task.Delay(TimeSpan.FromMilliseconds(400));

            Assert.NotNull(heartbeat);

            await appManager.FinalizeAsync(new AppContext(ambientServices));
        }

        [Test]
        public async Task InitializeAsync_Heartbeat()
        {
            var ambientServices = this.CreateAmbientServices();
            var messageBroker = Substitute.For<IMessageBroker>();
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime[IAppRuntime.AppIdKey].Returns("hi");
            appRuntime[IAppRuntime.AppInstanceIdKey].Returns("there");

            var hostInfoProvider = Substitute.For<IHostInfoProvider>();
            hostInfoProvider.GetHostAddress().Returns(IPAddress.Loopback);

            var eventHub = this.CreateEventHubMock();

            var injector = Substitute.For<IServiceProvider>();
            new AppServiceCollectionBuilder(ambientServices).WithAppRuntime(appRuntime);
            var appContext = new AppContext(ambientServices);

            var config = Substitute.For<IConfiguration<OrchestrationSettings>>();
            config.GetSettings(Arg.Any<IContext>()).Returns(new OrchestrationSettings());

            var manager = new DefaultOrchestrationManager(
                appRuntime, 
                eventHub, 
                messageBroker, 
                Substitute.For<IMessageProcessor>(), 
                Substitute.For<IExportFactory<IProcessStarterFactory>>(),
                config,
                hostInfoProvider,
                Substitute.For<ILogManager>());
            manager.HeartbeatDueTime = TimeSpan.FromMilliseconds(100);
            manager.HeartbeatInterval = TimeSpan.FromMilliseconds(100);

            var messages = new List<object>();

            // ensure that the heartbeat is sent
            messageBroker
                .DispatchAsync(
                    Arg.Any<object>(),
                    Arg.Any<Action<IDispatchingContext>>(),
                    Arg.Any<CancellationToken>())
                .Returns(ci =>
                {
                    messages.Add(ci.Arg<object>());
                    return (IMessage)null;
                });

            await manager.InitializeAsync(appContext);
            await eventHub.PublishAsync(new AppStartedEvent { AppInfo = new RuntimeAppInfo { AppId = "hi", AppInstanceId = "there" } }, new Context(injector));
            await Task.Delay(TimeSpan.FromMilliseconds(400));
            await manager.FinalizeAsync(appContext);

            CollectionAssert.IsNotEmpty(messages);
            CollectionAssert.AllItemsAreInstancesOfType(messages.Select(m => m.GetContent()), typeof(AppHeartbeatEvent));
        }
    }
}