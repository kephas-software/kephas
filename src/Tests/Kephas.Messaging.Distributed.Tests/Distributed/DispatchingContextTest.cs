// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessageBuilderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the brokered message builder test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;
using Kephas.Services;

namespace Kephas.Messaging.Tests.Distributed
{
    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Messaging.Distributed;
    using Kephas.Security;
    using Kephas.Security.Authentication;

    using NSubstitute;

    using NUnit.Framework;
    using System;

    [TestFixture]
    public class DispatchingContextTest
    {
        [Test]
        public void OneWay()
        {
            var builder = new DispatchingContext(
                Substitute.For<IInjector>(),
                Substitute.For<IConfiguration<DistributedMessagingSettings>>(),
                Substitute.For<IMessageBroker>(),
                Substitute.For<IAppRuntime>(),
                Substitute.For<IAuthenticationService>());
            var message = builder.OneWay().BrokeredMessage;

            Assert.IsTrue(message.IsOneWay);
        }

        [Test]
        public void Content()
        {
            var builder = new DispatchingContext(
                Substitute.For<IInjector>(),
                Substitute.For<IConfiguration<DistributedMessagingSettings>>(),
                Substitute.For<IMessageBroker>(),
                Substitute.For<IAppRuntime>(),
                Substitute.For<IAuthenticationService>());
            var content = Substitute.For<IMessage>();
            var message = builder.Content(content).BrokeredMessage;

            Assert.AreSame(content, message.Content);
        }

        [Test]
        public void From_sender_id()
        {
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime[IAppRuntime.AppIdKey].Returns("app-id");
            appRuntime[IAppRuntime.AppInstanceIdKey].Returns("app-instance-id");

            var builder = new DispatchingContext(
                Substitute.For<IInjector>(),
                Substitute.For<IConfiguration<DistributedMessagingSettings>>(),
                Substitute.For<IMessageBroker>(),
                appRuntime,
                Substitute.For<IAuthenticationService>());
            var message = builder.From("123").BrokeredMessage;

            Assert.AreEqual("123", message.Sender.EndpointId);
            Assert.AreEqual("app-id", message.Sender.AppId);
            Assert.AreEqual("app-instance-id", message.Sender.AppInstanceId);
        }

        [Test]
        public void Timeout_from_config()
        {
            var config = Substitute.For<IConfiguration<DistributedMessagingSettings>>();
            config.GetSettings(Arg.Any<IContext>()).Returns(ci => new DistributedMessagingSettings { DefaultTimeout = TimeSpan.FromMinutes(10) });

            var builder = new DispatchingContext(
                Substitute.For<IInjector>(),
                config,
                Substitute.For<IMessageBroker>(),
                Substitute.For<IAppRuntime>(),
                Substitute.For<IAuthenticationService>());
            var message = builder.BrokeredMessage;

            Assert.AreEqual(TimeSpan.FromMinutes(10), message.Timeout);
        }

        [Test]
        public void Timeout_from_default()
        {
            var config = Substitute.For<IConfiguration<DistributedMessagingSettings>>();
            config.GetSettings(Arg.Any<IContext>()).Returns(ci => null);

            var builder = new DispatchingContext(
                Substitute.For<IInjector>(),
                config,
                Substitute.For<IMessageBroker>(),
                Substitute.For<IAppRuntime>(),
                Substitute.For<IAuthenticationService>());
            var message = builder.BrokeredMessage;

            Assert.AreEqual(BrokeredMessage.DefaultTimeout, message.Timeout);
        }
    }
}