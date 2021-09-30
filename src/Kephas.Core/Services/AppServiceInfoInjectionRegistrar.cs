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
    using Kephas.Injection.Hosting;
    using Kephas.Logging;
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

            var logger = this.GetLogger(buildContext);

            // get all type infos from the injection assemblies
            var appServiceInfoList = appServiceInfoProviders
                .SelectMany(p => p.GetAppServiceInfos(buildContext))
                .ToList();

            var appServiceInfoMap = this.BuildAppServiceInfoMap(
                appServiceInfoList,
                appServiceInfoProviders.SelectMany(p => p.GetAppServiceTypes(buildContext)),
                logger);

            buildContext.AmbientServices.SetAppServiceInfos(appServiceInfoList);

            foreach (var (contractDeclarationType, appServiceInfos) in appServiceInfoMap)
            {
                // first: split the contract info and the rest of the registrations.
                var appContractDefinition = appServiceInfos[0];
                if (appContractDefinition.IsContractDefinition())
                {
                    appServiceInfos.RemoveAt(0);
                }

                if (appServiceInfos.Count == 0)
                {
                    logger.Warn("No service types registered for {contractDeclarationType}.", contractDeclarationType);
                    continue;
                }

                var contractType = appContractDefinition.ContractType ?? contractDeclarationType;
                if (appServiceInfos.Count == 1)
                {
                    // register one service, no matter if multiple or single.
                    this.RegisterService(builder, contractDeclarationType, contractType, appServiceInfos[0], logger);
                    continue;
                }

                // order the services by override and processing priority, resolve overrides
                // and with the rest of the services:
                // 1. if multiple are registered, register them in the computed order.
                // 2. for single mode, pick the first one in the order and register it.
                var sortedServices = this.SortServiceInfos(appServiceInfos);
                if (!appContractDefinition.AllowMultiple)
                {
                    if (sortedServices.Count > 1 && sortedServices[0].overridePriority == sortedServices[1].overridePriority)
                    {
                        throw new AmbiguousServiceResolutionException(
                            string.Format(
                                Strings.AmbiguousOverrideForAppServiceContract,
                                contractDeclarationType,
                                string.Join(
                                    ", ",
                                    sortedServices
                                        .Select(item => $"{item.appServiceInfo}:{item.overridePriority}"))));
                    }

                    this.RegisterService(builder, contractDeclarationType, contractType, sortedServices[0].appServiceInfo, logger);
                }
                else
                {
                    foreach (var (appServiceInfo, _) in sortedServices)
                    {
                        this.RegisterService(builder, contractDeclarationType, contractType, appServiceInfo, logger);
                    }
                }
            }
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
                null => null,
                var instance => injectorBuilder
                    .ForInstance(instance),
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
        }

        private IList<(IAppServiceInfo appServiceInfo, Priority overridePriority)> SortServiceInfos(IEnumerable<IAppServiceInfo> appServiceInfos)
        {
            // leave the implementation with the runtime type info
            // so that it may be possible to use runtime added attributes
            var overrideChain = appServiceInfos
                .Select(si => (appServiceInfo: si, overridePriority: (Priority)(si.Metadata?.TryGetValue(nameof(AppServiceMetadata.OverridePriority)) ?? Priority.Normal)))
                .OrderBy(item => item.overridePriority)
                .ToList();

            // get the overridden services which should be eliminated
            var overriddenTypes = overrideChain
                .Where(kv => (kv.appServiceInfo.Metadata?.TryGetValue(nameof(AppServiceMetadata.IsOverride)) ?? false) != null && kv.appServiceInfo.InstanceType?.BaseType != null)
                .Select(kv => kv.appServiceInfo.InstanceType?.BaseType)
                .ToList();

            if (overriddenTypes.Count > 0)
            {
                // eliminate the overridden services
                overrideChain = overrideChain
                    .Where(kv => !overriddenTypes.Contains(kv.appServiceInfo.InstanceType))
                    .ToList();
            }

            return overrideChain;
        }

        private IDictionary<Type, IList<IAppServiceInfo>> BuildAppServiceInfoMap(
                IList<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> appServiceInfoList,
                IEnumerable<(Type serviceType, Type contractDeclarationType)> serviceTypes,
                ILogger logger)
        {
            var dictionary = new Dictionary<Type, IList<IAppServiceInfo>>();

            foreach (var (contractDeclarationType, appServiceInfo) in appServiceInfoList)
            {
                if (!dictionary.TryGetValue(contractDeclarationType, out var appServiceInfos))
                {
                    appServiceInfos = new List<IAppServiceInfo>();
                    dictionary.Add(contractDeclarationType, appServiceInfos);
                }

                appServiceInfos.Add(appServiceInfo);
            }

            serviceTypes.ForEach(si => this.AddServiceType(dictionary, si.contractDeclarationType, si.serviceType, logger));

            return dictionary;
        }

        private void AddServiceType(
            IDictionary<Type, IList<IAppServiceInfo>> appServiceInfoMap,
            Type contractDeclarationType,
            Type serviceType,
            ILogger logger)
        {
            if (!appServiceInfoMap.TryGetValue(contractDeclarationType, out var appServiceInfos))
            {
                // if the contract declaration type is not found in the map,
                // it may be because it is a constructed generic type and the
                // registration contains the generic type definition.
                if (contractDeclarationType.IsConstructedGenericType)
                {
                    var contractGenericDefinitionType = contractDeclarationType.GetGenericTypeDefinition();
                    if (appServiceInfoMap.TryGetValue(contractDeclarationType, out appServiceInfos))
                    {
                        // if the contract declaration based on the generic type definition is found,
                        // build a new contract declaration based on the constructed generic type
                        // and add a new entry in the map.
                        var appServiceInfoOpenGeneric = appServiceInfos.First();
                        var appServiceInfoDeclaration = new AppServiceInfo(appServiceInfoOpenGeneric, contractDeclarationType);
                        var appServiceInfo = new AppServiceInfo(appServiceInfoDeclaration, contractDeclarationType, serviceType)
                            .AddMetadata(this.GetServiceMetadata(serviceType, contractDeclarationType));

                        // add to the list of service infos on the first place the declaration.
                        appServiceInfoMap[contractDeclarationType] = new List<IAppServiceInfo> { appServiceInfoDeclaration, appServiceInfo };
                        return;
                    }
                }
            }
            else
            {
                // The first app service info in the list must be the contract declaration.
                var appServiceInfoDeclaration = appServiceInfos.First();
                var appServiceInfo = new AppServiceInfo(appServiceInfoDeclaration, appServiceInfoDeclaration.ContractType ?? contractDeclarationType, serviceType)
                    .AddMetadata(this.GetServiceMetadata(serviceType, contractDeclarationType));
                appServiceInfos.Add(appServiceInfo);
                return;
            }

            logger.Warn(
                "Service type {serviceType} declares a contract of {contractDeclarationType}, but the contract is not registered as an application service contract.",
                serviceType,
                contractDeclarationType);
        }

        private IDictionary<string, object?> GetServiceMetadata(Type serviceType, Type contractDeclarationType)
        {
            var metadata = new Dictionary<string, object?>();
            serviceType.GetCustomAttributes()
                .OfType<IMetadataProvider>()
                .SelectMany(p => p.GetMetadata())
                .ForEach(m => metadata[m.name] = m.value);

            metadata.Add(nameof(AppServiceMetadata.ServiceType), serviceType);

            // add metadata from generic parameters
            if (contractDeclarationType.IsGenericType)
            {
                var metadataSourceGenericType = contractDeclarationType.IsConstructedGenericType
                    ? contractDeclarationType
                    : serviceType.IsGenericType
                        ? null
                        : serviceType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == contractDeclarationType);
                if (metadataSourceGenericType != null)
                {
                    IMetadataProvider.GetGenericTypeMetadataProvider(metadataSourceGenericType)
                        .GetMetadata()
                        .ForEach(m => metadata[m.name] = m.value);
                }
            }

            return metadata;
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
                    Strings.AppServiceContractTypeDoesNotMatchServiceContract,
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
                        Strings.AppServiceMultipleInjectConstructors,
                        typeof(InjectConstructorAttribute),
                        constructorsList[0].DeclaringType,
                        contractDeclarationType));
            }

            return explicitlyMarkedConstructors[0];
        }
    }
}