﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageMatchService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default message match service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;

    using Kephas.Data;
    using Kephas.Messaging.Composition;
    using Kephas.Messaging.Distributed;
    using Kephas.Services;

    /// <summary>
    /// A default message match service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultMessageMatchService : IMessageMatchService
    {
        /// <summary>
        /// Gets the message type.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The message type.
        /// </returns>
        public virtual Type GetMessageType(object message)
        {
            return message.GetContent().GetType();
        }

        /// <summary>
        /// Gets the message ID.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The message ID.
        /// </returns>
        public virtual object? GetMessageId(object message)
        {
            // brokered messages are not identifiable in the sense
            // that the ID is a logical behavior/handler discriminator.
            if (message is IBrokeredMessage)
            {
                return null;
            }

            var expandoMessage = message.GetContent().ToDynamic();
            var messageId = expandoMessage[nameof(IIdentifiable.Id)]
                            ?? expandoMessage[nameof(MessageHandlerMetadata.MessageId)];
            return messageId;
        }

        /// <summary>
        /// Checks whether the message type and message ID matches the provided criteria.
        /// </summary>
        /// <param name="messageMatch">Provides the matching criteria.</param>
        /// <param name="envelopeType">Type of the envelope.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageId">Identifier for the message.</param>
        /// <returns>
        /// True if the message type and ID matches the criteria, false if not.
        /// </returns>
        public virtual bool IsMatch(IMessageMatch messageMatch, Type envelopeType, Type messageType, object messageId)
        {
            var m = messageMatch;
            return (m.MessageType == null
                    || (m.MessageTypeMatching == MessageTypeMatching.Type && m.MessageType == messageType)
                    || (m.MessageTypeMatching == MessageTypeMatching.TypeOrHierarchy && m.MessageType.IsAssignableFrom(messageType)))
                   && ((m.MessageIdMatching == MessageIdMatching.Id && object.Equals(m.MessageId, messageId))
                    || m.MessageIdMatching == MessageIdMatching.All)
                   && (m.EnvelopeType == null
                    || (m.EnvelopeTypeMatching == MessageTypeMatching.Type && m.EnvelopeType == envelopeType)
                    || (m.EnvelopeTypeMatching == MessageTypeMatching.TypeOrHierarchy && m.EnvelopeType.IsAssignableFrom(envelopeType)));
        }
    }
}