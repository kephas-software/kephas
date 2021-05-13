// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPermissionService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization.Permissions
{
    using System.Collections.Generic;
    using System.Security.Principal;

    using Kephas.Services;

    /// <summary>
    /// Service for handling permissions.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IPermissionService
    {
        /// <summary>
        /// Gets the granted permissions for the provided identity.
        /// </summary>
        /// <param name="identity">The identity holding the grants.</param>
        /// <param name="context">Optional. The context for the operation.</param>
        /// <returns>An enumeration of permissions.</returns>
        IEnumerable<IPermission> GetGrantedPermissions(IIdentity identity, IContext? context = null);
    }
}