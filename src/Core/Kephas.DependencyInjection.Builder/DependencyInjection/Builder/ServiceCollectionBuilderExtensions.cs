// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Kephas.Collections;
using Kephas.DependencyInjection.Builder.Resources;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Reflection;

namespace Kephas.DependencyInjection.Builder;

internal static class ServiceInfoCollectionBuilderExtensions
{
    /// <summary>
    /// Adds the application services from the provided <see cref="IServiceInfoProvider"/>s.
    /// </summary>
    /// <param name="services">The application services.</param>
    /// <param name="serviceDeclarationInfos">The service declarations.</param>
    /// <param name="implementationDeclarations">The service declarations.</param>
    /// <returns>The provided app services.</returns>
    internal static IServiceInfoCollection AddServices(
        this IServiceInfoCollection services,
        IEnumerable<ServiceDeclarationInfo> serviceDeclarationInfos,
        IEnumerable<ServiceImplementationDeclaration> implementationDeclarations)
    {
        var existingDeclarations = services.TryGetServiceInstance<IServiceDeclarationInfoCollection>();
        serviceDeclarationInfos = existingDeclarations is null
            ? new ServiceDeclarationInfoCollection(serviceDeclarationInfos.ToList())
            : new ServiceDeclarationInfoCollection(existingDeclarations.Union(serviceDeclarationInfos).ToList());
        services.Replace((IServiceDeclarationInfoCollection)serviceDeclarationInfos);

        var serviceMap = BuildServiceMap(
            serviceDeclarationInfos,
            implementationDeclarations);

        var validationResults = new List<ValidationResult>();
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
                var validationResult = services.AddAppService(appServiceInfo);
                if (validationResult != ValidationResult.Success)
                {
                    validationResults.Add(validationResult!);
                }
                continue;
            }

