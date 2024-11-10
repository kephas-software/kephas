// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueryPermission.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Security.Permissions
{
    using Kephas.Security.Authorization;
    using Kephas.Security.Permissions.AttributedModel;

    /// <summary>
    /// Declares the 'query' permission.
    /// Data can be basically accessed using queries but its content can be read in a limited way, like the ID and name/title.
    /// This is required for displaying drop downs for referenced entities or for changing the entity owner.
    /// </summary>
    [PermissionInfo(DataPermissionTokenName.Query, Scoping.Type | Scoping.Instance)]
    public interface IQueryPermission
    {
    }
}