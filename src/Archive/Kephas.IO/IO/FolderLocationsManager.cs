// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderLocationsManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.IO;

using System.Reflection;
using Kephas.Reflection;

/// <summary>
/// Service implementation for <see cref="ILocationsManager"/> resolving folder locations.
/// </summary>
/// <seealso cref="Kephas.IO.ILocationsManager" />
public class FolderLocationsManager : ILocationsManager
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FolderLocationsManager"/> class.
    /// </summary>
    /// <param name="defaultBasePath">The default base path.</param>
    public FolderLocationsManager(string? defaultBasePath = null)
    {
        this.DefaultBasePath = defaultBasePath ?? (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetLocationDirectory();
    }

    /// <summary>
    /// Gets the default base path.
    /// </summary>
    /// <value>
    /// The default base path.
    /// </value>
    public string DefaultBasePath { get; }

    /// <summary>
    /// Gets the folder locations.
    /// </summary>
    /// <param name="relativePaths">The relative paths.</param>
    /// <param name="basePath">Optional. The base path. If not provided, the application directory is considered.</param>
    /// <param name="name">Optional. The location name. If not provided, a name will be generated.</param>
    /// <returns>
    /// An instance implementing <see cref="ILocations" />.
    /// </returns>
    public virtual ILocations GetLocations(IEnumerable<string> relativePaths, string? basePath = null, string? name = null)
        => new FolderLocations(relativePaths, basePath ?? this.DefaultBasePath, name);
}