// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandlerMetadataTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="MessageHandlerMetadata" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Messaging.Composition;

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
            var metadata = new Dictionary<string, object> { { nameof(MessageHandlerMetadata.MessageType), typeof(string) } };
            var instance = new MessageHandlerMetadata(metadata);
            var messageType = instance.MessageType;
            Assert.AreEqual(messageType, typeof(string));
        }

        [Test]
        public void MessageHandlerMetadata_metadata_type_mismatch()
        {
            var metadata = new Dictionary<string, object> { { nameof(MessageHandlerMetadata.MessageType), "hello" } };
            Assert.Throws<InvalidCastException>(() => new MessageHandlerMetadata(metadata));
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
