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
    using Kephas.Composition;
    using Kephas.Configuration;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Orchestration;
    using Kephas.Orchestration.Configuration;
    using Kephas.Orchestration.Diagnostics;
    using Kephas.Orchestration.Interaction;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    using AppContext = Kephas.Application.AppContext;

    [TestFixture]
    public class DefaultOrchestrationManagerTest : OrchestrationTestBase
    {
        [Test]
        public void Composition()
        {
            var container = this.CreateContainer(new AmbientServices());

            var manager = container.GetExport<IOrchestrationManager>();

            Assert.IsInstanceOf<DefaultOrchestrationManager>(manager);
        }

        [Test]
        public async Task InitializeAsync_Heartbeat_integration()
        {
            var ambientServices = new AmbientServices();
            var container = this.CreateContainer(ambientServices);

            var appManager = container.GetExport<IAppManager>();
            var orchManager = (DefaultOrchestrationManager)container.GetExport<IOrchestrationManager>();
            orchManager.HeartbeatDueTime = TimeSpan.FromMilliseconds(100);
            orchManager.HeartbeatInterval = TimeSpan.FromMilliseconds(100);
            var registry = container.GetExport<IMessageHandlerRegistry>();

            AppHeartbeatEvent? heartbeat = null;
            registry.RegisterHandler<AppHeartbeatEvent>((e, ctx) => (heartbeat = e).ToEvent());

            await appManager.InitializeAppAsync(new AppContext(ambientServices));
            await Task.Delay(TimeSpan.FromMilliseconds(400));

            Assert.NotNull(heartbeat);

            await appManager.FinalizeAppAsync(new AppContext(ambientServices));
        }

        [Test]
        public async Task InitializeAsync_Heartbeat()
        {
            var ambientServices = new AmbientServices();
            var messageBroker = Substitute.For<IMessageBroker>();
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime[AppRuntimeBase.AppIdKey].Returns("hi");
            appRuntime[AppRuntimeBase.AppInstanceIdKey].Returns("there");

            var hostInfoProvider = Substitute.For<IHostInfoProvider>();
            hostInfoProvider.GetHostAddress().Returns(IPAddress.Loopback);

            var eventHub = this.CreateEventHubMock();

            var compositionContext = Substitute.For<IInjector>();
            ambientServices.WithAppRuntime(appRuntime).WithCompositionContainer(compositionContext);
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
            await eventHub.PublishAsync(new AppStartedEvent { AppInfo = new RuntimeAppInfo { AppId = "hi", AppInstanceId = "there" } }, new Context(compositionContext));
            await Task.Delay(TimeSpan.FromMilliseconds(400));
            await manager.FinalizeAsync(appContext);

            CollectionAssert.IsNotEmpty(messages);
            CollectionAssert.AllItemsAreInstancesOfType(messages.Select(m => m.GetContent()), typeof(AppHeartbeatEvent));
        }
    }
}