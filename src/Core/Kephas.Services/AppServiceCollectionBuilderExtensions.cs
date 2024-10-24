// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Logging;
using Kephas.Services;
using Kephas.Services.Resources;
using Microsoft.Extensions.Logging;

namespace Kephas;

/// <summary>
/// Extensions for <see cref="IAppServiceCollection"/> for the builder.
/// </summary>
internal static class AppServiceCollectionBuilderExtensions
{
    /// <summary>
    /// Adds the application services from the provided <see cref="IServiceInfoProvider"/>s.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <param name="contracts">The contract declarations.</param>
    /// <param name="services">The service declarations.</param>
    /// <param name="resolutionStrategy">The resolution strategy for ambiguous registrations.</param>
    /// <param name="logger">Optional. The logger.</param>
    /// <returns>The provided app services.</returns>
    internal static IAppServiceCollection AddAppServices(
        this IAppServiceCollection appServices,
        IEnumerable<ServiceDeclarationInfo> contracts,
        IEnumerable<ServiceImplementationDeclaration> services,
        AmbiguousServiceResolutionStrategy resolutionStrategy = AmbiguousServiceResolutionStrategy.ForcePriority,
        ILogger? logger = null)
    {
        var existingContracts = appServices.TryGetServiceInstance<IContractDeclarationCollection>();
        contracts = existingContracts is null
            ? new ContractDeclarationCollection(contracts)
            : new ContractDeclarationCollection(
                ((ICollection<ServiceDeclarationInfo>)new List<ServiceDeclarationInfo>(existingContracts)).AddRange(contracts));
        appServices.Replace((IContractDeclarationCollection)contracts);

        if (logger.IsDebugEnabled())
        {
            logger?.LogDebug("Building the service map...");
        }

        var serviceMap = BuildServiceMap(
            contracts,
            services,
            logger);

        if (logger.IsDebugEnabled())
        {
            logger?.LogDebug("Service map built.");
        }

        foreach (var (serviceDeclarationType, serviceEntry) in serviceMap)
        {
            // first: split the contract info and the rest of the registrations.
            var registrations = serviceEntry.Registrations;
            var appContractDefinition = serviceEntry.ServiceInfo;

            if (registrations.Count == 0)
            {
                if (logger.IsDebugEnabled())
                {
                    logger?.LogWarning("No service types registered for {ServiceDeclarationType}.", serviceDeclarationType);
                }

                continue;
            }

            if (registrations.Count == 1)
            {
                // register one service, no matter if multiple or single.
                var appServiceInfo = registrations[0];
                appServices.AddAppService(appServiceInfo, logger);
                continue;
            }

            // order the services by override and processing priority, resolve overrides
            // and with the rest of the services:
            // 1. if multiple are registered, register them in the computed order.
            // 2. for single mode, pick the first one in the order and register it.
            var (sortedRegistrations, overriddenTypes) = SortRegistrations(registrations, logger);
            if (!appContractDefinition.AllowMultiple)
            {
                var appServiceInfo = ResolveAmbiguousRegistration(
                    serviceDeclarationType,
                    sortedRegistrations,
                    resolutionStrategy,
                    logger);
                appServices.AddAppService(appServiceInfo, logger);
            }
            else
            {
                var filteredServiceInfos = overriddenTypes.Count == 0
                    ? registrations
                    : registrations.Where(i => !overriddenTypes.Contains(((IServiceInfo)i).ImplementationType!));
                foreach (var appServiceInfo in filteredServiceInfos)
                {
                    appServices.AddAppService(appServiceInfo, logger);
                }
            }
        }

        return appServices;
    }

    private static void AddAppService(
        this IAppServiceCollection appServices,
        IServiceInfo serviceInfo,
        ILogger? logger)
    {
        CheckContractType(serviceInfo, logger);

        if (logger.IsDebugEnabled())
        {
            logger?.LogDebug(ToJsonString(serviceInfo));
        }

        appServices.Add(serviceInfo);
    }

    private static string ToJsonString(IServiceInfo serviceInfo)
    {
        return $"{{ '{(serviceInfo.ServiceDeclarationType ?? serviceInfo.ServiceType)?.Name}': {serviceInfo:json} }}";
    }

