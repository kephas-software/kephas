// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionServiceBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization.Permissions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;

    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Security.Authorization.AttributedModel;
    using Kephas.Services;

    /// <summary>
    /// Base abstract implementation of the <see cref="IPermissionService"/>.
    /// </summary>
    public abstract class PermissionServiceBase : Loggable, IPermissionService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionServiceBase"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        protected PermissionServiceBase(
            ITypeRegistry typeRegistry,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.TypeRegistry = typeRegistry;
        }

        /// <summary>
        /// Gets the type registry.
        /// </summary>
        protected ITypeRegistry TypeRegistry { get; }

        /// <summary>
        /// Gets the granted permissions for the provided identity.
        /// </summary>
        /// <param name="identity">The identity holding the grants.</param>
        /// <param name="context">Optional. The context for the operation.</param>
        /// <returns>An enumeration of permissions.</returns>
        public virtual IEnumerable<IPermission> GetGrantedPermissions(IIdentity identity, IContext? context = null)
        {
            var rawGrantedPermissions = this.GetGrantedPermissionsCore(identity, context);
            if (!rawGrantedPermissions.Any())
            {
                return Array.Empty<IPermission>();
            }

            var rawRootPermissions = rawGrantedPermissions
                .Select(p => new Permission(p).TokenName)
                .Distinct()
                .ToArray();

            // get scoped permissions
            var scopeTypes = this.TypeRegistry.Where(e => e.GetAttribute<PermissionScopeAttribute>() != null).ToList();

            //....
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the granted permissions as an enumeration of parsable strings.
        /// </summary>
        /// <param name="identity">The identity holding the grants.</param>
        /// <param name="context">Optional. The context for the operation.</param>
        /// <returns>An enumeration of permissions.</returns>
        protected virtual IEnumerable<string> GetGrantedPermissionsCore(IIdentity identity, IContext? context = null)
        {
            switch (identity)
            {
                case ClaimsIdentity claimsIdentity:
                    var permClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == Kephas.Security.Authorization.ClaimTypes.GrantedPermissions);
                    return permClaim?.Value?.Split(' ', ',', ';') ?? Array.Empty<string>();
                default:
                    return Array.Empty<string>();
            }
        }
    }
}