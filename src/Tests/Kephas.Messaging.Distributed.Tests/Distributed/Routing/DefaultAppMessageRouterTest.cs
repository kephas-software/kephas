// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppMessageRouterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Distributed.Routing;

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
public class DefaultAppMessageRouterTest : MessagingTestBase
{
    [Test]
    public void DefaultAppMessageRouter_Injection_success()
    {
        var container = this.CreateInjector();
        var messageRouters = container.ResolveMany<IMessageRouter>();
        Assert.IsTrue(messageRouters.OfType<DefaultAppMessageRouter>().Any());
    }

    [Test]
    public async Task DispatchAsync_with_request()
    {
        var container = this.CreateInjector();
        using (var inProcessRouter = container.ResolveMany<IMessageRouter>().OfType<DefaultAppMessageRouter>().Single())
        {
            var appContext = new Context(container);
            await ServiceHelper.InitializeAsync(inProcessRouter, appContext);

            ReplyReceivedEventArgs eventArgs = null;
            inProcessRouter.ReplyReceived += (s, e) => eventArgs = e;
            var request = new BrokeredMessage { Content = new PingMessage() };
            var result = await inProcessRouter.DispatchAsync(request, Substitute.For<IDispatchingContext>(), default);

            await Task.Delay(200);
            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(RoutingInstruction.None, result.action);
            var contentMessage = eventArgs.Message.Content;
            if (contentMessage is ExceptionResponseMessage exMessage)
            {
                Assert.Fail(exMessage.Exception.ToString());
            }

            Assert.IsInstanceOf<PingBackMessage>(eventArgs.Message.Content);
        }
    }

    [Test]
    public async Task DispatchAsync_with_reply()
    {
        var container = this.CreateInjector();
        using (var inProcessRouter = container.ResolveMany<IMessageRouter>().OfType<DefaultAppMessageRouter>().Single())
        {
            var appContext = new Context(container);
            await ServiceHelper.InitializeAsync(inProcessRouter, appContext);

            ReplyReceivedEventArgs eventArgs = null;
            inProcessRouter.ReplyReceived += (s, e) => eventArgs = e;
            var context = Substitute.For<IDispatchingContext>();
            var reply = new BrokeredMessage { Content = new PingBackMessage(), ReplyToMessageId = "hello" };
            var result = await inProcessRouter.DispatchAsync(reply, context, default);

            Thread.Sleep(100);
            Assert.AreSame(reply, eventArgs.Message);
            Assert.AreSame(appContext, eventArgs.Context);
            Assert.AreEqual(RoutingInstruction.None, result.action);
            Assert.IsNull(result.reply);
        }
    }
}