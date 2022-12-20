// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILocationsManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.IO;

/// <summary>
/// Service for resolving and managing locations.
/// </summary>
public interface ILocationsManager
{
    /// <summary>
    /// Gets the locations.
    /// </summary>
    /// <param name="relativePaths">The relative paths.</param>
    /// <param name="basePath">Optional. The base path. If not provided, the application directory is considered.</param>
    /// <param name="name">Optional. The location name. If not provided, a name will be generated.</param>
    /// <returns>An instance implementing <see cref="ILocations"/>.</returns>
    ILocations GetLocations(IEnumerable<string> relativePaths, string? basePath = null, string? name = null);
}
