// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantFolderLocations.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.IO;

using Kephas.Tenants.Resources;

/// <summary>
/// Folder locations for a tenant.
/// </summary>
/// <seealso cref="FolderLocations" />
public class TenantFolderLocations : FolderLocations
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantFolderLocations"/> class.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    /// <param name="relativePaths">The relative paths.</param>
    /// <param name="basePath">Optional. The base path. If not provided, the application directory is considered.</param>
    /// <param name="name">Optional. The location name. If not provided, a name will be generated.</param>
    public TenantFolderLocations(string tenant, IEnumerable<string> relativePaths, string? basePath, string? name)
        : base(GetRelativePaths(tenant, relativePaths), basePath, name)
    {
        if (string.IsNullOrEmpty(tenant))
        {
            throw new ArgumentException(Strings.TenantFolderLocations_tenant_not_set, nameof(tenant));
        }
    }

    private static IEnumerable<string> GetRelativePaths(string tenant, IEnumerable<string> relativePaths)
        => relativePaths.Select(p => Path.Combine(p, tenant.MakeHiddenLocation()));
}