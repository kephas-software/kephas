// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventMessageHandlerSelectorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the event message handler selector test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.HandlerSelectors
{
    using Kephas.Composition;
    using Kephas.Messaging.Composition;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.HandlerSelectors;
    using NSubstitute;
    using NUnit.Framework;
    using System.Collections.Generic;

    [TestFixture]
    public class EventMessageHandlerSelectorTest
    {
        [Test]
        public void CanHandle_non_events()
        {
            var selector = new EventMessageHandlerSelector(Substitute.For<IMessageMatchService>(), new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>());
            Assert.IsFalse(selector.CanHandle(typeof(string), typeof(string), null));
        }

        [Test]
        public void CanHandle_envelope_events()
        {
            var selector = new EventMessageHandlerSelector(Substitute.For<IMessageMatchService>(), new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>());
            Assert.IsTrue(selector.CanHandle(typeof(IEvent), typeof(string), null));
        }

        [Test]
        public void CanHandle_message_events()
        {
            var selector = new EventMessageHandlerSelector(Substitute.For<IMessageMatchService>(), new List<IExportFactory<IMessageHandler, MessageHandlerMetadata>>());
            Assert.IsTrue(selector.CanHandle(typeof(string), typeof(IEvent), null));
        }
    }
}
