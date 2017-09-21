// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultProjectedTypeResolver.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default projected type resolver class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Services
{
    using System;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Resources;
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
        /// <param name="projectionType">The projectionType.</param>
        /// <param name="throwOnNotFound">Indicates whether to throw or not when the indicated type is not found.</param>
        /// <returns>
        /// A Type.
        /// </returns>
        public Type ResolveProjectedType(Type projectionType, bool throwOnNotFound = true)
        {
            Requires.NotNull(projectionType, nameof(projectionType));

            var projectionAttr = projectionType.GetTypeInfo().GetCustomAttribute<ProjectionForAttribute>();
            if (projectionAttr == null)
            {
                if (throwOnNotFound)
                {
                    throw new ModelException(string.Format(Strings.DefaultProjectTypeResolver_ResolveProjectedType_MissingProjectionForAttribute_Exception, projectionType.FullName, typeof(ProjectionForAttribute).Name));
                }

                return null;
            }

            var projectedType = projectionAttr.ProjectedType
                                   ?? this.typeResolver.ResolveType(projectionAttr.ProjectedTypeName, throwOnNotFound);

            return projectedType;
        }
    }
}