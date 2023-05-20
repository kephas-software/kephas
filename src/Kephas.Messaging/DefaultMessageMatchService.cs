// --------------------------------------------------------------------------------------------------------------------
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
        /// Checks whether the message type and message ID matches the provided criteria.
        /// </summary>
        /// <param name="messageMatch">Provides the matching criteria.</param>
        /// <param name="context"></param>
        /// <returns>
        /// True if the message type and ID matches the criteria, false if not.
        /// </returns>
        public virtual bool IsMatch(IMessageMatch messageMatch, IMessagingContext context)
        {
            var m = messageMatch;
            return (m.MessageType == null
                    || (m.MessageTypeMatching == MessageTypeMatching.Type && m.MessageType == context.MessageType)
                    || (m.MessageTypeMatching == MessageTypeMatching.TypeOrHierarchy && m.MessageType.IsAssignableFrom(context.MessageType)))
                   && (m.EnvelopeType == null
                    || (m.EnvelopeTypeMatching == MessageTypeMatching.Type && m.EnvelopeType == context.EnvelopeType)
                    || (m.EnvelopeTypeMatching == MessageTypeMatching.TypeOrHierarchy && m.EnvelopeType.IsAssignableFrom(context.EnvelopeType)));
        }
    }
}