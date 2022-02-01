// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderLocations.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.IO;

using System.Collections;

/// <summary>
/// A named enumeration of locations based on physical folders.
/// </summary>
public class FolderLocations : ILocations
{
    private readonly string rootPath;
    private readonly IEnumerable<string> relativePaths;
    private List<string>? fullPaths;

    /// <summary>
    /// Initializes a new instance of the <see cref="FolderLocations"/> class.
    /// </summary>
    /// <param name="name">The location name.</param>
    /// <param name="rootPath">The root path.</param>
    /// <param name="relativePaths">An enumeration of relative paths.</param>
    public FolderLocations(string name, string rootPath, IEnumerable<string> relativePaths)
    {
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
        this.rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
        this.relativePaths = relativePaths ?? throw new ArgumentNullException(nameof(relativePaths));
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
            .Select(p => FileSystem.GetFullPath(p, this.rootPath))
            .Distinct()
            .ToList()).GetEnumerator();
    }

    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}