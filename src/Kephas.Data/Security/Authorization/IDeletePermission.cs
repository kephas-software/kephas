// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDeletePermission.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Security.Authorization
{
    using Kephas.Security.Authorization;
    using Kephas.Security.Authorization.AttributedModel;

    /// <summary>
    /// Declares the 'delete' permission.
    /// Data can be deleted. May be intersected with other permissions to further restrict specific sections.
    /// </summary>
    [PermissionInfo(DataPermissionTokenName.Delete, Scoping.Type | Scoping.Instance)]
    public interface IDeletePermission : IPermission
    {
    }
}