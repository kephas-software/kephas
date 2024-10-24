// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Kephas.Collections;
using Kephas.DependencyInjection.Configuration;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Reflection;
using Kephas.Runtime;

namespace Kephas.DependencyInjection.Builder;

/// <summary>
/// A context for building a collection of <see cref="IServiceInfo"/>.
/// </summary>
public class ServiceCollectionBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceCollectionBuilder"/> class.
    /// </summary>
    /// <param name="settings">Optional. The injection settings.</param>
    public ServiceCollectionBuilder(DependencyInjectionSettings? settings = null)
        : this(new List<IServiceInfo>(), settings)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceCollectionBuilder"/> class.
    /// </summary>
    /// <param name="services">The application services.</param>
    /// <param name="settings">Optional. The injection settings.</param>
    public ServiceCollectionBuilder(IEnumerable<IServiceInfo> services, DependencyInjectionSettings? settings = null)
    {
        Services = new List<IServiceInfo>(services);
        Settings = settings ?? new DependencyInjectionSettings();
    }

    /// <summary>
    /// Gets The application services.
    /// </summary>
    protected internal IEnumerable<IServiceInfo> Services { get; }

    /// <summary>
    /// Gets the application service information providers.
    /// </summary>
    /// <value>
    /// The application service information providers.
    /// </value>
    protected internal ICollection<IServiceInfoProvider> Providers { get; } = new List<IServiceInfoProvider>();

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
    protected internal DependencyInjectionSettings Settings { get; }

    /// <summary>
    /// Adds the <see cref="IServiceInfoProvider"/>s.
    /// </summary>
    /// <param name="providers">An array of providers.</param>
    /// <returns>This builder.</returns>
    public ServiceCollectionBuilder AddProviders(params IServiceInfoProvider[] providers)
    {
        Providers.AddRange(providers);
        return this;
    }

    /// <summary>
    /// Adds the <see cref="Assembly"/>s.
    /// </summary>
    /// <param name="assemblies">An array of assemblies.</param>
    /// <returns>This builder.</returns>
    public ServiceCollectionBuilder AddAssemblies(params Assembly[] assemblies)
    {
        Assemblies.AddRange(assemblies);
        return this;
    }

    /// <summary>
    /// Adds the <see cref="Type"/>s.
    /// </summary>
    /// <param name="types">An array of types.</param>
    /// <returns>This builder.</returns>
    public ServiceCollectionBuilder AddTypes(params Type[] types)
    {
        Types.AddRange(types);
        return this;
    }

    /// <summary>
    /// Adds the application services from the <see cref="IServiceInfoProvider"/>s identified in the assemblies.
    /// </summary>
    /// <returns>The provided app services.</returns>
    public IServiceInfoCollection Build() => AddServices(new ServiceInfoCollection(Services), GetProviders());

    private IServiceInfoCollection AddServices(
        IServiceInfoCollection services,
        IEnumerable<IServiceInfoProvider> serviceInfoProviders)
    {
        // get all type infos from the injection assemblies
        var serviceDeclarationInfos = serviceInfoProviders
            .SelectMany(p => p.GetServiceInfos())
            .Select(t => t with
            {
                ServiceDeclarationType = t.ServiceDeclarationType.ToNormalizedType(),
            })
            .ToList();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerable<ServiceImplementationDeclaration> GetImplementationDeclarations(IServiceInfoProvider appServiceInfoProvider)
        {
            var serviceImplementations = appServiceInfoProvider.GetImplementationDeclarations();

            return serviceImplementations;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ServiceImplementationDeclaration NormalizeDeclaration(ServiceImplementationDeclaration serviceDeclaration)
        {
            var (serviceType, serviceDeclarationType) = serviceDeclaration;

            return new ServiceImplementationDeclaration(serviceType.ToNormalizedType(), serviceDeclarationType.ToNormalizedType());
        }

        var serviceTypes = serviceInfoProviders
            .SelectMany(GetImplementationDeclarations)
            .Select(NormalizeDeclaration)
            .ToList();

        return services.AddServices(serviceDeclarationInfos, serviceTypes);
    }

    /// <summary>
    /// Gets the application service information providers.
    /// </summary>
    /// <param name="appAssemblies">The application assemblies.</param>
    /// <returns>
    /// An enumeration of <see cref="IServiceInfoProvider"/> objects.
    /// </returns>
    private static IEnumerable<IServiceInfoProvider> GetProviders(IEnumerable<Assembly> appAssemblies)
    {
        appAssemblies = appAssemblies ?? throw new ArgumentNullException(nameof(appAssemblies));

        var providers = appAssemblies
            .SelectMany(a => a.GetCustomAttributes().OfType<IServiceInfoProvider>())
            .OrderBy(a => a is IHasPriority hasPriority ? hasPriority.Priority : Priority.Normal);

        return providers;
    }

    /// <summary>
    /// Gets the application service information providers.
    /// </summary>
    /// <returns>
    /// An enumeration of <see cref="IServiceInfoProvider"/> objects.
    /// </returns>
    private IEnumerable<IServiceInfoProvider> GetProviders()
    {
        var assemblies = GetServiceAssemblies();

        var providers = GetProviders(assemblies)
            .Union(Providers)
            .ToList();

        if (Types.Count > 0)
        {
            providers.Add(new TypesServiceInfoProvider(Types));
        }

        return providers;
    }

    private IList<Assembly> GetServiceAssemblies()
    {
        var searchPattern = Settings.ServiceAssemblyNamePattern;
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
