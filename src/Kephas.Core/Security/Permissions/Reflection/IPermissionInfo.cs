// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPermissionInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Permissions.Reflection
{
    using System.Collections.Generic;

    using Kephas.Reflection;
    using Kephas.Security.Authorization.Reflection;

    /// <summary>
    /// Reflective information about a permission.
    /// </summary>
    public interface IPermissionInfo : IElementInfo, IScoped, IToken
    {
        /// <summary>
        /// Gets the granted permissions.
        /// </summary>
        /// <remarks>
        /// When this permission is granted, the permissions granted by this are also granted.
        /// Using this mechanism one can define a hierarchy of permissions.
        /// </remarks>
        /// <value>
        /// The granted permissions.
        /// </value>
        IEnumerable<IPermissionInfo> GrantedPermissions { get; }

        /// <summary>
        /// Gets the required permissions to access this permission.
        /// </summary>
        /// <value>
        /// The required permissions.
        /// </value>
        IEnumerable<IPermissionInfo> RequiredPermissions { get; }
    }
}