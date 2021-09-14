// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Marker interface for messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using Kephas.Messaging.Messages;

    /// <summary>
    /// Marker interface for messages.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets the content of the message.
        /// </summary>
        /// <remarks>
        /// In case of a message envelope, it returns the contained message, otherwise the message itself.
        /// </remarks>
        /// <returns>
        /// The message content.
        /// </returns>
        public object GetContent()
        {
            return this is IMessageEnvelope envelope ? envelope.GetContent() : this;
        }
    }
}