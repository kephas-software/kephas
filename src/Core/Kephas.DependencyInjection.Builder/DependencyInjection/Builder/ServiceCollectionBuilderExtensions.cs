// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections;
using Kephas.DependencyInjection.Builder.Resources;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.ServiceProviders;

namespace Kephas.DependencyInjection.Builder;

internal static class ServiceInfoCollectionBuilderExtensions
{
    /// <summary>
    /// Adds the application services from the provided <see cref="IServiceInfoProvider"/>s.
    /// </summary>
    /// <param name="services">The application services.</param>
    /// <param name="serviceDeclarations">The contract declarations.</param>
    /// <param name="implementationDeclarations">The service declarations.</param>
    /// <returns>The provided app services.</returns>
    internal static IServiceInfoCollection AddServices(
        this IServiceInfoCollection services,
        IEnumerable<ServiceDeclarationInfo> serviceDeclarations,
        IEnumerable<ServiceImplementationDeclaration> implementationDeclarations)
    {
        var existingDeclarations = services.TryGetServiceInstance<IServiceDeclarationInfoCollection>();
        serviceDeclarations = existingDeclarations is null
            ? new ServiceDeclarationInfoCollection(serviceDeclarations.ToList())
            : new ServiceDeclarationInfoCollection(existingDeclarations.Union(serviceDeclarations).ToList());
        services.Replace((IServiceDeclarationInfoCollection)serviceDeclarations);

        var serviceMap = BuildServiceMap(
            serviceDeclarations,
            implementationDeclarations);

        foreach (var (_, serviceEntry) in serviceMap)
        {
            // first: split the contract info and the rest of the registrations.
            var registrations = serviceEntry.Registrations;

            if (registrations.Count == 0)
            {
                continue;
            }

            if (registrations.Count == 1)
            {
                // register one service, no matter if multiple or single.
                var appServiceInfo = registrations[0];
                services.AddAppService(appServiceInfo);
                continue;
            }

            // order the services by override and processing priority, resolve overrides
            // and with the rest of the services register them in the computed order.
            var (sortedRegistrations, overriddenTypes) = SortRegistrations(registrations);
            var filteredServiceInfos = overriddenTypes.Count == 0
                ? sortedRegistrations
                : sortedRegistrations.Where(i => !overriddenTypes.Contains(((IServiceInfo)i.serviceInfo).ImplementationType!));
            foreach (var appServiceInfo in filteredServiceInfos)
            {
                services.AddAppService(appServiceInfo.serviceInfo);
            }
        }

        return services;
    }

    private static void AddAppService(
        this IServiceInfoCollection appServices,
        IServiceInfo serviceInfo)
    {
        CheckContractType(serviceInfo);

        appServices.Add(serviceInfo);
    }

    private static void CheckContractType(IServiceInfo serviceInfo)
    {
        var serviceDeclarationType = serviceInfo.ServiceDeclarationType;
        var serviceType = serviceInfo.ServiceType;
        if (serviceDeclarationType is null || serviceType is null)
        {
            return;
        }

        if (serviceType.IsGenericTypeDefinition)
        {
            // TODO check to see if any of the interfaces have as generic definition the exported contract.
        }
        else if (!serviceType.IsAssignableFrom(serviceDeclarationType))
        {
            var serviceValidationMessage = string.Format(
                Strings.AppServiceContractTypeDoesNotMatchServiceContract,
                serviceType,
                serviceDeclarationType);
            throw new ServiceException(serviceValidationMessage);
        }
    }

