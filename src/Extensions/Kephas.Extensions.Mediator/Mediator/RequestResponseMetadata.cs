// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestHandlerMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;
using MediatR;

namespace Kephas.Extensions.Mediator;

/// <summary>
/// Metadata for <see cref="IRequestHandler{TRequest}"/> and <see cref="IRequestHandler{TRequest,TResponse}"/> and <see cref="IStreamRequestHandler{TRequest,TResponse}"/>.
/// </summary>
public record RequestResponseMetadata : ServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequestResponseMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public RequestResponseMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
        if (metadata == null)
        {
            return;
        }

        RequestType = (Type?)TryGetValue(metadata, nameof(RequestType));
        ResponseType = (Type?)TryGetValue(metadata, nameof(ResponseType));
    }

    /// <summary>
    /// Gets the request type.
    /// </summary>
    public Type? RequestType { get; }

    /// <summary>
    /// Gets the response type.
    /// </summary>
    public Type? ResponseType { get; }
}