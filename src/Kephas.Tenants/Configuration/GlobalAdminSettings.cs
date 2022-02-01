// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalAdminSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration;

/// <summary>
/// Provides the settings for the global admin.
/// </summary>
public class GlobalAdminSettings : TenantSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalAdminSettings"/> class.
    /// </summary>
    public GlobalAdminSettings()
    {
        base.TenantId = "@GlobalAdmin";
        this.DisplayName = "Global Administrator";
    }

    /// <summary>
    /// Gets or sets the tenant ID.
    /// </summary>
    public override string TenantId
    {
        get => base.TenantId;
        set { }
    }
}