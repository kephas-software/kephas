// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceMetadataResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application service metadata resolver class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition.Metadata;
    using Kephas.Reflection;
    using Kephas.Runtime;

    internal class AppServiceMetadataResolver : IAppServiceMetadataResolver
    {
        private readonly IRuntimeTypeRegistry? typeRegistry;

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
        private IRuntimeTypeInfo? metadataValueTypeInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceMetadataResolver"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type serviceRegistry.</param>
        internal AppServiceMetadataResolver(IRuntimeTypeRegistry? typeRegistry)
        {
            this.typeRegistry = typeRegistry;
        }

        /// <summary>
        /// Gets information describing the metadata value type.
        /// </summary>
        /// <value>
        /// Information describing the metadata value type.
        /// </value>
        private IRuntimeTypeInfo MetadataValueTypeInfo
            => this.metadataValueTypeInfo ??= typeof(IMetadataValue).AsRuntimeTypeInfo(this.typeRegistry);

        /// <summary>
        /// Gets the metadata value properties which should be retrieved from the attribute.
        /// </summary>
        /// <param name="attributeType">The type of the attribute providing metadata.</param>
        /// <returns>
        /// The metadata properties.
        /// </returns>
        public IDictionary<string, IPropertyInfo> GetMetadataValueProperties(Type attributeType)
        {
            var attributeTypeInfo = attributeType.AsRuntimeTypeInfo(this.typeRegistry);

            const string MetadataValuePropertiesName = "__MetadataValueProperties";
            if (attributeTypeInfo[MetadataValuePropertiesName] is IDictionary<string, IPropertyInfo> metadataValueProperties)
            {
                return metadataValueProperties;
            }

            metadataValueProperties = new Dictionary<string, IPropertyInfo>();
            var baseMetadataName = this.GetMetadataNameFromAttributeType(attributeType);

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
                else if (attrPropInfo.Name == nameof(IMetadataValue.Value) && this.IsMetadataValueAttribute(attributeType))
                {
                    metadataValueProperties.Add(metadataValueName, attrPropInfo);
                }
            }

            attributeTypeInfo[MetadataValuePropertiesName] = metadataValueProperties;

            return metadataValueProperties;
        }

        /// <summary>
        /// Gets the metadata value from attribute.
        /// </summary>
        /// <param name="implementationType">The service implementation type.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="property">The metadata property.</param>
        /// <returns>
        /// The metadata value from attribute.
        /// </returns>
        public object GetMetadataValueFromAttribute(Type implementationType, Type attributeType, IPropertyInfo property)
        {
            var attr =
                implementationType.GetTypeInfo()
                    .GetCustomAttributes(attributeType, inherit: true)
                    .FirstOrDefault();

            var value = attr == null ? null : property.GetValue(attr);
            return value;
        }

        /// <summary>
        /// Gets the metadata name from the attribute type.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>The metadata name from the attribute type.</returns>
        public string GetMetadataNameFromAttributeType(Type attributeType)
        {
            var name = attributeType.Name;
            return name.EndsWith(AttributeSuffix) ? name.Substring(0, name.Length - AttributeSuffix.Length) : name;
        }

        /// <summary>
        /// Gets the metadata name from generic type parameter.
        /// </summary>
        /// <param name="genericTypeParameter">The generic type parameter.</param>
        /// <returns>The metadata name.</returns>
        public string GetMetadataNameFromGenericTypeParameter(Type genericTypeParameter)
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
        /// Gets the metadata value from generic parameter.
        /// </summary>
        /// <param name="implementationType">The service implementation type.</param>
        /// <param name="position">The position.</param>
        /// <param name="serviceType">Type of the service contract.</param>
        /// <returns>The metadata value.</returns>
        public object GetMetadataValueFromGenericParameter(Type implementationType, int position, Type serviceType)
        {
            var typeInfo = implementationType.GetTypeInfo();
            var closedGeneric = typeInfo.ImplementedInterfaces
                .Select(i => i.GetTypeInfo())
                .FirstOrDefault(
                    i =>
                        i.IsGenericType && !i.IsGenericTypeDefinition
                                        && i.GetGenericTypeDefinition() == serviceType);

            if (closedGeneric == null && implementationType.IsConstructedGenericType && implementationType.GetGenericTypeDefinition() == serviceType)
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
        /// Query if 'attributeType' is metadata value attribute.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>
        /// True if metadata value attribute, false if not.
        /// </returns>
        private bool IsMetadataValueAttribute(Type attributeType)
        {
            return MetadataValueTypeInfo.Type.IsAssignableFrom(attributeType);
        }
    }
}