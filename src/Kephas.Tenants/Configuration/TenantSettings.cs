// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration;

/// <summary>
/// Provides information about a tenant.
/// </summary>
public class TenantSettings : ISettings
{
    /// <summary>
    /// Gets or sets the tenant ID.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name;
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
}