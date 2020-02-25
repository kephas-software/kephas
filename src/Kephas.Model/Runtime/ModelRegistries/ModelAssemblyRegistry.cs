// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelAssemblyRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the model assembly registry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.ModelRegistries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Reflection;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Registry reading the <see cref="ModelAssemblyAttribute"/> and providing the types
    /// exported by the attribute.
    /// </summary>
    public class ModelAssemblyRegistry : IRuntimeModelRegistry
    {
        /// <summary>
        /// The application runtime.
        /// </summary>
        private readonly IAppRuntime appRuntime;

        /// <summary>
        /// The type loader.
        /// </summary>
        private readonly ITypeLoader typeLoader;

        /// <summary>
        /// The model assembly attribute provider.
        /// </summary>
        private readonly IModelAssemblyAttributeProvider modelAssemblyAttributeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelAssemblyRegistry"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="typeLoader">The type loader.</param>
        /// <param name="modelAssemblyAttributeProvider">The model assembly attribute provider.</param>
        public ModelAssemblyRegistry(IAppRuntime appRuntime, ITypeLoader typeLoader, IModelAssemblyAttributeProvider modelAssemblyAttributeProvider)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));
            Requires.NotNull(typeLoader, nameof(typeLoader));
            Requires.NotNull(modelAssemblyAttributeProvider, nameof(modelAssemblyAttributeProvider));

            this.appRuntime = appRuntime;
            this.typeLoader = typeLoader;
            this.modelAssemblyAttributeProvider = modelAssemblyAttributeProvider;
        }

        /// <summary>
        /// Gets the runtime elements from the application assemblies.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of runtime elements.
        /// </returns>
        public Task<IEnumerable<object>> GetRuntimeElementsAsync(CancellationToken cancellationToken = default)
        {
            var assemblies = this.appRuntime.GetAppAssemblies();
            var eligibleAssemblyPairs = (from kv in from a in assemblies
                                                    select
                                                    new KeyValuePair<Assembly, IList<ModelAssemblyAttribute>>(
                                                        a,
                                                        this.modelAssemblyAttributeProvider.GetModelAssemblyAttributes(a).ToList())
                                         where kv.Value.Count > 0
                                         select kv).ToList();

            var types = new HashSet<Type>();
            foreach (var kv in eligibleAssemblyPairs)
            {
                var assembly = kv.Key;

                var attrs = kv.Value;
                var assemblyTypes = this.typeLoader.GetLoadableExportedTypes(assembly).ToList();
                foreach (var attr in attrs)
                {
                    var filterSet = false;

                    // first of all process all explicitely provided model types.
                    if (attr.ModelTypes != null && attr.ModelTypes.Length > 0)
                    {
                        this.AddModelTypes(types, attr.ModelTypes, attr);
                        filterSet = true;
                    }

                    // then add the types indicated by their namespace.
                    if (attr.ModelNamespaces != null && attr.ModelNamespaces.Length > 0)
                    {
                        var namespaceFilter = ModelAssemblyAttribute.GetModelAssemblyNamespaceFilter(new[] { attr });
                        IEnumerable<Type> eligibleTypes = assemblyTypes;
                        if (namespaceFilter != null)
                        {
                            eligibleTypes = eligibleTypes.Where(namespaceFilter);
                        }

                        var namespaceTypes = eligibleTypes.ToList();

                        this.AddModelTypes(types, namespaceTypes, attr);
                        filterSet = true;
                    }

                    // if no filter was set, then add all the types in the assembly
                    if (!filterSet)
                    {
                        this.AddModelTypes(types, assemblyTypes, attr);
                    }
                }
            }

            return Task.FromResult<IEnumerable<object>>(types);
        }

        /// <summary>
        /// Adds the model types to the collection.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="collectedTypes">List of collected types.</param>
        /// <param name="attr">The model assembly attribute.</param>
        private void AddModelTypes(HashSet<Type> types, IEnumerable<Type> collectedTypes, ModelAssemblyAttribute attr)
        {
            var nonExcludedTypes = collectedTypes.Where(t => !t.IsExcludedFromModel()).ToList();
            this.ProcessModelTypes(nonExcludedTypes, attr);
            types.AddRange(nonExcludedTypes);
        }

        /// <summary>
        /// Process the model types by setting the classifier kind.
        /// </summary>
        /// <param name="modelTypes">List of types of the models.</param>
        /// <param name="modelAssemblyAttribute">The model assembly attribute.</param>
        private void ProcessModelTypes(IEnumerable<Type> modelTypes, ModelAssemblyAttribute modelAssemblyAttribute)
        {
            // process the matching types by marking them as model types and setting the default classifier kind.
            var defaultClassifierKind = modelAssemblyAttribute.DefaultClassifierKindAttribute?.ClassifierType;
            foreach (var type in modelTypes)
            {
                var runtimeTypeInfo = type.AsRuntimeTypeInfo();
                if (defaultClassifierKind != null)
                {
                    runtimeTypeInfo.SetClassifierKind(defaultClassifierKind);
                    runtimeTypeInfo.SetIsModelType(true);
                }
            }
        }
    }
}