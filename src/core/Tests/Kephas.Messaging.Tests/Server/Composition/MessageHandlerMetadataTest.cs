// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandlerMetadataTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="MessageHandlerMetadata" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Server.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Messaging.Server.Composition;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="MessageHandlerMetadata"/>
    /// </summary>
    [TestFixture]
    public class MessageHandlerMetadataTest
    {
        [Test]
        public void MessageHandlerMetadata_metadata_null()
        {
            var instance = new MessageHandlerMetadata(null);
            var messageType = instance.MessageType;
            Assert.AreEqual(messageType, null);
        }

        [Test]
        public void MessageHandlerMetadata_metadata_valid()
        {
            var metadata = new Dictionary<string, object> { { MessageHandlerMetadata.MessageTypeKey, typeof(string) } };
            var instance = new MessageHandlerMetadata(metadata);
            var messageType = instance.MessageType;
            Assert.AreEqual(messageType, typeof(string));
        }

        [Test]
        [ExpectedException(typeof(InvalidCastException))]
        public void MessageHandlerMetadata_metadata_type_mismatch()
        {
            var metadata = new Dictionary<string, object> { { MessageHandlerMetadata.MessageTypeKey, "hello" } };
            var instance = new MessageHandlerMetadata(metadata);
        }

        [Test]
        public void MessageHandlerMetadata_metadata_type()
        {
            var instance = new MessageHandlerMetadata(typeof(int));
            var result = instance.MessageType;
            Assert.AreEqual(typeof(int), result);
        }
    }
}
