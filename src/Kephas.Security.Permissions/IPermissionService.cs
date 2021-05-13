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

    using Kephas.Reflection;
    using Kephas.Security.Authorization.Reflection;
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
        /// <returns>An enumeration of <see cref="IPermission"/>.</returns>
        IEnumerable<IPermission> GetGrantedPermissions(IIdentity identity, IContext? context = null);

        /// <summary>
        /// Gets the supported permissions for the provided type.
        /// </summary>
        /// <param name="typeInfo">Type of the entity.</param>
        /// <param name="context">Optional. The context for the operation.</param>
        /// <returns>
        /// An enumeration of <see cref="IPermissionInfo"/>.
        /// </returns>
        IEnumerable<IPermissionInfo> GetSupportedPermissions(ITypeInfo typeInfo, IContext? context = null);
    }
}