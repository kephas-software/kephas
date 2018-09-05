// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessageBuilderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the brokered message builder test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Distributed
{
    using Kephas.Application;
    using Kephas.Messaging.Distributed;
    using Kephas.Security;
    using Kephas.Security.Authentication;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class BrokeredMessageBuilderTest
    {
        [Test]
        public void OneWay()
        {
            var builder = new BrokeredMessageBuilder<BrokeredMessage>(Substitute.For<IAppManifest>(), Substitute.For<IAuthenticationService>());
            var message = builder.OneWay().BrokeredMessage;

            Assert.IsTrue(message.IsOneWay);
        }

        [Test]
        public void WithContent()
        {
            var builder = new BrokeredMessageBuilder<BrokeredMessage>(Substitute.For<IAppManifest>(), Substitute.For<IAuthenticationService>());
            var content = Substitute.For<IMessage>();
            var message = builder.WithContent(content).BrokeredMessage;

            Assert.AreSame(content, message.Content);
        }

        [Test]
        public void WithSender_sender_id()
        {
            var appManifest = Substitute.For<IAppManifest>();
            appManifest.AppId.Returns("app-id");
            appManifest.AppInstanceId.Returns("app-instance-id");

            var builder = new BrokeredMessageBuilder<BrokeredMessage>(appManifest, Substitute.For<IAuthenticationService>());
            var message = builder.WithSender("123").BrokeredMessage;

            Assert.AreEqual("123", message.Sender.EndpointId);
            Assert.AreEqual("app-id", message.Sender.AppId);
            Assert.AreEqual("app-instance-id", message.Sender.AppInstanceId);
        }
    }
}