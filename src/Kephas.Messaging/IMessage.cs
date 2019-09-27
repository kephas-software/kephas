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
    }

    /// <summary>
    /// Extension methods for <see cref="IMessage"/>.
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// Gets the content of the message.
        /// </summary>
        /// <remarks>
        /// In case of a message envelope, it returns the contained message, otherwise the message itself.
        /// </remarks>
        /// <param name="message">The message to act on.</param>
        /// <returns>
        /// The message content.
        /// </returns>
        public static object GetContent(this IMessage message)
        {
            return message is IMessageEnvelope envelope ? envelope.GetContent() : message;
        }

        /// <summary>
        /// Gets the content of the message.
        /// </summary>
        /// <remarks>
        /// In case of a message envelope, it returns the contained message, otherwise the message itself.
        /// </remarks>
        /// <param name="message">The message to act on.</param>
        /// <returns>
        /// The message content.
        /// </returns>
        internal static object GetContent(this object message)
        {
            return message is IMessageEnvelope envelope ? envelope.GetContent() : message;
        }
    }
}