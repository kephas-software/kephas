// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;

    using Kephas.Services.Builder;

    /// <summary>
    /// Default implementation of <see cref="AppBase"/>.
    /// </summary>
    public class App : AppBase
    {
        private readonly Action<IAppServiceCollectionBuilder> servicesConfig;
        private readonly Func<IAppServiceCollectionBuilder, IServiceProvider> servicesProviderBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <param name="servicesConfig">The services configuration.</param>
        /// <param name="servicesProviderBuilder">The services provider builder.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        public App(
            Action<IAppServiceCollectionBuilder> servicesConfig,
            Func<IAppServiceCollectionBuilder, IServiceProvider> servicesProviderBuilder,
            IAppArgs? appArgs = null)
            : base(appArgs)
        {
            this.servicesConfig = servicesConfig ?? throw new ArgumentNullException(nameof(servicesConfig));
            this.servicesProviderBuilder = servicesProviderBuilder ?? throw new ArgumentNullException(nameof(servicesProviderBuilder));
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="servicesBuilder">The service builder.</param>
        protected override void ConfigureServices(IAppServiceCollectionBuilder servicesBuilder)
        {
            this.servicesConfig(servicesBuilder);
        }

        /// <summary>
        /// This is the last step in the app's configuration, when all the services are set up
        /// and the container is built. For inheritors, this is the last place where services can
        /// be added before calling. By default, it only builds the Lite container, but any other container adapter
        /// can be used, like Autofac or System.Composition.
        /// </summary>
        /// <remarks>
        /// Override this method to initialize the startup services, like log manager and configuration manager.
        /// </remarks>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <returns>The service provider.</returns>
        protected override IServiceProvider BuildServiceProvider(IAppServiceCollectionBuilder servicesBuilder)
        {
            return this.servicesProviderBuilder(servicesBuilder);
        }
    }
}