// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceInfoCollectionBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Kephas.Collections;
using Kephas.DependencyInjection.Builder.Configuration;
using Kephas.DependencyInjection.Builder.Resources;
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
        var eligibleImplementationTypes = DependencyInjectionHelper.GetEligibleImplementationTypes(serviceAssemblies, IncludeInternal).ToList();
        return AddServices(Services, eligibleImplementationTypes, GetProviders(serviceAssemblies));
    }

    private IServiceInfoCollection AddServices(
        IServiceInfoCollection services,
        IList<Type> eligibleImplementationTypes,
        IEnumerable<IServiceInfoProvider> serviceInfoProviders)
    {
        // get all type infos from the injection assemblies
        var serviceDeclarationInfos = serviceInfoProviders
            .SelectMany(p => p.GetServiceDeclarationInfos())
            .Select(t => t with
            {
                ServiceDeclarationType = t.ServiceDeclarationType.ToNormalizedType(),
            })
            .ToList();

        // create and normalize declarations
        var serviceImplementationDeclarations = serviceInfoProviders
            .SelectMany(p => p.GetImplementationDeclarations(eligibleImplementationTypes))
            .Select(d => new ServiceImplementationDeclaration(d.ImplementationType.ToNormalizedType(), d.ServiceDeclarationType.ToNormalizedType()))
            .ToList();

        return AddServices(services, serviceDeclarationInfos, serviceImplementationDeclarations);
    }

    private IEnumerable<IServiceInfoProvider> GetProviders(IList<Assembly> serviceAssemblies)
    {
        var assemblyProviders = serviceAssemblies
            .SelectMany(a =>
                a.GetCustomAttributes().OfType<IServiceInfoProvider>()
                    .Select(p => (provider: p, priority: p.GetPriority())));

        var builderProviders = Providers
            .Select(p => (provider: p, priority: Priority.Normal));
        
        var providers = builderProviders
            .Union([(provider: new TypesServiceInfoProvider(Types, IncludeInternal), priority: Priority.Normal)])
            .Union(assemblyProviders);

        return providers
            .OrderBy(kv => kv.priority)
            .Select(kv => kv.provider);
    }

    private IList<Assembly> GetServiceAssemblies()
    {
        // exclude the assemblies marked with the ExcludeAssembliesFromDependencyInjectionAttribute
        // but only from the collected assemblies
        // leave the ones the user added explicitly
        var collectedAssemblies = GetAppAssemblies().ToList();
        var exclusionAttributes = collectedAssemblies.Union(Assemblies)
            .SelectMany(a => a.GetCustomAttributes<ExcludeAssembliesFromDependencyInjectionAttribute>())
            .ToList();
        collectedAssemblies = collectedAssemblies
            .Where(a => exclusionAttributes.TrueForAll(e => !e.IsExcluded(a)))
            .ToList();
        
        collectedAssemblies.AddRange(Assemblies);
        return collectedAssemblies;
    }
    
    private static IServiceInfoCollection AddServices(
        IServiceInfoCollection services,
        IEnumerable<ServiceDeclarationInfo> serviceDeclarationInfos,
        IEnumerable<ServiceImplementationDeclaration> implementationDeclarations)
    {
        var existingDeclarations = services.TryGetServiceInstance<IServiceDeclarationInfoCollection>();
        serviceDeclarationInfos = existingDeclarations is null
            ? new ServiceDeclarationInfoCollection(serviceDeclarationInfos.ToList())
            : new ServiceDeclarationInfoCollection(existingDeclarations.Union(serviceDeclarationInfos).ToList());
        services.Replace((IServiceDeclarationInfoCollection)serviceDeclarationInfos);

        var clusters = BuildServicesClusters(
            serviceDeclarationInfos,
            implementationDeclarations);

        var validationResults = new List<ValidationResult>();
        foreach (var servicesCluster in clusters)
        {
            // order the services by override and processing priority, resolve overrides
            // and with the rest of the services register them in the computed order.
            var sortedRegistrations = servicesCluster.SortRegistrationsDescending();
            foreach (var appServiceInfo in sortedRegistrations)
            {
                var validationResult = AddAppService(services, appServiceInfo);
                if (validationResult != ValidationResult.Success)
                {
                    validationResults.Add(validationResult!);
                }
            }
        }

        return services;
    }

    private static ValidationResult? AddAppService(
        IServiceInfoCollection appServices,
        IServiceInfo serviceInfo)
    {
        var result = CheckServiceType(serviceInfo);

        appServices.Add(serviceInfo);

        return result;
    }

    internal static ValidationResult? CheckServiceType(IServiceInfo serviceInfo)
    {
        var serviceDeclarationType = serviceInfo.ServiceDeclarationType;
        var serviceType = serviceInfo.ServiceType;
        if (serviceDeclarationType is null || serviceType is null)
        {
            return ValidationResult.Success;
        }

        if (serviceType.IsGenericTypeDefinition)
        {
            // TODO check to see if any of the interfaces have as generic definition the exported contract.
        }
        else if (!serviceType.IsAssignableFrom(serviceDeclarationType))
        {
            // Warning: the serviceType and the declaration type might not be compatible.
            var serviceValidationMessage = string.Format(
                Strings.AppServiceContractTypeDoesNotMatchServiceContract,
                serviceType,
                serviceDeclarationType);
            return new ValidationResult(serviceValidationMessage);
        }
        
        return ValidationResult.Success;
    }

    private static IEnumerable<ServicesCluster> BuildServicesClusters(
        IEnumerable<ServiceDeclarationInfo> serviceDeclarationInfos,
        IEnumerable<ServiceImplementationDeclaration> serviceImplementationDeclarations)
    {
        var serviceMap = new ServicesClusterMap();

        foreach (var serviceDeclarationInfo in serviceDeclarationInfos)
        {
            serviceMap.AddServiceDeclarationInfo(serviceDeclarationInfo);
        }

        serviceImplementationDeclarations.ForEach(si => serviceMap.AddServiceImplementationDeclaration(si));

        return serviceMap.GetClusters();
    }
}
