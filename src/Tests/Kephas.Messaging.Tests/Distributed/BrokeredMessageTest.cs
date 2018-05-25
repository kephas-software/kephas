// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokeredMessageTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the brokered message test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Distributed
{
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Messages;

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
                                  Sender = new Endpoint(appInstanceId: "app-instance"),
                                  Recipients =
                                      new[]
                                          {
                                              new Endpoint(endpointId: "endpoint1"),
                                              new Endpoint(appId: "app1")
                                          }
                              };

            var resultFormat = "BrokeredMessage (#{0}) {{PingMessage/app://.//app-instance/ > app://.///endpoint1,app://./app1//}}";
            var result = message.ToString();

            Assert.AreEqual(string.Format(resultFormat, message.Id), result);
        }
    }
}