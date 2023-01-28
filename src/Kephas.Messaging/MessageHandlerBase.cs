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

    using Kephas;
    using Kephas.Logging;
    using Kephas.Messaging.Resources;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Provides a base implementation of a message handler.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public abstract class MessageHandlerBase<TMessage, TResponse> : IMessageHandler<TMessage>
        where TMessage : class, IMessage<TResponse>
        where TResponse : class?
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerBase{TMessage, TResponse}"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected MessageHandlerBase(ILogger<MessageHandlerBase<TMessage, TResponse>>? logger = null)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        protected ILogger? Logger { get; set; }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public abstract Task<TResponse> ProcessAsync(TMessage message, IMessagingContext context, CancellationToken token);

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        async Task<object?> IMessageHandler<TMessage>.ProcessAsync(TMessage message, IMessagingContext context, CancellationToken token)
        {
            message = message ?? throw new ArgumentNullException(nameof(message));
            context = context ?? throw new ArgumentNullException(nameof(context));

            this.Logger ??= this.GetLogger(context);
            var response = await this.ProcessAsync(message, context, token).PreserveThreadContext();
            return response;
        }
    }
}