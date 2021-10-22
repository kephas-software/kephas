// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivatorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the activator base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Activation
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Base abstract class activators.
    /// </summary>
    public abstract class ActivatorBase : IActivator
    {
        /// <summary>
        /// The implementation type map.
        /// </summary>
        private readonly ConcurrentDictionary<ITypeInfo, ITypeInfo?> implementationTypeMap = new ();

        /// <summary>
        /// Creates an instance of the provided <see cref="ITypeInfo"/>.
        /// </summary>
        /// <param name="typeInfo">Indicates the <see cref="ITypeInfo"/> used for the instantiation.</param>
        /// <param name="args">Constructor arguments.</param>
        /// <param name="activationContext">Context for the activation.</param>
        /// <remarks>
        /// The <paramref name="typeInfo"/> may be either an implementation type or an abstract type.
        /// If an abstract type is provided, then an implementation type is determined first and then instantiated.
        /// </remarks>
        /// <returns>
        /// An instance of the provided <see cref="ITypeInfo"/>.
        /// </returns>
        public virtual object CreateInstance(
            ITypeInfo typeInfo,
            IEnumerable<object?>? args = null,
            IContext? activationContext = null)
        {
            var implementationType = this.GetImplementationType(typeInfo, activationContext);
            if (implementationType == null)
            {
                throw new ActivationException(string.Format(ReflectionStrings.ActivatorBase_CannotInstantiateAbstractTypeInfo_Exception, typeInfo));
            }

            return implementationType.CreateInstance(args);
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
        public virtual ITypeInfo? GetImplementationType(
            ITypeInfo abstractType,
            IContext? activationContext = null,
            bool throwOnNotFound = true)
        {
            var implementationType = this.implementationTypeMap.GetOrAdd(
                abstractType,
                _ => this.ComputeImplementationType(abstractType, activationContext, false));

            if (implementationType == null && throwOnNotFound)
            {
                throw new ActivationException(string.Format(ReflectionStrings.ActivatorBase_CannotInstantiateAbstractTypeInfo_Exception, abstractType));
            }

            return implementationType;
        }

        /// <summary>
        /// Gets the supported implementation types.
        /// </summary>
        /// <remarks>
        /// The <see cref="ActivatorBase"/> class provides always an empty list
        /// of implementation types. The inheritors should provide a proper list of
        /// supported implementation types annotated with <see cref="ImplementationForAttribute"/>,
        /// otherwise only non-abstract types will be resolved.
        /// </remarks>
        /// <returns>
        /// An enumeration of implementation types.
        /// </returns>
        protected virtual IEnumerable<ITypeInfo> GetImplementationTypes() => Enumerable.Empty<ITypeInfo>();

        /// <summary>
        /// Tries to get the native type information.
        /// </summary>
        /// <param name="abstractType">Indicates the abstract type.</param>
        /// <returns>
        /// A TypeInfo.
        /// </returns>
        protected virtual TypeInfo? TryGetTypeInfo(ITypeInfo abstractType)
        {
            return (abstractType as IRuntimeTypeInfo)?.TypeInfo;
        }

        /// <summary>
        /// Computes the type implementing the abstract type provided as the parameter.
        /// </summary>
        /// <param name="abstractType">Indicates the abstract type.</param>
        /// <param name="activationContext">Context for the activation.</param>
        /// <param name="throwOnNotFound">Indicates whether to throw an exception if an implementation type is not found.</param>
        /// <returns>
        /// The implementation type for the provided <see cref="ITypeInfo"/>.
        /// </returns>
        protected virtual ITypeInfo? ComputeImplementationType(
            ITypeInfo abstractType,
            IContext? activationContext = null,
            bool throwOnNotFound = true)
        {
            var runtimeTypeInfo = this.TryGetTypeInfo(abstractType);
            if (runtimeTypeInfo != null && (runtimeTypeInfo.IsAbstract || runtimeTypeInfo.IsInterface))
            {
                var runtimeType = runtimeTypeInfo.AsType();
                var entityType = this.GetImplementationTypes().FirstOrDefault(
                    ti => ti.Annotations.OfType<ImplementationForAttribute>().Any(
                        a => a.AbstractType == runtimeType || a.AbstractTypeParts.Contains(runtimeType)));
                return entityType;
            }

            return abstractType;
        }
    }
}