// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message adapter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Messages
{
    using System;

    /// <summary>
    /// A message envelope.
    /// </summary>
    public record MessageEnvelope<T> : IMessageEnvelope<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEnvelope{T}"/>.
        /// </summary>
        public MessageEnvelope()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEnvelope{T}"/>.
        /// </summary>
        /// <param name="message">The native message.</param>
        public MessageEnvelope(T message)
        {
            Content = message ?? throw new ArgumentNullException(nameof(message));
        }
        
        /// <summary>
        /// Gets or sets the native message.
        /// </summary>
        /// <value>
        /// The native message.
        /// </value>
        public T? Content { get; set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the message is not set.</exception>
        /// <returns>
        /// The message, or if the message is not set, an <see cref="InvalidOperationException"/> occurs.
        /// </returns>
        public object GetContent() =>
            this.Content ?? throw new InvalidOperationException("The message is not set in the envelope.");
    }
}
