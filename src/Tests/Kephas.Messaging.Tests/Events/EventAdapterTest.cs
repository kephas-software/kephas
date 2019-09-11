// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageAdapterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message adapter test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Events
{
    using System;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.Messages;
    using NUnit.Framework;

    [TestFixture]
    public class EventAdapterTest
    {
        [Test]
        public void GetMessage_not_set()
        {
            var adapter = new EventAdapter();
            Assert.Throws<InvalidOperationException>(() => (adapter as IMessageAdapter).GetMessage());
        }
    }
}
