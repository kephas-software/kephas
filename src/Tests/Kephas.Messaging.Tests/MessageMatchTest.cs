// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageMatchTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message match test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests
{
    using Kephas.Messaging.Events;

    using NUnit.Framework;

    [TestFixture]
    public class MessageMatchTest
    {
        [Test]
        public void ToString_type_and_id()
        {
            var match = new MessageMatch { MessageType = typeof(IEvent), MessageTypeMatching = MessageTypeMatching.TypeOrHierarchy };
            var matchString = match.ToString();
            Assert.AreEqual("TypeOrHierarchy/Kephas.Messaging.Events.IEvent:All/", matchString);
        }
    }
}