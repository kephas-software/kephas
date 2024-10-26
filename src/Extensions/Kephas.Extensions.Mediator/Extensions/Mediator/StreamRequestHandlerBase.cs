// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamRequestHandlerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MediatR;

namespace Kephas.Extensions.Mediator;

/// <summary>
/// Marker interface for request handlers implementing <see cref="IStreamRequestHandler{TRequest,TResponse}"/>.
/// </summary>
/// <remarks>
/// Handlers implementing this interface benefit from <see cref="RequestHandlerMetadata"/>.
/// </remarks>
public interface IStreamRequestHandlerBase;

/// <summary>
/// Base class for stream request handlers that return a response.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public abstract class StreamRequestHandlerBase<TRequest, TResponse> : IStreamRequestHandler<TRequest, TResponse>, IStreamRequestHandlerBase
    where TRequest : IStreamRequest<TResponse>
{
    /// <summary>
    /// Handles a stream request with IAsyncEnumerable as return type.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Response from the request.</returns>
    public abstract IAsyncEnumerable<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}