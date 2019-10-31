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
    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Security.Authentication;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class MessageRouterBaseTest : MefMessagingTestBase
    {
        [Test]
        public async Task DispatchAsync_calls_RouteOutputAsync()
        {
            var router = new TestMessageRouter(Substitute.For<IMessageProcessor>());
            var message = Substitute.For<IBrokeredMessage>();
            IBrokeredMessage receivedReply = null;
            router.ReplyReceived += (s, e) => receivedReply = e.Message;
            var (action, reply) = await router.DispatchAsync(message, Substitute.For<IContext>(), default);

            Assert.Contains(message, router.Out);
            Assert.AreEqual(RoutingInstruction.None, action);
            Assert.IsNull(reply);
            Assert.IsNull(receivedReply);
        }

        [Test]
        public async Task Receive_reply_raises_ReplyReceived_event()
        {
            var router = new TestMessageRouter(Substitute.For<IMessageProcessor>());
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
            var messageProcessor = Substitute.For<IMessageProcessor>();
            var router = new TestMessageRouter(messageProcessor);

            var message = Substitute.For<IBrokeredMessage>();
            message.ReplyToMessageId.Returns((string)null);
            message.Id.Returns("gigi");
            message.IsOneWay.Returns(false);
            var content = Substitute.For<IMessage>();
            message.Content.Returns(content);

            var dispatchReply = Substitute.For<IMessage>();
            messageProcessor.ProcessAsync(content, Arg.Any<Action<IMessagingContext>>(), Arg.Any<CancellationToken>())
                .Returns(dispatchReply);

            IBrokeredMessage receivedReply = null;
            router.ReplyReceived += (s, e) => receivedReply = e.Message;
            router.Receive(message, new Context(this.CreateSubstituteContainer()));

            Assert.AreEqual(1, router.Out.Count);
            var brokeredReply = router.Out.Dequeue();

            Assert.AreSame(dispatchReply, brokeredReply.Content);
            Assert.AreEqual("gigi", brokeredReply.ReplyToMessageId);
            Assert.IsNull(receivedReply);
        }

        public class TestMessageRouter : MessageRouterBase
        {
            public TestMessageRouter(IMessageProcessor messageProcessor)
                : base(messageProcessor, CreateMessageBuilderFactory())
            {
            }

            private static IExportFactory<IBrokeredMessageBuilder> CreateMessageBuilderFactory()
            {
                return new ExportFactory<IBrokeredMessageBuilder>(() =>
                {
                    var builder = new BrokeredMessageBuilder(
                        Substitute.For<IAppRuntime>(),
                        Substitute.For<IAuthenticationService>(),
                        Substitute.For<IContext>());
                    return builder;
                });
            }

            public Queue<IBrokeredMessage> In { get; } = new Queue<IBrokeredMessage>();

            public Queue<IBrokeredMessage> Out { get; } = new Queue<IBrokeredMessage>();

            public void Receive(IBrokeredMessage message, IContext context)
            {
                this.RouteInputAsync(message, context, default).WaitNonLocking();
            }

            protected override async Task<(RoutingInstruction action, IMessage reply)> RouteOutputAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
            {
                this.Out.Enqueue(brokeredMessage);
                return (RoutingInstruction.None, null);
            }
        }
    }
}
