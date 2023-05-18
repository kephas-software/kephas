// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageEnvelope.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message envelope interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Messages
{
    /// <summary>
    /// Base envelope interface for messages not implementing <see cref="IMessage{TResult}"/>.
    /// </summary>
    public interface IMessageEnvelopeBase : IMessage<object?>
    {
    }
    
    /// <summary>
    /// Envelope for messages not implementing <see cref="IMessage{TResult}"/>.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    public interface IMessageEnvelope<out T> : IMessageEnvelopeBase
        where T : class
    {
        /// <summary>
        /// Gets the native message.
        /// </summary>
        public T? Message { get; }
    }
}
