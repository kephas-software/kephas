// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionBuildContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the injector builder context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Dynamic;
    using Kephas.Injection.Configuration;
    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// A context for building the <see cref="IAmbientServices"/>.
    /// </summary>
    public class AppServiceCollectionBuildContext : Expando, IAppServiceCollectionBuildContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceCollectionBuildContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="settings">Optional. The injection settings.</param>
        /// <param name="logger">Optional. The logger.</param>
        public AppServiceCollectionBuildContext(IAmbientServices ambientServices, InjectionSettings? settings = null, ILogger? logger = null)
        {
            this.AmbientServices = ambientServices;
            this.Assemblies = ambientServices.GetAppRuntime().GetAppAssemblies().ToList();
            this.Settings = settings ?? new InjectionSettings();
            this.Logger = logger ?? ambientServices.TryGetServiceInstance<ILogManager>()?.GetLogger(this.GetType());
        }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        public IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <value>
        /// The application service information providers.
        /// </value>
        public ICollection<IAppServiceInfosProvider> AppServiceInfosProviders { get; } = new List<IAppServiceInfosProvider>();

        /// <summary>
        /// Gets the list of assemblies used in injection.
        /// </summary>
        public ICollection<Assembly> Assemblies { get; }

        /// <summary>
        /// Gets the injection settings.
        /// </summary>
        public InjectionSettings Settings { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger? Logger { get; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        void IDisposable.Dispose()
        {
        }
    }
}