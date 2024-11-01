// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceInfoCollectionBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using Kephas.Collections;
using Kephas.DependencyInjection.Builder.Configuration;
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
    /// <param name="settings">The builder settings.</param>
    public ServiceInfoCollectionBuilder(ServiceInfosBuilderSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        GetAppAssemblies = settings.GetAppAssemblies;
        Services = settings.Services ?? new ServiceInfoCollection();
        IncludeInternal = settings.IncludeInternal;
        ConfigureBuilder = settings.ConfigureBuilder;
    }

    /// <summary>
    /// Builder to configure the service information collection.
    /// </summary>
    protected internal Action<ServiceInfoCollectionBuilder>? ConfigureBuilder { get; }

    /// <summary>
    /// Indicates whether the internal implementations should be considered or not.
    /// </summary>
    protected internal bool IncludeInternal { get; }

    /// <summary>
    /// Gets the function for retrieving the application assemblies.
    /// </summary>
    protected internal Func<IEnumerable<Assembly>> GetAppAssemblies { get; }

    /// <summary>
    /// Gets the application services.
    /// </summary>
    protected internal IServiceInfoCollection Services { get; }

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
    /// Adds a range of <see cref="IServiceInfo"/>s.
    /// </summary>
    /// <param name="serviceInfos">An array of <see cref="IServiceInfo"/>.</param>
    /// <returns>This builder.</returns>
    public ServiceInfoCollectionBuilder AddServiceInfos(params IServiceInfo[] serviceInfos)
    {
        Services.AddRange(serviceInfos);
        return this;
    }

    /// <summary>
    /// Adds a range of <see cref="IServiceInfoProvider"/>s.
    /// </summary>
    /// <param name="providers">An array of providers.</param>
    /// <returns>This builder.</returns>
    public ServiceInfoCollectionBuilder AddProviders(params IServiceInfoProvider[] providers)
    {
        Providers.AddRange(providers);
        return this;
    }

    /// <summary>
    /// Adds a range of <see cref="Assembly"/>s.
    /// </summary>
    /// <param name="assemblies">An array of assemblies.</param>
    /// <returns>This builder.</returns>
    public ServiceInfoCollectionBuilder AddAssemblies(params Assembly[] assemblies)
    {
        Assemblies.AddRange(assemblies);
        return this;
    }

    /// <summary>
    /// Adds a range of <see cref="Type"/>s.
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
    public IServiceInfoCollection Build()
    {
        ConfigureBuilder?.Invoke(this);
        var serviceAssemblies = GetServiceAssemblies();
        var implementationTypes = DependencyInjectionHelper.GetEligibleImplementationTypes(serviceAssemblies, IncludeInternal).ToList();
        return AddServices(Services, implementationTypes, GetProviders(serviceAssemblies, implementationTypes));
    }

    private IServiceInfoCollection AddServices(
        IServiceInfoCollection services,
        IList<Type> implementationTypes,
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

        // create and normalize declarations
        var serviceImplementationDeclarations = serviceInfoProviders
            .SelectMany(p => p.GetImplementationDeclarations(implementationTypes))
            .Select(d => new ServiceImplementationDeclaration(d.ImplementationType.ToNormalizedType(), d.ServiceDeclarationType.ToNormalizedType()))
            .ToList();

        return services.AddServices(serviceDeclarationInfos, serviceImplementationDeclarations);
    }

    private IEnumerable<IServiceInfoProvider> GetProviders(IList<Assembly> serviceAssemblies, IList<Type> implementationTypes)
    {
        var assemblyProviders = serviceAssemblies
            .SelectMany(a =>
                a.GetCustomAttributes().OfType<IServiceInfoProvider>()
                    .Select(p => (provider: p, priority: p.GetPriority())));
        
        var userDefinedProviders = implementationTypes
            .Where(t => typeof(IServiceInfoProvider).IsAssignableFrom(t))
            .Select(t => (provider: (IServiceInfoProvider)Activator.CreateInstance(t)!, priority: t.GetCustomAttribute<PriorityAttribute>()?.Value ?? Priority.Normal));

        var builderProviders = Providers
            .Select(p => (provider: p, priority: Priority.Normal));
        
        var providers = builderProviders
            .Union([(provider: new TypesServiceInfoProvider(Types), priority: Priority.Normal)])
            .Union(userDefinedProviders)
            .Union(assemblyProviders);

        return providers
            .OrderBy(kv => kv.priority)
            .Select(kv => kv.provider);
    }

    private IList<Assembly> GetServiceAssemblies()
    {
        var assemblies = GetAppAssemblies().ToList();
        assemblies.AddRange(Assemblies);
        var appAssemblies = assemblies;

        return appAssemblies.ToList();
    }
}
