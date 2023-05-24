// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActionMessageHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Threading.Tasks;

namespace Kephas.Messaging
{
    /// <summary>
    /// Message handler for actions
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IActionMessageHandler<in TMessage> : IMessageHandler<TMessage, object?>
        where TMessage : IActionMessage
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
        async Task<object?> IMessageHandler<TMessage, object?>.ProcessAsync(TMessage message, IMessagingContext context, CancellationToken token)
        {
            await ProcessAsync(message, context, token).PreserveThreadContext();
            return null;
        }
        
        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        new Task ProcessAsync(TMessage message, IMessagingContext context, CancellationToken token);
    }
}