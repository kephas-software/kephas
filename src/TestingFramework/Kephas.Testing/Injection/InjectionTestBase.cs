// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Injection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Diagnostics.Logging;
    using Kephas.Injection;
    using Kephas.Injection.Builder;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Serialization;
    using Kephas.Testing;

    /// <summary>
    /// Base class for tests using dependency injection.
    /// </summary>
    /// <content>
    /// It includes:
    /// * Creating injector in different configurations.
    /// </content>
    public class InjectionTestBase : TestBase
    {
        /// <summary>
        /// Creates a <see cref="IAppServiceCollectionBuilder"/> for further configuration.
        /// </summary>
        /// <param name="ambientServices">Optional. The ambient services. If not provided, a new instance
        ///                               will be created as linked to the newly created container.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        /// <param name="appRuntime">Optional. The application runtime.</param>
        /// <returns>
        /// A LiteInjectorBuilder.
        /// </returns>
        private IAppServiceCollectionBuilder CreateAppServiceCollectionBuilder(
            IAmbientServices ambientServices,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var log = new StringBuilder();
            logManager ??= ambientServices?.TryGetServiceInstance<ILogManager>() ?? new DebugLogManager(log);
            appRuntime ??= this.CreateDefaultAppRuntime(logManager);

            ambientServices = ambientServices
                .Add(logManager)
                .WithAppRuntime(appRuntime)
                .Add(log);
            return new AppServiceCollectionBuilder(ambientServices);
        }

        /// <summary>
        /// Creates a container for the provided convention assemblies.
        /// </summary>
        /// <param name="assemblies">A variable-length parameters list containing assemblies.</param>
        /// <returns>
        /// The new container.
        /// </returns>
        public IServiceProvider BuildServiceProvider(params Assembly[] assemblies)
        {
            return this.BuildServiceProvider(assemblies: (IEnumerable<Assembly>)assemblies);
        }

        /// <summary>
        /// Creates a container for the provided convention assemblies and parts.
        /// </summary>
        /// <param name="assemblies">Optional. A variable-length parameters list containing assemblies.</param>
        /// <param name="parts">Optional. The parts.</param>
        /// <param name="config">Optional. The configuration.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        /// <param name="appRuntime">Optional. The application runtime.</param>
        /// <returns>
        /// The new container.
        /// </returns>
        public virtual IServiceProvider BuildServiceProvider(
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<IAmbientServices>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var ambientServices = this.CreateAmbientServices();
            var allParts = this.GetDefaultParts().ToList();
            allParts.AddRange(parts ?? Type.EmptyTypes);
            var servicesBuilder = this.CreateAppServiceCollectionBuilder(ambientServices, logManager, appRuntime);
            servicesBuilder.Assemblies
                .AddRange(this.GetAssemblies())
                .AddRange(assemblies ?? Array.Empty<Assembly>());
            servicesBuilder.Assemblies
                .WithParts(allParts);

            config?.Invoke(servicesBuilder);

            var container = servicesBuilder.Build();
            ambientServices.Add(container);
            return container;
        }

        /// <summary>
        /// Gets the default convention assemblies to be considered when building the container. By default it includes Kephas.Core.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the default convention assemblies in
        /// this collection.
        /// </returns>
        public virtual IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>
                       {
                           typeof(IAmbientServices).Assembly,       /* Kephas.Injection */
                           typeof(IEventHub).Assembly,              /* Kephas.Interaction */
                           typeof(ISerializationService).Assembly,  /* Kephas.Serialization */
                       };
        }

        /// <summary>
        /// Gets the default parts to be included in the container.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the default parts in this collection.
        /// </returns>
        public virtual IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>();
        }
    }
}