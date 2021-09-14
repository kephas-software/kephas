﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingEventHubTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default event hub test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Events
{
    using System.Threading.Tasks;

    using Kephas.Interaction;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Messages;
    using Kephas.Services;
    using NUnit.Framework;

    [TestFixture]
    public class MessagingEventHubTest : MefMessagingTestBase
    {
        [Test]
        public async Task Subscribe_integration_subscription_called()
        {
            var container = this.CreateContainer();
            var hub = container.GetExport<IEventHub>();
            var broker = container.GetExport<IMessageBroker>();
            await (broker as IAsyncInitializable).InitializeAsync(new Context(container));

            var calls = 0;
            using (var s = hub.Subscribe<PingMessage>(async (e, c, t) => calls++))
            {
                await broker.PublishAsync<PingMessage>();
                await Task.Delay(100);
                Assert.AreEqual(1, calls);
            }
        }

    }
}