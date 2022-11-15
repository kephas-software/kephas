﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConventionsRuntimeModelRegistryBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Model.Reflection;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Base class for conventions based runtime model serviceRegistry.
    /// </summary>
    public abstract class ConventionsRuntimeModelRegistryBase : Loggable, IRuntimeModelRegistry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionsRuntimeModelRegistryBase"/> class.
        /// </summary>
        /// <param name="injectableFactory">The injectable factory.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="typeLoader">Optional. The type loader.</param>
        /// <param name="options">Optional. The configuration options.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        protected ConventionsRuntimeModelRegistryBase(
            IInjectableFactory injectableFactory,
            IAmbientServices ambientServices,
            ITypeLoader? typeLoader = null,
            Action<ModelRegistryConventions>? options = null,
            ILogManager? logManager = null)
            : this(injectableFactory, () => ResolveTypes(ambientServices, typeLoader ?? DefaultTypeLoader.Instance), options, logManager)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionsRuntimeModelRegistryBase"/> class.
        /// </summary>
        /// <param name="injectableFactory">The injectable factory.</param>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="options">Optional. The configuration options.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        protected ConventionsRuntimeModelRegistryBase(
            IInjectableFactory injectableFactory,
            Func<IEnumerable<Type>> typeResolver,
            Action<ModelRegistryConventions>? options = null,
            ILogManager? logManager = null)
            : base(logManager)
        {
            typeResolver = typeResolver ?? throw new ArgumentNullException(nameof(typeResolver));
            this.TypeResolver = typeResolver;

            this.Conventions = injectableFactory.Create<ModelRegistryConventions>();
            options?.Invoke(this.Conventions);
        }

        /// <summary>
        /// Gets the type resolver.
        /// </summary>
        protected Func<IEnumerable<Type>> TypeResolver { get; }

        /// <summary>
        /// Gets the conventions.
        /// </summary>
        protected ModelRegistryConventions Conventions { get; }

        /// <summary>
        /// Gets the runtime elements.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding an enumeration of runtime elements.
        /// </returns>
        public async Task<IEnumerable<object>> GetRuntimeElementsAsync(CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            var markerBaseTypes = this.Conventions.MarkerBaseTypes;
            var markerAttributeTypes = this.Conventions.MarkerAttributeTypes;
            var includeInterfaces = this.Conventions.IncludeInterfaces;
            var includeClasses = this.Conventions.IncludeClasses;
            var includeAbstractClasses = this.Conventions.IncludeAbstractClasses;
            var excludeMarkers = this.Conventions.ExcludeMarkers;
            return new HashSet<Type>(this.TypeResolver()
                .Where(t => IsModelType(t, markerBaseTypes, markerAttributeTypes, excludeMarkers, includeClasses, includeAbstractClasses, includeInterfaces)
                            && !t.IsExcludedFromModel()));
        }

        private static bool IsModelType(
            Type type,
            Type[]? markerBaseTypes,
            Type[]? markerAttributeTypes,
            bool excludeMarkers,
            bool includeClasses,
            bool includeAbstractClasses,
            bool includeInterfaces)
        {
            if (markerBaseTypes != null
                && markerBaseTypes.Any(
                    marker =>
                        IncludeType(type, includeClasses, includeAbstractClasses, includeInterfaces)
                        && marker.IsAssignableFrom(type)
                        && (!excludeMarkers || marker != type)))
            {
                return true;
            }

            if (markerAttributeTypes != null
                && markerAttributeTypes.Any(
                    attrType =>
                        IncludeType(type, includeClasses, includeAbstractClasses, includeInterfaces)
                        && HasAttributeOfType(type, attrType)))
            {
                return true;
            }

            return false;
        }

        private static bool HasAttributeOfType(Type type, Type attrType)
            => type.GetCustomAttributes().Any(attrType.IsInstanceOfType);

        private static IEnumerable<Type> ResolveTypes(IAmbientServices ambientServices, ITypeLoader typeLoader)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            typeLoader = typeLoader ?? throw new System.ArgumentNullException(nameof(typeLoader));

            var assemblies = ambientServices.GetAppAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (var type in typeLoader.GetExportedTypes(assembly))
                {
                    yield return type;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IncludeType(Type type, bool includeClasses, bool includeAbstractClasses, bool includeInterfaces)
        {
            return (includeClasses && type.IsClass && (includeAbstractClasses || !type.IsAbstract))
                   || (includeInterfaces && type.IsInterface);
        }
    }
}