// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceInfoConventionsRegistrar.cs" company="Kephas Software SRL">
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
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Injection.AttributedModel;
    using Kephas.Injection.Conventions;
    using Kephas.Injection.Hosting;
    using Kephas.Logging;
    using Kephas.Model.AttributedModel;
    using Kephas.Resources;
    using Kephas.Runtime;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Base for conventions registrars of application services.
    /// </summary>
    internal class AppServiceInfoConventionsRegistrar : IConventionsRegistrar
    {
        private readonly IAppServiceMetadataResolver metadataResolver;

        /// <summary>
        /// The default metadata attribute types.
        /// </summary>
        internal static readonly IReadOnlyCollection<Type> DefaultMetadataAttributeTypes;

        /// <summary>
        /// The default metadata attribute types.
        /// </summary>
        internal static readonly IList<Type> WritableDefaultMetadataAttributeTypes
            = new List<Type>
            {
                typeof(ProcessingPriorityAttribute),
                typeof(OverridePriorityAttribute),
                typeof(ServiceNameAttribute),
                typeof(OverrideAttribute),
            };

        /// <summary>
        /// Initializes static members of the <see cref="AppServiceInfoConventionsRegistrar"/> class.
        /// </summary>
        static AppServiceInfoConventionsRegistrar()
        {
            DefaultMetadataAttributeTypes = new ReadOnlyCollection<Type>(WritableDefaultMetadataAttributeTypes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceInfoConventionsRegistrar"/> class.
        /// </summary>
        /// <param name="metadataResolver">The metadata resolver.</param>
        public AppServiceInfoConventionsRegistrar(IAppServiceMetadataResolver metadataResolver)
        {
            this.metadataResolver = metadataResolver;
        }

        /// <summary>
        /// Registers the provided metadata attribute types as default attributes.
        /// </summary>
        /// <param name="attributeTypes">A variable-length parameters list containing attribute types.</param>
        public static void RegisterDefaultMetadataAttributeTypes(params Type[] attributeTypes)
        {
            foreach (var attributeType in attributeTypes)
            {
                if (!WritableDefaultMetadataAttributeTypes.Contains(attributeType))
                {
                    WritableDefaultMetadataAttributeTypes.Add(attributeType);
                }
            }
        }

        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <param name="builder">The registration builder.</param>
        /// <param name="buildContext">Context for the registration.</param>
        public void RegisterConventions(IConventionsBuilder builder, IInjectionBuildContext buildContext)
        {
            Requires.NotNull(builder, nameof(builder));
            Requires.NotNull(buildContext, nameof(buildContext));

            var logger = this.GetLogger(buildContext);

            var conventions = builder;

            // get all type infos from the injection assemblies
            var appServiceInfoProviders = this.GetAppServiceInfosProviders(buildContext).ToList();
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
                var appContractInfo = appServiceInfos[0];
                if (appContractInfo.IsContractDefinition())
                {
                    appServiceInfos.RemoveAt(0);
                }

                if (appContractInfo.AllowMultiple)
                {
                    
                }

                foreach (var appServiceInfo in appServiceInfos)
                {
                    if (appServiceInfo.IsContractDefinition())
                    {
                        continue;
                    }

                    // TODO
                    
                    hasServiceTypes = true;
                }
                
                if (!hasServiceTypes)
                {
                    logger.Warn("No service types registered for {contractDeclarationType}}.", contractDeclarationType);
                }

                
                var contractType = appServiceInfo.ContractType ?? contractDeclarationType;
                if (appServiceInfo.IsContractDefinition())
                {
                    if (serviceTypes.Count == 0)
                    {
                        logger.Warn("No service types registered for {contractDeclarationType}/{contractType}.", contractDeclarationType, contractType);
                        continue;
                    }

                    // select from service types the ordered ones.
                }
                else
                {
                    if (serviceTypes.Count > 0 && !appServiceInfo.AllowMultiple)
                    {
                        logger.Warn("Explicit registration {appServiceInfo} overrides collected service types: {serviceTypes}.", appServiceInfo, serviceTypes);
                        continue;
                    }
                }
            }
            
            var contractDeclarationTypes = appServiceInfoList.Select(e => e.contractDeclarationType).ToList();

            foreach (var (appServiceContract, appServiceInfo) in appServiceInfoMap)
            {
                var isPartBuilder = this.TryConfigurePartBuilder(
                    appServiceInfo,
                    appServiceContract,
                    conventions);

                if (!isPartBuilder)
                {
                    var partConventionsBuilder = this.TryGetPartConventionsBuilder(
                        appServiceInfo,
                        appServiceContract,
                        conventions,
                        typeInfos,
                        logger);
                    if (partConventionsBuilder != null)
                    {
                        this.ConfigurePartBuilder(partConventionsBuilder, appServiceContract, appServiceInfo,
                            contractDeclarationTypes, logger);
                    }
                    else
                    {
                        logger.Warn("No part conventions builders nor part builders found for {serviceContractType}.",
                            appServiceContract);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <param name="buildContext">Context for the registration.</param>
        /// <returns>
        /// An enumeration of <see cref="IAppServiceInfosProvider"/> objects.
        /// </returns>
        protected virtual IEnumerable<IAppServiceInfosProvider> GetAppServiceInfosProviders(
            IInjectionBuildContext buildContext)
        {
            return buildContext.AppServiceInfosProviders ?? Array.Empty<IAppServiceInfosProvider>();
        }

        /// <summary>
        /// Configures the part builder.
        /// </summary>
        /// <param name="partBuilder">The part builder.</param>
        /// <param name="serviceContract">The service contract.</param>
        /// <param name="appServiceInfo">The application service metadata.</param>
        /// <param name="appServiceContracts">The application service contracts.</param>
        /// <param name="logger">The logger.</param>
        protected void ConfigurePartBuilder(
            IPartConventionsBuilder partBuilder,
            Type serviceContract,
            IAppServiceInfo appServiceInfo,
            IList<Type> appServiceContracts,
            ILogger logger)
        {
            var serviceContractType = serviceContract;
            var exportedContractType = appServiceInfo.ContractType ?? serviceContractType;
            var exportedContract = exportedContractType.GetTypeInfo();
            if (!this.CheckExportedContractType(exportedContractType, serviceContract, serviceContractType, logger))
            {
                return;
            }

            if (logger.IsDebugEnabled())
            {
                logger.Debug(this.SerializeServiceContractMetadata(serviceContract, appServiceInfo));
            }

            var metadataAttributes = this.GetMetadataAttributes(appServiceInfo);
            if (exportedContract.IsGenericTypeDefinition)
            {
                if (appServiceInfo.AsOpenGeneric)
                {
                    partBuilder.ExportInterface(
                        exportedContract,
                        (t, b) => this.ConfigureExport(serviceContract, b, exportedContractType, t, metadataAttributes));

                    if (metadataAttributes.Count > 0)
                    {
                        var hasCustomMetadataAttributes = metadataAttributes.Any(
                            a => !typeof(IAppServiceInfo).IsAssignableFrom(a));

                        // warn about metadata on open generic exports only if custom attributes are provided.
                        if (hasCustomMetadataAttributes)
                        {
                            logger.Warn(Strings.AppServiceConventionsRegistrarBase_AsOpenGenericDoesNotSupportMetadataAttributes_Warning, exportedContract);
                        }
                    }
                }
                else
                {
                    partBuilder.ExportInterface(
                        exportedContract,
                        (t, b) => this.ConfigureExport(serviceContract, b, t, t, metadataAttributes));
                }
            }
            else
            {
                partBuilder.Export(
                    b => this.ConfigureExport(serviceContract, b, exportedContractType, null, metadataAttributes));
            }

            partBuilder.SelectConstructor(ctorInfos => this.TrySelectAppServiceConstructor(serviceContract, ctorInfos));

            if (appServiceInfo.IsSingleton())
            {
                partBuilder.Singleton();
            }
            else if (appServiceInfo.IsScoped())
            {
                partBuilder.Scoped();
            }
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
                    if (appServiceInfoMap.TryGetValue(contractGenericDefinitionType, out appServiceInfos))
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

            metadata.Add(nameof(AppServiceMetadata.ServiceInstanceType), serviceType);

            // add metadata from generic parameters
            if (contractDeclarationType.IsConstructedGenericType)
            {
                IMetadataProvider.GetGenericTypeMetadataProvider(contractDeclarationType)
                    .GetMetadata()
                    .ForEach(m => metadata[m.name] = m.value);
            }

            return metadata;
        }

        private string SerializeServiceContractMetadata(Type serviceContract, IAppServiceInfo contractMetadata)
        {
            var sb = new StringBuilder();
            sb.Append("{'")
                .Append(serviceContract.Name)
                .Append("': { multi: ")
                .Append(contractMetadata.AllowMultiple)
                .Append(", lifetime: '").Append(contractMetadata.Lifetime).Append("'");

            if (serviceContract.IsGenericTypeDefinition)
            {
                sb.Append(", asOpenGeneric: ").Append(contractMetadata.AsOpenGeneric);
            }

            if (contractMetadata.InstanceType != null)
            {
                sb.Append(", instanceType: '")
                    .Append(contractMetadata.InstanceType)
                    .Append("'");
            }

            if (contractMetadata.Instance != null)
            {
                sb.Append(", instance: '")
                    .Append(contractMetadata.Instance)
                    .Append("'");
            }

            if (contractMetadata.InstanceFactory != null)
            {
                sb.Append(", instanceFactory: '(function)'");
            }

            sb.Append("} }");

            return sb.ToString();
        }

        private IReadOnlyCollection<Type> GetMetadataAttributes(IAppServiceInfo appServiceInfo)
        {
            if (appServiceInfo.MetadataAttributes == null || appServiceInfo.MetadataAttributes.Length == 0)
            {
                return DefaultMetadataAttributeTypes;
            }

            var attrs = appServiceInfo.MetadataAttributes.ToList();
            foreach (var attr in DefaultMetadataAttributeTypes)
            {
                if (!attrs.Contains(attr))
                {
                    attrs.Add(attr);
                }
            }

            return attrs;
        }

        private void ConfigureExport(
            Type serviceContract,
            IExportConventionsBuilder exportBuilder,
            Type exportedContractType,
            Type? serviceImplementationType,
            IEnumerable<Type> metadataAttributes)
        {
            exportBuilder.AsContractType(exportedContractType);
            this.AddInjectionMetadata(exportBuilder, serviceImplementationType, metadataAttributes);
            this.AddInjectionMetadataForGenerics(exportBuilder, serviceContract);
        }

        /// <summary>
        /// Checks the type of the exported contract.
        /// </summary>
        /// <exception cref="InjectionException">Thrown when an injection error condition occurs.</exception>
        /// <param name="exportedContractType">Type of the exported contract.</param>
        /// <param name="serviceContract">The service contract.</param>
        /// <param name="serviceContractType">Type of the service contract.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// <c>true</c> if the service contract is valid, false otherwise.
        /// </returns>
        private bool CheckExportedContractType(
            Type exportedContractType,
            Type serviceContract,
            Type serviceContractType,
            ILogger logger)
        {
            var exportedContract = exportedContractType.GetTypeInfo();
            if (exportedContract.IsGenericTypeDefinition)
            {
                // TODO check to see if any of the interfaces have as generic definition the exported contract.
            }
            else if (!exportedContract.IsAssignableFrom(serviceContract))
            {
                var contractValidationMessage = string.Format(
                    Strings.AppServiceContractTypeDoesNotMatchServiceContract,
                    exportedContractType,
                    serviceContractType);
                logger.Error(contractValidationMessage);
                throw new InjectionException(contractValidationMessage);
            }

            return true;
        }

        /// <summary>
        /// Selects the application service constructor.
        /// </summary>
        /// <param name="serviceContract">The service contract.</param>
        /// <param name="constructors">The constructors.</param>
        /// <returns>
        /// The application service constructor.
        /// </returns>
        private ConstructorInfo? TrySelectAppServiceConstructor(
            Type serviceContract,
            IEnumerable<ConstructorInfo> constructors)
        {
            var constructorsList = constructors.Where(c => !c.IsStatic && c.IsPublic).ToList();

            // get the one constructor marked as InjectConstructor.
            var explicitlyMarkedConstructors = constructorsList
                .Where(c => c.GetCustomAttribute<InjectConstructorAttribute>() != null).ToList();
            if (explicitlyMarkedConstructors.Count == 0)
            {
                // none marked explicitly, leave the decision up to the IoC implementation.
                return null;
            }

            if (explicitlyMarkedConstructors.Count > 1)
            {
                throw new InjectionException(string.Format(Strings.AppServiceMultipleInjectConstructors,
                    typeof(InjectConstructorAttribute), constructorsList[0].DeclaringType, serviceContract));
            }

            return explicitlyMarkedConstructors[0];
        }

        private void AddInjectionMetadataForGenerics(IExportConventionsBuilder builder, Type serviceContract)
        {
            if (!serviceContract.IsGenericTypeDefinition)
            {
                return;
            }

            var serviceContractType = serviceContract;
            var genericTypeParameters = serviceContract.GetTypeInfo().GenericTypeParameters;
            for (var i = 0; i < genericTypeParameters.Length; i++)
            {
                var genericTypeParameter = genericTypeParameters[i];
                var position = i;
                builder.AddMetadata(
                    this.metadataResolver.GetMetadataNameFromGenericTypeParameter(genericTypeParameter),
                    t => this.metadataResolver.GetMetadataValueFromGenericParameter(t, position, serviceContractType));
            }
        }

        private void AddInjectionMetadata(IExportConventionsBuilder builder, Type? serviceImplementationType, IEnumerable<Type> attributeTypes)
        {
            // add the service type.
            builder.AddMetadata(nameof(AppServiceMetadata.ServiceInstanceType), t => serviceImplementationType ?? t);

            // add the rest of the metadata indicated by the attributes.
            if (attributeTypes == null)
            {
                return;
            }

            foreach (var attributeType in attributeTypes)
            {
                var valueProperties = this.metadataResolver.GetMetadataValueProperties(attributeType);
                foreach (var valuePropertyEntry in valueProperties)
                {
                    builder.AddMetadata(
                        valuePropertyEntry.Key,
                        t => this.metadataResolver.GetMetadataValueFromAttribute(t, attributeType, valuePropertyEntry.Value));
                }
            }
        }

        private IPartConventionsBuilder? TryGetPartConventionsBuilder(
            IAppServiceInfo appServiceInfo,
            Type serviceContract,
            IConventionsBuilder conventions,
            IEnumerable<Type> typeInfos,
            ILogger logger)
        {
            var serviceContractType = serviceContract;

            if (appServiceInfo.InstanceType != null)
            {
                if (logger.IsDebugEnabled())
                {
                    logger.Debug("Service {serviceContractType} matches {serviceInstanceType}.", serviceContractType,
                        appServiceInfo.InstanceType);
                }

                return conventions
                    .ForType(appServiceInfo.InstanceType)
                    .AsServiceType(serviceContract)
                    .AllowMultiple(appServiceInfo.AllowMultiple);
            }

            if (serviceContract.IsGenericTypeDefinition)
            {
                if (logger.IsDebugEnabled())
                {
                    logger.Debug("Service {serviceContractType} matches open generic contract types.",
                        serviceContractType);
                }

                // for open generics select a single implementation type
                if (appServiceInfo.AsOpenGeneric)
                {
                    var (isOverride, selectedInstanceType) = this.TrySelectSingleServiceImplementationType(
                        serviceContract,
                        typeInfos,
                        t => this.MatchOpenGenericContractType(t, serviceContractType));
                    if (logger.IsDebugEnabled())
                    {
                        logger.Debug(
                            "Service {serviceContractType} matches open generic implementation type {serviceInstanceType}.",
                            serviceContractType, selectedInstanceType?.ToString() ?? "<not found>");
                    }

                    // TODO HACK: remove the *isOverride* check when the BUG https://github.com/dotnet/corefx/issues/40094 is fixed
                    // the meaning is that for non-overrides (no multiple implementations) it is safe to let the matching
                    // to be done through the lambda criteria
                    if (isOverride && selectedInstanceType != null)
                    {
                        return conventions
                            .ForType(selectedInstanceType)
                            .AsServiceType(serviceContract)
                            .AllowMultiple(appServiceInfo.AllowMultiple);
                    }
                }

                if (logger.IsDebugEnabled())
                {
                    logger.Debug("Service {serviceContractType} matches open generic contract types.",
                        serviceContractType);
                }

                // if there is non-generic service contract with the same full name
                // then add just the conventions for the derived types.
                return conventions
                    .ForTypesMatching(t => this.MatchOpenGenericContractType(t, serviceContractType))
                    .AsServiceType(serviceContract)
                    .AllowMultiple(appServiceInfo.AllowMultiple);
            }

            if (appServiceInfo.AllowMultiple)
            {
                if (logger.IsDebugEnabled())
                {
                    logger.Debug("Service {serviceContractType} matches multiple derived from it.",
                        serviceContractType);
                }

                // if the service contract metadata allows multiple service registrations
                // then add just the conventions for the derived types.
                return conventions
                    .ForTypesMatching(t => this.MatchDerivedFromContractType(t, serviceContract))
                    .AsServiceType(serviceContract)
                    .AllowMultiple(appServiceInfo.AllowMultiple);
            }

            var (_, selectedPart) = this.TrySelectSingleServiceImplementationType(
                serviceContract,
                typeInfos,
                part => this.MatchDerivedFromContractType(part, serviceContract));

            if (selectedPart != null)
            {
                if (logger.IsDebugEnabled())
                {
                    logger.Debug("Service {serviceContractType} matches {serviceInstanceType}.", serviceContractType,
                        selectedPart);
                }

                return conventions
                    .ForType(selectedPart)
                    .AsServiceType(serviceContract)
                    .AllowMultiple(appServiceInfo.AllowMultiple);
            }

            return null;
        }

        /// <summary>
        /// Select a single service implementation type based on the provided implementation criteria
        /// and the override priority of the possible implementations.
        /// </summary>
        /// <param name="serviceContract">The service contract.</param>
        /// <param name="typeInfos">The type infos.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>
        /// An implementation type and a flag indicating if the selected implementation type is an override.
        /// </returns>
        private (bool isOverride, Type? implementationType) TrySelectSingleServiceImplementationType(
            Type serviceContract,
            IEnumerable<Type> typeInfos,
            Func<Type, bool> criteria)
        {
            var parts = typeInfos.Where(criteria).ToList();
            if (parts.Count == 1)
            {
                var selectedPart = parts[0];
                return (false, selectedPart);
            }

            if (parts.Count > 1)
            {
                // leave the implementation with the runtime type info
                // so that it may be possible to use runtime added attributes
                var overrideChain = parts
                    .ToDictionary(
                        ti => ti,
                        ti => ti.GetCustomAttribute<OverridePriorityAttribute>()
                              ?? new OverridePriorityAttribute(Priority.Normal))
                    .OrderBy(item => item.Value.Value)
                    .ToList();

                // get the overridden services which should be eliminated
                var overriddenTypes = overrideChain
                    .Where(kv => kv.Key.GetCustomAttribute<OverrideAttribute>() != null && kv.Key.BaseType != null)
                    .Select(kv => kv.Key.BaseType)
                    .ToList();

                if (overriddenTypes.Count > 0)
                {
                    // eliminate the overridden services
                    overrideChain = overrideChain
                        .Where(kv => !overriddenTypes.Contains(kv.Key))
                        .ToList();
                }

                var selectedPart = overrideChain[0].Key;
                if (overrideChain.Count > 1 && overrideChain[0].Value.Value == overrideChain[1].Value.Value)
                {
                    throw new AmbiguousServiceResolutionException(
                        string.Format(
                            Strings.AmbiguousOverrideForAppServiceContract,
                            serviceContract,
                            string.Join(
                                ", ",
                                overrideChain.Select(item => $"{item.Key}:{item.Value.Value}"))));
                }

                return (true, selectedPart);
            }

            return (false, null);
        }

        /// <summary>
        /// Tries to configure the conventions part builder.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when there is an ambiguous override in the service implementations.</exception>
        /// <param name="appServiceInfo">The service contract metadata.</param>
        /// <param name="contractDeclarationType">The contract declaration type.</param>
        /// <param name="conventions">The conventions.</param>
        /// <returns>
        /// True if a part builder could be configured, false otherwise.
        /// </returns>
        private IPartBuilder? TryConfigurePartBuilder(
            IAppServiceInfo appServiceInfo,
            Type contractDeclarationType,
            IConventionsBuilder conventions)
        {
            if (appServiceInfo.Instance != null)
            {
                return conventions
                    .ForInstance(contractDeclarationType, appServiceInfo.Instance)
                    .AllowMultiple(appServiceInfo.AllowMultiple);
            }

            if (appServiceInfo.InstanceFactory != null)
            {
                var partBuilder = conventions
                    .ForInstanceFactory(contractDeclarationType, appServiceInfo.InstanceFactory)
                    .AllowMultiple(appServiceInfo.AllowMultiple);
                if (appServiceInfo.IsSingleton())
                {
                    partBuilder.Singleton();
                }
                else if (appServiceInfo.IsScoped())
                {
                    partBuilder.Scoped();
                }

                return partBuilder;
            }

            return null;
        }

        private bool IsEligiblePart(Type typeInfo)
        {
            // leave here the AsRuntimeTypeInfo conversion so that
            // runtime added attributes may be used
            return typeInfo.IsClass
                   && !typeInfo.IsAbstract
                   && !typeInfo.IsNestedPrivate
                   && typeInfo.GetCustomAttribute<ExcludeFromInjectionAttribute>() == null;
        }

        /// <summary>
        /// Checks whether the part type matches the type of the open generic contract.
        /// </summary>
        /// <returns><c>true</c> if the part type matches the type of the generic contract, otherwise <c>false</c>.</returns>
        private bool MatchDerivedFromContractType(Type partTypeInfo, Type serviceContract)
        {
            if (!this.IsEligiblePart(partTypeInfo) || partTypeInfo.IsGenericTypeDefinition)
            {
                return false;
            }

            return serviceContract.IsAssignableFrom(partTypeInfo);
        }

        /// <summary>
        /// Checks whether the part type matches the type of the open generic contract.
        /// </summary>
        /// <param name="partType">Type of the part.</param>
        /// <param name="serviceContractType">Type of the service contract.</param>
        /// <returns><c>true</c> if the part type matches the type of the generic contract, otherwise <c>false</c>.</returns>
        private bool MatchOpenGenericContractType(Type partType, Type serviceContractType)
        {
            var partTypeInfo = partType.GetTypeInfo();
            if (!this.IsEligiblePart(partTypeInfo))
            {
                return false;
            }

            var implementedInterfaces = partTypeInfo.ImplementedInterfaces;
            return implementedInterfaces.Any(i =>
                i.IsConstructedGenericType && i.GetGenericTypeDefinition() == serviceContractType);
        }
    }
}