// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipelineBehaviorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MediatR;

namespace Kephas.Extensions.Mediator;

/// <summary>
/// Marker interface for pipeline behaviors implementing <see cref="IPipelineBehavior{TRequest,TResponse}"/>.
/// </summary>
/// <remarks>
/// Pipeline behaviors implementing this interface benefit from <see cref="RequestHandlerMetadata"/>.
/// </remarks>
public interface IPipelineBehaviorBase : IDeclaredHandlerService;

/// <summary>
/// Base class for behaviors that intercept the send pipeline.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public abstract class PipelineBehaviorBase<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, IPipelineBehaviorBase
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Pipeline handler. Perform any additional behavior and await the <paramref name="next" /> delegate as necessary.
    /// </summary>
    /// <param name="request">Incoming request.</param>
    /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Awaitable task yielding the <typeparamref name="TResponse" />.</returns>
    public abstract Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}