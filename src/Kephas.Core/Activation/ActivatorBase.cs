// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivatorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the activator base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Activation
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Services;

    /// <summary>
    /// Base abstract class activators.
    /// </summary>
    public abstract class ActivatorBase : IActivator
    {
        /// <summary>
        /// The implementation type map.
        /// </summary>
        private readonly ConcurrentDictionary<ITypeInfo, ITypeInfo> implementationTypeMap = new ConcurrentDictionary<ITypeInfo, ITypeInfo>();

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
            IEnumerable<object> args = null,
            IContext activationContext = null)
        {
            var implementationType = this.GetImplementationType(typeInfo, activationContext);
            var runtimeType = implementationType as IDynamicTypeInfo;
            if (runtimeType == null)
            {
                throw new ActivationException(string.Format(Strings.ActivatorBase_CannotInstantiateAbstractTypeInfo_Exception, implementationType, typeof(IDynamicTypeInfo), typeInfo));
            }

            return runtimeType.CreateInstance(args);
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
        public virtual ITypeInfo GetImplementationType(
            ITypeInfo abstractType,
            IContext activationContext = null,
            bool throwOnNotFound = true)
        {
            var implementationType = this.implementationTypeMap.GetOrAdd(
                abstractType,
                _ => this.GetImplementationTypeCore(abstractType, activationContext, false));

            if (implementationType == null && throwOnNotFound)
            {
                throw new ActivationException(string.Format(Strings.ActivatorBase_CannotInstantiateAbstractTypeInfo_Exception, abstractType));
            }

            return implementationType;
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
        protected virtual ITypeInfo GetImplementationTypeCore(
            ITypeInfo abstractType,
            IContext activationContext = null,
            bool throwOnNotFound = true)
        {
            return abstractType;
        }
    }
}