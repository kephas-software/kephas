// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessageBuilderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the brokered message builder test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Distributed
{
    using Kephas.Messaging.Distributed;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class BrokeredMessageBuilderTest
    {
        [Test]
        public void OneWay()
        {
            var builder = new BrokeredMessageBuilder();
            var message = builder.OneWay().BrokeredMessage;

            Assert.IsTrue(message.IsOneWay);
        }

        [Test]
        public void WithContent()
        {
            var builder = new BrokeredMessageBuilder();
            var content = Substitute.For<IMessage>();
            var message = builder.WithContent(content).BrokeredMessage;

            Assert.AreSame(content, message.Content);
        }
    }
}