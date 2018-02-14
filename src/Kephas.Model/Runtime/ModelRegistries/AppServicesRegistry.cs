// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceContractsRegistry.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application service contracts registry class.
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
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An application service contracts registry.
    /// </summary>
    public class AppServicesRegistry : IRuntimeModelRegistry
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
        /// Initializes a new instance of the <see cref="AppServicesRegistry"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="typeLoader">The type loader.</param>
        public AppServicesRegistry(IAppRuntime appRuntime, ITypeLoader typeLoader)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));

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
        public async Task<IEnumerable<object>> GetRuntimeElementsAsync(CancellationToken cancellationToken = default)
        {
            var assemblies = await this.appRuntime.GetAppAssembliesAsync(cancellationToken: cancellationToken).PreserveThreadContext();

            var types = new HashSet<Type>();
            foreach (var assembly in assemblies)
            {
                types.AddRange(this.typeLoader.GetLoadableExportedTypes(assembly).Where(
                    t =>
                        {
                            var ti = t.GetTypeInfo();
                            return ti.GetCustomAttribute<AppServiceContractAttribute>() != null;
                        }));
            }

            return types;
        }
    }
}