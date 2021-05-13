// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityModelPermissionScopeResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Security.Permissions
{
    using Kephas.Data.Security.Permissions;
    using Kephas.Model;
    using Kephas.Services;

    /// <summary>
    /// Permission scope resolver using the model space.
    /// </summary>
    [OverridePriority(Priority.BelowNormal)]
    public class EntityModelPermissionScopeResolver : EntityPermissionScopeResolverBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityModelPermissionScopeResolver"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="contextFactory">The context factory.</param>
        public EntityModelPermissionScopeResolver(IModelSpace typeRegistry, IContextFactory contextFactory)
            : base(typeRegistry, contextFactory)
        {
        }
    }
}