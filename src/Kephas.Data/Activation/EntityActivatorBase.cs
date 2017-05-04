// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityActivatorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the entity activator base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Activation
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Activation;
    using Kephas.Data.AttributedModel;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Base class for activators of entities.
    /// </summary>
    public abstract class EntityActivatorBase : ActivatorBase
    {
        /// <summary>
        /// Gets the entity types.
        /// </summary>
        /// <returns>
        /// An enumeration of entity types.
        /// </returns>
        protected abstract IEnumerable<ITypeInfo> GetEntityTypes();

        /// <summary>
        /// Tries to get the native type information.
        /// </summary>
        /// <param name="abstractType">Indicates the abstract type.</param>
        /// <returns>
        /// A TypeInfo.
        /// </returns>
        protected virtual TypeInfo TryGetTypeInfo(ITypeInfo abstractType)
        {
            return (abstractType as IRuntimeTypeInfo)?.TypeInfo;
        }

        /// <summary>
        /// Gets the type implementing the abstract type provided as the parameter.
        /// </summary>
        /// <param name="abstractType">Indicates the abstract type.</param>
        /// <param name="activationContext">Context for the activation.</param>
        /// <param name="throwOnNotFound">Indicates whether to throw an exception if an implementation type is not found.</param>
        /// <returns>
        /// The implementation type for the provided <see cref="ITypeInfo"/>.
        /// </returns>
        protected override ITypeInfo GetImplementationTypeCore(
            ITypeInfo abstractType,
            IContext activationContext = null,
            bool throwOnNotFound = true)
        {
            var runtimeTypeInfo = this.TryGetTypeInfo(abstractType);
            if (runtimeTypeInfo.IsAbstract || runtimeTypeInfo.IsInterface)
            {
                var entityType = this.GetEntityTypes().FirstOrDefault(ti => ti.Annotations.OfType<EntityForAttribute>().Any(a => a.ModelType == runtimeTypeInfo.AsType()));
                return entityType;
            }

            return abstractType;
        }
    }
}