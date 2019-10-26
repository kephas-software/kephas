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
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.Messages;
    using Kephas.Services;

    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class MessageBrokerExtensionsTest : MefMessagingTestBase
    {
        [Test]
        public async Task PublishAsync_anonymous_event()
        {
            var broker = Substitute.For<IMessageBroker>();
            var container = this.CreateSubstituteContainer();
            var context = new Context(container);

            IMessage content = null;
            broker.DispatchAsync(Arg.Any<IBrokeredMessage>(), Arg.Any<IContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult(content = ci.Arg<IBrokeredMessage>().Content));

            await broker.PublishAsync("hello", context);

            Assert.IsInstanceOf<EventEnvelope>(content);
            Assert.AreEqual("hello", ((EventEnvelope)content).Event);
        }

        [Test]
        public async Task ProcessAsync_anonymous_message()
        {
            var broker = Substitute.For<IMessageBroker>();
            var container = this.CreateSubstituteContainer();
            var context = new Context(container);

            IMessage content = null;
            broker.DispatchAsync(Arg.Any<IBrokeredMessage>(), Arg.Any<IContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult(content = ci.Arg<IBrokeredMessage>().Content));

            await broker.ProcessAsync("hello", context);

            Assert.IsInstanceOf<MessageEnvelope>(content);
            Assert.AreEqual("hello", ((MessageEnvelope)content).Message);
        }
    }
}