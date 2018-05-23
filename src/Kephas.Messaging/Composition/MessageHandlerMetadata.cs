// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandlerMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Composition metadata for <see cref="IMessageHandler" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services.Composition;

    /// <summary>
    /// Composition metadata for <see cref="IMessageHandler"/>.
    /// </summary>
    public class MessageHandlerMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public MessageHandlerMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.MessageType = (Type)metadata.TryGetValue(nameof(this.MessageType));
            this.MessageTypeMatching = metadata.TryGetValue(nameof(this.MessageTypeMatching)) as MessageTypeMatching? ?? default;
            this.MessageId = metadata.TryGetValue(nameof(this.MessageId));
            this.MessageIdMatching = metadata.TryGetValue(nameof(this.MessageIdMatching)) as MessageIdMatching? ?? default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerMetadata"/> class.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageTypeMatching">The message type matching (optional).</param>
        /// <param name="messageId">The ID of the message (optional).</param>
        /// <param name="messageIdMatching">The message ID matching (optional).</param>
        /// <param name="processingPriority">The processing priority (optional).</param>
        /// <param name="overridePriority">The override priority (optional).</param>
        public MessageHandlerMetadata(Type messageType, MessageTypeMatching messageTypeMatching = default, object messageId = null, MessageIdMatching messageIdMatching = default, int processingPriority = 0, int overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            this.MessageType = messageType;
            this.MessageTypeMatching = messageTypeMatching;
            this.MessageId = messageId;
            this.MessageIdMatching = messageIdMatching;
        }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        public Type MessageType { get; }

        /// <summary>
        /// Gets the message type matching.
        /// </summary>
        /// <value>
        /// The message type matching.
        /// </value>
        public MessageTypeMatching MessageTypeMatching { get; }

        /// <summary>
        /// Gets the ID of the message.
        /// </summary>
        /// <value>
        /// The ID of the message.
        /// </value>
        public object MessageId { get; }

        /// <summary>
        /// Gets the message ID matching.
        /// </summary>
        /// <value>
        /// The message ID matching.
        /// </value>
        public MessageIdMatching MessageIdMatching { get; }
    }
}