// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultEventHubTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default event hub test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Events
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Interaction;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.Messages;
    using Kephas.Services;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class MessagingEventHubTest : MefMessagingTestBase
    {
        [Test]
        public void Composition()
        {
            var container = this.CreateContainer();
            var hub = container.GetExport<IEventHub>();

            Assert.IsNotNull(hub);
            Assert.IsInstanceOf<MessagingEventHub>(hub);
        }

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

        [Test]
        public async Task Subscribe_subscription_called()
        {
            var matchService = Substitute.For<IMessageMatchService>();
            matchService.IsMatch(Arg.Any<IMessageMatch>(), Arg.Any<Type>(), Arg.Any<Type>(), Arg.Any<object>()).Returns(true);
            var hub = new MessagingEventHub(matchService, Substitute.For<IMessageHandlerRegistry>());
            var calls = 0;
            using (var s = hub.Subscribe(Substitute.For<IMessageMatch>(), async (e, c, t) => calls++))
            {
                await hub.PublishAsync(
                    Substitute.For<IEvent>(),
                    Substitute.For<IContext>(),
                    default);

                Assert.AreEqual(1, calls);
            }
        }

        [Test]
        public async Task Subscribe_subscription_not_called_if_no_match()
        {
            var matchService = Substitute.For<IMessageMatchService>();
            matchService.IsMatch(Arg.Any<IMessageMatch>(), Arg.Any<Type>(), Arg.Any<Type>(), Arg.Any<object>()).Returns(false);
            var hub = new MessagingEventHub(matchService, Substitute.For<IMessageHandlerRegistry>());
            var calls = 0;
            using (var s = hub.Subscribe(Substitute.For<IMessageMatch>(), async (e, c, t) => calls++))
            {
                await hub.PublishAsync(
                    Substitute.For<IEvent>(),
                    Substitute.For<IContext>(),
                    default);

                Assert.AreEqual(0, calls);
            }
        }

        [Test]
        public async Task Subscribe_multiple_subscriptions_called()
        {
            var matchService = Substitute.For<IMessageMatchService>();
            matchService.IsMatch(Arg.Any<IMessageMatch>(), Arg.Any<Type>(), Arg.Any<Type>(), Arg.Any<object>()).Returns(true);
            var hub = new MessagingEventHub(matchService, Substitute.For<IMessageHandlerRegistry>());
            var s1calls = 0;
            var s2calls = 0;
            using var s1 = hub.Subscribe(Substitute.For<IMessageMatch>(), async (e, c, t) => s1calls++);
            using var s2 = hub.Subscribe(Substitute.For<IMessageMatch>(), async (e, c, t) => s2calls++);
            await hub.PublishAsync(
                Substitute.For<IEvent>(),
                Substitute.For<IContext>(),
                default);

            Assert.AreEqual(1, s1calls);
            Assert.AreEqual(1, s2calls);
        }
    }
}