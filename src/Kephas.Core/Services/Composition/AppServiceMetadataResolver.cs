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
        /// <summary>
        /// The attribute suffix.
        /// </summary>
        private const string AttributeSuffix = "Attribute";

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
        /// Gets the metadata value properties which should be retrieved from the attribute.
        /// </summary>
        /// <param name="attributeTypeInfo">Information describing the attribute type.</param>
        /// <returns>
        /// The metadata properties.
        /// </returns>
        public IDictionary<string, IPropertyInfo> GetMetadataValueProperties(IRuntimeTypeInfo attributeTypeInfo)
        {
            const string MetadataValuePropertiesName = "__MetadataValueProperties";
            if (attributeTypeInfo[MetadataValuePropertiesName] is IDictionary<string, IPropertyInfo> metadataValueProperties)
            {
                return metadataValueProperties;
            }

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

            return metadataValueProperties;
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
        public object GetMetadataValueFromAttribute(Type partType, Type attributeType, IPropertyInfo property)
        {
            var attr =
                partType.GetTypeInfo()
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
    }
}