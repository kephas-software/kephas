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
using Kephas.Services;
using Kephas.Threading.Tasks;

namespace Kephas.Messaging.Behaviors;

/// <summary>
/// Pipeline behavior for message processing interception.
/// </summary>
public interface IMessagingBehavior<in TMessage, out TResult> : IAsyncPipelineBehavior<IMessageProcessor, TMessage, TResult>
    where TMessage : IMessage<TResult>
{
    /// <summary>
    /// Interception called before invoking the handler to process the message.
    /// </summary>
    /// <param name="context">The processing context.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task.</returns>
    Task BeforeProcessAsync(IMessagingContext context, CancellationToken token) => Task.CompletedTask;
        
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

    /// <summary>
    /// Invokes the behavior.
    /// </summary>
    /// <remarks>
    /// Make sure to return a result convertible to <typeparamref name="TResult"/>.
    /// Due to the fact that the <typeparamref name="TResult"/> must be contravariant
    /// so that generic pipeline behaviors may handle all kind of results.
    /// </remarks>
    /// <param name="next">The pipeline continuation delegate.</param>
    /// <param name="target">The target.</param>
    /// <param name="args">The operation arguments.</param>
    /// <param name="context">The operation context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task yielding the invocation result.</returns>
    async Task<object?> IAsyncPipelineBehavior<IMessageProcessor, TMessage, TResult>.InvokeAsync(
        Func<Task<object?>> next,
        IMessageProcessor target,
        TMessage args,
        IContext context,
        CancellationToken cancellationToken)
    {
        await this.BeforeProcessAsync((IMessagingContext)context, cancellationToken).PreserveThreadContext();

        var result = await next().PreserveThreadContext();
            
        await this.AfterProcessAsync((IMessagingContext)context, cancellationToken).PreserveThreadContext();

        return result;
    }
}