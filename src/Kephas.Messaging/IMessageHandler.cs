// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Application service for handling messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging.Resources;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Application service for handling messages.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        Task<object?> ProcessAsync(IMessage message, IMessagingContext context, CancellationToken token);
    }

    /// <summary>
    /// Application service for handling requests.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    [AppServiceContract(ContractType = typeof(IMessageHandler), AllowMultiple = true)]
    public interface IMessageHandler<in TMessage> : IMessageHandler
    {
        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        Task<object?> ProcessAsync(TMessage message, IMessagingContext context, CancellationToken token);

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        async Task<object?> IMessageHandler.ProcessAsync(IMessage message, IMessagingContext context, CancellationToken token)
        {
            // typed message handlers register themselves for a message type which may not implement IMessage
            // therefore the actual processed message is the message content.
            var content = message.GetContent();
            if (content is not TMessage typedMessage)
            {
                throw new ArgumentException(Strings.MessageHandler_BadMessageType_Exception.FormatWith(typeof(TMessage), content?.GetType()), nameof(message));
            }

            var response = await this.ProcessAsync(typedMessage, context, token).PreserveThreadContext();
            return response;
        }
    }
}