// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppAdminPermission.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using Kephas.Security.Authorization.AttributedModel;

    /// <summary>
    /// Defines the global application administration permission.
    /// </summary>
    [PermissionInfo("appadmin", Scoping.Global)]
    public interface IAppAdminPermission
    {
    }
}