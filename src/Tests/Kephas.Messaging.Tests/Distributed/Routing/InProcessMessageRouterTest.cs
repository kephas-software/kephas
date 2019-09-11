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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Composition;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Messages;
    using Kephas.Services;
    using Kephas.Testing.Composition.Mef;

    using NUnit.Framework;

    [TestFixture]
    public class InProcessMessageRouterTest : CompositionTestBase
    {
        public override ICompositionContext CreateContainer(
            IAmbientServices ambientServices = null,
            IEnumerable<Assembly> assemblies = null,
            IEnumerable<Type> parts = null,
            Action<MefCompositionContainerBuilder> config = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? new Assembly[0]);
            assemblyList.Add(typeof(IMessageProcessor).GetTypeInfo().Assembly); /* Kephas.Messaging */
            return base.CreateContainer(ambientServices, assemblyList, parts, config);
        }

        [Test]
        public void InProcessMessageRouter_Composition_success()
        {
            var container = this.CreateContainer();
            var messageRouters = container.GetExports<IMessageRouter>();
            Assert.IsTrue(messageRouters.OfType<InProcessMessageRouter>().Any());
        }

        [Test]
        public async Task SendAsync_with_request()
        {
            var container = this.CreateContainer();
            var inProcessRouter = container.GetExports<IMessageRouter>().OfType<InProcessMessageRouter>().Single();

            ReplyReceivedEventArgs eventArgs = null;
            inProcessRouter.ReplyReceived += (s, e) => eventArgs = e;
            var request = new BrokeredMessage { Content = new PingMessage() };
            var result = await inProcessRouter.SendAsync(request, new Context(container), default);

            Thread.Sleep(100);
            Assert.IsNull(eventArgs);
            Assert.AreEqual(RoutingInstruction.Reply, result.action);
            Assert.IsInstanceOf<PingBackMessage>(result.reply);
        }

        [Test]
        public async Task SendAsync_with_reply()
        {
            var container = this.CreateContainer();
            var inProcessRouter = container.GetExports<IMessageRouter>().OfType<InProcessMessageRouter>().Single();

            ReplyReceivedEventArgs eventArgs = null;
            inProcessRouter.ReplyReceived += (s, e) => eventArgs = e;
            var context = new Context(container);
            var reply = new BrokeredMessage { Content = new PingBackMessage(), ReplyToMessageId = "hello" };
            var result = await inProcessRouter.SendAsync(reply, context, default);

            Thread.Sleep(100);
            Assert.AreSame(reply, eventArgs.Message);
            Assert.AreSame(context, eventArgs.Context);
            Assert.AreEqual(RoutingInstruction.None, result.action);
            Assert.IsNull(result.reply);
        }
    }
}