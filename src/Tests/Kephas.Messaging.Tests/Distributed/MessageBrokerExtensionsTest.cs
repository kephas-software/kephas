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

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.Messages;
    using Kephas.Security.Authentication;
    using Kephas.Services;

    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class MessageBrokerExtensionsTest
    {
        [Test]
        public async Task PublishAsync_anonymous_event()
        {
            var broker = Substitute.For<IMessageBroker>();
            var container = Substitute.For<ICompositionContext>();
            var context = new Context(container);

            var expectedBuilder = new BrokeredMessageBuilder(Substitute.For<IAppManifest>(), Substitute.For<IAuthenticationService>());
            expectedBuilder.Initialize(context);
            broker.CreateBrokeredMesssageBuilder(context).Returns(expectedBuilder);

            IMessage content = null;
            broker.DispatchAsync(Arg.Any<IBrokeredMessage>(), Arg.Any<IContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult(content = ci.Arg<IBrokeredMessage>().Content));

            await broker.PublishAsync("hello", context);

            Assert.IsInstanceOf<EventAdapter>(content);
            Assert.AreEqual("hello", ((EventAdapter)content).Event);
        }

        [Test]
        public async Task ProcessAsync_anonymous_message()
        {
            var broker = Substitute.For<IMessageBroker>();
            var container = Substitute.For<ICompositionContext>();
            var context = new Context(container);

            var expectedBuilder = new BrokeredMessageBuilder(Substitute.For<IAppManifest>(), Substitute.For<IAuthenticationService>());
            expectedBuilder.Initialize(context);
            broker.CreateBrokeredMesssageBuilder(context).Returns(expectedBuilder);

            IMessage content = null;
            broker.DispatchAsync(Arg.Any<IBrokeredMessage>(), Arg.Any<IContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult(content = ci.Arg<IBrokeredMessage>().Content));

            await broker.ProcessAsync("hello", context);

            Assert.IsInstanceOf<MessageAdapter>(content);
            Assert.AreEqual("hello", ((MessageAdapter)content).Message);
        }
    }
}