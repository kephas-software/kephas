// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoJsonConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Converters
{
    using System;

    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Serialization.Json.ContractResolvers;
    using Newtonsoft.Json;

    /// <summary>
    /// JSON converter for <see cref="IExpando"/> instances.
    /// </summary>
    public class ExpandoJsonConverter : JsonConverterBase
    {
        private readonly IRuntimeTypeRegistry typeRegistry;
        private readonly ITypeResolver typeResolver;
        private readonly Type expandoInterfaceType = typeof(IExpando);

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandoJsonConverter"/> class.
        /// </summary>
        /// <param name="typeRegistry">The runtime type registry.</param>
        /// <param name="typeResolver">The type resolver.</param>
        public ExpandoJsonConverter(IRuntimeTypeRegistry typeRegistry, ITypeResolver typeResolver)
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
        public override bool CanConvert(Type objectType) =>
            this.expandoInterfaceType.IsAssignableFrom(objectType);

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var valueTypeInfo = this.typeRegistry.GetTypeInfo(existingValue?.GetType() ?? objectType);
            if (valueTypeInfo.Type == this.expandoInterfaceType)
            {
                valueTypeInfo = this.typeRegistry.GetTypeInfo(typeof(Expando));
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

            if (!this.expandoInterfaceType.IsAssignableFrom(valueTypeInfo.Type))
            {
                throw new SerializationException($"Cannot read values of type {valueTypeInfo}. Path: {reader.Path}.");
            }

            var expando = (IExpando)(createInstance ? valueTypeInfo.CreateInstance() : existingValue)!;

            var casingResolver = serializer.ContractResolver as ICasingContractResolver;
            var typeProperties = valueTypeInfo.Properties;
            while (reader.TokenType != JsonToken.EndObject)
            {
                propName = (string)reader.Value!;

                reader.Read();
                var propValue = reader.Value;
                if (casingResolver != null)
                {
                    var pascalPropName = casingResolver.GetDeserializedPropertyName(propName);
                    if (pascalPropName != propName && typeProperties.ContainsKey(pascalPropName))
                    {
                        propName = pascalPropName;
                    }
                }

                expando[propName] = propValue;
                reader.Read();
            }

            return expando;
        }

        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var valueTypeInfo = this.typeRegistry.GetTypeInfo(value.GetType());
            if (!(value is IExpando expando))
            {
                throw new SerializationException($"Cannot write values of type {valueTypeInfo}.");
            }

            writer.WriteStartObject();

            // write type information.
            if (valueTypeInfo.Type != typeof(Expando)
                && (serializer.TypeNameHandling.HasFlag(TypeNameHandling.Objects)
                    || serializer.TypeNameHandling.HasFlag(TypeNameHandling.Auto)))
            {
                var typeName = serializer.TypeNameAssemblyFormatHandling == TypeNameAssemblyFormatHandling.Simple
                    ? valueTypeInfo.FullName
                    : valueTypeInfo.QualifiedFullName;
                writer.WritePropertyName(JsonHelper.TypePropertyName);
                writer.WriteValue(typeName);
            }

            // write other properties
            var casingResolver = serializer.ContractResolver as ICasingContractResolver;
            var typeProperties = valueTypeInfo.Properties;
#if NETSTANDARD2_1
            foreach (var (key, item) in expando.ToDictionary())
            {
                var propName = casingResolver != null && typeProperties.ContainsKey(key)
                    ? casingResolver.GetSerializedPropertyName(key)
                    : key;
                writer.WritePropertyName(propName);
                serializer.Serialize(writer, item);
            }
#else
            foreach (var kv in expando.ToDictionary())
            {
                var propName = kv.Key;
                propName = casingResolver != null && typeProperties.ContainsKey(propName)
                    ? casingResolver.GetSerializedPropertyName(propName)
                    : propName;
                writer.WritePropertyName(propName);
                serializer.Serialize(writer, kv.Value);
            }
#endif

            writer.WriteEndObject();
        }
    }
}