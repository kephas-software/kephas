// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DuplexMessageRouterBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the duplex message router base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Distributed.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Application;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Security.Authentication;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DuplexMessageRouterBaseTest : MessagingTestBase
    {
        [Test]
        public async Task SendAsync_calls_RouteOutputAsync()
        {
            var router = new TestDuplexMessageRouter(new Lazy<IMessageBroker>(() => Substitute.For<IMessageBroker>()));
            var message = Substitute.For<IBrokeredMessage>();
            IBrokeredMessage receivedReply = null;
            router.ReplyReceived += (s, e) => receivedReply = e.Message;
            var (action, reply) = await router.SendAsync(message, Substitute.For<IContext>(), default);

            Assert.Contains(message, router.Out);
            Assert.AreEqual(RoutingInstruction.None, action);
            Assert.IsNull(reply);
            Assert.IsNull(receivedReply);
        }

        [Test]
        public async Task Receive_reply_raises_ReplyReceived_event()
        {
            var router = new TestDuplexMessageRouter(new Lazy<IMessageBroker>(() => Substitute.For<IMessageBroker>()));
            var message = Substitute.For<IBrokeredMessage>();
            message.ReplyToMessageId.Returns("some-id");

            IBrokeredMessage reply = null;
            router.ReplyReceived += (s, e) => reply = e.Message;
            router.Receive(message, Substitute.For<IContext>());

            Assert.AreSame(reply, message);
        }

        [Test]
        public async Task Receive_request_sends_response()
        {
            var messageBroker = Substitute.For<IMessageBroker>();
            var router = new TestDuplexMessageRouter(new Lazy<IMessageBroker>(() => messageBroker));
            var message = Substitute.For<IBrokeredMessage>();
            message.ReplyToMessageId.Returns((string)null);
            message.Id.Returns("gigi");
            message.IsOneWay.Returns(false);

            var dispatchReply = Substitute.For<IMessage>();
            messageBroker.DispatchAsync(message, Arg.Any<IContext>(), Arg.Any<CancellationToken>())
                .Returns(dispatchReply);
            messageBroker.CreateBrokeredMessageBuilder(Arg.Any<IContext>())
                .Returns(ci =>
                {
                    var builder = new BrokeredMessageBuilder(Substitute.For<IAppManifest>(), Substitute.For<IAuthenticationService>());
                    builder.Initialize(Substitute.For<IContext>());
                    return builder;
                });

            IBrokeredMessage receivedReply = null;
            router.ReplyReceived += (s, e) => receivedReply = e.Message;
            router.Receive(message, Substitute.For<IContext>());

            Assert.AreEqual(1, router.Out.Count);
            var brokeredReply = router.Out.Dequeue();

            Assert.AreSame(dispatchReply, brokeredReply.Content);
            Assert.AreEqual("gigi", brokeredReply.ReplyToMessageId);
            Assert.IsNull(receivedReply);
        }

        public class TestDuplexMessageRouter : DuplexMessageRouterBase
        {
            public TestDuplexMessageRouter(Lazy<IMessageBroker> lazyMessageBroker)
                : base(lazyMessageBroker)
            {
            }

            public Queue<IBrokeredMessage> In { get; } = new Queue<IBrokeredMessage>();

            public Queue<IBrokeredMessage> Out { get; } = new Queue<IBrokeredMessage>();

            public void Receive(IBrokeredMessage message, IContext context)
            {
                this.RouteInputAsync(message, context, default).WaitNonLocking();
            }

            protected override Task RouteOutputAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
            {
                this.Out.Enqueue(brokeredMessage);
                return TaskHelper.CompletedTask;
            }
        }
    }
}
