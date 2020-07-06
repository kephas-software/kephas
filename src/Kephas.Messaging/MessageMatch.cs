// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageMatch.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the event match class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;

    using Kephas.Dynamic;
    using Kephas.Messaging.Composition;

    /// <summary>
    /// Criteria for event matching.
    /// </summary>
    public class MessageMatch : Expando, IMessageMatch
    {
        /// <summary>
        /// Gets or sets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        public Type? MessageType { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the event.
        /// </summary>
        /// <value>
        /// The identifier of the event.
        /// </value>
        public object? MessageId { get; set; }

        /// <summary>
        /// Gets or sets the type of the envelope.
        /// </summary>
        /// <value>
        /// The type of the envelope.
        /// </value>
        public Type? EnvelopeType { get; set; }

        /// <summary>
        /// Gets or sets the event type matching.
        /// </summary>
        /// <value>
        /// The event type matching.
        /// </value>
        public MessageTypeMatching MessageTypeMatching { get; set; }

        /// <summary>
        /// Gets or sets the event identifier matching.
        /// </summary>
        /// <value>
        /// The event identifier matching.
        /// </value>
        public MessageIdMatching MessageIdMatching { get; set; }

        /// <summary>
        /// Gets or sets the envelope type matching.
        /// </summary>
        /// <value>
        /// The envelope type matching.
        /// </value>
        public MessageTypeMatching EnvelopeTypeMatching { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var envelopeMatching = this.EnvelopeType != null ? $"{this.EnvelopeTypeMatching}/{this.EnvelopeType}:" : string.Empty;
            return $"{envelopeMatching}{this.MessageTypeMatching}/{this.MessageType}:{this.MessageIdMatching}/{this.MessageId}";
        }
    }
}