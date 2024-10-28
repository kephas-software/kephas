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
public class RequestResponseMetadata : ServiceMetadata
{
    /// <summary>
    /// Gets or sets the request type.
    /// </summary>
    public Type? RequestType { get; set; }

    /// <summary>
    /// Gets or sets the response type.
    /// </summary>
    public Type? ResponseType { get; set; }
}