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

                var exportedContract = serviceContractMetadata.ContractType ?? serviceContractType;
                if (!exportedContract.GetTypeInfo().IsAssignableFrom(serviceContract))
                {
                    throw new CompositionException(string.Format(Strings.AppServiceCompositionContractTypeDoesNotMatchServiceContract, exportedContract, serviceContractType));
                }

                var metadataAttributes = appServiceContractInfo.Value.MetadataAttributes;
                partBuilder.Export(
                    b =>
                        {
                            b.AsContractType(exportedContract);
                            this.AddCompositionMetadata(b, metadataAttributes);
                            this.AddCompositionMetadataForGenerics(b, serviceContract);
                        });

                partBuilder.SelectConstructor(this.SelectAppServiceConstructor);

                if (serviceContractMetadata.IsShared)
                {
                    partBuilder.Shared();
                }
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
                throw new CompositionException(string.Format("There are no constructors marked with {0} for service {1}.", typeof(CompositionConstructorAttribute), constructorsList[0].DeclaringType));
            }

            if (eligibleConstructors.Count > 1)
            {
                throw new CompositionException(string.Format("Multiple constructors marked with {0} are declared for service {1}.", typeof(CompositionConstructorAttribute), constructorsList[0].DeclaringType));
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
                return conventions.ForTypesMatching(t => t.GetTypeInfo().ImplementedInterfaces.Any(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == serviceContractType));
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
    }
}