// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventMessageHandlerProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the event message handler provider test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.HandlerProviders
{
    using Kephas.Messaging.Events;
    using Kephas.Messaging.HandlerProviders;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class EventMessageHandlerProviderTest
    {
        [Test]
        public void CanHandle_non_events()
        {
            var selector = new EventMessageHandlerResolveBehavior(Substitute.For<IMessageMatchService>());
            Assert.IsFalse(selector.CanHandle(typeof(string)));
        }

        [Test]
        public void CanHandle_envelope_events()
        {
            var selector = new EventMessageHandlerResolveBehavior(Substitute.For<IMessageMatchService>());
            Assert.IsTrue(selector.CanHandle(typeof(IEvent)));
        }

        [Test]
        public void CanHandle_message_events()
        {
            var selector = new EventMessageHandlerResolveBehavior(Substitute.For<IMessageMatchService>());
            Assert.IsTrue(selector.CanHandle(typeof(string)));
        }
    }
}
