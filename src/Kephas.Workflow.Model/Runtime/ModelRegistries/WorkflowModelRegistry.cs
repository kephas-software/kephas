// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowModelRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging model registry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

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
        private readonly ITypeLoader typeLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowModelRegistry"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="typeLoader">The type loader.</param>
        public WorkflowModelRegistry(IAppRuntime appRuntime, ITypeLoader typeLoader)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));
            Requires.NotNull(typeLoader, nameof(typeLoader));

            this.appRuntime = appRuntime;
            this.typeLoader = typeLoader;
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
                types.AddRange(this.typeLoader.GetExportedTypes(assembly).Where(
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