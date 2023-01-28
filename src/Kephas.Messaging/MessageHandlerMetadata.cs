// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandlerMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Composition metadata for <see cref="IMessageHandler" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;
    using System.Collections.Generic;
    using Kephas.Collections;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Injection metadata for <see cref="IMessageHandler"/>.
    /// </summary>
    public class MessageHandlerMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public MessageHandlerMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                this.MessageMatch = new MessageMatch();
                return;
            }

            this.SetMessageType((Type?)metadata.TryGetValue(nameof(this.MessageType)));
            this.MessageTypeMatching = metadata.TryGetValue(nameof(this.MessageTypeMatching)) as MessageTypeMatching? ?? default;
            this.MessageId = metadata.TryGetValue(nameof(this.MessageId));
            this.MessageIdMatching = metadata.TryGetValue(nameof(this.MessageIdMatching)) as MessageIdMatching? ?? default;
            this.EnvelopeType = (Type?)metadata.TryGetValue(nameof(this.EnvelopeType));
            this.EnvelopeTypeMatching = metadata.TryGetValue(nameof(this.EnvelopeTypeMatching)) as MessageTypeMatching? ?? default;

            this.MessageMatch = new MessageMatch
                                    {
                                        MessageType = this.MessageType,
                                        MessageTypeMatching = this.MessageTypeMatching,
                                        MessageId = this.MessageId,
                                        MessageIdMatching = this.MessageIdMatching,
                                        EnvelopeType = this.EnvelopeType,
                                        EnvelopeTypeMatching = this.EnvelopeTypeMatching,
                                    };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerMetadata"/> class.
        /// </summary>
        /// <param name="messageType">Optional. Type of the message.</param>
        /// <param name="messageTypeMatching">Optional. The message type matching.</param>
        /// <param name="messageId">Optional. The ID of the message.</param>
        /// <param name="messageIdMatching">Optional. The message ID matching.</param>
        /// <param name="envelopeType">Optional. Type of the envelope.</param>
        /// <param name="envelopeTypeMatching">Optional. The envelope type matching.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        public MessageHandlerMetadata(Type? messageType = null, MessageTypeMatching messageTypeMatching = default, object? messageId = null, MessageIdMatching messageIdMatching = default, Type? envelopeType = null, MessageTypeMatching envelopeTypeMatching = default, Priority processingPriority = 0, Priority overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            this.SetMessageType(messageType);
            this.MessageTypeMatching = messageTypeMatching;
            this.MessageId = messageId;
            this.MessageIdMatching = messageIdMatching;
            this.EnvelopeType = envelopeType;
            this.EnvelopeTypeMatching = envelopeTypeMatching;

            this.MessageMatch = new MessageMatch
                                    {
                                        MessageType = this.MessageType,
                                        MessageTypeMatching = this.MessageTypeMatching,
                                        MessageId = this.MessageId,
                                        MessageIdMatching = this.MessageIdMatching,
                                        EnvelopeType = this.EnvelopeType,
                                        EnvelopeTypeMatching = this.EnvelopeTypeMatching,
                                    };
        }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        public Type? MessageType { get; private set; }

        /// <summary>
        /// Gets the type of the response.
        /// </summary>
        /// <value>
        /// The type of the response.
        /// </value>
        public Type? ResponseType { get; private set; }

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
        /// Gets the type of the envelope.
        /// </summary>
        /// <value>
        /// The type of the envelope.
        /// </value>
        public Type? EnvelopeType { get; }

        /// <summary>
        /// Gets the envelope type matching.
        /// </summary>
        /// <value>
        /// The envelope type matching.
        /// </value>
        public MessageTypeMatching EnvelopeTypeMatching { get; }

        /// <summary>
        /// Gets the message match.
        /// </summary>
        /// <value>
        /// The message match.
        /// </value>
        public IMessageMatch MessageMatch { get; }

        private void SetMessageType(Type? messageType)
        {
            this.MessageType = messageType;

            if (messageType is not null)
            {
                var closedMessageType = messageType.GetBaseConstructedGenericOf(typeof(IMessage<>));
                this.ResponseType = closedMessageType?.GenericTypeArguments[0];
            }
        }
    }
}