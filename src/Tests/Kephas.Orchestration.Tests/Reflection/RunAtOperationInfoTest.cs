// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunAtOperationInfoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Tests.Reflection
{
    using System;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Commands;
    using Kephas.Commands.Endpoints;
    using Kephas.Configuration;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Reflection;
    using Kephas.Security.Authentication;
    using Kephas.Services;
    using Kephas.Tests.Orchestration;
    using Kephas.Threading.Tasks;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class RunAtOperationInfoTest : OrchestrationTestBase
    {
        [Test]
        public void RunAtOperationInfo_runat_appinstanceid()
        {
            var broker = Substitute.For<IMessageBroker>();
            var opInfo = new RunAtOperationInfo(
                new Lazy<IMessageBroker>(() => broker),
                "webapp",
                "help",
                null);

            Assert.AreEqual(opInfo.Endpoint, new Endpoint(appInstanceId: "webapp"));
        }

        [Test]
        public void RunAtOperationInfo_runat_uri_as_string()
        {
            var broker = Substitute.For<IMessageBroker>();
            var opInfo = new RunAtOperationInfo(
                new Lazy<IMessageBroker>(() => broker),
                "app://./webapp",
                "help",
                null);

            Assert.AreEqual(opInfo.Endpoint, new Endpoint(new Uri("app://./webapp")));
        }

        [Test]
        public void RunAtOperationInfo_runat_uri()
        {
            var broker = Substitute.For<IMessageBroker>();
            var opInfo = new RunAtOperationInfo(
                new Lazy<IMessageBroker>(() => broker),
                new Uri("app://./webapp"),
                "help",
                null);

            Assert.AreEqual(opInfo.Endpoint, new Endpoint(new Uri("app://./webapp")));
        }

        [Test]
        public void RunAtOperationInfo_runat_endpoint()
        {
            var broker = Substitute.For<IMessageBroker>();
            var opInfo = new RunAtOperationInfo(
                new Lazy<IMessageBroker>(() => broker),
                new Endpoint(appInstanceId: "webapp"),
                "help",
                null);

            Assert.AreEqual(opInfo.Endpoint, new Endpoint(appInstanceId: "webapp"));
        }

        [Test]
        public async Task Invoke()
        {
            var broker = Substitute.For<IMessageBroker>();
            var opInfo = new RunAtOperationInfo(
                new Lazy<IMessageBroker>(() => broker),
                new Endpoint(appInstanceId: "webapp"),
                "help",
                new Args { ["mycmd"] = string.Empty });

            var context = Substitute.For<IContext>();
            var identity = Substitute.For<IIdentity>();
            context.Identity.Returns(identity);
            var token = new CancellationToken(false);
            var opArgs = new object?[]
            {
                new Args { ["yourcmd"] = string.Empty },
                context,
                token,
            };

            broker.DispatchAsync(
                    Arg.Any<object>(),
                    Arg.Any<Action<IDispatchingContext>>(),
                    Arg.Any<CancellationToken>())
                .Returns(ci =>
                {
                    var dispContext = new DispatchingContext(
                        Substitute.For<IInjector>(),
                        Substitute.For<IConfiguration<DistributedMessagingSettings>>(),
                        broker,
                        Substitute.For<IAppRuntime>(),
                        Substitute.For<IAuthenticationService>());
                    var msg = (ExecuteCommandMessage)(ci.Arg<object>() is IMessage message ? message.GetContent() : ci.Arg<object>());

                    ci.Arg<Action<IDispatchingContext>>()(dispContext);
                    Assert.AreSame(identity, dispContext.Identity);
                    Assert.IsFalse(dispContext.BrokeredMessage.IsOneWay);

                    return Task.FromResult<IMessage>(new ExecuteCommandResponseMessage
                    {
                        ReturnValue = $"result for {msg.Command}({string.Join(" ", msg.Args.ToCommandArgs())})",
                    });
                });

            var resultTask = (Task)opInfo.Invoke(null, opArgs);
            await resultTask;
            var result = resultTask.GetResult();
            Assert.AreEqual("result for help(-yourcmd)", result);
        }

        [Test]
        public async Task Invoke_oneway()
        {
            var broker = Substitute.For<IMessageBroker>();
            var opInfo = new RunAtOperationInfo(
                new Lazy<IMessageBroker>(() => broker),
                new Endpoint(appInstanceId: "webapp"),
                "help",
                new Args { ["mycmd"] = string.Empty },
                isOneWay: true);

            var context = Substitute.For<IContext>();
            var identity = Substitute.For<IIdentity>();
            context.Identity.Returns(identity);
            var token = new CancellationToken(false);
            var opArgs = new object?[]
            {
                new Args { ["yourcmd"] = string.Empty },
                context,
                token,
            };

            broker.DispatchAsync(
                    Arg.Any<object>(),
                    Arg.Any<Action<IDispatchingContext>>(),
                    Arg.Any<CancellationToken>())
                .Returns(ci =>
                {
                    var dispContext = new DispatchingContext(
                        Substitute.For<IInjector>(),
                        Substitute.For<IConfiguration<DistributedMessagingSettings>>(),
                        broker,
                        Substitute.For<IAppRuntime>(),
                        Substitute.For<IAuthenticationService>());
                    var msg = (ExecuteCommandMessage)(ci.Arg<object>() is IMessage message ? message.GetContent() : ci.Arg<object>());

                    ci.Arg<Action<IDispatchingContext>>()(dispContext);
                    Assert.AreSame(identity, dispContext.Identity);
                    Assert.IsTrue(dispContext.BrokeredMessage.IsOneWay);

                    return Task.FromResult<IMessage>(null);
                });

            var resultTask = (Task)opInfo.Invoke(null, opArgs);
            await resultTask;
            var result = resultTask.GetResult();
            Assert.IsNull(result);
        }
    }
}