    private static void CheckContractType(IServiceInfo serviceInfo, ILogger? logger)
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
            var contractValidationMessage = string.Format(
                Strings.AppServiceContractTypeDoesNotMatchServiceContract,
                serviceType,
                serviceDeclarationType);
            logger?.LogError(contractValidationMessage);
            throw new ServiceException(contractValidationMessage);
        }
    }


    private static ServiceInfo ResolveAmbiguousRegistration(
        Type contractDeclarationType,
        IList<(ServiceInfo appServiceInfo, Priority overridePriority)> sortedRegistrations,
        AmbiguousServiceResolutionStrategy serviceResolutionStrategy,
        ILogger? logger)
    {
        if (sortedRegistrations.Count == 1)
        {
            return sortedRegistrations[0].appServiceInfo;
        }

        if (logger.IsDebugEnabled())
        {
            logger?.LogDebug(
                Strings.MultipleRegistrationsForAppServiceContract,
                contractDeclarationType,
                serviceResolutionStrategy,
                sortedRegistrations.Select(item => $"{item.appServiceInfo}:{item.overridePriority}"));
        }

        var priority = sortedRegistrations[0].overridePriority;
        return serviceResolutionStrategy switch
        {
            AmbiguousServiceResolutionStrategy.UseFirst =>
                sortedRegistrations[0].appServiceInfo,
            AmbiguousServiceResolutionStrategy.UseLast =>
                sortedRegistrations.Last(s => s.overridePriority == priority).appServiceInfo,
            AmbiguousServiceResolutionStrategy.ForcePriority when priority == sortedRegistrations[1].overridePriority =>
                throw new AmbiguousServiceResolutionException(string.Format(
                    Strings.AmbiguousOverrideForAppServiceContract,
                    contractDeclarationType,
                    string.Join(", ", sortedRegistrations.Select(item => $"{item.appServiceInfo}:{item.overridePriority}")))),
            _ => sortedRegistrations[0].appServiceInfo
        };
    }

    private static (IList<(ServiceInfo appServiceInfo, Priority overridePriority)> sortedRegistrations, IList<Type> overriddenTypes) SortRegistrations(IEnumerable<ServiceInfo> appServiceInfos, ILogger? logger)
    {
        // leave the implementation with the runtime type info
        // so that it may be possible to use runtime added attributes
        var overrideChain = appServiceInfos
            .Select(si => (appServiceInfo: si, overridePriority: (Priority)(si.Metadata?.TryGetValue(nameof(ServiceMetadata.Priority)) ?? Priority.Normal)))
            .OrderBy(item => item.overridePriority)
            .ToList();

        // get the overridden services which should be eliminated
        var overriddenTypes = overrideChain
            .Where(kv => (bool)(kv.appServiceInfo.Metadata?.TryGetValue(nameof(ServiceMetadata.OverrideBaseImplementation)) ?? false) && ((IServiceInfo)kv.appServiceInfo).ImplementationType?.BaseType != null)
            .Select(kv => ((IServiceInfo)kv.appServiceInfo).ImplementationType?.BaseType)
            .ToList();

        if (overriddenTypes.Count > 0)
        {
            if (logger.IsDebugEnabled())
            {
                logger?.LogDebug("Excluding the following overridden services: {ServiceInfos}", appServiceInfos.Where(i => overriddenTypes.Contains(((IServiceInfo)i).ImplementationType)).ToList());
            }

            // eliminate the overridden services
            overrideChain = overrideChain
                .Where(kv => !overriddenTypes.Contains(((IServiceInfo)kv.appServiceInfo).ImplementationType))
                .ToList();
        }

        return (overrideChain, overriddenTypes!);
    }

    private static IDictionary<Type, ServiceEntry> BuildServiceMap(
        IEnumerable<ServiceDeclarationInfo> contractDeclarations,
        IEnumerable<ServiceImplementationDeclaration> serviceDeclarations,
        ILogger? logger)
    {
        var serviceMap = new Dictionary<Type, ServiceEntry>();

        if (logger.IsDebugEnabled())
        {
            logger?.LogDebug("Entering {Operation}...", nameof(BuildServiceMap));
        }

        foreach (var (contractDeclarationType, appServiceInfo) in contractDeclarations)
        {
            if (!serviceMap.TryGetValue(contractDeclarationType, out var serviceEntry))
            {
                serviceMap.Add(contractDeclarationType, new ServiceEntry(contractDeclarationType, appServiceInfo));
            }
            else
            {
                logger?.LogWarning(
                    Strings.AppServiceCollectionMultipleContractDeclarationsWithSameType,
                    contractDeclarationType,
                    serviceEntry.ServiceInfo,
                    appServiceInfo);
                serviceEntry.ServiceInfo = appServiceInfo;
            }
        }

        serviceDeclarations.ForEach(si => AddService(serviceMap, si.ServiceDeclarationType, si.ImplementationType, logger));

        if (logger.IsDebugEnabled())
        {
            logger?.LogDebug("Exiting {Operation}...", nameof(BuildServiceMap));
        }

        return serviceMap;
    }


    private static void AddService(
        IDictionary<Type, ServiceEntry> serviceMap,
        Type serviceDeclarationType,
        Type implementationType,
        ILogger? logger)
    {
        if (logger.IsDebugEnabled())
        {
            logger?.LogDebug("Adding service {ImplementationType} for {ServiceDeclarationType}", implementationType, serviceDeclarationType);
        }

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
                    serviceMap[serviceDeclarationType] = new ServiceEntry(serviceDeclarationType, appServiceInfoDeclaration) { Registrations = { appServiceInfo } };
                    return;
                }
            }
        }
        else
        {
            var appServiceInfoDeclaration = serviceEntry.ServiceInfo;
            var appServiceInfo = new ServiceInfo(appServiceInfoDeclaration, appServiceInfoDeclaration.ServiceType ?? serviceDeclarationType, serviceDeclarationType, implementationType);
            ((IServiceInfo)appServiceInfo).AddMetadata(ServiceHelper.GetServiceMetadata(implementationType, serviceDeclarationType));
            serviceEntry.Registrations.Add(appServiceInfo);
            return;
        }

        logger?.LogWarning(
            "Service {ImplementationType} declares a contract of {ServiceDeclarationType}, but the contract is not registered as an application service contract.",
            implementationType,
            serviceDeclarationType);
    }

    private record ServiceEntry(Type ServiceDeclarationType, IServiceInfo ServiceInfo)
    {
        public IList<ServiceInfo> Registrations { get; } = new List<ServiceInfo>();

        public IList<Type> OverriddenTypes { get; } = new List<Type>();

        public IServiceInfo ServiceInfo { get; set; } = ServiceInfo;
    }
}
