// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationAppLifecycleBehaviorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Orchestration.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Configuration.Interaction;
    using Kephas.Injection;
    using Kephas.Interaction;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Orchestration;
    using Kephas.Orchestration.Application;
    using Kephas.Security.Authentication;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class ConfigurationAppLifecycleBehaviorTest : OrchestrationTestBase
    {
        [Test]
        public async Task BeforeAppInitializeAsync_from_other_appinstance()
        {
            var appRuntime = new StaticAppRuntime();
            var eventHub = this.CreateEventHub();
            var messageBroker = Substitute.For<IMessageBroker>();
            var orchManager = Substitute.For<IOrchestrationManager>();
            messageBroker.DispatchAsync(
                    Arg.Any<object>(),
                    Arg.Any<Action<IDispatchingContext>>(),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IMessage?>(null));

            var behavior = new ConfigurationAppLifecycleBehavior(appRuntime, eventHub, messageBroker, new Lazy<IOrchestrationManager>(() => orchManager));

            await behavior.BeforeAppInitializeAsync(Substitute.For<IAppContext>());

            var signal = new ConfigurationChangedSignal("test")
            {
                SourceAppInstanceId = Guid.NewGuid().ToString(),
                SettingsType = typeof(string).FullName,
            };

            await eventHub.PublishAsync(signal, Substitute.For<IContext>());

            messageBroker.Received(0)
                .DispatchAsync(Arg.Any<object>(), Arg.Any<Action<IDispatchingContext>>(), Arg.Any<CancellationToken>());

            await behavior.AfterAppFinalizeAsync(Substitute.For<IAppContext>());
        }

        [Test]
        public async Task BeforeAppInitializeAsync_from_this_appinstance_only()
        {
            IAppRuntime appRuntime = new StaticAppRuntime();
            var eventHub = this.CreateEventHub();
            var messageBroker = Substitute.For<IMessageBroker>();
            var orchManager = Substitute.For<IOrchestrationManager>();

            var thisAppInstanceId = appRuntime.GetAppInstanceId();
            messageBroker.DispatchAsync(
                    Arg.Any<object>(),
                    Arg.Any<Action<IDispatchingContext>>(),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IMessage?>(null));

            var behavior = new ConfigurationAppLifecycleBehavior(appRuntime, eventHub, messageBroker, new Lazy<IOrchestrationManager>(() => orchManager));

            await behavior.BeforeAppInitializeAsync(Substitute.For<IAppContext>());

            var signal = new ConfigurationChangedSignal("test")
            {
                SourceAppInstanceId = thisAppInstanceId,
                SettingsType = typeof(string).FullName,
            };

            await eventHub.PublishAsync(signal, Substitute.For<IContext>());

            messageBroker.Received(0)
                .DispatchAsync(Arg.Any<object>(), Arg.Any<Action<IDispatchingContext>>(), Arg.Any<CancellationToken>());

            await behavior.AfterAppFinalizeAsync(Substitute.For<IAppContext>());
        }

        [Test]
        public async Task BeforeAppInitializeAsync_from_this_appinstance()
        {
            IAppRuntime appRuntime = new StaticAppRuntime();
            var eventHub = this.CreateEventHub();
            var messageBroker = Substitute.For<IMessageBroker>();
            var orchManager = Substitute.For<IOrchestrationManager>();

            var thisAppInstanceId = appRuntime.GetAppInstanceId();
            messageBroker.DispatchAsync(
                    Arg.Any<object>(),
                    Arg.Any<Action<IDispatchingContext>>(),
                    Arg.Any<CancellationToken>())
                .Returns(ci =>
                {
                    var ctx = new DispatchingContext(
                        Substitute.For<IServiceProvider>(),
                        Substitute.For<IConfiguration<DistributedMessagingSettings>>(),
                        Substitute.For<IMessageBroker>(),
                        appRuntime,
                        Substitute.For<IAuthenticationService>());
                    ci.Arg<Action<IDispatchingContext>>()?.Invoke(ctx);
                    Assert.AreEqual(1, ctx.BrokeredMessage.Recipients.Count());
                    Assert.AreNotEqual(thisAppInstanceId, ctx.BrokeredMessage.Recipients.Single().AppInstanceId);
                    return Task.FromResult<IMessage?>(null);
                });

            var behavior = new ConfigurationAppLifecycleBehavior(appRuntime, eventHub, messageBroker, new Lazy<IOrchestrationManager>(() => orchManager));

            await behavior.BeforeAppInitializeAsync(Substitute.For<IAppContext>());

            var signal = new ConfigurationChangedSignal("test")
            {
                SourceAppInstanceId = thisAppInstanceId,
                SettingsType = typeof(string).FullName,
            };

            orchManager.GetLiveAppsAsync(Arg.Any<Action<IContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult(this.GetLiveApps(thisAppInstanceId!, Guid.NewGuid().ToString())));

            await eventHub.PublishAsync(signal, Substitute.For<IContext>());

            messageBroker.Received(1)
                .DispatchAsync(Arg.Any<object>(), Arg.Any<Action<IDispatchingContext>>(), Arg.Any<CancellationToken>());

            await behavior.AfterAppFinalizeAsync(Substitute.For<IAppContext>());
        }

        private IEnumerable<IRuntimeAppInfo> GetLiveApps(params string[] instanceIds)
        {
            return instanceIds.Select(iid => new RuntimeAppInfo { AppInstanceId = iid });
        }

        private IEventHub CreateEventHub(IInjectableFactory? contextFactory = null)
        {
            return new DefaultEventHub(Substitute.For<IInjectableFactory>());
        }
    }
}