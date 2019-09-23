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
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Events;
    using Kephas.Orchestration.Endpoints;
    using Kephas.Security.Authentication;
    using Kephas.Services;
    using Kephas.Testing.Composition;

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
            var appManifest = Substitute.For<IAppManifest>();
            appManifest.AppId.Returns("hi");
            appManifest.AppInstanceId.Returns("there");
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime.GetHostAddress().Returns(IPAddress.Loopback);

            var eventHub = this.CreateEventHub();

            var compositionContext = Substitute.For<ICompositionContext>();
            var context = new Context(compositionContext);

            messageBroker.CreateBrokeredMessageBuilder(Arg.Any<IContext>())
                .Returns(ci => this.CreateMessageBuilder(ci.Arg<IContext>()));

            var manager = new DefaultOrchestrationManager(appManifest, appRuntime, eventHub, messageBroker);
            manager.TimerDueTime = TimeSpan.FromMilliseconds(100);
            manager.TimerPeriod = TimeSpan.FromMilliseconds(100);

            await manager.InitializeAsync(context);
            await eventHub.NotifySubscribersAsync(new AppStartedEvent { AppInfo = new RuntimeAppInfo { AppId = "hi", AppInstanceId = "there" } }, new Context(compositionContext));
            await Task.Delay(TimeSpan.FromMilliseconds(400));
            await manager.FinalizeAsync(context);

            // ensure that the heartbeat is sent
            messageBroker
                .Received()
                .DispatchAsync(Arg.Is<IBrokeredMessage>(bm => bm.Content is AppHeartbeatEvent), Arg.Any<IContext>(), Arg.Any<CancellationToken>());
        }

        private BrokeredMessageBuilder CreateMessageBuilder(IContext context)
        {
            var builder = new BrokeredMessageBuilder(Substitute.For<IAppManifest>(), Substitute.For<IAuthenticationService>());
            builder.Initialize(context);
            return builder;
        }

        private IEventHub CreateEventHub()
        {
            var subscriptions = new Dictionary<string, (MessageMatch match, Func<object, IContext, CancellationToken, Task> callback, string id)>();
            var eventHub = Substitute.For<IEventHub>();
            var i = 0;
            eventHub.Subscribe(Arg.Any<MessageMatch>(), Arg.Any<Func<object, IContext, CancellationToken, Task>>())
                .Returns(ci =>
                {
                    var id = i++.ToString();
                    subscriptions.Add(id, (ci.Arg<MessageMatch>(), ci.Arg<Func<object, IContext, CancellationToken, Task>>(), id));
                    var subscription = Substitute.For<IEventSubscription>();
                    subscription
                        .When(s => s.Dispose())
                        .Do(_ => subscriptions.Remove(id));
                    return subscription;
                });
            eventHub.NotifySubscribersAsync(Arg.Any<object>(), Arg.Any<IContext>(), Arg.Any<CancellationToken>())
                .Returns(async ci =>
                {
                    var tasks = subscriptions.Values
                        .Where(t => t.match.MessageType == ci.Arg<object>().GetType())
                        .Select(t => t.callback(ci.Arg<object>(), ci.Arg<IContext>(), ci.Arg<CancellationToken>()));
                    await Task.WhenAll(tasks);
                });
            return eventHub;
        }
    }
}