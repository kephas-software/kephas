// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultWebHost.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default web host class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ServiceStack.Hosting
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Funq;

    using global::ServiceStack;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.ServiceStack.Hosting.Composition;

    /// <summary>
    /// The default web host.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultWebHost
#if NETSTANDARD2_0
        : AppHostBase, IWebHost
#else
        : AppSelfHostBase, IWebHost
#endif
    {
        /// <summary>
        /// The ambient services.
        /// </summary>
        private readonly IAmbientServices ambientServices;

        /// <summary>
        /// The application manifest.
        /// </summary>
        private readonly IAppManifest appManifest;

        /// <summary>
        /// The endpoint service provider.
        /// </summary>
        private readonly IEndpointServiceProvider endpointServiceProvider;

        /// <summary>
        /// The host configurator factories.
        /// </summary>
        private readonly ICollection<IExportFactory<IHostConfigurator, EndpointMetadata>> hostConfiguratorFactories;

        /// <summary>
        /// The base urls.
        /// </summary>
        private string[] baseUrls;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWebHost"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="appManifest">The application manifest.</param>
        /// <param name="endpointServiceProvider">The endpoint service provider.</param>
        /// <param name="hostConfiguratorFactories">The host configurator factories.</param>
        public DefaultWebHost(
            IAmbientServices ambientServices,
            IAppManifest appManifest,
            IEndpointServiceProvider endpointServiceProvider,
            ICollection<IExportFactory<IHostConfigurator, EndpointMetadata>> hostConfiguratorFactories)
            : base(endpointServiceProvider.ServiceName, endpointServiceProvider.ServiceAssemblies)
        {
            this.ambientServices = ambientServices;
            this.appManifest = appManifest;
            this.endpointServiceProvider = endpointServiceProvider;
            this.hostConfiguratorFactories = hostConfiguratorFactories;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<DefaultWebHost> Logger { get; set; }

        /// <summary>
        ///     Configures the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        public override void Configure(Container container)
        {
            var hostConfigurators = (from s in this.hostConfiguratorFactories
                                     where s.Metadata.RequiredFeature == null || this.appManifest.ContainsFeature(s.Metadata.RequiredFeature)
                                     orderby s.Metadata.ProcessingPriority
                                     select s.CreateExportedValue()).ToList();
            this.Logger.Info("Endpoint configurators: " + string.Join(", ", hostConfigurators.Select(c => c.GetType().Name)));

            var configContext = new HostConfigurationContext(this.ambientServices, this, new HostConfig());

            foreach (var configurator in hostConfigurators)
            {
                configurator.Configure(configContext);
            }

            this.SetConfig(configContext.HostConfig);
            this.baseUrls = configContext.BaseUrls;
        }

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        public virtual async Task InitializeAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
            this.Logger.Info($"Initializing {this.GetType()} for service {this.endpointServiceProvider.ServiceName}...");
            this.Init();
            this.Logger.Info($"Finished Initializing {this.GetType()}");

            var configuredBaseUrls = this.GetBaseUrls();
            this.Logger.Info($"Starting {this.GetType()} for service {this.endpointServiceProvider.ServiceName} at URLs { string.Join(",", configuredBaseUrls)}...");
            this.Start(configuredBaseUrls);

            // show welcome messages
            this.Logger.Info($"Welcome to {this.appManifest.AppId}.");
        }

        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">(Optional) An optional context for finalization.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public virtual Task FinalizeAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
#if NETSTANDARD2_0
#else
            this.Stop();
#endif
            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets the base urls.
        /// </summary>
        /// <returns>
        /// An array of string.
        /// </returns>
        protected virtual string[] GetBaseUrls()
        {
            return this.baseUrls ?? new[] { "http://localhost", "https://localhost" };
        }
    }
}