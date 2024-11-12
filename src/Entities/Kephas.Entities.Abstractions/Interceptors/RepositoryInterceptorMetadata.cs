// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryInterceptorMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.Entities.Interceptors;

/// <summary>
/// Metadata for <see cref="IRepositoryInterceptor"/> services.
/// </summary>
public record RepositoryInterceptorMetadata : ServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryInterceptorMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public RepositoryInterceptorMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
        if (metadata == null)
        {
            return;
        }

        OperationType = (Type?)TryGetValue(metadata, nameof(OperationType));
        ResultType = (Type?)TryGetValue(metadata, nameof(ResultType));
    }

    /// <summary>
    /// Gets the operation type.
    /// </summary>
    public Type? OperationType { get; }

    /// <summary>
    /// Gets the result type.
    /// </summary>
    public Type? ResultType { get; }
}