// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantFolderLocationsManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.IO;

/// <summary>
/// Locations manager with tenant support.
/// </summary>
/// <seealso cref="ILocationsManager" />
public class TenantFolderLocationsManager : ILocationsManager
{
    private readonly string tenant;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantFolderLocationsManager"/> class.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    public TenantFolderLocationsManager(string tenant)
    {
        this.tenant = tenant;
    }

    /// <summary>
    /// Gets the locations.
    /// </summary>
    /// <param name="name">The logical locations name.</param>
    /// <param name="basePath">The base path.</param>
    /// <param name="relativePaths">The relative paths.</param>
    /// <returns>
    /// An instance implementing <see cref="T:Kephas.IO.ILocations" />.
    /// </returns>
    public ILocations GetLocations(string name, string basePath, IEnumerable<string> relativePaths)
        => new TenantFolderLocations(this.tenant, name, basePath, relativePaths);
}