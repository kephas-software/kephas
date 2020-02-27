// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowModelRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging model registry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Model.Runtime.ModelRegistries
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
    using Kephas.Model.Reflection;
    using Kephas.Model.Runtime;
    using Kephas.Reflection;

    /// <summary>
    /// A workflow model registry.
    /// </summary>
    public class WorkflowModelRegistry : IRuntimeModelRegistry
    {
        private readonly IAppRuntime appRuntime;
        private readonly IAssemblyLoader assemblyLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowModelRegistry"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="assemblyLoader">The type loader.</param>
        public WorkflowModelRegistry(IAppRuntime appRuntime, IAssemblyLoader assemblyLoader)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));
            Requires.NotNull(assemblyLoader, nameof(assemblyLoader));

            this.appRuntime = appRuntime;
            this.assemblyLoader = assemblyLoader;
        }

        /// <summary>
        /// Gets the runtime elements.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of runtime elements.
        /// </returns>
        public Task<IEnumerable<object>> GetRuntimeElementsAsync(CancellationToken cancellationToken = default)
        {
            var assemblies = this.appRuntime.GetAppAssemblies();

            var types = new HashSet<Type>();
            var markerInterface = typeof(IActivity).GetTypeInfo();
            foreach (var assembly in assemblies)
            {
                types.AddRange(this.assemblyLoader.GetExportedTypes(assembly).Where(
                    t =>
                        {
                            var ti = t.GetTypeInfo();
                            return this.IsActivity(ti, markerInterface) && !ti.IsExcludedFromModel();
                        }));
            }

            return Task.FromResult<IEnumerable<object>>(types);
        }

        /// <summary>
        /// Query if 'type' is activity.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="markerInterface">The marker interface.</param>
        /// <returns>
        /// True if the type is a activity, false if not.
        /// </returns>
        private bool IsActivity(TypeInfo type, TypeInfo markerInterface)
        {
            return type.IsClass && !type.IsAbstract && markerInterface.IsAssignableFrom(type);
        }
    }
}