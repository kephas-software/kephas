// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessingFilterBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base implementation of a message processing filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Server
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base implementation of a message processing filter.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    public abstract class MessageProcessingFilterBase<TMessage> : IMessageProcessingFilter<TMessage>
        where TMessage : IMessage
    {
        /// <summary>
        /// Interception called before invoking the handler to process the message.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task IMessageProcessingFilter.BeforeProcessAsync(IMessageProcessingContext context, CancellationToken token)
        {
            var message = (TMessage)context.Message;
            return this.BeforeProcessAsync(message, context, token);
        }

        /// <summary>
        /// Interception called after invoking the handler to process the message.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task.</returns>
        /// <remarks>
        /// The context will contain the response returned by the handler. 
        /// The interceptor may change the response or even replace it with another one.
        /// </remarks>
        Task IMessageProcessingFilter.AfterProcessAsync(IMessageProcessingContext context, CancellationToken token)
        {
            var message = (TMessage)context.Message;
            return this.AfterProcessAsync(message, context, token);
        }

        /// <summary>
        /// Interception called before invoking the handler to process the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// A task.
        /// </returns>
        public virtual Task BeforeProcessAsync(TMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            return CompletedTask.Value;
        }

        /// <summary>
        /// Interception called after invoking the handler to process the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// A task.
        /// </returns>
        /// <remarks>
        /// The context will contain the response returned by the handler.
        /// The interceptor may change the response or even replace it with another one.
        /// </remarks>
        public Task AfterProcessAsync(TMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            return CompletedTask.Value;
        }
    }
}