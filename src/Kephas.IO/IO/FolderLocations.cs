// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderLocations.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.IO;

using System.Collections;
using System.Reflection;
using Kephas.Reflection;

/// <summary>
/// A named enumeration of locations based on physical folders.
/// </summary>
public class FolderLocations : ILocations
{
    private readonly string basePath;
    private readonly IEnumerable<string> relativePaths;
    private List<string>? fullPaths;

    /// <summary>
    /// Initializes a new instance of the <see cref="FolderLocations"/> class.
    /// </summary>
    /// <param name="relativePaths">An enumeration of relative paths.</param>
    /// <param name="basePath">Optional. The base path. If not provided, the application directory is considered.</param>
    /// <param name="name">Optional. The location name. If not provided, a name will be generated.</param>
    public FolderLocations(IEnumerable<string> relativePaths, string? basePath, string? name)
    {
        this.relativePaths = relativePaths ?? throw new ArgumentNullException(nameof(relativePaths));
        this.basePath = basePath ?? (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetLocationDirectory();
        this.Name = name ?? $"location-{Guid.NewGuid():N}";
    }

    /// <summary>
    /// Gets the logical name of the location.
    /// </summary>
    public string Name { get; }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<string> GetEnumerator()
    {
        return (this.fullPaths ??= this.relativePaths
            .Select(p => FileSystem.GetFullPath(p, this.basePath))
            .Distinct()
            .ToList()).GetEnumerator();
    }

    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}