// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMetadataReferenceManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

using System.Reflection;
using Kephas.Services;
using Microsoft.CodeAnalysis;

/// <summary>
/// Service for resolving metadata references.
/// </summary>
[SingletonAppServiceContract]
public interface IMetadataReferenceManager
{
    /// <summary>
    /// Resolves the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>A <see cref="IReadOnlyList{T}"/> of <see cref="MetadataReference"/>s.</returns>
    IReadOnlyList<MetadataReference> Resolve(IEnumerable<Assembly> assemblies);

    /// <summary>
    /// Resolves the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>A <see cref="IReadOnlyList{T}"/> of <see cref="MetadataReference"/>s.</returns>
    IReadOnlyList<MetadataReference> Resolve(params Assembly[] assemblies)
        => this.Resolve((IEnumerable<Assembly>)assemblies);
}