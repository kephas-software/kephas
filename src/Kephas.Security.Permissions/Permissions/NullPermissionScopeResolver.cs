// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullPermissionScopeResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Permissions
{
    using System.Linq;

    using Kephas.Reflection;
    using Kephas.Security.Permissions.AttributedModel;
    using Kephas.Services;

    /// <summary>
    /// Permission scope resolver returning only the first level in scope.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullPermissionScopeResolver : IPermissionScopeResolver
    {
        /// <summary>
        /// Tries to resolve the permission scope name of the provided type information,
        /// provided it is defined within a scope.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <param name="context">Optional. The operation context.</param>
        /// <returns>The permission scope name.</returns>
        public string? ResolveScopeName(ITypeInfo typeInfo, IContext? context = null)
        {
            var attr = typeInfo.Annotations.OfType<IPermissionScopeAnnotation>().FirstOrDefault();
            return attr?.ScopeName;
        }
    }
}