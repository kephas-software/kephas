// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the injector builder context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Builder
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using Kephas.Dynamic;
    using Kephas.Injection.Configuration;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A context for building the <see cref="IAmbientServices"/>.
    /// </summary>
    public class AppServiceCollectionBuilder : Expando, IAppServiceCollectionBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceCollectionBuilder"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="settings">Optional. The injection settings.</param>
        /// <param name="logger">Optional. The logger.</param>
        public AppServiceCollectionBuilder(IAmbientServices ambientServices, InjectionSettings? settings = null, ILogger? logger = null)
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

        /// <summary>
        /// Adds the application services from the <see cref="IAppServiceInfosProvider"/>s identified in the assemblies.
        /// </summary>
        /// <returns>The provided ambient services.</returns>
        public IAmbientServices Build()
        {
            var providers = this.GetAppServiceInfosProviders();
            return this.AddAppServices(this.AmbientServices, providers, this.Settings.AmbiguousResolutionStrategy);
        }

        private IAmbientServices AddAppServices(
            IAmbientServices ambientServices,
            IEnumerable<IAppServiceInfosProvider> appServiceInfoProviders,
            AmbiguousServiceResolutionStrategy resolutionStrategy = AmbiguousServiceResolutionStrategy.ForcePriority)
        {
            var logger = this.Logger;
            if (logger.IsDebugEnabled())
            {
                logger.Debug("Adding app services from providers '{appServiceInfosProviders}...", appServiceInfoProviders);
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
            IEnumerable<ServiceDeclaration> GetAppServices(IAppServiceInfosProvider appServiceInfosProvider)
            {
                if (logger.IsDebugEnabled())
                {
                    logger.Debug("Getting the app services from provider {provider}...", appServiceInfosProvider);
                }

                var appServices = appServiceInfosProvider.GetAppServices();

                if (logger.IsTraceEnabled())
                {
                    logger.Trace("Getting the app services from provider {provider} succeeded.", appServiceInfosProvider);
                }

                return appServices;
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

            return ambientServices.AddAppServices(appServiceInfoList, serviceTypes, resolutionStrategy, logger);
        }

        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <param name="appAssemblies">The application assemblies.</param>
        /// <returns>
        /// An enumeration of <see cref="IAppServiceInfosProvider"/> objects.
        /// </returns>
        private IEnumerable<IAppServiceInfosProvider> GetAppServiceInfosProviders(IEnumerable<Assembly> appAssemblies)
        {
            appAssemblies = appAssemblies ?? throw new ArgumentNullException(nameof(appAssemblies));

            var providers = appAssemblies
                .SelectMany(a => a.GetCustomAttributes().OfType<IAppServiceInfosProvider>())
                .OrderBy(a => a is IHasProcessingPriority hasPriority ? hasPriority.ProcessingPriority : Priority.Normal);

            return providers;
        }

        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <returns>
        /// An enumeration of <see cref="IAppServiceInfosProvider"/> objects.
        /// </returns>
        private IEnumerable<IAppServiceInfosProvider> GetAppServiceInfosProviders()
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

            var providers = this.GetAppServiceInfosProviders(assemblies)
                .Union(this.AppServiceInfosProviders)
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

            var appAssemblies = this.Assemblies.Where(a => !a.IsSystemAssembly());

            if (string.IsNullOrWhiteSpace(searchPattern))
            {
                return appAssemblies.ToList();
            }

            var regex = new Regex(searchPattern);
            return appAssemblies.Where(a => regex.IsMatch(a.FullName!)).ToList();
        }
    }
}