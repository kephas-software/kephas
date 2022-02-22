// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderLocationsManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.IO;

/// <summary>
/// Service implementation for <see cref="ILocationsManager"/> resolving folder locations.
/// </summary>
/// <seealso cref="Kephas.IO.ILocationsManager" />
public class FolderLocationsManager : ILocationsManager
{
    /// <summary>
    /// Gets the folder locations.
    /// </summary>
    /// <param name="name">The logical locations name.</param>
    /// <param name="basePath">The base path.</param>
    /// <param name="relativePaths">The relative paths.</param>
    /// <returns>
    /// An instance implementing <see cref="ILocations" />.
    /// </returns>
    public ILocations GetLocations(string name, string basePath, IEnumerable<string> relativePaths)
        => new FolderLocations(name, basePath, relativePaths);
}