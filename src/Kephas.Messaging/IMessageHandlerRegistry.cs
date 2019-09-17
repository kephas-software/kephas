// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandlerRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMessageHandlerRegistry interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System.Collections.Generic;

    using Kephas.Messaging.Composition;

    /// <summary>
    /// Interface for message handler registry.
    /// </summary>
    public interface IMessageHandlerRegistry
    {
        /// <summary>
        /// Registers the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>
        /// This message handler registry.
        /// </returns>
        IMessageHandlerRegistry RegisterHandler(IMessageHandler handler, MessageHandlerMetadata metadata);

        /// <summary>
        /// Resolves the message handlers for the provided message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The matching message handlers.</returns>
        IEnumerable<IMessageHandler> ResolveMessageHandlers(IMessage message);
    }
}
