// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingModelRegistry.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the messaging model registry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Model.Runtime.ModelRegistries
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.Runtime;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A messaging model registry.
    /// </summary>
    public class MessagingModelRegistry : IRuntimeModelRegistry
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
        /// Initializes a new instance of the <see cref="MessagingModelRegistry"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="typeLoader">The type loader.</param>
        public MessagingModelRegistry(IAppRuntime appRuntime, ITypeLoader typeLoader)
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
        public async Task<IEnumerable<object>> GetRuntimeElementsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var assemblies = await this.appRuntime.GetAppAssembliesAsync(cancellationToken: cancellationToken).PreserveThreadContext();

            var types = new HashSet<Type>();
            var markerInterface = typeof(IMessage).GetTypeInfo();
            foreach (var assembly in assemblies)
            {
                types.AddRange(this.typeLoader.GetLoadableExportedTypes(assembly).Where(
                    t =>
                        {
                            var ti = t.GetTypeInfo();
                            return !Equals(ti, markerInterface) && markerInterface.IsAssignableFrom(ti) && ti.IsClass;
                        }));
            }

            return types;
        }
    }
}