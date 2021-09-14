// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System.Runtime.CompilerServices;

    using Kephas.Messaging.Events;
    using Kephas.Messaging.Messages;

    /// <summary>
    /// Extension methods for <see cref="IMessage"/>.
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// Converts the provided object to a message.
        /// </summary>
        /// <param name="data">The object to be converted.</param>
        /// <returns>
        /// The object as an <see cref="IMessage"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IMessage? ToMessage(this object? data)
        {
            return data == null
                ? null
                : data is IMessage message
                    ? message
                    : new MessageEnvelope { Message = data };
        }

        /// <summary>
        /// Converts the provided object to an event.
        /// </summary>
        /// <param name="data">The object to be converted.</param>
        /// <returns>
        /// The object as an <see cref="IEvent"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEvent? ToEvent(this object? data)
        {
            return data == null
                ? null
                : data is IEvent @event
                    ? @event
                    : new EventEnvelope { Event = data };
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