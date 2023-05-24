// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the event adapter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Events
{
    using Kephas.Messaging.Messages;

    /// <summary>
    /// An event envelope.
    /// </summary>
    public record EventEnvelope<T> : MessageEnvelope<T>, IEvent
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEnvelope{T}"/>.
        /// </summary>
        public EventEnvelope()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEnvelope{T}"/>.
        /// </summary>
        /// <param name="message">The native message.</param>
        public EventEnvelope(T message)
            : base(message)
        {
        }
    }
}
