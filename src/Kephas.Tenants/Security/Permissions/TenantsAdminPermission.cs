// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantsAdminPermission.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Permissions;

using Kephas.Security.Authorization;
using Kephas.Security.Permissions.AttributedModel;

/// <summary>
/// Defines the global tenants administration permission.
/// </summary>
[PermissionInfo(TokenName, Scoping.Global)]
public sealed class TenantsAdminPermission
{
    /// <summary>
    /// The name of the TenantsAdmin permission.
    /// </summary>
    public const string TokenName = "tenantsadmin";

    private TenantsAdminPermission()
    {
    }
}