// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessageTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the brokered message test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Distributed
{
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Ping;

    using NUnit.Framework;

    [TestFixture]
    public class BrokeredMessageTest
    {
        [Test]
        public void ToString_success()
        {
            var message = new BrokeredMessage
                              {
                                  Content = new PingMessage(),
                                  Sender = new Endpoint { AppInstanceId = "app-instance" },
                                  Recipients =
                                      new[]
                                          {
                                              new Endpoint { EndpointId = "endpoint1" },
                                              new Endpoint { AppId = "app1" }
                                          }
                              };

            var resultFormat = "BrokeredMessage (#{0}) {{PingMessage/app-instance > endpoint1,app1}}";
            var result = message.ToString();

            Assert.AreEqual(string.Format(resultFormat, message.Id), result);
        }
    }
}