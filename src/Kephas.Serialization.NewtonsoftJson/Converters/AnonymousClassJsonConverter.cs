// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnonymousClassJsonConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Converters
{
    using System;

    using Kephas.Runtime;
    using Kephas.Serialization.Json.ContractResolvers;
    using Newtonsoft.Json;

    /// <summary>
    /// JSON converter class for anonymous types.
    /// </summary>
    public class AnonymousClassJsonConverter : JsonConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousClassJsonConverter"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        public AnonymousClassJsonConverter(IRuntimeTypeRegistry typeRegistry)
        {
            this.TypeRegistry = typeRegistry;
        }

        /// <summary>
        /// Gets the type registry.
        /// </summary>
        protected IRuntimeTypeRegistry TypeRegistry { get; }

        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();

            var valueTypeInfo = this.TypeRegistry.GetTypeInfo(value.GetType());
            var casingResolver = serializer.ContractResolver as ICasingContractResolver;
            foreach (var propInfo in valueTypeInfo.Properties.Values)
            {
                var key = propInfo.Name;
                var propName = casingResolver != null
                    ? casingResolver.GetSerializedPropertyName(key)
                    : key;
                var propValue = propInfo.GetValue(value);

                writer.WritePropertyName(propName);
                serializer.Serialize(writer, propValue);
            }

            writer.WriteEndObject();
        }

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            // TODO - should read all properties and put them in the constructor.
            return existingValue;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            var name = objectType.Name;
            return objectType.IsClass 
                   && (name.IndexOf('<') >= 0 // C#
                        || name.IndexOf('$') >= 0); // VB
        }
    }
}