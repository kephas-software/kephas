// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingBehaviorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base implementation of a message processing filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Behaviors
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base implementation of a messaging behavior.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <typeparam name="TResult">Rge result type.</typeparam>
    public abstract class MessagingBehaviorBase<TMessage, TResult> : Loggable, IMessagingBehavior<TMessage, TResult>
        where TMessage : IMessage<TResult>
    {
        private bool isInitialized;

        /// <summary>
        /// Interception called before invoking the handler to process the message.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task IMessagingBehavior<TMessage, TResult>.BeforeProcessAsync(IMessagingContext context, CancellationToken token)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            this.EnsureInitialized(context);
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
        Task IMessagingBehavior<TMessage, TResult>.AfterProcessAsync(IMessagingContext context, CancellationToken token)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            this.EnsureInitialized(context);
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
        public virtual Task BeforeProcessAsync(TMessage message, IMessagingContext context, CancellationToken token)
        {
            return Task.CompletedTask;
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
        public virtual Task AfterProcessAsync(TMessage message, IMessagingContext context, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureInitialized(IContext context)
        {
            if (!this.isInitialized)
            {
                this.Logger = this.GetLogger(context);
                this.isInitialized = true;
            }
        }
    }
}