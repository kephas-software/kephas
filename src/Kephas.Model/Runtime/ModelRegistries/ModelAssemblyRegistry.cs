// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelAssemblyRegistry.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        /// Initializes a new instance of the <see cref="ModelAssemblyRegistry"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="typeLoader">The type loader.</param>
        public ModelAssemblyRegistry(IAppRuntime appRuntime, ITypeLoader typeLoader)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));

            this.appRuntime = appRuntime;
            this.typeLoader = typeLoader;
        }

        /// <summary>
        /// Gets the runtime elements from the application assemblies.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of runtime elements.
        /// </returns>
        public async Task<IEnumerable<object>> GetRuntimeElementsAsync(
            CancellationToken cancellationToken = default)
        {
            var assemblies = await this.appRuntime.GetAppAssembliesAsync(cancellationToken: cancellationToken).PreserveThreadContext();
            var eligibleAssemblyPairs = (from kv in from a in assemblies
                                                    select
                                                    new KeyValuePair<Assembly, IList<ModelAssemblyAttribute>>(
                                                        a,
                                                        a.GetCustomAttributes<ModelAssemblyAttribute>().ToList())
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
                        this.ProcessModelTypes(attr.ModelTypes, attr);
                        types.AddRange(attr.ModelTypes);
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
                        this.ProcessModelTypes(namespaceTypes, attr);

                        types.AddRange(namespaceTypes);
                        filterSet = true;
                    }

                    // if no filter was set, then add all the types in the assembly
                    if (!filterSet)
                    {
                        this.ProcessModelTypes(assemblyTypes, attr);
                        types.AddRange(assemblyTypes);
                    }
                }
            }

            return types;
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
                }
            }
        }
    }
}