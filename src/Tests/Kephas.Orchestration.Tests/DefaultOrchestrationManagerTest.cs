// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultOrchestrationManagerTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default orchestration manager test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Tests
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Events;
    using Kephas.Orchestration.Endpoints;
    using Kephas.Security;
    using Kephas.Services;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultOrchestrationManagerTest
    {
        [Test]
        public async Task InitializeAsync_Heartbeat()
        {
            var messageBroker = Substitute.For<IMessageBroker>();
            messageBroker.CreateBrokeredMessageBuilder<BrokeredMessage>().Returns(
                ci => new BrokeredMessageBuilder<BrokeredMessage>(
                    Substitute.For<IAppManifest>(),
                    Substitute.For<ISecurityService>(),
                    Substitute.For<IContext>()));
            var appManifest = Substitute.For<IAppManifest>();
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime.GetHostAddress().Returns(IPAddress.Loopback);
            var eventHub = Substitute.For<IEventHub>();

            var manager = new DefaultOrchestrationManager(appManifest, appRuntime, eventHub, messageBroker);
            manager.TimerDueTime = TimeSpan.FromMilliseconds(100);
            manager.TimerPeriod = TimeSpan.FromMilliseconds(100);

            await manager.InitializeAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(400));
            await manager.FinalizeAsync();
            
            // ensure that the heartbeat is sent
            messageBroker
                .Received()
                .DispatchAsync(Arg.Is<IBrokeredMessage>(bm => bm.Content is AppHeartbeatEvent), Arg.Any<IContext>(), Arg.Any<CancellationToken>());
        }
    }
}