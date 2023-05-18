// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Reflection;

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
        public static IMessageBase ToMessage(this object data)
        {
            _ = data ?? throw new ArgumentNullException(nameof(data));

            var messageType = data.GetType();
            var resultType = messageType.GetBaseConstructedGenericOf(typeof(IMessage<>));
            return resultType is null
                ? new MessageEnvelope { Message = data }
                : (IMessageBase)data;
        }

        /// <summary>
        /// Converts the provided object to an event.
        /// </summary>
        /// <param name="data">The object to be converted.</param>
        /// <returns>
        /// The object as an <see cref="IEvent"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEvent ToEvent(this object data) =>
            data is null
                ? throw new ArgumentNullException(nameof(data))
                : data as IEvent ?? new EventEnvelope { Event = data };

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetContent(this object message) =>
            message is IMessageEnvelope envelope ? envelope.GetContent() : message;
    }
}