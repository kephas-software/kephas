// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceInfoConventionsRegistrar.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base for conventions registrars of application services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Injection.AttributedModel;
    using Kephas.Injection.Conventions;
    using Kephas.Injection.Hosting;
    using Kephas.Logging;
    using Kephas.Model.AttributedModel;
    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Runtime;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Base for conventions registrars of application services.
    /// </summary>
    public class AppServiceInfoConventionsRegistrar : IConventionsRegistrar
    {
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
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        public void RegisterConventions(
            IConventionsBuilder builder,
            IList<Type> candidateTypes,
            IInjectionRegistrationContext registrationContext)
        {
            Requires.NotNull(builder, nameof(builder));
            Requires.NotNull(candidateTypes, nameof(candidateTypes));
            Requires.NotNull(registrationContext, nameof(registrationContext));

            var logger = this.GetLogger(registrationContext);

            var conventions = builder;
            var typeInfos = candidateTypes;

            // get all type infos from the composition assemblies
            var appServiceContractsInfos = this.GetAppServiceContracts(typeInfos, registrationContext)?.ToList();
            if (appServiceContractsInfos == null)
            {
                return;
            }

            registrationContext.AmbientServices?.SetAppServiceInfos(appServiceContractsInfos);

            var contractDeclarationTypes = appServiceContractsInfos.Select(e => e.contractDeclarationType).ToList();

            var typeRegistry = registrationContext.AmbientServices?.TypeRegistry ?? RuntimeTypeRegistry.Instance;
            var metadataResolver = new AppServiceMetadataResolver(typeRegistry);
            foreach (var appServiceContractInfo in appServiceContractsInfos)
            {
                var appServiceContract = appServiceContractInfo.contractDeclarationType;
                var appServiceInfo = appServiceContractInfo.appServiceInfo;

                var isPartBuilder = this.TryConfigurePartBuilder(
                    appServiceInfo,
                    appServiceContract,
                    conventions,
                    typeInfos,
                    logger);

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
                        this.ConfigurePartBuilder(partConventionsBuilder, appServiceContract, appServiceInfo, contractDeclarationTypes, metadataResolver, logger);
                    }
                    else
                    {
                        logger.Warn("No part conventions builders nor part builders found for {serviceContractType}.", appServiceContract);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the application service contracts to register.
        /// </summary>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">The registration context.</param>
        /// <returns>
        /// An enumeration of key-value pairs, where the key is the <see cref="T:TypeInfo"/> and the
        /// value is the <see cref="IAppServiceInfo"/>.
        /// </returns>
        protected internal virtual IEnumerable<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> GetAppServiceContracts(
            IList<Type> candidateTypes,
            IInjectionRegistrationContext registrationContext)
        {
            var appServiceInfoProviders = this.GetAppServiceInfoProviders(registrationContext);
            if (appServiceInfoProviders == null)
            {
                yield break;
            }

            var parts = new List<Type>(candidateTypes);
            if (registrationContext.Parts != null)
            {
                parts.AddRange(registrationContext.Parts);
            }

            var oldParts = registrationContext.Parts;
            registrationContext.Parts = parts;

            foreach (var appServiceInfoProvider in appServiceInfoProviders)
            {
                foreach (var item in appServiceInfoProvider.GetAppServiceInfos(registrationContext))
                {
                    yield return item;
                }
            }

            registrationContext.Parts = oldParts;
        }

        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <param name="registrationContext">Context for the registration.</param>
        /// <returns>
        /// An enumeration of <see cref="IAppServiceInfoProvider"/> objects.
        /// </returns>
        protected virtual IEnumerable<IAppServiceInfoProvider> GetAppServiceInfoProviders(IInjectionRegistrationContext registrationContext)
        {
            var ambientServicesProviders = registrationContext.AmbientServices?.GetService<IEnumerable<Lazy<IAppServiceInfoProvider, AppServiceMetadata>>>();
            if (ambientServicesProviders != null)
            {
                var orderedProviders = ambientServicesProviders.Order().Select(p => p.Value);
                foreach (var provider in orderedProviders)
                {
                    yield return provider;
                }
            }

            var contextProviders = registrationContext.AppServiceInfoProviders;
            if (contextProviders != null)
            {
                foreach (var provider in contextProviders)
                {
                    yield return provider;
                }
            }
        }

        /// <summary>
        /// Configures the part builder.
        /// </summary>
        /// <param name="partBuilder">The part builder.</param>
        /// <param name="serviceContract">The service contract.</param>
        /// <param name="appServiceInfo">The application service metadata.</param>
        /// <param name="appServiceContracts">The application service contracts.</param>
        /// <param name="metadataResolver">The metadata resolver.</param>
        /// <param name="logger">The logger.</param>
        protected void ConfigurePartBuilder(
            IPartConventionsBuilder partBuilder,
            Type serviceContract,
            IAppServiceInfo appServiceInfo,
            IList<Type> appServiceContracts,
            IAppServiceMetadataResolver metadataResolver,
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
                        (t, b) => this.ConfigureExport(serviceContract, b, exportedContractType, t, metadataAttributes, metadataResolver));

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
                        (t, b) => this.ConfigureExport(serviceContract, b, t, t, metadataAttributes, metadataResolver));
                }
            }
            else
            {
                partBuilder.Export(
                    b => this.ConfigureExport(serviceContract, b, exportedContractType, null, metadataAttributes, metadataResolver));
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

        /// <summary>
        /// Serializes the service contract metadata.
        /// </summary>
        /// <param name="serviceContract">The service contract.</param>
        /// <param name="contractMetadata">The contract metadata.</param>
        /// <returns>
        /// The serialized service contract information.
        /// </returns>
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

        /// <summary>
        /// Gets metadata attributes.
        /// </summary>
        /// <param name="appServiceInfo">The contract information.</param>
        /// <returns>
        /// An collection of attribute types.
        /// </returns>
        private IReadOnlyCollection<Type> GetMetadataAttributes(IAppServiceInfo appServiceInfo)
        {
            if (appServiceInfo.MetadataAttributes == null || appServiceInfo.MetadataAttributes.Length == 0)
            {
                return appServiceInfo.AsOpenGeneric
                           ? Type.EmptyTypes
                           : DefaultMetadataAttributeTypes;
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

        /// <summary>
        /// Determines whether the specified property imports an application service.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <param name="appServiceContracts">The application service contracts.</param>
        /// <returns><c>true</c> if the specified property imports an application service, otherwise <c>false</c>.</returns>
        private bool IsAppServiceImport(PropertyInfo pi, IList<Type> appServiceContracts)
        {
            if (pi == null || !pi.CanWrite || !pi.SetMethod.IsPublic)
            {
                return false;
            }

            var propertyType = pi.PropertyType;
            Type serviceContractType;
            if (propertyType.IsArray)
            {
                serviceContractType = propertyType.GetElementType();
                serviceContractType = this.TryGetServiceContractTypeFromExportFactory(serviceContractType)
                                      ?? serviceContractType;
            }
            else if (propertyType.IsConstructedGenericType)
            {
                var genericTypeDefinition = propertyType.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(IList<>) || genericTypeDefinition == typeof(ICollection<>)
                                                             || genericTypeDefinition == typeof(IEnumerable<>))
                {
                    serviceContractType = propertyType.GetTypeInfo().GenericTypeArguments[0];
                    serviceContractType = this.TryGetServiceContractTypeFromExportFactory(serviceContractType)
                                          ?? serviceContractType;
                }
                else
                {
                    serviceContractType = genericTypeDefinition;
                }
            }
            else
            {
                serviceContractType = propertyType;
            }

            var serviceContractTypeInfo = serviceContractType.GetTypeInfo();
            var isImport = appServiceContracts.Any(svc => svc.Equals(serviceContractTypeInfo));
            return isImport;
        }

        /// <summary>
        /// Tries to get the service contract type from export factory.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The service type referenced by the export factory, or <c>null</c> if the type is not an export factory.</returns>
        private Type? TryGetServiceContractTypeFromExportFactory(Type type)
        {
            if (type.IsConstructedGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(IExportFactory<>)
                    || genericTypeDefinition == typeof(IExportFactory<,>))
                {
                    return type.GetTypeInfo().GenericTypeArguments[0];
                }
            }

            return null;
        }

        private void ConfigureExport(
            Type serviceContract,
            IExportConventionsBuilder exportBuilder,
            Type exportedContractType,
            Type? serviceImplementationType,
            IEnumerable<Type> metadataAttributes,
            IAppServiceMetadataResolver metadataResolver)
        {
            exportBuilder.AsContractType(exportedContractType);
            this.AddCompositionMetadata(exportBuilder, serviceImplementationType, metadataAttributes, metadataResolver);
            this.AddCompositionMetadataForGenerics(exportBuilder, serviceContract, metadataResolver);
        }

        /// <summary>
        /// Checks the type of the exported contract.
        /// </summary>
        /// <exception cref="InjectionException">Thrown when a Composition error condition occurs.</exception>
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
                        Strings.AppServiceCompositionContractTypeDoesNotMatchServiceContract,
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

            // get the one constructor marked as CompositionConstructor.
            var explicitlyMarkedConstructors = constructorsList.Where(c => c.GetCustomAttribute<InjectConstructorAttribute>() != null).ToList();
            if (explicitlyMarkedConstructors.Count == 0)
            {
                // none marked explicitly, leave the decision up to the IoC implementation.
                return null;
            }

            if (explicitlyMarkedConstructors.Count > 1)
            {
                throw new InjectionException(string.Format(Strings.AppServiceMultipleCompositionConstructors, typeof(InjectConstructorAttribute), constructorsList[0].DeclaringType, serviceContract));
            }

            return explicitlyMarkedConstructors[0];
        }

        private void AddCompositionMetadataForGenerics(IExportConventionsBuilder builder, Type serviceContract, IAppServiceMetadataResolver metadataResolver)
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
                    metadataResolver.GetMetadataNameFromGenericTypeParameter(genericTypeParameter),
                    t => metadataResolver.GetMetadataValueFromGenericParameter(t, position, serviceContractType));
            }
        }

        private void AddCompositionMetadata(IExportConventionsBuilder builder, Type? serviceImplementationType, IEnumerable<Type> attributeTypes, IAppServiceMetadataResolver metadataResolver)
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
                var valueProperties = metadataResolver.GetMetadataValueProperties(attributeType);
                foreach (var valuePropertyEntry in valueProperties)
                {
                    builder.AddMetadata(
                        valuePropertyEntry.Key,
                        t => metadataResolver.GetMetadataValueFromAttribute(t, attributeType, valuePropertyEntry.Value));
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
                    logger.Debug("Service {serviceContractType} matches {serviceInstanceType}.", serviceContractType, appServiceInfo.InstanceType);
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
                    logger.Debug("Service {serviceContractType} matches open generic contract types.", serviceContractType);
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
                        logger.Debug("Service {serviceContractType} matches open generic implementation type {serviceInstanceType}.", serviceContractType, selectedInstanceType?.ToString() ?? "<not found>");
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
                    logger.Debug("Service {serviceContractType} matches open generic contract types.", serviceContractType);
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
                    logger.Debug("Service {serviceContractType} matches multiple derived from it.", serviceContractType);
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
                    logger.Debug("Service {serviceContractType} matches {serviceInstanceType}.", serviceContractType, selectedPart);
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
        /// <param name="typeInfos">The type infos.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// True if a part builder could be configured, false otherwise.
        /// </returns>
        private bool TryConfigurePartBuilder(
            IAppServiceInfo appServiceInfo,
            Type contractDeclarationType,
            IConventionsBuilder conventions,
            IEnumerable<Type> typeInfos,
            ILogger logger)
        {
            var serviceContractType = contractDeclarationType;

            if (appServiceInfo.Instance != null)
            {
                conventions
                    .ForInstance(serviceContractType, appServiceInfo.Instance)
                    .AllowMultiple(appServiceInfo.AllowMultiple);
                return true;
            }

            if (appServiceInfo.InstanceFactory != null)
            {
                var partBuilder = conventions
                    .ForInstanceFactory(serviceContractType, appServiceInfo.InstanceFactory)
                    .AllowMultiple(appServiceInfo.AllowMultiple);
                if (appServiceInfo.IsSingleton())
                {
                    partBuilder.Singleton();
                }
                else if (appServiceInfo.IsScoped())
                {
                    partBuilder.Scoped();
                }

                return true;
            }

            return false;
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
        /// <param name="serviceContract">Type of the service contract.</param>
        /// <returns><c>true</c> if the part type matches the type of the generic contract, otherwise <c>false</c>.</returns>
        private bool MatchDerivedFromContractType(Type partType, TypeInfo serviceContract)
        {
            return this.MatchDerivedFromContractType(partType.GetTypeInfo(), (Type)serviceContract);
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
            return implementedInterfaces.Any(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == serviceContractType);
        }
    }
}