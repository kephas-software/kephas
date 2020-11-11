// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectJsonConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Converters
{
    using System;
    using System.Collections.Generic;

    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using Newtonsoft.Json;

    /// <summary>
    /// JSON converter for untyped objects.
    /// </summary>
    [ProcessingPriority(Priority.Lowest)]
    public class ObjectJsonConverter : JsonConverterBase
    {
        private readonly IRuntimeTypeRegistry typeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectJsonConverter"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        public ObjectJsonConverter(IRuntimeTypeRegistry typeRegistry)
        {
            this.typeRegistry = typeRegistry;
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
            return objectType == typeof(object);
        }

        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var valueType = value.GetType();
            if (valueType == typeof(object))
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
            }
            else
            {
                serializer.Serialize(writer, value, valueType);
            }
        }

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var existingValueType = existingValue?.GetType();
            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    if (existingValue == null || existingValueType!.IsArray || !existingValueType.IsCollection())
                    {
                        existingValue = objectType == null || objectType == typeof(object) ? new List<object?>() : this.typeRegistry.GetTypeInfo(objectType).CreateInstance();
                    }

                    serializer.Populate(reader, existingValue);
                    return existingValue;
                case JsonToken.StartObject:
                    if (existingValue == null)
                    {
                        existingValue = objectType == null || objectType == typeof(object) ? new Dictionary<string, object?>() : this.typeRegistry.GetTypeInfo(objectType).CreateInstance();
                    }

                    serializer.Populate(reader, existingValue);
                    return existingValue;
                default:
                    return serializer.Deserialize(reader);
            }
        }
    }
}