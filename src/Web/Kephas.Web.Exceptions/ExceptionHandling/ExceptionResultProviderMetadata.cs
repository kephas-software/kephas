// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionResultProviderMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.Web.ExceptionHandling;

/// <summary>
/// Metadata for <see cref="IExceptionResultProvider"/> services.
/// </summary>
public record ExceptionResultProviderMetadata : ServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionResultProviderMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public ExceptionResultProviderMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
        if (metadata == null)
        {
            return;
        }

        ExceptionType = (Type?)TryGetValue(metadata, nameof(ExceptionType));
    }

    /// <summary>
    /// Gets the exception type it handles.
    /// </summary>
    public Type? ExceptionType { get; }
}