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
    using Kephas.Security.Authorization.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Base abstract implementation of the <see cref="IPermissionService"/>.
    /// </summary>
    public abstract class PermissionServiceBase : Loggable, IPermissionService
    {
        private IDictionary<Type, IPermissionInfo>? permissionTypeMap;
        private IDictionary<string, IPermissionInfo>? permissionNameMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionServiceBase"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="scopeResolver">The permission scope resolver.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        protected PermissionServiceBase(
            ITypeRegistry typeRegistry,
            IPermissionScopeResolver scopeResolver,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.TypeRegistry = typeRegistry;
            this.ScopeResolver = scopeResolver;
        }

        /// <summary>
        /// Gets the type registry.
        /// </summary>
        protected ITypeRegistry TypeRegistry { get; }

        /// <summary>
        /// Gets the permission scope resolver.
        /// </summary>
        protected IPermissionScopeResolver ScopeResolver { get; }

        /// <summary>
        /// Gets the granted permissions for the provided identity.
        /// </summary>
        /// <param name="identity">The identity holding the grants.</param>
        /// <param name="context">Optional. The context for the operation.</param>
        /// <returns>An enumeration of permissions.</returns>
        public virtual IEnumerable<IPermission> GetGrantedPermissions(IIdentity identity, IContext? context = null)
        {
            this.EnsureInitialized();

            var rawGrantedPermissions = this.GetGrantedPermissionsCore(identity, context);
            if (!rawGrantedPermissions.Any())
            {
                return Array.Empty<IPermission>();
            }

            var grantedPermissionTokens = rawGrantedPermissions
                .Select(p => Permission.Parse(p).TokenName)
                .Distinct()
                .ToArray();

            // get scope types for scoped permissions...
            var scopeTypes = this.GetScopeTypes(this.TypeRegistry, context);
            // ...and the scoped permissions
            var perms = scopeTypes
                .SelectMany(
                    e => this.GetSupportedPermissions(e).Cast<IPermissionInfo>()
                        .Where(p => p.Scoping != Scoping.Global && this.IsPermissionAccessible(p, grantedPermissionTokens))
                        .Select(p => new Permission(p.TokenName, this.ScopeResolver.ResolveScopeName(e, context), null)))
                .ToList();


            // get non-scoped permissions
            perms.AddRange(this.permissionTypeMap!.Values
                .Where(p => p.Scoping == Scoping.Global && this.IsPermissionAccessible(p, grantedPermissionTokens))
                .Select(p => p.TokenName));

            var availablePermissions = perms.Distinct().OrderBy(p => p).ToArray();
            return availablePermissions;
        }

        /// <summary>
        /// Gets the supported permissions for the provided type.
        /// </summary>
        /// <param name="typeInfo">Type of the entity.</param>
        /// <param name="context">Optional. The context for the operation.</param>
        /// <returns>
        /// An enumeration of <see cref="IPermissionInfo"/>.
        /// </returns>
        public virtual IEnumerable<IPermissionInfo> GetSupportedPermissions(ITypeInfo typeInfo, IContext? context = null)
        {
            this.EnsureInitialized();

            typeInfo = this.TypeRegistry.GetTypeInfo(typeInfo, throwOnNotFound: false) ?? typeInfo;
            return typeInfo.Annotations.OfType<ISupportsPermissionAnnotation>().SelectMany(
                a => a.PermissionTypes.Select(p => this.permissionTypeMap![p]));
        }

        /// <summary>
        /// Gets the scope types.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>A list of scope types.</returns>
        protected virtual IList<ITypeInfo> GetScopeTypes(ITypeRegistry typeRegistry, IContext? context = null)
        {
            return typeRegistry.Where(e => e.Annotations.OfType<IPermissionScopeAnnotation>().Any()).ToList();
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

        /// <summary>
        /// Tries to get the runtime type information from the <see cref="IPermissionInfo"/>.
        /// </summary>
        /// <param name="permissionInfo">The permission information.</param>
        /// <returns>The corresponding <see cref="IRuntimeTypeInfo"/>.</returns>
        protected virtual IRuntimeTypeInfo? TryGetPermissionType(IPermissionInfo permissionInfo) =>
            permissionInfo switch
            {
                IRuntimeTypeInfo runtimeTypeInfo => runtimeTypeInfo,
                IAggregatedElementInfo aggregatedElementInfo => aggregatedElementInfo.Parts.OfType<IRuntimeTypeInfo>().FirstOrDefault(),
                _ => null
            };

        /// <summary>
        /// Indicates whether the all of the required permissions of the provided permission definition are among the
        /// granted permission tokens.
        /// </summary>
        /// <param name="permissionInfo">The permission definition.</param>
        /// <param name="grantedPermissionTokens">The granted permission tokens.</param>
        /// <returns>A boolean value indicating whether the permission is accessible.</returns>
        protected virtual bool IsPermissionAccessible(IPermissionInfo permissionInfo, string[] grantedPermissionTokens)
        {
            return permissionInfo.RequiredPermissions.All(p => grantedPermissionTokens.Contains(p.TokenName));
        }

        private void EnsureInitialized()
        {
            // check the last variable set to avoid race conditions.
            if (this.permissionNameMap != null)
            {
                return;
            }

            var permissionTypes = this.TypeRegistry.OfType<IPermissionInfo>().ToList();
            this.permissionTypeMap = permissionTypes
                .Select(t => (type: this.TryGetPermissionType(t)?.Type, permInfo: t))
                .Where(kv => kv.type != null)
                .ToDictionary(
                kv => kv.type!,
                kv => kv.permInfo);
            this.permissionNameMap = permissionTypes.ToDictionary(
                e => e.TokenName,
                e => e);
        }
    }
}