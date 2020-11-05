// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Permissions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Models.PermissionsModel
{
    using Kephas.Model.Security.Authorization.AttributedModel;
    using Kephas.Security.Authorization;
    using Kephas.Security.Authorization.AttributedModel;

    [PermissionInfo("do", Scoping.Type | Scoping.Instance)]
    public interface IDoPermission : IPermission
    {
    }

    [PermissionType("special-do", Scoping.Type)]
    public interface ISpecialDoPermission : IDoPermission
    {
    }
}