// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceConventionsRegistrar.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Conventions registrar for application services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Composition.AttributedModel;
    using Kephas.Composition.Conventions;
    using Kephas.Composition.Metadata;
    using Kephas.Resources;

    /// <summary>
    /// Conventions registrar for application services.
    /// </summary>
    public class AppServiceConventionsRegistrar : IConventionsRegistrar
    {
        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <param name="builder">The registration builder.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        public void RegisterConventions(IConventionsBuilder builder, IEnumerable<TypeInfo> candidateTypes)
        {
            var conventions = builder;
            var typeInfos = candidateTypes.ToList();

            // get all type infos from the composition assemblies
            var appServiceContractsInfos =
                typeInfos.ToDictionary(ti => ti, ti => ti.GetCustomAttribute<AppServiceContractAttribute>())
                    .Where(ta => ta.Value != null)
                    .ToList();

            foreach (var appServiceContractInfo in appServiceContractsInfos)
            {
                var serviceContract = appServiceContractInfo.Key;
                var serviceContractType = serviceContract.AsType();
                var serviceContractMetadata = appServiceContractInfo.Value;

                var partBuilder = this.TryGetPartBuilder(serviceContractMetadata, serviceContract, conventions, typeInfos);

                if (partBuilder == null)
                {
                    continue;
                }

                var exportedContractType = serviceContractMetadata.ContractType ?? serviceContractType;
                var exportedContract = exportedContractType.GetTypeInfo();
                this.CheckExportedContractType(exportedContractType, serviceContract, serviceContractType);

                var metadataAttributes = appServiceContractInfo.Value.MetadataAttributes;
                if (exportedContract.IsGenericTypeDefinition)
                {
                    if (serviceContractMetadata.AsOpenGeneric)
                    {
                        partBuilder.ExportInterfaces(
                            t => this.IsClosedGenericOf(exportedContract, t.GetTypeInfo()),
                            (t, b) => this.ConfigureExport(serviceContract, b, exportedContractType, metadataAttributes));
                    }
                    else
                    {
                        partBuilder.ExportInterfaces(
                            t => this.IsClosedGenericOf(exportedContract, t.GetTypeInfo()),
                            (t, b) => this.ConfigureExport(serviceContract, b, t, metadataAttributes));
                    }
                }
                else
                {
                    partBuilder.Export(
                        b => this.ConfigureExport(serviceContract, b, exportedContractType, metadataAttributes));
                }

                partBuilder.SelectConstructor(this.SelectAppServiceConstructor);

                partBuilder.ImportProperties(pi => this.IsAppServiceImport(pi, appServiceContractsInfos));

                if (serviceContractMetadata.IsShared)
                {
                    partBuilder.Shared();
                }
            }
        }

        /// <summary>
        /// Determines whether the specified property imports an application service.
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <param name="appServiceContractsInfos">The application service contracts infos.</param>
        /// <returns><c>true</c> if the specified property imports an application service, otherwise <c>false</c>.</returns>
        private bool IsAppServiceImport(PropertyInfo pi, List<KeyValuePair<TypeInfo, AppServiceContractAttribute>> appServiceContractsInfos)
        {
            if (!pi.CanWrite || !pi.SetMethod.IsPublic)
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
            var isImport = appServiceContractsInfos.Any(kv => kv.Key.Equals(serviceContractTypeInfo));
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
        /// <param name="metadataAttributes">The metadata attributes.</param>
        private void ConfigureExport(TypeInfo serviceContract, IExportConventionsBuilder exportBuilder, Type exportedContractType, Type[] metadataAttributes)
        {
            exportBuilder.AsContractType(exportedContractType);
            this.AddCompositionMetadata(exportBuilder, metadataAttributes);
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
        /// <param name="exportedContractType">Type of the exported contract.</param>
        /// <param name="serviceContract">The service contract.</param>
        /// <param name="serviceContractType">Type of the service contract.</param>
        private void CheckExportedContractType(
            Type exportedContractType,
            TypeInfo serviceContract,
            Type serviceContractType)
        {
            var exportedContract = exportedContractType.GetTypeInfo();
            if (exportedContract.IsGenericTypeDefinition)
            {
                // TODO check to see if any of the interfaces have as generic definition the exported contract.
            }
            else if (!exportedContract.IsAssignableFrom(serviceContract))
            {
                throw new CompositionException(
                    string.Format(
                        Strings.AppServiceCompositionContractTypeDoesNotMatchServiceContract,
                        exportedContractType,
                        serviceContractType));
            }
        }

        /// <summary>
        /// Selects the application service constructor.
        /// </summary>
        /// <param name="constructors">The constructors.</param>
        /// <returns>The application service constructor.</returns>
        private ConstructorInfo SelectAppServiceConstructor(IEnumerable<ConstructorInfo> constructors)
        {
            var constructorsList = constructors.ToList();

            if (constructorsList.Count == 1)
            {
                return constructorsList[0];
            }

            var eligibleConstructors = constructorsList.Where(c => c.GetCustomAttribute<CompositionConstructorAttribute>() != null).ToList();
            if (eligibleConstructors.Count == 0)
            {
                throw new CompositionException(string.Format(Strings.AppServiceMissingCompositionConstructor, typeof(CompositionConstructorAttribute), constructorsList[0].DeclaringType));
            }

            if (eligibleConstructors.Count > 1)
            {
                throw new CompositionException(string.Format(Strings.AppServiceMultipleCompositionConstructors, typeof(CompositionConstructorAttribute), constructorsList[0].DeclaringType));
            }

            return eligibleConstructors[1];
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
            var closedGeneric =
                type.GetTypeInfo()
                    .ImplementedInterfaces.Select(i => i.GetTypeInfo())
                    .FirstOrDefault(
                        i =>
                        i.IsGenericType && !i.IsGenericTypeDefinition
                        && i.GetGenericTypeDefinition() == serviceContractType);

            if (closedGeneric == null)
            {
                return null;
            }

            return closedGeneric.GenericTypeArguments[position];
        }

        /// <summary>
        /// Gets the metadata name from generic type parameter.
        /// </summary>
        /// <param name="genericTypeParameter">The generic type parameter.</param>
        /// <returns>The metadata name.</returns>
        private string GetMetadataNameFromGenericTypeParameter(Type genericTypeParameter)
        {
            var name = genericTypeParameter.Name;
            if (name.StartsWith("T") && name.Length > 1 && name[1] == char.ToUpperInvariant(name[1]))
            {
                name = name.Substring(1);
            }

            if (!name.EndsWith("Type"))
            {
                name = name + "Type";
            }

            return name;
        }

        /// <summary>
        /// Adds the composition metadata.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="attributeTypes">The attribute types.</param>
        private void AddCompositionMetadata(IExportConventionsBuilder builder, Type[] attributeTypes)
        {
            if (attributeTypes == null || attributeTypes.Length == 0)
            {
                return;
            }

            foreach (var attributeType in attributeTypes)
            {
                var attrType = attributeType;
                builder.AddMetadata(
                    this.GetMetadataNameFromAttributeType(attributeType),
                    t => this.GetMetadataValueFromAttribute(t, attrType));
            }
        }

        /// <summary>
        /// Gets the metadata name from the attribute type.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>The metadata name from the attribute type.</returns>
        private string GetMetadataNameFromAttributeType(Type attributeType)
        {
            var name = attributeType.Name;
            const string AttributeSuffix = "Attribute";
            return name.EndsWith(AttributeSuffix) ? name.Substring(0, name.Length - AttributeSuffix.Length) : name;
        }

        /// <summary>
        /// Gets the metadata value from attribute.
        /// </summary>
        /// <param name="partType">Type of the part.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>The metadata value from attribute.</returns>
        private object GetMetadataValueFromAttribute(Type partType, Type attributeType)
        {
            var value =
                partType.GetTypeInfo()
                    .GetCustomAttributes(attributeType, inherit: true)
                    .OfType<IMetadataValue>()
                    .Select(a => a.Value)
                    .FirstOrDefault();
            return value;
        }

        /// <summary>
        /// Tries to get the part builder.
        /// </summary>
        /// <param name="serviceContractMetadata">The service contract metadata.</param>
        /// <param name="serviceContract">The service contract.</param>
        /// <param name="conventions">The conventions.</param>
        /// <param name="typeInfos">The type infos.</param>
        /// <returns>
        /// The part builder or <c>null</c>.
        /// </returns>
        private IPartConventionsBuilder TryGetPartBuilder(
            AppServiceContractAttribute serviceContractMetadata,
            TypeInfo serviceContract,
            IConventionsBuilder conventions,
            IEnumerable<TypeInfo> typeInfos)
        {
            var serviceContractType = serviceContract.AsType();

            if (serviceContract.IsGenericTypeDefinition)
            {
                // if there is non-generic service contract with the same full name
                // then add just the conventions for the derived types.
                return conventions.ForTypesMatching(t => this.MatchOpenGenericContractType(t, serviceContractType));
            }

            if (serviceContractMetadata.AllowMultiple)
            {
                // if the service contract metadata allows multiple service registrations
                // then add just the conventions for the derived types.
                return conventions.ForTypesDerivedFrom(serviceContractType);
            }

            var parts =
                typeInfos.Where(
                    ti =>
                    serviceContract.IsAssignableFrom(ti) && ti.IsClass && !ti.IsAbstract
                    && ti.GetCustomAttribute<ExcludeFromCompositionAttribute>() == null).ToList();
            if (parts.Count == 1)
            {
                return conventions.ForType(parts[0].AsType());
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

                return conventions.ForType(selectedPart.AsType());
            }

            return null;
        }

        /// <summary>
        /// Checks whether the part type matches the type of the open generic contract.
        /// </summary>
        /// <param name="partType">Type of the part.</param>
        /// <param name="serviceContractType">Type of the service contract.</param>
        /// <returns><c>true</c> if the part type matches the type of the generic contract, otherwise <c>false</c>.</returns>
        private bool MatchOpenGenericContractType(Type partType, Type serviceContractType)
        {
            var implementedInterfaces = partType.GetTypeInfo().ImplementedInterfaces;
            return implementedInterfaces.Any(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == serviceContractType);
        }
    }
}