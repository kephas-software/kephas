// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantFolderLocationsManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.IO;

using Kephas.Tenants.Resources;

/// <summary>
/// Locations manager with tenant support.
/// </summary>
/// <seealso cref="ILocationsManager" />
public class TenantFolderLocationsManager : FolderLocationsManager
{
    private readonly string tenant;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantFolderLocationsManager" /> class.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    /// <param name="defaultBasePath">The default base path.</param>
    public TenantFolderLocationsManager(string tenant, string? defaultBasePath = null)
        : base(defaultBasePath)
    {
        if (string.IsNullOrEmpty(tenant))
        {
            throw new ArgumentException(Strings.TenantFolderLocations_tenant_not_set, nameof(tenant));
        }

        this.tenant = tenant;
    }

    /// <summary>
    /// Gets the locations.
    /// </summary>
    /// <param name="relativePaths">The relative paths.</param>
    /// <param name="basePath">Optional. The base path. If not provided, the application directory is considered.</param>
    /// <param name="name">Optional. The location name. If not provided, a name will be generated.</param>
    /// <returns>
    /// An instance implementing <see cref="T:Kephas.IO.ILocations" />.
    /// </returns>
    public override ILocations GetLocations(IEnumerable<string> relativePaths, string? basePath = null, string? name = null)
        => new TenantFolderLocations(this.tenant, relativePaths, basePath ?? this.DefaultBasePath, name);
}