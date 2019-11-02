// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessageTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the brokered message test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Distributed
{
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.Messages;
    using NSubstitute;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class BrokeredMessageTest
    {
        [Test]
        public void ToString_success()
        {
            var message = new BrokeredMessage
                              {
                                  Content = new PingMessage(),
                                  Sender = new Endpoint(appInstanceId: "app-instance"),
                                  Recipients =
                                      new[]
                                          {
                                              new Endpoint(endpointId: "endpoint1"),
                                              new Endpoint(appId: "app1"),
                                          },
                              };

            var resultFormat = "BrokeredMessage (#{0}) {{PingMessage/app://.//app-instance/ > app://.///endpoint1,app://./app1//}}, Normal priority";
            var result = message.ToString();

            Assert.AreEqual(string.Format(resultFormat, message.Id), result);
        }

        [Test]
        public void Content_delegate_not_allowed()
        {
            var message = new BrokeredMessage();
            Assert.Throws<ArgumentException>(() => message.Content = new MessageEnvelope { Message = (Func<string, string>)((string e) => e) });
        }

        [Test]
        public void Content_event_one_way()
        {
            var message = new BrokeredMessage(Substitute.For<IEvent>());
            Assert.IsTrue(message.IsOneWay);
        }

        [Test]
        public void Content_message_two_way()
        {
            var message = new BrokeredMessage(Substitute.For<IMessage>());
            Assert.IsFalse(message.IsOneWay);
        }

        [Test]
        public void Content_not_null()
        {
            var message = new BrokeredMessage();
            Assert.Throws<ArgumentNullException>(() => message.Content = null);
        }

        [Test]
        public void Content_null_when_reply()
        {
            var message = new BrokeredMessage();
            message.ReplyToMessageId = "hello";
            message.Content = null;

            Assert.IsNull(message.Content);
        }
    }
}