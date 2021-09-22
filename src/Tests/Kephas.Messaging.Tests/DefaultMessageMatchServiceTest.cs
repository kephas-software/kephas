// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageMatchServiceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default message match service test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests
{
    using Kephas.Data;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.Messages;
    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultMessageMatchServiceTest
    {
        [Test]
        public void GetMessageType_adapter()
        {
            var matchService = new DefaultMessageMatchService();
            var message = Substitute.For<IMessageEnvelope>();
            message.GetContent().Returns("gigi");

            Assert.AreSame(typeof(string), matchService.GetMessageType(message));
        }

        [Test]
        public void GetMessageType_ping_message()
        {
            var matchService = new DefaultMessageMatchService();
            var message = new PingMessage();

            Assert.AreSame(typeof(PingMessage), matchService.GetMessageType(message));
        }

        [Test]
        public void IsMatch_id_exact()
        {
            var matchService = new DefaultMessageMatchService();
            var match = new MessageMatch { MessageId = "123", MessageIdMatching = MessageIdMatching.Id };
            Assert.IsTrue(matchService.IsMatch(match, null, null, "123"));
            Assert.IsFalse(matchService.IsMatch(match, null, null, 123));
            Assert.IsFalse(matchService.IsMatch(match, null, null, "1234"));
        }

        [Test]
        public void IsMatch_id_all()
        {
            var matchService = new DefaultMessageMatchService();
            var match = new MessageMatch { MessageId = "123", MessageIdMatching = MessageIdMatching.All };
            Assert.IsTrue(matchService.IsMatch(match, null, null, "123"));
            Assert.IsTrue(matchService.IsMatch(match, null, null, 123));
            Assert.IsTrue(matchService.IsMatch(match, null, null, "1234"));
        }

        [Test]
        public void IsMatch_type_exact()
        {
            var matchService = new DefaultMessageMatchService();
            var match = new MessageMatch { MessageType = typeof(IMessage), MessageTypeMatching = MessageTypeMatching.Type };
            Assert.IsTrue(matchService.IsMatch(match, null, typeof(IMessage), null));
            Assert.IsFalse(matchService.IsMatch(match, null, typeof(IEvent), null));  // inherited
            Assert.IsFalse(matchService.IsMatch(match, null, typeof(string), null));
        }

        [Test]
        public void IsMatch_type_hierarchy()
        {
            var matchService = new DefaultMessageMatchService();
            var match = new MessageMatch { MessageType = typeof(IMessage), MessageTypeMatching = MessageTypeMatching.TypeOrHierarchy };
            Assert.IsTrue(matchService.IsMatch(match, null, typeof(IMessage), null));
            Assert.IsTrue(matchService.IsMatch(match, null, typeof(IEvent), null));  // inherited
            Assert.IsFalse(matchService.IsMatch(match, null, typeof(string), null));
        }

        [Test]
        public void IsMatch_envelope_type_exact()
        {
            var matchService = new DefaultMessageMatchService();
            var match = new MessageMatch { EnvelopeType = typeof(int), EnvelopeTypeMatching = MessageTypeMatching.Type };
            Assert.IsFalse(matchService.IsMatch(match, typeof(IMessage), null, null));
            Assert.IsFalse(matchService.IsMatch(match, typeof(IEvent), null, null));  // inherited
            Assert.IsTrue(matchService.IsMatch(match, typeof(int), null, null));
        }

        [Test]
        public void IsMatch_envelope_type_hierarchy()
        {
            var matchService = new DefaultMessageMatchService();
            var match = new MessageMatch { EnvelopeType = typeof(IMessage), EnvelopeTypeMatching = MessageTypeMatching.TypeOrHierarchy };
            Assert.IsTrue(matchService.IsMatch(match, typeof(IMessage), null, null));
            Assert.IsTrue(matchService.IsMatch(match, typeof(PingMessage), null, null));  // inherited
            Assert.IsFalse(matchService.IsMatch(match, typeof(string), null, null));
        }

        [Test]
        public void GetMessageId_non_identifiable_message_is_null()
        {
            var message = Substitute.For<INonIdentifiableMessage, IIdentifiable>();
            ((IIdentifiable)message).Id.Returns("abcd");
            var matchService = new DefaultMessageMatchService();
            var messageId = matchService.GetMessageId(message);
            Assert.IsNull(messageId);
        }

        [Test]
        public void GetMessageId_message_is_not_null()
        {
            var message = Substitute.For<IMessage, IIdentifiable>();
            ((IIdentifiable)message).Id.Returns("abcd");
            var matchService = new DefaultMessageMatchService();
            var messageId = matchService.GetMessageId(message);
            Assert.AreEqual("abcd", (string)messageId);
        }

        [Test]
        public void GetMessageId_message_adapter_is_not_null()
        {
            var message = Substitute.For<IMessage, IIdentifiable>();
            ((IIdentifiable)message).Id.Returns("abcd");
            var adapter = Substitute.For<IMessageEnvelope>();
            adapter.GetContent().Returns(message);
            var matchService = new DefaultMessageMatchService();
            var messageId = matchService.GetMessageId(adapter);
            Assert.AreEqual("abcd", (string)messageId);
        }
    }
}