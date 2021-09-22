// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingBehaviorMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Composition metadata for <see cref="IMessageProcessingBehavior" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Behaviors.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Messaging.Behaviors;
    using Kephas.Messaging.Composition;
    using Kephas.Services;

    /// <summary>
    /// Composition metadata for <see cref="IMessagingBehavior"/>.
    /// </summary>
    public class MessagingBehaviorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public MessagingBehaviorMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.MessageType = (Type)metadata.TryGetValue(nameof(this.MessageType));
            this.MessageTypeMatching = metadata.TryGetValue(nameof(this.MessageTypeMatching)) as MessageTypeMatching? ?? MessageTypeMatching.TypeOrHierarchy;
            this.MessageId = metadata.TryGetValue(nameof(this.MessageId));
            this.MessageIdMatching = metadata.TryGetValue(nameof(this.MessageIdMatching)) as MessageIdMatching? ?? MessageIdMatching.All;

            this.MessageMatch = new MessageMatch
                                    {
                                        MessageType = this.MessageType,
                                        MessageTypeMatching = this.MessageTypeMatching,
                                        MessageId = this.MessageId,
                                        MessageIdMatching = this.MessageIdMatching,
                                    };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageTypeMatching">The message type matching (optional).</param>
        /// <param name="messageId">The ID of the message (optional).</param>
        /// <param name="messageIdMatching">The message ID matching (optional).</param>
        /// <param name="processingPriority">The processing priority (optional).</param>
        /// <param name="overridePriority">The override priority (optional).</param>
        public MessagingBehaviorMetadata(Type messageType, MessageTypeMatching messageTypeMatching = MessageTypeMatching.TypeOrHierarchy, object? messageId = null, MessageIdMatching messageIdMatching = MessageIdMatching.All, Priority processingPriority = 0, Priority overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            this.MessageType = messageType;
            this.MessageTypeMatching = messageTypeMatching;
            this.MessageId = messageId;
            this.MessageIdMatching = messageIdMatching;

            this.MessageMatch = new MessageMatch
                                    {
                                        MessageType = this.MessageType,
                                        MessageTypeMatching = this.MessageTypeMatching,
                                        MessageId = this.MessageId,
                                        MessageIdMatching = this.MessageIdMatching,
                                    };
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
        public object? MessageId { get; }

        /// <summary>
        /// Gets the message ID matching.
        /// </summary>
        /// <value>
        /// The message ID matching.
        /// </value>
        public MessageIdMatching MessageIdMatching { get; }

        /// <summary>
        /// Gets the message match.
        /// </summary>
        /// <value>
        /// The message match.
        /// </value>
        public IMessageMatch MessageMatch { get; }
    }
}