﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingEventHubExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the event hub extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Events
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Interaction;
    using Kephas.Messaging.Events;
    using Kephas.Services;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class MessagingEventHubExtensionsTest
    {
        [Test]
        public void Subscribe_wrong_args()
        {
            var calls = 0;
            Assert.Throws<ArgumentNullException>(() => MessagingEventHubExtensions.Subscribe<ITestEvent>(null, async (e, c, t) => calls++, MessageTypeMatching.Type));

            Assert.Throws<ArgumentNullException>(() => MessagingEventHubExtensions.Subscribe<ITestEvent>(Substitute.For<IEventHub>(), null, MessageTypeMatching.TypeOrHierarchy));
        }

        [Test]
        public void Subscribe_match_exact()
        {
            var hub = (IMessagingEventHub)Substitute.For<IEventHub, IMessagingEventHub>();
            var calls = 0;
            hub.Subscribe(Arg.Any<IMessageMatch>(), Arg.Any<Func<object, IContext, CancellationToken, Task>>()).Returns(
                ci =>
                    {
                        var match = ci.Arg<IMessageMatch>();
                        if (match.MessageId == null && match.MessageType == typeof(ITestEvent)
                                                    && match.MessageTypeMatching == MessageTypeMatching.Type)
                        {
                            return Substitute.For<IEventSubscription>();
                        }

                        return null;
                    });

            var subscription = MessagingEventHubExtensions.Subscribe<ITestEvent>((IEventHub)hub, async (e, c, t) => calls++, MessageTypeMatching.Type);
            Assert.IsNotNull(subscription);

            subscription = MessagingEventHubExtensions.Subscribe<IEvent>((IEventHub)hub, async (e, c, t) => calls++, MessageTypeMatching.Type);
            Assert.IsNull(subscription);
        }

        [Test]
        public void Subscribe_match_hierarchy()
        {
            var hub = (IMessagingEventHub)Substitute.For<IEventHub, IMessagingEventHub>();
            var calls = 0;
            hub.Subscribe(Arg.Any<IMessageMatch>(), Arg.Any<Func<object, IContext, CancellationToken, Task>>()).Returns(
                ci =>
                    {
                        var match = ci.Arg<IMessageMatch>();
                        if (match.MessageId == null && match.MessageType == typeof(ITestEvent)
                                                    && match.MessageTypeMatching == MessageTypeMatching.TypeOrHierarchy)
                        {
                            return Substitute.For<IEventSubscription>();
                        }

                        return null;
                    });

            var subscription = MessagingEventHubExtensions.Subscribe<ITestEvent>((IEventHub)hub, async (e, c, t) => calls++, MessageTypeMatching.TypeOrHierarchy);
            Assert.IsNotNull(subscription);

            subscription = MessagingEventHubExtensions.Subscribe<IEvent>((IEventHub)hub, async (e, c, t) => calls++, MessageTypeMatching.TypeOrHierarchy);
            Assert.IsNull(subscription);
        }

        public interface ITestEvent : IEvent {}
    }
}