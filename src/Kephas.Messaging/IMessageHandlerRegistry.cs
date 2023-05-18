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

    using Kephas.Services;

    /// <summary>
    /// Interface for message handler registry.
    /// </summary>
    [AppServiceContract]
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
        /// Registers the handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="handlerFunction">The handler function.</param>
        /// <returns>
        /// This message handler registry.
        /// </returns>
        IMessageHandlerRegistry RegisterHandler<TMessage>(
            Func<TMessage, IMessagingContext, CancellationToken, Task<object?>> handlerFunction)
            where TMessage : class
        {
            this.RegisterHandler(
                new FuncMessageHandler<TMessage>(handlerFunction),
                new MessageHandlerMetadata(typeof(TMessage)));

            return this;
        }

        /// <summary>
        /// Registers the handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="handlerFunction">The synchronous handler function.</param>
        /// <returns>
        /// This message handler registry.
        /// </returns>
        IMessageHandlerRegistry RegisterHandler<TMessage>(Func<TMessage, IMessagingContext, object?> handlerFunction)
            where TMessage : class
        {
            this.RegisterHandler(
                new FuncMessageHandler<TMessage>((msg, ctx, _) => Task.FromResult(handlerFunction(msg, ctx))),
                new MessageHandlerMetadata(typeof(TMessage)));

            return this;
        }

        /// <summary>
        /// Resolves the message handlers for the provided message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The matching message handlers.</returns>
        IEnumerable<IMessageHandler> ResolveMessageHandlers<TMessage, TResult>(TMessage message)
            where TMessage : IMessage<TResult>;
    }
}
