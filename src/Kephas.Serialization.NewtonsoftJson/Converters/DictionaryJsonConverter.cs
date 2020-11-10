// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryJsonConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Reflection;
    using Kephas.Serialization.Json.ContractResolvers;
    using Newtonsoft.Json;

    /// <summary>
    /// JSON converter for dictionaries.
    /// </summary>
    public class DictionaryJsonConverter : JsonConverterBase
    {
        private static readonly MethodInfo WriteJsonMethod =
            ReflectionHelper.GetGenericMethodOf(_ =>
                WriteJson<int, int>(null!, null!, null!));

        private readonly Type dictionaryInterfaceType = typeof(IDictionary<string, object?>);

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON.
        /// </summary>
        /// <value><c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON; otherwise, <c>false</c>.</value>
        public override bool CanRead => false;

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            if (this.dictionaryInterfaceType.IsAssignableFrom(objectType))
            {
                return true;
            }

            var keyItem = objectType.TryGetDictionaryKeyItemType(objectType);
            if (keyItem?.keyType == typeof(string))
            {
                return true;
            }

            return false;
        }

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var objectType = value.GetType();
            var keyItem = objectType.TryGetDictionaryKeyItemType();
            if (keyItem == null)
            {
                throw new SerializationException($"Cannot write values of type {objectType}.");
            }

            var writeJson = WriteJsonMethod.MakeGenericMethod(keyItem.Value.keyType, keyItem.Value.itemType);
            writeJson.Call(null, writer, value, serializer);
        }

        private static JsonWriter WriteJson<TKey, TItem>(JsonWriter writer, IDictionary<TKey, TItem> value, JsonSerializer serializer)
        {
            var casingResolver = serializer.ContractResolver as ICasingContractResolver;
            writer.WriteStartObject();
#if NETSTANDARD2_1
            foreach (var (key, item) in value)
            {
                var propName = key.ToString();
                propName = casingResolver?.GetSerializedPropertyName(propName) ?? propName;
                writer.WritePropertyName(propName);
                serializer.Serialize(writer, item);
            }
#else
            foreach (var kv in value)
            {
                var propName = kv.Key.ToString();
                propName = casingResolver?.GetSerializedPropertyName(propName) ?? propName;
                writer.WritePropertyName(propName);
                serializer.Serialize(writer, kv.Value);
            }
#endif
            writer.WriteEndObject();
            return writer;
        }
    }
}