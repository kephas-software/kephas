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
    /// Interface for message envelopes.
    /// </summary>
    public interface IMessageEnvelope : IMessage
    {
        /// <summary>
        /// Gets the contained message.
        /// </summary>
        /// <returns>
        /// The contained message.
        /// </returns>
        object GetContent();
    }
}
