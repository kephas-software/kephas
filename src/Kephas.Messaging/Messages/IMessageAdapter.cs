// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message adapter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Messages
{
    /// <summary>
    /// Interface for message adapter.
    /// </summary>
    public interface IMessageAdapter : IMessage
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <returns>
        /// The message.
        /// </returns>
        object GetMessage();
    }
}
