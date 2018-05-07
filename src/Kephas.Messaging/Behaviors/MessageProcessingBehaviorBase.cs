// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessingBehaviorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base implementation of a message processing filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base implementation of a message processing filter.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    public abstract class MessageProcessingBehaviorBase<TMessage> : IMessageProcessingBehavior<TMessage>
        where TMessage : IMessage
    {
        /// <summary>
        /// Interception called before invoking the handler to process the message.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task IMessageProcessingBehavior.BeforeProcessAsync(IMessageProcessingContext context, CancellationToken token)
        {
            Requires.NotNull(context, nameof(context));

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
        Task IMessageProcessingBehavior.AfterProcessAsync(IMessageProcessingContext context, CancellationToken token)
        {
            Requires.NotNull(context, nameof(context));

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
            return TaskHelper.CompletedTask;
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
        public virtual Task AfterProcessAsync(TMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            return TaskHelper.CompletedTask;
        }
    }
}