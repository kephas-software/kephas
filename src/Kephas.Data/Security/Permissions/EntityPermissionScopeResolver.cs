// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityPermissionScopeResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Security.Permissions
{
    using Kephas.Data.Model.Associations;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Permission scope resolver for entities.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class EntityPermissionScopeResolver : EntityPermissionScopeResolverBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPermissionScopeResolver"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="associationGraphProvider">The association graph provider.</param>
        /// <param name="contextFactory">The context factory.</param>
        public EntityPermissionScopeResolver(
            IRuntimeTypeRegistry typeRegistry,
            ITypeAssociationGraphProvider associationGraphProvider,
            IContextFactory contextFactory)
            : base(typeRegistry, associationGraphProvider, contextFactory)
        {
        }
    }
}