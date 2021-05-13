// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadPermission.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Security.Permissions
{
    using Kephas.Security.Authorization;
    using Kephas.Security.Permissions.AttributedModel;

    /// <summary>
    /// Declares the 'read' permission.
    /// The content of the data can be read. May be intersected with other permissions to further restrict specific sections.
    /// </summary>
    [PermissionInfo(DataPermissionTokenName.Read, Scoping.Type | Scoping.Instance)]
    public interface IReadPermission : IQueryPermission
    {
    }
}