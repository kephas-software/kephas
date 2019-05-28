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
    using Kephas.Messaging.Distributed;
    using Kephas.Services;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class MessageBrokerExtensionsTest
    {
        [Test]
        public void CreateBrokeredMessageBuilder_with_message_type()
        {
            var broker = Substitute.For<IMessageBroker>();
            var context = Substitute.For<IContext>();
            var expectedBuilder = Substitute.For<IBrokeredMessageBuilder>();
            broker.CreateBrokeredMessageBuilder<BrokeredMessage>(context).Returns(expectedBuilder);
            var builder = broker.CreateBrokeredMessageBuilder(typeof(BrokeredMessage), context);
            Assert.AreSame(expectedBuilder, builder);
        }
    }
}