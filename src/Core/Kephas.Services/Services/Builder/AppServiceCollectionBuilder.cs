// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the injector builder context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Kephas.Collections;
using Microsoft.Extensions.Logging;
using Kephas.Logging;
using Kephas.Reflection;
using Kephas.Runtime;
using Kephas.Services.Configuration;

namespace Kephas.Services.Builder;

/// <summary>
/// A context for building the <see cref="IAppServiceCollection"/>.
/// </summary>
public class AppServiceCollectionBuilder : IAppServiceCollectionBuilder
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
        AppServices = appServices;
        Settings = settings ?? new AppServicesSettings();
        Logger = logger;
    }

    /// <summary>
    /// Gets The application services.
    /// </summary>
    protected internal IAppServiceCollection AppServices { get; }

    /// <summary>
    /// Gets the application service information providers.
    /// </summary>
    /// <value>
    /// The application service information providers.
    /// </value>
    protected internal ICollection<IAppServiceInfoProvider> Providers { get; } = new List<IAppServiceInfoProvider>();

    /// <summary>
    /// Gets the list of assemblies used in injection.
    /// </summary>
    protected internal ICollection<Assembly> Assemblies { get; } = new List<Assembly>();

    /// <summary>
    /// Gets the list of types used in injection.
    /// </summary>
    protected internal ICollection<Type> Types { get; } = new List<Type>();

    /// <summary>
    /// Gets the injection settings.
    /// </summary>
    protected internal AppServicesSettings Settings { get; }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    /// <value>
    /// The logger.
    /// </value>
    protected ILogger? Logger { get; }

    /// <summary>
    /// Adds the <see cref="IAppServiceInfoProvider"/>s.
    /// </summary>
    /// <param name="providers">An array of providers.</param>
    /// <returns>This builder.</returns>
    public IAppServiceCollectionBuilder AddProviders(params IAppServiceInfoProvider[] providers)
    {
        Providers.AddRange(providers);
        return this;
    }

    /// <summary>
    /// Adds the <see cref="Assembly"/>s.
    /// </summary>
    /// <param name="assemblies">An array of assemblies.</param>
    /// <returns>This builder.</returns>
    public IAppServiceCollectionBuilder AddAssemblies(params Assembly[] assemblies)
    {
        Assemblies.AddRange(assemblies);
        return this;
    }

    /// <summary>
    /// Adds the <see cref="Type"/>s.
    /// </summary>
    /// <param name="types">An array of types.</param>
    /// <returns>This builder.</returns>
    public IAppServiceCollectionBuilder AddTypes(params Type[] types)
    {
        Types.AddRange(types);
        return this;
    }

    /// <summary>
    /// Adds the application services from the <see cref="IAppServiceInfoProvider"/>s identified in the assemblies.
    /// </summary>
    /// <returns>The provided app services.</returns>
    public IAppServiceCollection Build()
    {
        var providers = GetProviders();
        IAppServiceCollection.RegisterCollectedAppServices(AppServices);
        return AddAppServices(AppServices, providers, Settings.AmbiguousResolutionStrategy);
    }

    private IAppServiceCollection AddAppServices(
        IAppServiceCollection appServices,
        IEnumerable<IAppServiceInfoProvider> appServiceInfoProviders,
        AmbiguousServiceResolutionStrategy resolutionStrategy = AmbiguousServiceResolutionStrategy.ForcePriority)
    {
        var logger = Logger;
        if (logger.IsDebugEnabled())
        {
            logger?.LogDebug("Adding app services from providers '{appServiceInfoProviders}...", appServiceInfoProviders);
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
            logger?.LogDebug("Aggregating the service types...");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerable<ServiceDeclaration> GetAppServices(IAppServiceInfoProvider appServiceInfoProvider)
        {
            if (logger.IsDebugEnabled())
            {
                logger?.LogDebug("Getting the app services from provider {provider}...", appServiceInfoProvider);
            }

            var providerAppServices = appServiceInfoProvider.GetAppServices();

            if (logger.IsTraceEnabled())
            {
                logger?.LogTrace("Getting the app services from provider {provider} succeeded.", appServiceInfoProvider);
            }

            return providerAppServices;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ServiceDeclaration NormalizeAppService(ServiceDeclaration serviceDeclaration)
        {
            var (serviceType, contractDeclarationType) = serviceDeclaration;

            if (logger.IsTraceEnabled())
            {
                logger?.LogTrace("Normalizing the service declaration for {serviceType}/{contractDeclarationType}.", serviceType, contractDeclarationType);
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
            .OrderBy(a => a is IHasPriority hasPriority ? hasPriority.Priority : Priority.Normal);

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
        var assemblies = GetServiceAssemblies();

        if (Logger.IsDebugEnabled())
        {
            try
            {
                Logger?.LogDebug("Using application assemblies: {assemblies}.", assemblies.Select(a => $"{a?.GetName()?.Name}, {a?.GetName()?.Version}").ToList());
            }
            catch (Exception ex)
            {
                Logger?.LogDebug(ex, "Error while logging application assemblies.");
            }
        }

        var providers = GetProviders(assemblies)
            .Union(Providers)
            .ToList();

        if (Types.Count > 0)
        {
            providers.Add(new TypesAppServiceInfoProvider(Types));
        }

        return providers;
    }

    private IList<Assembly> GetServiceAssemblies()
    {
        var searchPattern = Settings.ServiceAssemblyFileNamePattern;

        Logger?.LogDebug("{operation}. With assemblies matching pattern '{searchPattern}'.", nameof(GetServiceAssemblies), searchPattern);

        var getAssemblies = Settings.GetServiceAssemblies
                            ?? (() => AssemblyLoader.Instance.LoadAssemblies());

        var assemblies = getAssemblies()?.ToList() ?? new List<Assembly>();
        assemblies.AddRange(Assemblies);
        var appAssemblies = assemblies.Where(a => !a.IsSystemAssembly());

        if (string.IsNullOrWhiteSpace(searchPattern))
        {
            return appAssemblies.ToList();
        }

        var regex = new Regex(searchPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return appAssemblies.Where(a => regex.IsMatch(a.FullName!)).ToList();
    }
}
