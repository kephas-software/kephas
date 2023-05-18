// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessagingBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Application service for message processing interception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Pipelines;
using Kephas.Threading.Tasks;

namespace Kephas.Messaging.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Services;

    /// <summary>
    /// Application service for message processing interception.
    /// </summary>
    public interface IMessagingBehavior : IPipelineBehavior<IMessageProcessor, IMessagingContext, object?>
    {
        /// <summary>
        /// Interception called before invoking the handler to process the message.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task.</returns>
        Task BeforeProcessAsync(IMessagingContext context, CancellationToken token) => Task.CompletedTask;

        /// <summary>
        /// Invokes the behavior.
        /// </summary>
        /// <param name="next">The pipeline continuation delegate.</param>
        /// <param name="target">The target.</param>
        /// <param name="args">The operation arguments.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        async Task<object?> IPipelineBehavior<IMessageProcessor, IMessagingContext, object?>.InvokeAsync(
            Func<Task<object?>> next,
            IMessageProcessor target,
            IMessagingContext args,
            CancellationToken cancellationToken)
        {
            await this.BeforeProcessAsync(args, cancellationToken).PreserveThreadContext();

            var result = await next().PreserveThreadContext();
            
            await this.AfterProcessAsync(args, cancellationToken).PreserveThreadContext();

            return result;
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
        Task AfterProcessAsync(IMessagingContext context, CancellationToken token) => Task.CompletedTask;
    }

    public interface IMessagingBehavior<TMessage> : IPipelineBehavior<IMessageProcessor, IMessagingContext<TMessage>, object?>
        where TMessage : IMessage
    {
    }
}