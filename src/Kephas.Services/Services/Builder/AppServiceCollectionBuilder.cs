// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the injector builder context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Builder
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    using Kephas.Application;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Services.Configuration;

    /// <summary>
    /// A context for building the <see cref="IAppServiceCollection"/>.
    /// </summary>
    public class AppServiceCollectionBuilder : Expando, IAppServiceCollectionBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceCollectionBuilder"/> class.
        /// </summary>
        /// <param name="settings">Optional. The injection settings.</param>
        /// <param name="logger">Optional. The logger.</param>
        public AppServiceCollectionBuilder(AppServicesSettings? settings = null, ILogger? logger = null)
            : this(new AppServiceCollection(), settings, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceCollectionBuilder"/> class.
        /// </summary>
        /// <param name="appServices">The application services.</param>
        /// <param name="settings">Optional. The injection settings.</param>
        /// <param name="logger">Optional. The logger.</param>
        public AppServiceCollectionBuilder(IAppServiceCollection appServices, AppServicesSettings? settings = null, ILogger? logger = null)
        {
            this.AppServices = appServices;
            this.Settings = settings ?? new AppServicesSettings();
            this.Logger = logger ?? appServices.TryGetServiceInstance<ILogManager>()?.GetLogger(this.GetType());
        }

        /// <summary>
        /// Gets The application services.
        /// </summary>
        public IAppServiceCollection AppServices { get; }

        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <value>
        /// The application service information providers.
        /// </value>
        public ICollection<IAppServiceInfoProvider> Providers { get; } = new List<IAppServiceInfoProvider>();

        /// <summary>
        /// Gets the list of assemblies used in injection.
        /// </summary>
        public ICollection<Assembly> Assemblies { get; } = new List<Assembly>();

        /// <summary>
        /// Gets the injection settings.
        /// </summary>
        public AppServicesSettings Settings { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger? Logger { get; }

        /// <summary>
        /// Adds the application services from the <see cref="IAppServiceInfoProvider"/>s identified in the assemblies.
        /// </summary>
        /// <returns>The provided app services.</returns>
        public IAppServiceCollection Build()
        {
            var providers = this.GetProviders();
#if NETSTANDARD2_1
            // for versions prior to .NET 6.0 make sure that the assemblies are initialized.
            IAssemblyInitializer.EnsureAssembliesInitialized();
#endif
            IAppServiceCollection.RegisterCollectedAppServices(this.AppServices);
            return this.AddAppServices(this.AppServices, providers, this.Settings.AmbiguousResolutionStrategy);
        }

        private IAppServiceCollection AddAppServices(
            IAppServiceCollection appServices,
            IEnumerable<IAppServiceInfoProvider> appServiceInfoProviders,
            AmbiguousServiceResolutionStrategy resolutionStrategy = AmbiguousServiceResolutionStrategy.ForcePriority)
        {
            var logger = this.Logger;
            if (logger.IsDebugEnabled())
            {
                logger.Debug("Adding app services from providers '{appServiceInfoProviders}...", appServiceInfoProviders);
            }

            // get all type infos from the injection assemblies
            var appServiceInfoList = appServiceInfoProviders
                .SelectMany(p => p.GetAppServiceContracts())
                .Select(t => t with
                {
                    ContractDeclarationType = t.ContractDeclarationType.ToNormalizedType(),
                })
                .ToList();

            if (logger.IsDebugEnabled())
            {
                logger.Debug("Aggregating the service types...");
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            IEnumerable<ServiceDeclaration> GetAppServices(IAppServiceInfoProvider appServiceInfoProvider)
            {
                if (logger.IsDebugEnabled())
                {
                    logger.Debug("Getting the app services from provider {provider}...", appServiceInfoProvider);
                }

                var providerAppServices = appServiceInfoProvider.GetAppServices();

                if (logger.IsTraceEnabled())
                {
                    logger.Trace("Getting the app services from provider {provider} succeeded.", appServiceInfoProvider);
                }

                return providerAppServices;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ServiceDeclaration NormalizeAppService(ServiceDeclaration serviceDeclaration)
            {
                var (serviceType, contractDeclarationType) = serviceDeclaration;

                if (logger.IsTraceEnabled())
                {
                    logger.Trace("Normalizing the service declaration for {serviceType}/{contractDeclarationType}.", serviceType, contractDeclarationType);
                }

                return new ServiceDeclaration(serviceType.ToNormalizedType(), contractDeclarationType.ToNormalizedType());
            }

            var serviceTypes = appServiceInfoProviders
                .SelectMany(GetAppServices)
                .Select(NormalizeAppService)
                .ToList();

            return appServices.AddAppServices(appServiceInfoList, serviceTypes, resolutionStrategy, logger);
        }

        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <param name="appAssemblies">The application assemblies.</param>
        /// <returns>
        /// An enumeration of <see cref="IAppServiceInfoProvider"/> objects.
        /// </returns>
        private IEnumerable<IAppServiceInfoProvider> GetProviders(IEnumerable<Assembly> appAssemblies)
        {
            appAssemblies = appAssemblies ?? throw new ArgumentNullException(nameof(appAssemblies));

            var providers = appAssemblies
                .SelectMany(a => a.GetCustomAttributes().OfType<IAppServiceInfoProvider>())
                .OrderBy(a => a is IHasProcessingPriority hasPriority ? hasPriority.ProcessingPriority : Priority.Normal);

            return providers;
        }

        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <returns>
        /// An enumeration of <see cref="IAppServiceInfoProvider"/> objects.
        /// </returns>
        private IEnumerable<IAppServiceInfoProvider> GetProviders()
        {
            var assemblies = this.GetBuildAssemblies();

            if (this.Logger.IsDebugEnabled())
            {
                try
                {
                    this.Logger.Debug("Using application assemblies: {assemblies}.", assemblies.Select(a => $"{a?.GetName()?.Name}, {a?.GetName()?.Version}").ToList());
                }
                catch (Exception ex)
                {
                    this.Logger.Debug(ex, "Error while logging application assemblies.");
                }
            }

            var providers = this.GetProviders(assemblies)
                .Union(this.Providers)
                .ToList();

            return providers;
        }

        /// <summary>
        /// Gets the assemblies for the <see cref="Build"/>.
        /// </summary>
        /// <returns>A list of assemblies.</returns>
        private IList<Assembly> GetBuildAssemblies()
        {
            var searchPattern = this.Settings.AssemblyFileNamePattern;

            this.Logger.Debug("{operation}. With assemblies matching pattern '{searchPattern}'.", nameof(GetBuildAssemblies), searchPattern);

            var assemblies = this.AppServices.TryGetServiceInstance<IAppRuntime>()?.GetAppAssemblies().ToList() ?? new List<Assembly>();
            assemblies.AddRange(this.Assemblies);
            var appAssemblies = assemblies.Where(a => !a.IsSystemAssembly());

            if (string.IsNullOrWhiteSpace(searchPattern))
            {
                return appAssemblies.ToList();
            }

            var regex = new Regex(searchPattern);
            return appAssemblies.Where(a => regex.IsMatch(a.FullName!)).ToList();
        }
    }
}