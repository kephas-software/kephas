// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPermissionScopeResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization.Permissions
{
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Application service contract for resolving
    /// </summary>
    [AppServiceContract]
    public interface IPermissionScopeResolver
    {
        /// <summary>
        /// Tries to resolve the permission scope name of the provided type information,
        /// provided it is defined within a scope.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <param name="context">Optional. The operation context.</param>
        /// <returns>The permission scope name.</returns>
        string? ResolveScopeName(ITypeInfo typeInfo, IContext? context = null);
    }
}