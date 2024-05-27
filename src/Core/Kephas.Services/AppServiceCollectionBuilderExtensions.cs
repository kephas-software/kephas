// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections;
using Kephas.Logging;
using Kephas.Services;
using Kephas.Services.Reflection;
using Kephas.Services.Resources;

using Kephas.Services.Metadata;
using Microsoft.Extensions.Logging;

namespace Kephas;

/// <summary>
/// Extensions for <see cref="IAppServiceCollection"/> for the builder.
/// </summary>
internal static class AppServiceCollectionBuilderExtensions
{
    /// <summary>
    /// Adds the application services from the provided <see cref="IAppServiceInfoProvider"/>s.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <param name="contracts">The contract declarations.</param>
    /// <param name="services">The service declarations.</param>
    /// <param name="resolutionStrategy">The resolution strategy for ambiguous registrations.</param>
    /// <param name="logger">Optional. The logger.</param>
    /// <returns>The provided app services.</returns>
    internal static IAppServiceCollection AddAppServices(
        this IAppServiceCollection appServices,
        IEnumerable<ContractDeclaration> contracts,
        IEnumerable<ServiceDeclaration> services,
        AmbiguousServiceResolutionStrategy resolutionStrategy = AmbiguousServiceResolutionStrategy.ForcePriority,
        ILogger? logger = null)
    {
        var existingContracts = appServices.TryGetServiceInstance<IContractDeclarationCollection>();
        contracts = existingContracts is null
            ? new ContractDeclarationCollection(contracts)
            : new ContractDeclarationCollection(
                ((ICollection<ContractDeclaration>)new List<ContractDeclaration>(existingContracts)).AddRange(contracts));
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

        foreach (var (contractDeclarationType, serviceEntry) in serviceMap)
        {
            // first: split the contract info and the rest of the registrations.
            var registrations = serviceEntry.Registrations;
            var appContractDefinition = serviceEntry.ContractAppServiceInfo;

            if (registrations.Count == 0)
            {
                if (logger.IsDebugEnabled())
                {
                    logger?.LogWarning("No service types registered for {contractDeclarationType}.", contractDeclarationType);
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
                    contractDeclarationType,
                    sortedRegistrations,
                    resolutionStrategy,
                    logger);
                appServices.AddAppService(appServiceInfo, logger);
            }
            else
            {
                var filteredServiceInfos = overriddenTypes.Count == 0
                    ? registrations
                    : registrations.Where(i => !overriddenTypes.Contains(((IAppServiceInfo)i).InstanceType!));
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
        IAppServiceInfo appServiceInfo,
        ILogger? logger)
    {
        CheckContractType(appServiceInfo, logger);

        if (logger.IsDebugEnabled())
        {
            logger?.LogDebug(ToJsonString(appServiceInfo));
        }

        appServices.Add(appServiceInfo);
    }

    private static string ToJsonString(IAppServiceInfo appServiceInfo)
    {
        return $"{{ '{(appServiceInfo.ContractDeclarationType ?? appServiceInfo.ContractType)?.Name}': {appServiceInfo.ToJsonString()} }}";
    }

    private static void CheckContractType(IAppServiceInfo appServiceInfo, ILogger? logger)
    {
        var contractDeclarationType = appServiceInfo.ContractDeclarationType;
        var contractType = appServiceInfo.ContractType;
        if (contractDeclarationType is null || contractType is null)
        {
            return;
        }

        if (contractType.IsGenericTypeDefinition)
        {
            // TODO check to see if any of the interfaces have as generic definition the exported contract.
        }
        else if (!contractType.IsAssignableFrom(contractDeclarationType))
        {
            var contractValidationMessage = string.Format(
                Strings.AppServiceContractTypeDoesNotMatchServiceContract,
                contractType,
                contractDeclarationType);
            logger?.LogError(contractValidationMessage);
            throw new AppServiceException(contractValidationMessage);
        }
    }


    private static AppServiceInfo ResolveAmbiguousRegistration(
        Type contractDeclarationType,
        IList<(AppServiceInfo appServiceInfo, Priority overridePriority)> sortedRegistrations,
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

    private static (IList<(AppServiceInfo appServiceInfo, Priority overridePriority)> sortedRegistrations, IList<Type> overriddenTypes) SortRegistrations(IEnumerable<AppServiceInfo> appServiceInfos, ILogger? logger)
    {
        // leave the implementation with the runtime type info
        // so that it may be possible to use runtime added attributes
        var overrideChain = appServiceInfos
            .Select(si => (appServiceInfo: si, overridePriority: (Priority)(si.Metadata?.TryGetValue(nameof(AppServiceMetadata.Priority)) ?? Priority.Normal)))
            .OrderBy(item => item.overridePriority)
            .ToList();

        // get the overridden services which should be eliminated
        var overriddenTypes = overrideChain
            .Where(kv => (bool)(kv.appServiceInfo.Metadata?.TryGetValue(nameof(AppServiceMetadata.IsOverride)) ?? false) && ((IAppServiceInfo)kv.appServiceInfo).InstanceType?.BaseType != null)
            .Select(kv => ((IAppServiceInfo)kv.appServiceInfo).InstanceType?.BaseType)
            .ToList();

        if (overriddenTypes.Count > 0)
        {
            if (logger.IsDebugEnabled())
            {
                logger?.LogDebug("Excluding the following overridden services: {serviceInfos}", appServiceInfos.Where(i => overriddenTypes.Contains(((IAppServiceInfo)i).InstanceType)).ToList());
            }

            // eliminate the overridden services
            overrideChain = overrideChain
                .Where(kv => !overriddenTypes.Contains(((IAppServiceInfo)kv.appServiceInfo).InstanceType))
                .ToList();
        }

        return (overrideChain, overriddenTypes!);
    }

    private static IDictionary<Type, ServiceEntry> BuildServiceMap(
        IEnumerable<ContractDeclaration> contractDeclarations,
        IEnumerable<ServiceDeclaration> serviceDeclarations,
        ILogger? logger)
    {
        var serviceMap = new Dictionary<Type, ServiceEntry>();

        if (logger.IsDebugEnabled())
        {
            logger?.LogDebug("Entering {operation}...", nameof(BuildServiceMap));
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
                    serviceEntry.ContractAppServiceInfo,
                    appServiceInfo);
                serviceEntry.ContractAppServiceInfo = appServiceInfo;
            }
        }

        serviceDeclarations.ForEach(si => AddServiceType(serviceMap, si.ContractDeclarationType, si.ServiceType, logger));

        if (logger.IsDebugEnabled())
        {
            logger?.LogDebug("Exiting {operation}...", nameof(BuildServiceMap));
        }

        return serviceMap;
    }


    private static void AddServiceType(
        IDictionary<Type, ServiceEntry> serviceMap,
        Type contractDeclarationType,
        Type serviceType,
        ILogger? logger)
    {
        if (logger.IsDebugEnabled())
        {
            logger?.LogDebug("Adding service type {serviceType} for {contractDeclarationType}", serviceType, contractDeclarationType);
        }

        if (!serviceMap.TryGetValue(contractDeclarationType, out var serviceEntry))
        {
            // if the contract declaration type is not found in the map,
            // it may be because it is a constructed generic type and the
            // registration contains the generic type definition.
            if (contractDeclarationType.IsConstructedGenericType)
            {
                var contractDeclarationTypeGenericDefinition = contractDeclarationType.GetGenericTypeDefinition();
                if (serviceMap.TryGetValue(contractDeclarationTypeGenericDefinition, out serviceEntry))
                {
                    // if the contract declaration based on the generic type definition is found,
                    // build a new contract declaration based on the constructed generic type
                    // and add a new entry in the map.
                    var appServiceInfoGenericDefinition = serviceEntry.Registrations.First();
                    var appServiceInfoDeclaration = new AppServiceInfo(appServiceInfoGenericDefinition, contractDeclarationType, contractDeclarationType);
                    var appServiceInfo = new AppServiceInfo(appServiceInfoDeclaration, contractDeclarationType, contractDeclarationType, serviceType);
                    ((IAppServiceInfo)appServiceInfo).AddMetadata(ServiceHelper.GetServiceMetadata(serviceType, contractDeclarationType));

                    // add to the list of service infos on the first place the declaration.
                    serviceMap[contractDeclarationType] = new ServiceEntry(contractDeclarationType, appServiceInfoDeclaration) { Registrations = { appServiceInfo } };
                    return;
                }
            }
        }
        else
        {
            var appServiceInfoDeclaration = serviceEntry.ContractAppServiceInfo;
            var appServiceInfo = new AppServiceInfo(appServiceInfoDeclaration, appServiceInfoDeclaration.ContractType ?? contractDeclarationType, contractDeclarationType, serviceType);
            ((IAppServiceInfo)appServiceInfo).AddMetadata(ServiceHelper.GetServiceMetadata(serviceType, contractDeclarationType));
            serviceEntry.Registrations.Add(appServiceInfo);
            return;
        }

        logger?.LogWarning(
            "Service type {contractType} declares a contract of {contractDeclarationType}, but the contract is not registered as an application service contract.",
            serviceType,
            contractDeclarationType);
    }

    private record ServiceEntry(Type ContractDeclarationType, IAppServiceInfo ContractAppServiceInfo)
    {
        public IList<AppServiceInfo> Registrations { get; } = new List<AppServiceInfo>();

        public IList<Type> OverriddenTypes { get; } = new List<Type>();

        public IAppServiceInfo ContractAppServiceInfo { get; set; } = ContractAppServiceInfo;
    }
}
