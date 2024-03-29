﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CamelCaseContractResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.ContractResolvers
{
    using System;
    using System.Collections.Generic;

    using Kephas.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Optimized contract resolver for camel case properties.
    /// </summary>
    public class CamelCaseContractResolver : CamelCasePropertyNamesContractResolver, ICasingContractResolver
    {
        private readonly IEnumerable<JsonConverter> converters;

        /// <summary>
        /// Initializes a new instance of the <see cref="CamelCaseContractResolver"/> class.
        /// </summary>
        /// <param name="converters">The converters.</param>
        public CamelCaseContractResolver(IEnumerable<JsonConverter> converters)
        {
            this.converters = converters;
        }

        /// <summary>
        /// Gets the deserialized property name.
        /// </summary>
        /// <param name="propertyName">The serialized property name.</param>
        /// <returns>The deserialized property name.</returns>
        public string GetDeserializedPropertyName(string propertyName)
            => propertyName.ToPascalCase();

        /// <summary>
        /// Gets the serialized property name.
        /// </summary>
        /// <param name="propertyName">The deserialized property name.</param>
        /// <returns>The serialized property name.</returns>
        public string GetSerializedPropertyName(string propertyName)
            => propertyName.ToCamelCase();

        /// <summary>
        /// Determines which contract type is created for the given type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Serialization.JsonContract" /> for the given type.</returns>
        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);
            foreach (var converter in this.converters)
            {
                if (converter.CanConvert(objectType) && converter.CanWrite && converter.CanRead)
                {
                    contract.Converter = converter;
                    break;
                }
            }

            return contract;
        }
    }
}