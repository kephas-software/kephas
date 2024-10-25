// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceInfoCollectionBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using Kephas.Collections;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Reflection;

namespace Kephas.DependencyInjection.Builder;

/// <summary>
/// A context for building a collection of <see cref="IServiceInfo"/>.
/// </summary>
public class ServiceInfoCollectionBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceInfoCollectionBuilder"/> class.
    /// </summary>
    /// <param name="getAppAssemblies">The function for retrieving the application assemblies.</param>
    public ServiceInfoCollectionBuilder(Func<IEnumerable<Assembly>> getAppAssemblies)
        : this(new List<IServiceInfo>(), getAppAssemblies)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceInfoCollectionBuilder"/> class.
    /// </summary>
    /// <param name="services">The application services.</param>
    /// <param name="getAppAssemblies">The function for retrieving the application assemblies.</param>
    public ServiceInfoCollectionBuilder(IEnumerable<IServiceInfo> services, Func<IEnumerable<Assembly>> getAppAssemblies)
    {
        GetAppAssemblies = getAppAssemblies ?? throw new ArgumentNullException(nameof(getAppAssemblies));
        Services = new List<IServiceInfo>(services);
    }

    /// <summary>
    /// Gets the function for retrieving the application assemblies.
    /// </summary>
    protected internal Func<IEnumerable<Assembly>> GetAppAssemblies { get; }

    /// <summary>
    /// Gets the application services.
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
    /// Adds the <see cref="IServiceInfoProvider"/>s.
    /// </summary>
    /// <param name="providers">An array of providers.</param>
    /// <returns>This builder.</returns>
    public ServiceInfoCollectionBuilder AddProviders(params IServiceInfoProvider[] providers)
    {
        Providers.AddRange(providers);
        return this;
    }

    /// <summary>
    /// Adds the <see cref="Assembly"/>s.
    /// </summary>
    /// <param name="assemblies">An array of assemblies.</param>
    /// <returns>This builder.</returns>
    public ServiceInfoCollectionBuilder AddAssemblies(params Assembly[] assemblies)
    {
        Assemblies.AddRange(assemblies);
        return this;
    }

    /// <summary>
    /// Adds the <see cref="Type"/>s.
    /// </summary>
    /// <param name="types">An array of types.</param>
    /// <returns>This builder.</returns>
    public ServiceInfoCollectionBuilder AddTypes(params Type[] types)
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
        var assemblies = GetAppAssemblies().ToList();
        assemblies.AddRange(Assemblies);
        var appAssemblies = assemblies;

        return appAssemblies.ToList();
    }
}
