// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceInfoInjectionRegistrar.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base for conventions registrars of application services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Kephas.Collections;
    using Kephas.Injection;
    using Kephas.Injection.AttributedModel;
    using Kephas.Injection.Builder;
    using Kephas.Injection.Configuration;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Base for injectorBuilder registrars of application services.
    /// </summary>
    internal class AppServiceInfoInjectionRegistrar
    {
        /// <summary>
        /// Registers the injectorBuilder.
        /// </summary>
        /// <param name="builder">The registration builder.</param>
        /// <param name="buildContext">Context for the registration.</param>
        /// <param name="appServiceInfoProviders">The list of <see cref="IAppServiceInfosProvider"/>.</param>
        public void RegisterServices(
            IInjectorBuilder builder,
            IInjectionBuildContext buildContext,
            IList<IAppServiceInfosProvider> appServiceInfoProviders)
        {
            buildContext = buildContext ?? throw new ArgumentNullException(nameof(buildContext));
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            var logger = (buildContext.AmbientServices.LogManager ?? LoggingHelper.DefaultLogManager).GetLogger(this.GetType());

            // get all type infos from the injection assemblies
            var appServiceInfoList = appServiceInfoProviders
                .SelectMany(p => p.GetAppServiceContracts(buildContext))
                .Select(t => new ContractDeclaration(
                    t.ContractDeclarationType.ToNormalizedType(),
                    t.AppServiceInfo))
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

                var appServices = appServiceInfosProvider.GetAppServices(buildContext);

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

            if (logger.IsDebugEnabled())
            {
                logger.Debug("Building the service map...");
            }

            var serviceMap = this.BuildServiceMap(
                appServiceInfoList,
                serviceTypes,
                logger);

            if (logger.IsDebugEnabled())
            {
                logger.Debug("Service map built.");
            }

            buildContext.AmbientServices.SetAppServiceInfos(appServiceInfoList);

            foreach (var (contractDeclarationType, serviceEntry) in serviceMap)
            {
                // first: split the contract info and the rest of the registrations.
                var serviceRegistrations = serviceEntry.Registrations;
                var appContractDefinition = serviceRegistrations[0];
                if (appContractDefinition.IsContractDefinition())
                {
                    serviceRegistrations.RemoveAt(0);
                }

                if (serviceRegistrations.Count == 0)
                {
                    logger.Warn("No service types registered for {contractDeclarationType}.", contractDeclarationType);
                    continue;
                }

                var contractType = appContractDefinition.ContractType ?? contractDeclarationType;
                if (serviceRegistrations.Count == 1)
                {
                    // register one service, no matter if multiple or single.
                    this.RegisterService(builder, contractDeclarationType, contractType, serviceRegistrations[0], logger);
                    continue;
                }

                // order the services by override and processing priority, resolve overrides
                // and with the rest of the services:
                // 1. if multiple are registered, register them in the computed order.
                // 2. for single mode, pick the first one in the order and register it.
                var (sortedRegistrations, overriddenTypes) = this.SortRegistrations(serviceRegistrations, logger);
                if (!appContractDefinition.AllowMultiple)
                {
                    var appServiceInfo = ResolveAmbiguousRegistration(
                        contractDeclarationType,
                        sortedRegistrations,
                        buildContext.Settings?.AmbiguousResolutionStrategy ?? AmbiguousServiceResolutionStrategy.ForcePriority,
                        logger);
                    this.RegisterService(builder, contractDeclarationType, contractType, appServiceInfo, logger);
                }
                else
                {
                    var filteredServiceInfos = overriddenTypes.Count == 0
                        ? serviceRegistrations
                        : serviceRegistrations.Where(i => !overriddenTypes.Contains(i.InstanceType!));
                    foreach (var appServiceInfo in filteredServiceInfos)
                    {
                        this.RegisterService(builder, contractDeclarationType, contractType, appServiceInfo, logger);
                    }
                }
            }
        }

        private static IAppServiceInfo ResolveAmbiguousRegistration(
            Type contractDeclarationType,
            IList<(IAppServiceInfo appServiceInfo, Priority overridePriority)> sortedRegistrations,
            AmbiguousServiceResolutionStrategy serviceResolutionStrategy,
            ILogger logger)
        {
            if (sortedRegistrations.Count == 1)
            {
                return sortedRegistrations[0].appServiceInfo;
            }

            if (logger.IsDebugEnabled())
            {
                logger.Debug(
                    AbstractionStrings.MultipleRegistrationsForAppServiceContract,
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
                        AbstractionStrings.AmbiguousOverrideForAppServiceContract,
                        contractDeclarationType,
                        string.Join(", ", sortedRegistrations.Select(item => $"{item.appServiceInfo}:{item.overridePriority}")))),
                _ => sortedRegistrations[0].appServiceInfo
            };
        }

        private void RegisterService(
            IInjectorBuilder injectorBuilder,
            Type contractDeclarationType,
            Type contractType,
            IAppServiceInfo appServiceInfo,
            ILogger logger)
        {
            this.CheckContractType(contractDeclarationType, contractType, logger);

            if (logger.IsDebugEnabled())
            {
                logger.Debug(this.ToJsonString(contractDeclarationType, appServiceInfo));
            }

            var partBuilder = appServiceInfo.InstancingStrategy switch
            {
                Type type => injectorBuilder
                    .ForType(type)
                    .SelectConstructor(ctorInfos => this.TrySelectAppServiceConstructor(contractType, ctorInfos)),
                Func<IInjector, object> factory => injectorBuilder
                    .ForFactory(contractType, factory),
                { } instance => injectorBuilder
                    .ForInstance(instance),
                _ => null,
            };

            if (partBuilder == null)
            {
                return;
            }

            partBuilder
                .As(contractType)
                .AllowMultiple(appServiceInfo.AllowMultiple);
            if (appServiceInfo.IsSingleton())
            {
                partBuilder.Singleton();
            }
            else if (appServiceInfo.IsScoped())
            {
                partBuilder.Scoped();
            }

            if (appServiceInfo.Metadata != null)
            {
                partBuilder.AddMetadata(appServiceInfo.Metadata);
            }

            if (appServiceInfo.IsExternallyOwned)
            {
                partBuilder.ExternallyOwned();
            }
        }

        private (IList<(IAppServiceInfo appServiceInfo, Priority overridePriority)> sortedRegistrations, IList<Type> overriddenTypes) SortRegistrations(IEnumerable<IAppServiceInfo> appServiceInfos, ILogger logger)
        {
            // leave the implementation with the runtime type info
            // so that it may be possible to use runtime added attributes
            var overrideChain = appServiceInfos
                .Select(si => (appServiceInfo: si, overridePriority: (Priority)(si.Metadata?.TryGetValue(nameof(AppServiceMetadata.OverridePriority)) ?? Priority.Normal)))
                .OrderBy(item => item.overridePriority)
                .ToList();

            // get the overridden services which should be eliminated
            var overriddenTypes = overrideChain
                .Where(kv => (bool)(kv.appServiceInfo.Metadata?.TryGetValue(nameof(AppServiceMetadata.IsOverride)) ?? false) && kv.appServiceInfo.InstanceType?.BaseType != null)
                .Select(kv => kv.appServiceInfo.InstanceType?.BaseType)
                .ToList();

            if (overriddenTypes.Count > 0)
            {
                if (logger.IsDebugEnabled())
                {
                    logger.Debug("Excluding the following overridden services: {serviceInfos}", appServiceInfos.Where(i => overriddenTypes.Contains(i.InstanceType)).ToList());
                }

                // eliminate the overridden services
                overrideChain = overrideChain
                    .Where(kv => !overriddenTypes.Contains(kv.appServiceInfo.InstanceType))
                    .ToList();
            }

            return (overrideChain, overriddenTypes!);
        }

        private IDictionary<Type, ServiceEntry> BuildServiceMap(
                IList<ContractDeclaration> appServiceInfoList,
                IEnumerable<ServiceDeclaration> serviceTypes,
                ILogger logger)
        {
            var serviceMap = new Dictionary<Type, ServiceEntry>();

            if (logger.IsDebugEnabled())
            {
                logger.Debug("Entering {operation}...", nameof(this.BuildServiceMap));
            }

            foreach (var (contractDeclarationType, appServiceInfo) in appServiceInfoList)
            {
                if (!serviceMap.TryGetValue(contractDeclarationType, out var serviceEntry))
                {
                    serviceMap.Add(contractDeclarationType, serviceEntry = new ServiceEntry(contractDeclarationType));
                }

                serviceEntry.Registrations.Add(appServiceInfo);
            }

            serviceTypes.ForEach(si => this.AddServiceType(serviceMap, si.ContractDeclarationType, si.ServiceType, logger));

            return serviceMap;
        }

        private void AddServiceType(
            IDictionary<Type, ServiceEntry> serviceMap,
            Type contractDeclarationType,
            Type serviceType,
            ILogger logger)
        {
            if (logger.IsDebugEnabled())
            {
                logger.Debug("Adding service type {serviceType} for {contractDeclarationType}", serviceType, contractDeclarationType);
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
                        var appServiceInfoDeclaration = new AppServiceInfo(appServiceInfoGenericDefinition, contractDeclarationType);
                        IAppServiceInfo appServiceInfo = new AppServiceInfo(appServiceInfoDeclaration, contractDeclarationType, serviceType);
                        appServiceInfo.AddMetadata(ServiceHelper.GetServiceMetadata(serviceType, contractDeclarationType));

                        // add to the list of service infos on the first place the declaration.
                        serviceMap[contractDeclarationType] = new ServiceEntry(contractDeclarationType) { Registrations = { appServiceInfoDeclaration, appServiceInfo } };
                        return;
                    }
                }
            }
            else
            {
                // The first app service info in the list must be the contract declaration.
                var appServiceInfoDeclaration = serviceEntry.Registrations.First();
                IAppServiceInfo appServiceInfo = new AppServiceInfo(appServiceInfoDeclaration, appServiceInfoDeclaration.ContractType ?? contractDeclarationType, serviceType);
                appServiceInfo.AddMetadata(ServiceHelper.GetServiceMetadata(serviceType, contractDeclarationType));
                serviceEntry.Registrations.Add(appServiceInfo);
                return;
            }

            logger.Warn(
                "Service type {contractType} declares a contract of {contractDeclarationType}, but the contract is not registered as an application service contract.",
                serviceType,
                contractDeclarationType);
        }

        private string ToJsonString(Type contractDeclarationType, IAppServiceInfo appServiceInfo)
        {
            return $"{{ '{contractDeclarationType.Name}': {appServiceInfo.ToJsonString()} }}";
        }

        private void CheckContractType(Type contractDeclarationType, Type contractType, ILogger logger)
        {
            if (contractType.IsGenericTypeDefinition)
            {
                // TODO check to see if any of the interfaces have as generic definition the exported contract.
            }
            else if (!contractType.IsAssignableFrom(contractDeclarationType))
            {
                var contractValidationMessage = string.Format(
                    AbstractionStrings.AppServiceContractTypeDoesNotMatchServiceContract,
                    contractType,
                    contractDeclarationType);
                logger.Error(contractValidationMessage);
                throw new InjectionException(contractValidationMessage);
            }
        }

        private ConstructorInfo? TrySelectAppServiceConstructor(
            Type contractDeclarationType,
            IEnumerable<ConstructorInfo> constructors)
        {
            var constructorsList = constructors.Where(c => !c.IsStatic && c.IsPublic).ToList();

            // get the one constructor marked as InjectConstructor.
            var explicitlyMarkedConstructors = constructorsList
                .Where(c => c.GetCustomAttributes().OfType<IInjectConstructorAnnotation>().Any())
                .ToList();
            if (explicitlyMarkedConstructors.Count == 0)
            {
                // none marked explicitly, leave the decision up to the IoC implementation.
                return null;
            }

            if (explicitlyMarkedConstructors.Count > 1)
            {
                throw new InjectionException(
                    string.Format(
                        AbstractionStrings.AppServiceMultipleInjectConstructors,
                        typeof(InjectConstructorAttribute),
                        constructorsList[0].DeclaringType,
                        contractDeclarationType));
            }

            return explicitlyMarkedConstructors[0];
        }

        private class ServiceEntry
        {
            public ServiceEntry(Type contractDeclarationType)
            {
                this.ContractDeclarationType = contractDeclarationType;
            }

            public Type ContractDeclarationType { get; set; }

            public IList<IAppServiceInfo> Registrations { get; } = new List<IAppServiceInfo>();

            public IList<Type> OverriddenTypes { get; } = new List<Type>();
        }
    }
}