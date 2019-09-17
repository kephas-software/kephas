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
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Diagnostics.Contracts;
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

    /// <summary>
    /// A message handler registry extensions.
    /// </summary>
    public static class MessageHandlerRegistryExtensions
    {
        /// <summary>
        /// Registers the handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="this">The registry to act on.</param>
        /// <param name="handlerFunction">The handler function.</param>
        /// <returns>
        /// This message handler registry.
        /// </returns>
        public static IMessageHandlerRegistry RegisterHandler<TMessage>(this IMessageHandlerRegistry @this, Func<TMessage, IMessageProcessingContext, CancellationToken, Task<IMessage>> handlerFunction)
        {
            Requires.NotNull(@this, nameof(@this));

            @this.RegisterHandler(
                new FuncMessageHandler<TMessage>(handlerFunction),
                new MessageHandlerMetadata(typeof(TMessage)));

            return @this;
        }
    }
}
