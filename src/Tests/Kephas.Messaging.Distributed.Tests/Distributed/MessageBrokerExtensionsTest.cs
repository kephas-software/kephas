// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBrokerExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message broker extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Distributed
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Dynamic;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.Messages;
    using Kephas.Services;

    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class MessageBrokerExtensionsTest : MessagingTestBase
    {
        [Test]
        public async Task PublishAsync_anonymous_event()
        {
            var broker = Substitute.For<IMessageBroker>();
            var container = this.CreateMessagingContainerMock();
            var context = container.GetExport<IContextFactory>().CreateContext<DispatchingContext>();

            object content = null;
            broker.DispatchAsync(Arg.Any<object>(), Arg.Any<Action<IDispatchingContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult((IMessage)(content = ci.Arg<object>())));

            await broker.PublishAsync("hello");

            Assert.IsInstanceOf<EventEnvelope>(content);
            Assert.AreEqual("hello", ((EventEnvelope)content).Event);
        }

        [Test]
        public void Publish_anonymous_event()
        {
            var broker = Substitute.For<IMessageBroker>();
            var container = this.CreateMessagingContainerMock();
            var context = container.GetExport<IContextFactory>().CreateContext<DispatchingContext>();

            object content = null;
            broker.DispatchAsync(Arg.Any<object>(), Arg.Any<Action<IDispatchingContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult((IMessage)(content = ci.Arg<object>())));

            broker.Publish("hello");

            Assert.IsInstanceOf<EventEnvelope>(content);
            Assert.AreEqual("hello", ((EventEnvelope)content).Event);
        }

        [Test]
        public async Task PublishAsync_anonymous_event_endpoint()
        {
            var broker = Substitute.For<IMessageBroker>();
            var container = this.CreateMessagingContainerMock();
            var context = container.GetExport<IContextFactory>().CreateContext<DispatchingContext>();

            object content = null;
            broker.DispatchAsync(Arg.Any<object>(), Arg.Any<Action<IDispatchingContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult((IMessage)(content = ci.Arg<object>())));

            await broker.PublishAsync("hello", new Endpoint());

            Assert.IsInstanceOf<EventEnvelope>(content);
            Assert.AreEqual("hello", ((EventEnvelope)content).Event);
        }

        [Test]
        public void Publish_anonymous_event_endpoint()
        {
            var broker = Substitute.For<IMessageBroker>();
            var container = this.CreateMessagingContainerMock();
            var context = container.GetExport<IContextFactory>().CreateContext<DispatchingContext>();

            object content = null;
            broker.DispatchAsync(Arg.Any<object>(), Arg.Any<Action<IDispatchingContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult((IMessage)(content = ci.Arg<object>())));

            broker.Publish("hello", new Endpoint());

            Assert.IsInstanceOf<EventEnvelope>(content);
            Assert.AreEqual("hello", ((EventEnvelope)content).Event);
        }

        [Test]
        public async Task PublishAsync_identifiable_event()
        {
            var broker = Substitute.For<IMessageBroker>();
            var container = this.CreateMessagingContainerMock();
            var context = container.GetExport<IContextFactory>().CreateContext<DispatchingContext>();

            object content = null;
            broker.DispatchAsync(Arg.Any<object>(), Arg.Any<Action<IDispatchingContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult((IMessage)(content = ci.Arg<object>())));

            await broker.PublishAsync("hello", new Expando());

            Assert.IsInstanceOf<IdentifiableEvent>(content);

            var @event = (IdentifiableEvent)content;
            Assert.IsInstanceOf<IdentifiableEvent>(@event);
            Assert.AreEqual("hello", ((IdentifiableEvent)@event).Id);
        }

        [Test]
        public void Publish_identifiable_event()
        {
            var broker = Substitute.For<IMessageBroker>();
            var container = this.CreateMessagingContainerMock();
            var context = container.GetExport<IContextFactory>().CreateContext<DispatchingContext>();

            object content = null;
            broker.DispatchAsync(Arg.Any<object>(), Arg.Any<Action<IDispatchingContext>>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult((IMessage)(content = ci.Arg<object>())));

            broker.Publish("hello", new Expando());

            Assert.IsInstanceOf<IdentifiableEvent>(content);

            var @event = (IdentifiableEvent)content;
            Assert.IsInstanceOf<IdentifiableEvent>(@event);
            Assert.AreEqual("hello", ((IdentifiableEvent)@event).Id);
        }
    }
}