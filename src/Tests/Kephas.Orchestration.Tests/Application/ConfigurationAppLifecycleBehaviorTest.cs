// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationAppLifecycleBehaviorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Tests.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration.Interaction;
    using Kephas.Interaction;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Orchestration.Application;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class ConfigurationAppLifecycleBehaviorTest : OrchestrationTestBase
    {
        [Test]
        public async Task BeforeAppInitializeAsync()
        {
            var appRuntime = new StaticAppRuntime();
            var eventHub = new DefaultEventHub();
            var messageBroker = Substitute.For<IMessageBroker>();
            messageBroker.DispatchAsync(
                    Arg.Any<object>(),
                    Arg.Any<Action<IDispatchingContext>>(),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IMessage?>(null));

            var behavior = new ConfigurationAppLifecycleBehavior(appRuntime, eventHub, messageBroker);

            await behavior.BeforeAppInitializeAsync(Substitute.For<IAppContext>());

            var signal = new ConfigurationChangedSignal("test")
            {
                SourceAppInstanceId = appRuntime.GetAppInstanceId(),
                SettingsType = typeof(string),
            };

            await eventHub.PublishAsync(signal, Substitute.For<IContext>());

            messageBroker.Received(1)
                .DispatchAsync(Arg.Any<object>(), Arg.Any<Action<IDispatchingContext>>(), Arg.Any<CancellationToken>());

            await behavior.AfterAppFinalizeAsync(Substitute.For<IAppContext>());
        }
    }
}