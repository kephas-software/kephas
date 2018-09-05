// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandlerAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message handler attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.AttributedModel
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging.Composition;
    using Kephas.Services;

    /// <summary>
    /// Adds message matching criteria for <see cref="IMessageHandler"/> services.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MessageHandlerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerAttribute"/> class.
        /// </summary>
        /// <param name="messageId">The ID of the handled messages.</param>
        public MessageHandlerAttribute(object messageId)
        {
            Requires.NotNull(messageId, nameof(messageId));

            this.MessageId = messageId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerAttribute"/> class.
        /// </summary>
        /// <param name="messageTypeMatching">The message type matching.</param>
        public MessageHandlerAttribute(MessageTypeMatching messageTypeMatching)
        {
            this.MessageTypeMatching = messageTypeMatching;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerAttribute"/> class.
        /// </summary>
        /// <param name="messageTypeMatching">The message type matching.</param>
        /// <param name="messageId">The ID of the handled messages.</param>
        public MessageHandlerAttribute(MessageTypeMatching messageTypeMatching, object messageId)
            : this(messageId)
        {
            this.MessageTypeMatching = messageTypeMatching;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerAttribute"/> class.
        /// </summary>
        /// <param name="messageTypeMatching">The message type matching.</param>
        /// <param name="messageId">The ID of the handled messages.</param>
        /// <param name="messageIdMatching">The message ID matching.</param>
        public MessageHandlerAttribute(MessageTypeMatching messageTypeMatching, object messageId, MessageIdMatching messageIdMatching)
            : this(messageId)
        {
            this.MessageTypeMatching = messageTypeMatching;
            this.MessageIdMatching = messageIdMatching;
        }

        /// <summary>
        /// Gets or sets the message ID.
        /// </summary>
        /// <value>
        /// The message ID.
        /// </value>
        [MetadataValue(nameof(MessageId))]
        public object MessageId { get; set; }

        /// <summary>
        /// Gets or sets the message type matching.
        /// </summary>
        /// <value>
        /// The message type matching.
        /// </value>
        [MetadataValue(nameof(MessageTypeMatching))]
        public MessageTypeMatching MessageTypeMatching { get; set; }

        /// <summary>
        /// Gets or sets the message ID matching.
        /// </summary>
        /// <value>
        /// The message ID matching.
        /// </value>
        [MetadataValue(nameof(MessageIdMatching))]
        public MessageIdMatching MessageIdMatching { get; set; }
    }
}