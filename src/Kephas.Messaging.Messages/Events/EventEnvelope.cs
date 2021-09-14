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
    using System;

    using Kephas.Messaging.Messages;

    /// <summary>
    /// An event envelope.
    /// </summary>
    public class EventEnvelope : IEvent, IMessageEnvelope
    {
        /// <summary>
        /// Gets or sets the event.
        /// </summary>
        /// <value>
        /// The event.
        /// </value>
        public object? Event { get; set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the event is not set.</exception>
        /// <returns>
        /// The message.
        /// </returns>
        public object GetContent()
        {
            if (this.Event == null)
            {
                throw new InvalidOperationException("The message is not set in the event envelope.");
            }

            return this.Event;
        }
    }
}
