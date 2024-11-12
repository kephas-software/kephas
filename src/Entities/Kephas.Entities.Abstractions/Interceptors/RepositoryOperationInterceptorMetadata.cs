// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryOperationInterceptorMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.Entities.Interceptors;

/// <summary>
/// Metadata for <see cref="IRepositoryOperationInterceptor"/> services.
/// </summary>
public record RepositoryOperationInterceptorMetadata : ServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryOperationInterceptorMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public RepositoryOperationInterceptorMetadata(IDictionary<string, object?>? metadata)
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