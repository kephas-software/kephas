// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandlerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Provides a base implementation of a message handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Messaging.Messages;
    using Kephas.Messaging.Resources;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Provides a base implementation of a message handler.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public abstract class MessageHandlerBase<TMessage, TResponse> : Loggable, IMessageHandler<TMessage>
        where TMessage : class
        where TResponse : class
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
        public abstract Task<TResponse> ProcessAsync(TMessage message, IMessageProcessingContext context, CancellationToken token);

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        async Task<IMessage> IMessageHandler<TMessage>.ProcessAsync(TMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            Requires.NotNull(message, nameof(message));
            Requires.NotNull(context, nameof(context));

            var response = await this.ProcessAsync(message, context, token).PreserveThreadContext();
            return response == null ? null : (response as IMessage ?? new MessageAdapter { Message = response });
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
        async Task<IMessage> IMessageHandler.ProcessAsync(IMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            if (!(message is TMessage typedRequest))
            {
                typedRequest = message is IMessageAdapter messageAdapter ? messageAdapter.GetMessage() as TMessage : null;
                if (typedRequest == null)
                {
                    throw new ArgumentException(string.Format(Strings.MessageHandler_BadMessageType_Exception, typeof(TMessage)), nameof(message));
                }
            }

            var response = await this.ProcessAsync(typedRequest, context, token).PreserveThreadContext();
            return response == null ? null : (response as IMessage ?? new MessageAdapter { Message = response });
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}