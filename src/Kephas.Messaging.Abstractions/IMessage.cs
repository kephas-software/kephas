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
    /// <summary>
    /// Marker interface for messages.
    /// </summary>
    public interface IMessageBase
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
        object GetContent() => this;
    }

    /// <summary>
    /// Marker interface for messages requiring a typed response.
    /// </summary>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public interface IMessage<out TResponse> : IMessageBase
    {
    }

    /// <summary>
    /// Marker interface for messages with a void response.
    /// </summary>
    public interface IActionMessage : IMessage<object?>
    {
    }
}