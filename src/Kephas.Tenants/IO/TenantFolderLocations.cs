// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantFolderLocations.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.IO;

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
    /// <param name="name">The name.</param>
    /// <param name="rootPath">The root path.</param>
    /// <param name="relativePaths">The relative paths.</param>
    public TenantFolderLocations(string tenant, string name, string rootPath, IEnumerable<string> relativePaths)
        : base(name, rootPath, GetRelativePaths(tenant, relativePaths))
    {
    }

    private static IEnumerable<string> GetRelativePaths(string tenant, IEnumerable<string> relativePaths)
        => relativePaths.Select(p => Path.Combine(p, tenant));
}