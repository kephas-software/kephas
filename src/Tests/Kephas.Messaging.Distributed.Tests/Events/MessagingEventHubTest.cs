﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingEventHubTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Events;

using System.Threading.Tasks;

using Kephas.Interaction;
using Kephas.Messaging.Distributed;
using Kephas.Messaging.Messages;
using Kephas.Services;
using NUnit.Framework;

[TestFixture]
public class MessagingEventHubTest : MessagingTestBase
{
    protected IServiceProvider BuildServiceProvider()
    {
        return this.CreateServicesBuilder().BuildWithDependencyInjection();
    }

    [Test]
    public async Task Subscribe_integration_subscription_called()
    {
        var container = this.BuildServiceProvider();
        var hub = container.Resolve<IEventHub>();
        var broker = container.Resolve<IMessageBroker>();
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