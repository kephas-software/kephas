// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamPipelineBehaviorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MediatR;

namespace Kephas.Extensions.Mediator;

/// <summary>
/// Marker interface for stream pipeline behaviors implementing <see cref="IStreamPipelineBehavior{TRequest,TResponse}"/>.
/// </summary>
/// <remarks>
/// Stream pipeline behaviors implementing this interface benefit from <see cref="RequestHandlerMetadata"/>.
/// </remarks>
public interface IStreamPipelineBehaviorBase : IDeclaredHandlerService;

/// <summary>
/// Base class for behaviors that intercept the stream send pipeline.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public abstract class StreamPipelineBehaviorBase<TRequest, TResponse> : IStreamPipelineBehavior<TRequest, TResponse>, IStreamPipelineBehaviorBase
    where TRequest : notnull
{
    /// <summary>
    /// Stream Pipeline handler. Perform any additional behavior and iterate the <paramref name="next" /> delegate as necessary.
    /// </summary>
    /// <param name="request">Incoming request</param>
    /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Awaitable task yielding the <typeparamref name="TResponse" />.</returns>
    public abstract IAsyncEnumerable<TResponse> Handle(TRequest request, StreamHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}