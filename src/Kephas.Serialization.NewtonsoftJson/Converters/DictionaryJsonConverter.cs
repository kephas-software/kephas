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
    using Kephas.Runtime;
    using Kephas.Serialization.Json.ContractResolvers;
    using Kephas.Services;
    using Newtonsoft.Json;

    /// <summary>
    /// JSON converter for dictionaries.
    /// </summary>
    /// <remarks>
    /// Dictionaries should be processed before collections, that's why the higher priority.
    /// </remarks>
    [ProcessingPriority(Priority.AboveNormal)]
    public class DictionaryJsonConverter : JsonConverterBase
    {
        private static readonly MethodInfo WriteJsonMethod =
            ReflectionHelper.GetGenericMethodOf(_ =>
                WriteJson<int>(null!, null!, null!));

        private static readonly MethodInfo ReadJsonMethod =
            ReflectionHelper.GetGenericMethodOf(_ =>
                ReadJson<int>(null!, null!, null!, null!));

        private readonly IRuntimeTypeRegistry typeRegistry;
        private readonly ITypeResolver typeResolver;
        private readonly Type dictionaryInterfaceType = typeof(IDictionary<string, object?>);

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryJsonConverter"/> class.
        /// </summary>
        /// <param name="typeRegistry">The runtime type registry.</param>
        /// <param name="typeResolver">The type resolver.</param>
        public DictionaryJsonConverter(IRuntimeTypeRegistry typeRegistry, ITypeResolver typeResolver)
        {
            this.typeRegistry = typeRegistry;
            this.typeResolver = typeResolver;
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
            if (this.dictionaryInterfaceType.IsAssignableFrom(objectType))
            {
                return true;
            }

            var keyItem = objectType.TryGetDictionaryKeyItemType(objectType);
            return keyItem?.keyType == typeof(string);
        }

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var valueTypeInfo = this.typeRegistry.GetTypeInfo(existingValue?.GetType() ?? objectType);
            if (valueTypeInfo.Type == this.dictionaryInterfaceType)
            {
                valueTypeInfo = this.typeRegistry.GetTypeInfo(typeof(Dictionary<string, object?>));
            }
            else if (valueTypeInfo.Type.IsConstructedGenericOf(typeof(IDictionary<,>)))
            {
                var genericArgs = valueTypeInfo.Type.TryGetDictionaryKeyItemType()!;
                valueTypeInfo = this.typeRegistry.GetTypeInfo(
                    typeof(Dictionary<,>).MakeGenericType(genericArgs.Value.keyType, genericArgs.Value.itemType));
            }

            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new SerializationException($"Cannot read values of type {valueTypeInfo}. Expected an object at {reader.Path}.");
            }

            var createInstance = existingValue == null;

            // check the first property, if it is the type name metadata.
            reader.Read();
            var propName = (string)reader.Value;
            if (propName == JsonHelper.TypePropertyName)
            {
                reader.Read();
                var valueTypeName = reader.Value?.ToString();
                var valueType = this.typeResolver.ResolveType(valueTypeName)!;
                if (valueType != valueTypeInfo.Type)
                {
                    valueTypeInfo = this.typeRegistry.GetTypeInfo(valueType);
                    createInstance = true;
                }
            }

            var keyItem = valueTypeInfo.Type.TryGetDictionaryKeyItemType();
            if (keyItem == null)
            {
                throw new SerializationException($"Cannot read values of type {valueTypeInfo}. Path: {reader.Path}.");
            }

            var value = (createInstance ? valueTypeInfo.CreateInstance() : existingValue)!;

            var readJson = ReadJsonMethod.MakeGenericMethod(keyItem.Value.itemType);
            return readJson.Call(null, reader, value, valueTypeInfo, serializer);
        }

        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var objectType = value.GetType();
            var valueTypeInfo = this.typeRegistry.GetTypeInfo(objectType);
            var keyItem = objectType.TryGetDictionaryKeyItemType();
            if (keyItem == null)
            {
                throw new SerializationException($"Cannot write values of type {valueTypeInfo}. Path: {writer.Path}.");
            }

            var writeJson = WriteJsonMethod.MakeGenericMethod(keyItem.Value.itemType);
            writeJson.Call(null, writer, value, serializer);
        }

        private static object ReadJson<TItem>(JsonReader reader, IDictionary<string, TItem> value, IRuntimeTypeInfo valueTypeInfo, JsonSerializer serializer)
        {
            var casingResolver = serializer.ContractResolver as ICasingContractResolver;
            var typeProperties = valueTypeInfo.Properties;
            while (reader.TokenType != JsonToken.EndObject)
            {
                var propName = (string)reader.Value;

                reader.Read();
                var propValue = serializer.Deserialize(reader, typeof(TItem));
                if (casingResolver != null)
                {
                    var pascalPropName = casingResolver.GetDeserializedPropertyName(propName);
                    if (pascalPropName != propName && typeProperties.ContainsKey(pascalPropName))
                    {
                        propName = pascalPropName;
                    }
                }

                value[propName] = (TItem)propValue;
                reader.Read();
            }

            return value;
        }

        private static JsonWriter WriteJson<TItem>(JsonWriter writer, IDictionary<string, TItem> value, JsonSerializer serializer)
        {
            var casingResolver = serializer.ContractResolver as ICasingContractResolver;
            writer.WriteStartObject();
#if NETSTANDARD2_1
            foreach (var (key, item) in value)
            {
                var propName = casingResolver?.GetSerializedPropertyName(key) ?? key;
                writer.WritePropertyName(propName);
                serializer.Serialize(writer, item);
            }
#else
            foreach (var kv in value)
            {
                var propName = kv.Key;
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