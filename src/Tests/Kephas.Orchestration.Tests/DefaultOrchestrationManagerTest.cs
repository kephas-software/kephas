// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultOrchestrationManagerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default orchestration manager test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Diagnostics;
    using Kephas.Interaction;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Events;
    using Kephas.Orchestration.Interaction;
    using Kephas.Reflection;
    using Kephas.Security.Authentication;
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
            orchManager.TimerDueTime = TimeSpan.FromMilliseconds(100);
            orchManager.TimerPeriod = TimeSpan.FromMilliseconds(100);
            var registry = container.GetExport<IMessageHandlerRegistry>();

            AppHeartbeatEvent heartbeat = null;
            registry.RegisterHandler<AppHeartbeatEvent>((e, ctx) => heartbeat = e);

            await appManager.InitializeAppAsync(new AppContext(ambientServices));
            await Task.Delay(TimeSpan.FromMilliseconds(400));

            Assert.NotNull(heartbeat);

            await appManager.FinalizeAppAsync(new AppContext(ambientServices));
        }

        [Test]
        public async Task InitializeAsync_Heartbeat()
        {
            var messageBroker = Substitute.For<IMessageBroker>();
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime[AppRuntimeBase.AppIdKey].Returns("hi");
            appRuntime[AppRuntimeBase.AppInstanceIdKey].Returns("there");
            appRuntime.GetHostAddress().Returns(IPAddress.Loopback);

            var eventHub = this.CreateEventHubMock();

            var compositionContext = Substitute.For<ICompositionContext>();
            var appContext = new Context(compositionContext);

            var manager = new DefaultOrchestrationManager(appRuntime, eventHub, messageBroker, Substitute.For<IExportFactory<IProcessStarterFactory>>());
            manager.TimerDueTime = TimeSpan.FromMilliseconds(100);
            manager.TimerPeriod = TimeSpan.FromMilliseconds(100);

            await manager.InitializeAsync(appContext);
            await eventHub.PublishAsync(new AppStartedEvent { AppInfo = new RuntimeAppInfo { AppId = "hi", AppInstanceId = "there" } }, new Context(compositionContext));
            await Task.Delay(TimeSpan.FromMilliseconds(400));
            await manager.FinalizeAsync(appContext);

            // ensure that the heartbeat is sent
            messageBroker
                .Received()
                .DispatchAsync(
                    Arg.Is<IBrokeredMessage>(e => e.GetContent() is AppHeartbeatEvent),
                    Arg.Any<Action<IDispatchingContext>>(),
                    Arg.Any<CancellationToken>());
        }
    }
}