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
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Kephas.Composition;
    using Kephas.Composition.AttributedModel;
    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;
    using Kephas.Composition.Metadata;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Runtime;

    /// <summary>
    /// Base for conventions registrars of application services.
    /// </summary>
    public class AppServiceInfoConventionsRegistrar : IConventionsRegistrar
    {
        /// <summary>
        /// The attribute suffix.
        /// </summary>
        private const string AttributeSuffix = "Attribute";

        /// <summary>
        /// The 'T' prefix in generic type arguments.
        /// </summary>
        private const string TypePrefix = "T";

        /// <summary>
        /// The 'Type' suffix in generic type arguments.
        /// </summary>
        private const string TypeSuffix = "Type";

        /// <summary>
        /// Information describing the metadata value type.
        /// </summary>
        private static IRuntimeTypeInfo metadataValueTypeInfo;

        /// <summary>
        /// Gets information describing the metadata value type.
        /// </summary>
        /// <value>
        /// Information describing the metadata value type.
        /// </value>
        private static IRuntimeTypeInfo MetadataValueTypeInfo
            => metadataValueTypeInfo ?? (metadataValueTypeInfo = typeof(IMetadataValue).AsRuntimeTypeInfo());

        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <param name="builder">The registration builder.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        public void RegisterConventions(IConventionsBuilder builder, IEnumerable<TypeInfo> candidateTypes, ICompositionRegistrationContext registrationContext)
        {
            Requires.NotNull(builder, nameof(builder));
            Requires.NotNull(candidateTypes, nameof(candidateTypes));
            Requires.NotNull(registrationContext, nameof(registrationContext));

            var logger = this.GetLogger(registrationContext);

            var conventions = builder;
            var typeInfos = (candidateTypes as IList<TypeInfo>) ?? candidateTypes.ToList();

            // get all type infos from the composition assemblies
            var appServiceContractsInfos = this.GetAppServiceContracts(typeInfos, registrationContext)?.ToList();
            if (appServiceContractsInfos == null)
            {
                return;
            }

            var appServiceContracts = appServiceContractsInfos.Select(e => e.contractType).ToList();

            foreach (var appServiceContractInfo in appServiceContractsInfos)
            {
                var appServiceContract = appServiceContractInfo.contractType;
                var appServiceInfo = appServiceContractInfo.appServiceInfo;

                var partBuilder = this.TryGetPartBuilder(
                    appServiceInfo,
                    appServiceContract,
                    conventions,
                    typeInfos,
                    logger);

                if (partBuilder == null)
                {
                    var partConventionsBuilder = this.TryGetPartConventionsBuilder(
                        appServiceInfo,
                        appServiceContract,
                        conventions,
                        typeInfos,
                        logger);
                    if (partConventionsBuilder != null)
                    {
                        this.ConfigurePartBuilder(partConventionsBuilder, appServiceContract, appServiceInfo, appServiceContracts, logger);
                    }
                    else
                    {
                        logger.Warn($"No part conventions builders nor part builders found for {appServiceContract}.");
                    }
                }
            }
        }

        /// <summary>
        /// Gets the metadata name from the attribute type.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>The metadata name from the attribute type.</returns>
        internal static string GetMetadataNameFromAttributeType(Type attributeType)
        {
            var name = attributeType.Name;
            return name.EndsWith(AttributeSuffix) ? name.Substring(0, name.Length - AttributeSuffix.Length) : name;
        }

        /// <summary>
        /// Gets the metadata value from attribute.
        /// </summary>
        /// <param name="partType">Type of the part.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="property">The metadata property.</param>
        /// <returns>
        /// The metadata value from attribute.
        /// </returns>
        internal static object GetMetadataValueFromAttribute(Type partType, Type attributeType, IPropertyInfo property)
        {
            var attr =
                partType.GetTypeInfo()
                    .GetCustomAttributes(attributeType, inherit: true)
                    .FirstOrDefault();

            var value = attr == null ? null : property.GetValue(attr);
            return value;
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
        protected virtual IEnumerable<(TypeInfo contractType, IAppServiceInfo appServiceInfo)> GetAppServiceContracts(
            IList<TypeInfo> candidateTypes,
            ICompositionRegistrationContext registrationContext)
        {
            var appServiceInfoProviders = this.GetAppServiceInfoProviders(candidateTypes, registrationContext);
            if (appServiceInfoProviders == null)
            {
                yield break;
            }

            foreach (var appServiceInfoProvider in appServiceInfoProviders)
            {
                foreach (var item in appServiceInfoProvider.GetAppServiceInfos(candidateTypes, registrationContext))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        /// <returns>
        /// An enumeration of <see cref="IAppServiceInfoProvider"/> objects.
        /// </returns>
        protected virtual IEnumerable<IAppServiceInfoProvider> GetAppServiceInfoProviders(
            IList<TypeInfo> candidateTypes,
            ICompositionRegistrationContext registrationContext)
        {
            foreach (var candidateType in candidateTypes)
            {
                if (candidateType.IsInstantiableAppServiceInfoProviderType())
                {
                    yield return (IAppServiceInfoProvider)candidateType.AsRuntimeTypeInfo().CreateInstance();
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
        /// <param name="logger">The logger.</param>
        protected void ConfigurePartBuilder(
            IPartConventionsBuilder partBuilder,
            TypeInfo serviceContract,
            IAppServiceInfo appServiceInfo,
            IList<TypeInfo> appServiceContracts,
            ILogger logger)
        {
            var serviceContractType = serviceContract.AsType();
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
                    partBuilder.ExportInterfaces(
                        t => this.IsClosedGenericOf(exportedContract, t.GetTypeInfo()),
                        (t, b) => this.ConfigureExport(serviceContract, b, exportedContractType, t, metadataAttributes));

                    if (metadataAttributes.Count > 0)
                    {
                        var hasCustomMetadataAttributes = metadataAttributes.Any(
                            a => !typeof(IAppServiceInfo).IsAssignableFrom(a));

                        // warn about metadata on open generic exports only if custom attributes are provided.
                        if (hasCustomMetadataAttributes)
                        {
                            logger.Warn(string.Format(Strings.AppServiceConventionsRegistrarBase_AsOpenGenericDoesNotSupportMetadataAttributes_Warning, exportedContract));
                        }
                    }
                }
                else
                {
                    partBuilder.ExportInterfaces(
                        t => this.IsClosedGenericOf(exportedContract, t.GetTypeInfo()),
                        (t, b) => this.ConfigureExport(serviceContract, b, t, t, metadataAttributes));
                }
            }
            else
            {
                partBuilder.Export(
                    b => this.ConfigureExport(serviceContract, b, exportedContractType, null, metadataAttributes));
            }

            partBuilder.SelectConstructor(ctorInfos => this.SelectAppServiceConstructor(serviceContract, ctorInfos));

            partBuilder.ImportProperties(pi => this.IsAppServiceImport(pi, appServiceContracts));

            if (appServiceInfo.IsShared())
            {
                partBuilder.Shared();
            }
            else if (appServiceInfo.IsScopeShared())
            {
                var scopeName = appServiceInfo.ScopeName;
                partBuilder.ScopeShared(scopeName);
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
        private string SerializeServiceContractMetadata(TypeInfo serviceContract, IAppServiceInfo contractMetadata)
        {
            var sb = new StringBuilder();
            sb.Append("{'")
                .Append(serviceContract.Name)
                .Append("': { multi: ")
                .Append(contractMetadata.AllowMultiple)
                .Append(", lifetime: ")
                .Append(contractMetadata.Lifetime);
            if (serviceContract.IsGenericTypeDefinition)
            {
                sb.Append(", asOpenGeneric: ").Append(contractMetadata.AsOpenGeneric);
            }

            sb.Append("}");

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
                           ? AppServiceContractAttribute.EmptyMetadataAttributeTypes
                           : AppServiceContractAttribute.DefaultMetadataAttributeTypes;
            }

            var attrs = appServiceInfo.MetadataAttributes.ToList();
            foreach (var attr in AppServiceContractAttribute.DefaultMetadataAttributeTypes)
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
        private bool IsAppServiceImport(
            PropertyInfo pi,
            IList<TypeInfo> appServiceContracts)
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
        private Type TryGetServiceContractTypeFromExportFactory(Type type)
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

        /// <summary>
        /// Configures the export.
        /// </summary>
        /// <param name="serviceContract">The service contract.</param>
        /// <param name="exportBuilder">The export builder.</param>
        /// <param name="exportedContractType">Type of the exported contract.</param>
        /// <param name="serviceImplementationType">Type of the service implementation.</param>
        /// <param name="metadataAttributes">The metadata attributes.</param>
        private void ConfigureExport(TypeInfo serviceContract, IExportConventionsBuilder exportBuilder, Type exportedContractType, Type serviceImplementationType, IEnumerable<Type> metadataAttributes)
        {
            exportBuilder.AsContractType(exportedContractType);
            this.AddCompositionMetadata(exportBuilder, serviceImplementationType, metadataAttributes);
            this.AddCompositionMetadataForGenerics(exportBuilder, serviceContract);
        }

        /// <summary>
        /// Determines whether the provided interface is a closed generic of the specified open generic contract.
        /// </summary>
        /// <param name="openGenericContract">The open generic contract.</param>
        /// <param name="exportInterface">The export interface.</param>
        /// <returns><c>true</c> if the provided interface is a closed generic of the specified open generic contract, otherwise <c>false</c>.</returns>
        private bool IsClosedGenericOf(TypeInfo openGenericContract, TypeInfo exportInterface)
        {
            return exportInterface.IsGenericType && exportInterface.GetGenericTypeDefinition() == openGenericContract.AsType();
        }

        /// <summary>
        /// Checks the type of the exported contract.
        /// </summary>
        /// <exception cref="CompositionException">Thrown when a Composition error condition occurs.</exception>
        /// <param name="exportedContractType">Type of the exported contract.</param>
        /// <param name="serviceContract">The service contract.</param>
        /// <param name="serviceContractType">Type of the service contract.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// <c>true</c> if the service contract is valid, false otherwise.
        /// </returns>
        private bool CheckExportedContractType(Type exportedContractType, TypeInfo serviceContract, Type serviceContractType, ILogger logger)
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
                throw new CompositionException(contractValidationMessage);
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
        private ConstructorInfo SelectAppServiceConstructor(TypeInfo serviceContract, IEnumerable<ConstructorInfo> constructors)
        {
            var constructorsList = constructors.Where(c => !c.IsStatic && c.IsPublic).ToList();

            if (constructorsList.Count == 0)
            {
                throw new CompositionException(string.Format(Strings.AppServiceMissingCompositionConstructor, constructors.FirstOrDefault()?.DeclaringType, serviceContract));
            }

            if (constructorsList.Count == 1)
            {
                return constructorsList[0];
            }

            var explicitelyMarkedConstructors = constructorsList.Where(c => c.GetCustomAttribute<CompositionConstructorAttribute>() != null).ToList();
            if (explicitelyMarkedConstructors.Count == 0)
            {
                var sortedConstructors = constructorsList.ToDictionary(c => c, c => c.GetParameters().Length).OrderByDescending(kv => kv.Value).ToList();
                if (sortedConstructors[0].Value == sortedConstructors[1].Value)
                {
                    throw new CompositionException(string.Format(Strings.AppServiceAmbiguousCompositionConstructor, constructorsList[0].DeclaringType, serviceContract, typeof(CompositionConstructorAttribute)));
                }

                return sortedConstructors[0].Key;
            }

            if (explicitelyMarkedConstructors.Count > 1)
            {
                throw new CompositionException(string.Format(Strings.AppServiceMultipleCompositionConstructors, typeof(CompositionConstructorAttribute), constructorsList[0].DeclaringType, serviceContract));
            }

            return explicitelyMarkedConstructors[0];
        }

        /// <summary>
        /// Adds the composition metadata.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="serviceContract">The service contract.</param>
        private void AddCompositionMetadataForGenerics(IExportConventionsBuilder builder, TypeInfo serviceContract)
        {
            if (!serviceContract.IsGenericTypeDefinition)
            {
                return;
            }

            var serviceContractType = serviceContract.AsType();
            var genericTypeParameters = serviceContract.GenericTypeParameters;
            for (var i = 0; i < genericTypeParameters.Length; i++)
            {
                var genericTypeParameter = genericTypeParameters[i];
                var position = i;
                builder.AddMetadata(
                    this.GetMetadataNameFromGenericTypeParameter(genericTypeParameter),
                    t => this.GetMetadataValueFromGenericParameter(t, position, serviceContractType));
            }
        }

        /// <summary>
        /// Gets the metadata value from generic parameter.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="position">The position.</param>
        /// <param name="serviceContractType">Type of the service contract.</param>
        /// <returns>The metadata value.</returns>
        private object GetMetadataValueFromGenericParameter(Type type, int position, Type serviceContractType)
        {
            var typeInfo = type.GetTypeInfo();
            var closedGeneric = typeInfo.ImplementedInterfaces
                    .Select(i => i.GetTypeInfo())
                    .FirstOrDefault(
                        i =>
                        i.IsGenericType && !i.IsGenericTypeDefinition
                        && i.GetGenericTypeDefinition() == serviceContractType);

            if (closedGeneric == null && type.IsConstructedGenericType && type.GetGenericTypeDefinition() == serviceContractType)
            {
                closedGeneric = typeInfo;
            }

            var genericArg = closedGeneric?.GenericTypeArguments[position];
            if (genericArg?.IsGenericParameter ?? false)
            {
                genericArg = genericArg.GetTypeInfo().BaseType;
            }

            return genericArg;
        }

        /// <summary>
        /// Gets the metadata name from generic type parameter.
        /// </summary>
        /// <param name="genericTypeParameter">The generic type parameter.</param>
        /// <returns>The metadata name.</returns>
        private string GetMetadataNameFromGenericTypeParameter(Type genericTypeParameter)
        {
            var name = genericTypeParameter.Name;
            if (name.StartsWith(TypePrefix) && name.Length > 1 && name[1] == char.ToUpperInvariant(name[1]))
            {
                name = name.Substring(1);
            }

            if (!name.EndsWith(TypeSuffix))
            {
                name = name + TypeSuffix;
            }

            return name;
        }

        /// <summary>
        /// Adds the composition metadata.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="serviceImplementationType">Type of the service implementation.</param>
        /// <param name="attributeTypes">The attribute types.</param>
        private void AddCompositionMetadata(IExportConventionsBuilder builder, Type serviceImplementationType, IEnumerable<Type> attributeTypes)
        {
            // add the service type.
            builder.AddMetadata(nameof(AppServiceMetadata.AppServiceImplementationType), t => serviceImplementationType ?? t);

            // add the rest of the metadata indicated by the attributes.
            if (attributeTypes == null)
            {
                return;
            }

            foreach (var attributeType in attributeTypes)
            {
                var attrTypeInfo = attributeType.AsRuntimeTypeInfo();
                var valueProperties = this.GetMetadataValueProperties(attrTypeInfo);
                foreach (var valuePropertyEntry in valueProperties)
                {
                    builder.AddMetadata(
                        valuePropertyEntry.Key,
                        t => GetMetadataValueFromAttribute(t, attributeType, valuePropertyEntry.Value));
                }
            }
        }

        /// <summary>
        /// Gets the metadata value properties which should be retrieved from the attribute.
        /// </summary>
        /// <param name="attributeTypeInfo">Information describing the attribute type.</param>
        /// <returns>
        /// The metadata properties.
        /// </returns>
        private IDictionary<string, IPropertyInfo> GetMetadataValueProperties(IRuntimeTypeInfo attributeTypeInfo)
        {
            const string MetadataValuePropertiesName = "Kephas_MetadataValueProperties";
            var metadataValueProperties = attributeTypeInfo[MetadataValuePropertiesName] as IDictionary<string, IPropertyInfo>;
            if (metadataValueProperties == null)
            {
                metadataValueProperties = new Dictionary<string, IPropertyInfo>();
                var baseMetadataName = GetMetadataNameFromAttributeType(attributeTypeInfo.Type);

                foreach (var attrPropInfo in attributeTypeInfo.Properties.Values)
                {
                    var metadataValueAttribute = attrPropInfo.GetAttribute<MetadataValueAttribute>();
                    var explicitName = metadataValueAttribute?.ValueName;
                    var metadataValueName = !string.IsNullOrEmpty(explicitName)
                                                ? explicitName
                                                : attrPropInfo.Name == nameof(IMetadataValue.Value)
                                                    ? baseMetadataName
                                                    : baseMetadataName + attrPropInfo.Name;
                    if (metadataValueAttribute != null)
                    {
                        metadataValueProperties.Add(metadataValueName, attrPropInfo);
                    }
                    else if (attrPropInfo.Name == nameof(IMetadataValue.Value) && this.IsMetadataValueAttribute(attributeTypeInfo))
                    {
                        metadataValueProperties.Add(metadataValueName, attrPropInfo);
                    }
                }

                attributeTypeInfo[MetadataValuePropertiesName] = metadataValueProperties;
            }

            return metadataValueProperties;
        }

        /// <summary>
        /// Query if 'attributeType' is metadata value attribute.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>
        /// True if metadata value attribute, false if not.
        /// </returns>
        private bool IsMetadataValueAttribute(IRuntimeTypeInfo attributeType)
        {
            return MetadataValueTypeInfo.TypeInfo.IsAssignableFrom(attributeType.TypeInfo);
        }

        /// <summary>
        /// Tries to get the conventions part builder.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when there is an ambiguous override in the service implementations.</exception>
        /// <param name="appServiceInfo">The service contract metadata.</param>
        /// <param name="serviceContract">The service contract.</param>
        /// <param name="conventions">The conventions.</param>
        /// <param name="typeInfos">The type infos.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// The part builder or <c>null</c>.
        /// </returns>
        private IPartConventionsBuilder TryGetPartConventionsBuilder(
            IAppServiceInfo appServiceInfo,
            TypeInfo serviceContract,
            IConventionsBuilder conventions,
            IEnumerable<TypeInfo> typeInfos,
            ILogger logger)
        {
            var serviceContractType = serviceContract.AsType();

            if (serviceContract.IsGenericTypeDefinition)
            {
                if (logger.IsDebugEnabled())
                {
                    logger.Debug($"Service {serviceContractType} matches open generic contract types.");
                }

                // if there is non-generic service contract with the same full name
                // then add just the conventions for the derived types.
                return conventions.ForTypesMatching(t => this.MatchOpenGenericContractType(t, serviceContractType));
            }

            if (appServiceInfo.AllowMultiple)
            {
                if (logger.IsDebugEnabled())
                {
                    logger.Debug($"Service {serviceContractType} matches multiple derived from it.");
                }

                // if the service contract metadata allows multiple service registrations
                // then add just the conventions for the derived types.
                return conventions.ForTypesMatching(t => this.MatchDerivedFromContractType(t, serviceContract));
            }

            var parts = typeInfos.Where(part => this.MatchDerivedFromContractType(part, serviceContract)).ToList();
            if (parts.Count == 1)
            {
                var selectedPart = parts[0].AsType();

                if (logger.IsDebugEnabled())
                {
                    logger.Debug($"Service {serviceContractType} matches {selectedPart}.");
                }

                return conventions.ForType(selectedPart);
            }

            if (parts.Count > 1)
            {
                var overrideChain =
                    parts.ToDictionary(
                        ti => ti,
                        ti =>
                        ti.GetCustomAttribute<OverridePriorityAttribute>() ?? new OverridePriorityAttribute(Priority.Normal))
                        .OrderBy(item => item.Value.Value)
                        .ToList();

                var selectedPart = overrideChain[0].Key;
                if (overrideChain[0].Value.Value == overrideChain[1].Value.Value)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            Strings.AmbiguousOverrideForAppServiceContract,
                            serviceContract,
                            selectedPart,
                            string.Join(", ", overrideChain.Select(item => item.Key.ToString() + ":" + item.Value.Value))));
                }

                if (logger.IsDebugEnabled())
                {
                    logger.Debug($"Service {serviceContractType} matches {selectedPart}.");
                }

                return conventions.ForType(selectedPart.AsType());
            }

            return null;
        }

        /// <summary>
        /// Tries to get the conventions part builder.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when there is an ambiguous override in the service implementations.</exception>
        /// <param name="appServiceInfo">The service contract metadata.</param>
        /// <param name="serviceContract">The service contract.</param>
        /// <param name="conventions">The conventions.</param>
        /// <param name="typeInfos">The type infos.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// The part builder or <c>null</c>.
        /// </returns>
        private IPartBuilder TryGetPartBuilder(
            IAppServiceInfo appServiceInfo,
            TypeInfo serviceContract,
            IConventionsBuilder conventions,
            IEnumerable<TypeInfo> typeInfos,
            ILogger logger)
        {
            var serviceContractType = serviceContract.AsType();

            if (appServiceInfo.Instance != null)
            {
                return conventions.ForInstance(serviceContractType, appServiceInfo.Instance);
            }

            if (appServiceInfo.InstanceFactory != null)
            {
                return conventions.ForInstanceFactory(serviceContractType, appServiceInfo.InstanceFactory);
            }

            return null;
        }

        /// <summary>
        /// Determines whether the provided type info is an eligible part.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>
        /// <c>true</c> if the type information is an eligible part, otherwise <c>false</c>.
        /// </returns>
        private bool IsEligiblePart(TypeInfo typeInfo)
        {
            return typeInfo.IsClass && !typeInfo.IsAbstract && typeInfo.GetCustomAttribute<ExcludeFromCompositionAttribute>() == null;
        }

        /// <summary>
        /// Checks whether the part type matches the type of the open generic contract.
        /// </summary>
        /// <param name="partTypeInfo">Type of the part.</param>
        /// <param name="serviceContract">Type of the service contract.</param>
        /// <returns><c>true</c> if the part type matches the type of the generic contract, otherwise <c>false</c>.</returns>
        private bool MatchDerivedFromContractType(TypeInfo partTypeInfo, TypeInfo serviceContract)
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
            return this.MatchDerivedFromContractType(partType.GetTypeInfo(), serviceContract);
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