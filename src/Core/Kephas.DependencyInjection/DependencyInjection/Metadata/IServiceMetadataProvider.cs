// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceMetadataProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection.Metadata;

/// <summary>
/// Provides metadata for services used in the dependency injection.
/// </summary>
public interface IServiceMetadataProvider
{
    /// <summary>
    /// Gets the metadata as an enumeration of <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    /// <returns>An enumeration of <see cref="KeyValuePair{TKey,TValue}"/>.</returns>
    IEnumerable<KeyValuePair<string, object?>> GetMetadata();
}