    private static (IList<(ServiceInfo serviceInfo, Priority overridePriority)> sortedRegistrations, IList<Type> overriddenTypes) SortRegistrations(IEnumerable<ServiceInfo> appServiceInfos)
    {
        // leave the implementation with the runtime type info
        // so that it may be possible to use runtime added attributes
        var overrideChain = appServiceInfos
            .Select(si => (serviceInfo: si, overridePriority: (Priority)(si.Metadata?.TryGetValue(nameof(ServiceMetadata.Priority)) ?? Priority.Normal)))
            .OrderBy(item => item.overridePriority)
            .ToList();

        // get the overridden services which should be eliminated
        var overriddenTypes = overrideChain
            .Where(kv => (bool)(kv.serviceInfo.Metadata?.TryGetValue(nameof(ServiceMetadata.OverrideBaseImplementation)) ?? false) && ((IServiceInfo)kv.serviceInfo).ImplementationType?.BaseType != null)
            .Select(kv => ((IServiceInfo)kv.serviceInfo).ImplementationType?.BaseType)
            .ToList();

        if (overriddenTypes.Count > 0)
        {
            // eliminate the overridden services
            overrideChain = overrideChain
                .Where(kv => !overriddenTypes.Contains(((IServiceInfo)kv.serviceInfo).ImplementationType))
                .ToList();
        }

        return (overrideChain, overriddenTypes!);
    }

    private static IDictionary<Type, ServiceEntry> BuildServiceMap(
        IEnumerable<ServiceDeclarationInfo> serviceDeclarationInfos,
        IEnumerable<ServiceImplementationDeclaration> serviceImplementationDeclarations)
    {
        var serviceMap = new Dictionary<Type, ServiceEntry>();

        foreach (var (serviceDeclarationType, appServiceInfo) in serviceDeclarationInfos)
        {
            if (!serviceMap.TryGetValue(serviceDeclarationType, out var serviceEntry))
            {
                serviceMap.Add(serviceDeclarationType, new ServiceEntry(appServiceInfo));
            }
            else
            {
                serviceEntry.ServiceInfo = appServiceInfo;
            }
        }

        serviceImplementationDeclarations.ForEach(si => AddService(serviceMap, si.ServiceDeclarationType, si.ImplementationType));

        return serviceMap;
    }


    private static void AddService(
        IDictionary<Type, ServiceEntry> serviceMap,
        Type serviceDeclarationType,
        Type implementationType)
    {
        if (!serviceMap.TryGetValue(serviceDeclarationType, out var serviceEntry))
        {
            // if the contract declaration type is not found in the map,
            // it may be because it is a constructed generic type and the
            // registration contains the generic type definition.
            if (serviceDeclarationType.IsConstructedGenericType)
            {
                var contractDeclarationTypeGenericDefinition = serviceDeclarationType.GetGenericTypeDefinition();
                if (serviceMap.TryGetValue(contractDeclarationTypeGenericDefinition, out serviceEntry))
                {
                    // if the contract declaration based on the generic type definition is found,
                    // build a new contract declaration based on the constructed generic type
                    // and add a new entry in the map.
                    var appServiceInfoGenericDefinition = serviceEntry.Registrations.First();
                    var appServiceInfoDeclaration = new ServiceInfo(appServiceInfoGenericDefinition, serviceDeclarationType, serviceDeclarationType);
                    var appServiceInfo = new ServiceInfo(appServiceInfoDeclaration, serviceDeclarationType, serviceDeclarationType, implementationType);
                    ((IServiceInfo)appServiceInfo).AddMetadata(ServiceHelper.GetServiceMetadata(implementationType, serviceDeclarationType));

                    // add to the list of service infos on the first place the declaration.
                    serviceMap[serviceDeclarationType] = new ServiceEntry(appServiceInfoDeclaration) { Registrations = { appServiceInfo } };
                }
            }
        }
        else
        {
            var appServiceInfoDeclaration = serviceEntry.ServiceInfo;
            var appServiceInfo = new ServiceInfo(appServiceInfoDeclaration, appServiceInfoDeclaration.ServiceType ?? serviceDeclarationType, serviceDeclarationType, implementationType);
            ((IServiceInfo)appServiceInfo).AddMetadata(ServiceHelper.GetServiceMetadata(implementationType, serviceDeclarationType));
            serviceEntry.Registrations.Add(appServiceInfo);
        }
    }

    private record ServiceEntry(IServiceInfo ServiceInfo)
    {
        public IList<ServiceInfo> Registrations { get; } = new List<ServiceInfo>();

        public IServiceInfo ServiceInfo { get; set; } = ServiceInfo;
    }
}
