// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InProcessMessageBrokerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in process message broker test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Distributed.Routing
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Messages;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class InProcessMessageRouterTest : MefMessagingTestBase
    {
        [Test]
        public void InProcessMessageRouter_Composition_success()
        {
            var container = this.CreateContainer();
            var messageRouters = container.GetExports<IMessageRouter>();
            Assert.IsTrue(messageRouters.OfType<InProcessMessageRouter>().Any());
        }

        [Test]
        public async Task DispatchAsync_with_request()
        {
            var container = this.CreateContainer();
            using (var inProcessRouter = container.GetExports<IMessageRouter>().OfType<InProcessMessageRouter>().Single())
            {
                inProcessRouter.Initialize(new Context(container));
                var messageBroker = container.GetExport<IMessageBroker>();
                await (messageBroker as IAsyncInitializable).InitializeAsync(new Context(container));

                ReplyReceivedEventArgs eventArgs = null;
                inProcessRouter.ReplyReceived += (s, e) => eventArgs = e;
                var request = new BrokeredMessage { Content = new PingMessage() };
                var result = await inProcessRouter.DispatchAsync(request, Substitute.For<IDispatchingContext>(), default);

                Thread.Sleep(100);
                Assert.IsNull(eventArgs);
                Assert.AreEqual(RoutingInstruction.Reply, result.action);
                Assert.IsInstanceOf<PingBackMessage>(result.reply);
            }
        }

        [Test]
        public async Task DispatchAsync_with_reply()
        {
            var container = this.CreateContainer();
            using (var inProcessRouter = container.GetExports<IMessageRouter>().OfType<InProcessMessageRouter>().Single())
            {
                inProcessRouter.Initialize(new Context(container));

                ReplyReceivedEventArgs eventArgs = null;
                inProcessRouter.ReplyReceived += (s, e) => eventArgs = e;
                var context = Substitute.For<IDispatchingContext>();
                var reply = new BrokeredMessage { Content = new PingBackMessage(), ReplyToMessageId = "hello" };
                var result = await inProcessRouter.DispatchAsync(reply, context, default);

                Thread.Sleep(100);
                Assert.AreSame(reply, eventArgs.Message);
                Assert.AreSame(context, eventArgs.Context);
                Assert.AreEqual(RoutingInstruction.None, result.action);
                Assert.IsNull(result.reply);
            }
        }
    }
}