            // order the services by override and processing priority, resolve overrides
            // and with the rest of the services register them in the computed order.
            var sortedRegistrations = SortRegistrationsDescending(registrations);
            foreach (var appServiceInfo in sortedRegistrations)
            {
                var validationResult = services.AddAppService(appServiceInfo.serviceInfo);
                if (validationResult != ValidationResult.Success)
                {
                    validationResults.Add(validationResult!);
                }
            }
        }

        return services;
    }

    private static ValidationResult? AddAppService(
        this IServiceInfoCollection appServices,
        IServiceInfo serviceInfo)
    {
        var result = CheckContractType(serviceInfo);

        appServices.Add(serviceInfo);

        return result;
    }

    private static ValidationResult? CheckContractType(IServiceInfo serviceInfo)
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

    private static IList<(ServiceInfo serviceInfo, Priority priority, Type? implementationTypeBase, bool overrideBaseImplementation)>
        SortRegistrationsDescending(IEnumerable<ServiceInfo> appServiceInfos)
    {
        // leave the implementation with the runtime type info
        // so that it may be possible to use runtime added attributes
        // the registrations should be sorted in descending order,
        // so that the ones with higher priority are registered last,
        // to override the ones with a lower priority
        var overrideChain = appServiceInfos
            .Select(si => (serviceInfo: si, priority: si.GetPriority(), implementationTypeBase: si.GetImplementationTypeBase(), overrideBaseImplementation: si.GetOverrideBaseImplementation()))
            .OrderByDescending(item => item.priority)
            .ToList();

        // get the overridden services which should be eliminated
        var overriddenTypes = overrideChain
            .Where(kv => kv is { overrideBaseImplementation: true, implementationTypeBase: not null })
            .Select(kv => kv.implementationTypeBase!.ToNormalizedType())
            .ToList();

        if (overriddenTypes.Count > 0)
        {
            // eliminate the overridden services
            overrideChain = overrideChain
                .Where(kv => !overriddenTypes.Contains(((IServiceInfo)kv.serviceInfo).ImplementationType!))
                .ToList();
        }

        return overrideChain;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Priority GetPriority(this IServiceInfo serviceInfo) =>
        (Priority)(serviceInfo.Metadata?.TryGetValue(nameof(ServiceMetadata.Priority)) ?? Priority.Normal);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Type? GetImplementationTypeBase(this IServiceInfo serviceInfo) =>
        serviceInfo.ImplementationType?.BaseType;
    
    private static bool GetOverrideBaseImplementation(this IServiceInfo serviceInfo) =>
        (bool)(serviceInfo.Metadata?.TryGetValue(nameof(ServiceMetadata.OverrideBaseImplementation)) ?? false);

    private static IDictionary<Type, ServiceEntry> BuildServiceMap(
        IEnumerable<ServiceDeclarationInfo> serviceDeclarationInfos,
        IEnumerable<ServiceImplementationDeclaration> serviceImplementationDeclarations)
    {
        var serviceMap = new Dictionary<Type, ServiceEntry>();
        var serviceImplementationDeclarationsList = serviceImplementationDeclarations.ToList();

        foreach (var (serviceDeclarationType, serviceInfo) in serviceDeclarationInfos)
        {
            GetOrAddServiceEntry(serviceMap, serviceDeclarationType, serviceInfo);
        }

        serviceImplementationDeclarationsList.ForEach(si => AddService(serviceMap, si.ServiceDeclarationType, si.ImplementationType));

        return serviceMap;
    }

    private static ServiceEntry GetOrAddServiceEntry(Dictionary<Type, ServiceEntry> serviceMap, Type serviceDeclarationType, IServiceInfo serviceInfo)
    {
        if (!serviceMap.TryGetValue(serviceDeclarationType, out var serviceEntry))
        {
            serviceMap.Add(serviceDeclarationType, serviceEntry = new ServiceEntry(serviceInfo));
        }
        else
        {
            serviceEntry.ServiceInfo = serviceInfo;
        }

        return serviceEntry;
    }

    private static void AddService(
        IDictionary<Type, ServiceEntry> serviceMap,
        Type serviceDeclarationType,
        Type implementationType)
    {
        if (!serviceMap.TryGetValue(serviceDeclarationType, out var serviceEntry))
        {
            // if the service declaration type is not found in the map,
            // it may be because it is a constructed generic type and the
            // registration contains the generic type definition.
            if (serviceDeclarationType.IsConstructedGenericType)
            {
                var serviceDeclarationTypeGenericDefinition = serviceDeclarationType.GetGenericTypeDefinition();
                if (serviceMap.TryGetValue(serviceDeclarationTypeGenericDefinition, out serviceEntry))
                {
                    // if the service declaration based on the generic type definition is found,
                    // build a new service declaration based on the constructed generic type
                    // and add a new entry in the map.
                    var openGenericServiceInfo = serviceEntry.ServiceInfo;
                    var closedGenericServiceInfo = new ServiceInfo(openGenericServiceInfo, serviceDeclarationType, serviceDeclarationType);
                    var implementationServiceInfo = new ServiceInfo(closedGenericServiceInfo, serviceDeclarationType, serviceDeclarationType, implementationType);
                    ((IServiceInfo)implementationServiceInfo).AddMetadata(ServiceHelper.GetServiceMetadata(implementationType, serviceDeclarationType));

                    // add to the list of service infos on the first place the declaration.
                    serviceMap[serviceDeclarationType] = new ServiceEntry(closedGenericServiceInfo) { Registrations = { implementationServiceInfo } };
                }
            }
        }
        else
        {
            var declarationServiceInfo = serviceEntry.ServiceInfo;
            var implementationServiceInfo = new ServiceInfo(declarationServiceInfo, declarationServiceInfo.ServiceType ?? serviceDeclarationType, serviceDeclarationType, implementationType);
            ((IServiceInfo)implementationServiceInfo).AddMetadata(ServiceHelper.GetServiceMetadata(implementationType, serviceDeclarationType));
            serviceEntry.Registrations.Add(implementationServiceInfo);
        }
    }

    private record ServiceEntry(IServiceInfo ServiceInfo)
    {
        public IList<ServiceInfo> Registrations { get; } = new List<ServiceInfo>();

        public IServiceInfo ServiceInfo { get; set; } = ServiceInfo;
    }
}
