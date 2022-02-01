// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantsManagementSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration;

using Kephas.Security.Permissions;
using Kephas.Security.Permissions.AttributedModel;

/// <summary>
/// Settings for tenant management.
/// </summary>
[RequiresPermission(typeof(TenantsAdminPermission))]
[GlobalSettings]
public class TenantsManagementSettings : ISettings
{
    /// <summary>
    /// Gets or sets the global admin organization settings.
    /// </summary>
    public GlobalAdminSettings GlobalAdmin { get; set; } = new ();

    /// <summary>
    /// Gets the list of tenants.
    /// </summary>
    public IList<TenantSettings> Tenants { get; } = new List<TenantSettings>();
}