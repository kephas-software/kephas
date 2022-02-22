// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantAdminPermission.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Permissions;

using Kephas.Security.Authorization;
using Kephas.Security.Permissions.AttributedModel;

/// <summary>
/// Defines the tenant specific administrative permission.
/// </summary>
[PermissionInfo(TokenName, Scoping.Global)]
public sealed class TenantAdminPermission
{
    /// <summary>
    /// The name of the permission required for managing the current tenant.
    /// </summary>
    public const string TokenName = "tenantadmin";

    private TenantAdminPermission()
    {
    }
}