// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultProjectedTypeResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default projected type resolver class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.AttributedModel;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A default projected type resolver.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultProjectedTypeResolver : IProjectedTypeResolver
    {
        /// <summary>
        /// The type resolver.
        /// </summary>
        private readonly ITypeResolver typeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultProjectedTypeResolver"/> class.
        /// </summary>
        /// <param name="typeResolver">The type resolver.</param>
        public DefaultProjectedTypeResolver(ITypeResolver typeResolver)
        {
            Requires.NotNull(typeResolver, nameof(typeResolver));

            this.typeResolver = typeResolver;
        }

        /// <summary>
        /// Resolves the projected type.
        /// </summary>
        /// <exception cref="ServiceException">Thrown when a Service error condition occurs.</exception>
        /// <param name="projectionType">The projection type.</param>
        /// <param name="context">An optional context for the resolution.</param>
        /// <param name="throwOnNotFound">Indicates whether to throw or not when the indicated type is not found.</param>
        /// <returns>
        /// The resolved type or <c>null</c>, if <paramref name="throwOnNotFound" /> is set to false and
        /// a projected type could not be found.
        /// </returns>
        public virtual Type ResolveProjectedType(Type projectionType, IContext context = null, bool throwOnNotFound = true)
        {
            Requires.NotNull(projectionType, nameof(projectionType));

            var projectionAttr = this.GetProjectionForAttribute(projectionType, context);
            if (projectionAttr == null)
            {
                return projectionType;
            }

            var projectedType = projectionAttr.ProjectedType
                                   ?? this.typeResolver.ResolveType(projectionAttr.ProjectedTypeName, throwOnNotFound);

            return projectedType;
        }

        /// <summary>
        /// Gets the projection for attribute.
        /// </summary>
        /// <param name="projectionType">The projection type.</param>
        /// <param name="context">An optional context for the resolution.</param>
        /// <returns>
        /// The projection for attribute.
        /// </returns>
        protected virtual ProjectionForAttribute GetProjectionForAttribute(Type projectionType, IContext context)
        {
            var projectionAttr = projectionType.GetTypeInfo().GetCustomAttribute<ProjectionForAttribute>();
            return projectionAttr;
        }
    }
}