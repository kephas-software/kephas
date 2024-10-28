// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestHandlerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MediatR;

namespace Kephas.Extensions.Mediator;

/// <summary>
/// Marker interface for polymorphic request handlers implementing <see cref="IRequestHandler{TRequest}"/>.
/// </summary>
/// <remarks>
/// Handlers implementing this interface benefit from <see cref="RequestResponseMetadata"/>.
/// </remarks>
public interface IRequestHandlerBase : IDeclaredHandlerService;

/// <summary>
/// Marker interface for request handlers implementing <see cref="IRequestHandler{TRequest}"/>.
/// </summary>
/// <remarks>
/// Handlers implementing this interface benefit from <see cref="RequestResponseMetadata"/>.
/// </remarks>
public interface IRequestResponseHandlerBase : IDeclaredHandlerService;

/// <summary>
/// Base class for request handlers that do not return a response.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
public abstract class RequestHandlerBase<TRequest> : IRequestHandler<TRequest>, IRequestHandlerBase
    where TRequest : IRequest
{
    /// <summary>Handles the request.</summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task for the continuation.</returns>
    public abstract Task Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Base class for request handlers that return a response.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public abstract class RequestHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>, IRequestResponseHandlerBase
    where TRequest : IRequest<TResponse>
{
    /// <summary>Handles the request and returns a task yielding the response.</summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task yielding the response.</returns>
    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}