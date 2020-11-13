// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoJsonConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Converters
{
    using System;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Serialization.Json.ContractResolvers;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// JSON converter for <see cref="IExpando"/> instances.
    /// </summary>
    public class ExpandoJsonConverter : JsonConverterBase
    {
        private readonly IRuntimeTypeRegistry typeRegistry;
        private readonly ITypeResolver typeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandoJsonConverter"/> class.
        /// </summary>
        /// <param name="typeRegistry">The runtime type registry.</param>
        /// <param name="typeResolver">The type resolver.</param>
        public ExpandoJsonConverter(IRuntimeTypeRegistry typeRegistry, ITypeResolver typeResolver)
            : this(typeRegistry, typeResolver, typeof(IExpando), typeof(Expando))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandoJsonConverter"/> class.
        /// </summary>
        /// <param name="typeRegistry">The runtime type registry.</param>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="interfaceType">The expando interface type.</param>
        /// <param name="defaultImplementationType">The expando default implementation type.</param>
        protected ExpandoJsonConverter(IRuntimeTypeRegistry typeRegistry, ITypeResolver typeResolver, Type interfaceType, Type defaultImplementationType)
        {
            Requires.NotNull(interfaceType, nameof(interfaceType));
            Requires.NotNull(defaultImplementationType, nameof(defaultImplementationType));

            this.typeRegistry = typeRegistry;
            this.typeResolver = typeResolver;

            if (!typeof(IExpando).IsAssignableFrom(interfaceType))
            {
                throw new SerializationException($"The interface type {interfaceType} must be convertible to {typeof(IExpando)}.");
            }

            this.InterfaceType = interfaceType;
            this.DefaultImplementationType = defaultImplementationType;
        }

        /// <summary>
        /// Gets the expando interface type.
        /// </summary>
        protected Type InterfaceType { get; }

        /// <summary>
        /// Gets the expando default implementation type.
        /// </summary>
        protected Type DefaultImplementationType { get; }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType) =>
            this.InterfaceType.IsAssignableFrom(objectType);

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var valueTypeInfo = this.typeRegistry.GetTypeInfo(existingValue?.GetType() ?? objectType);
            if (valueTypeInfo.Type == this.InterfaceType)
            {
                valueTypeInfo = this.typeRegistry.GetTypeInfo(this.DefaultImplementationType);
            }

            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new SerializationException($"Cannot read values of type {valueTypeInfo}. Expected an object at {reader.Path}.");
            }

            var createInstance = existingValue == null;

            // first read type information, if applicable
            reader.Read();
            valueTypeInfo = JsonHelper.EnsureProperValueType(reader, this.typeResolver, this.typeRegistry, valueTypeInfo, ref createInstance);

            if (!this.InterfaceType.IsAssignableFrom(valueTypeInfo.Type))
            {
                throw new SerializationException($"Cannot read values of type {valueTypeInfo}. Path: {reader.Path}.");
            }

            var expando = (IExpando)(createInstance ? valueTypeInfo.CreateInstance() : existingValue)!;

            // then other properties
            var casingResolver = serializer.ContractResolver as ICasingContractResolver;
            var typeProperties = valueTypeInfo.Properties;
            var typeContractProperties = (serializer.ContractResolver.ResolveContract(valueTypeInfo.Type) as JsonDynamicContract)?.Properties;

            while (reader.TokenType != JsonToken.EndObject)
            {
                var propName = (string)reader.Value!;
                var serializedPropName = propName;
                if (casingResolver != null)
                {
                    var pascalPropName = casingResolver.GetDeserializedPropertyName(propName);
                    if (pascalPropName != propName && typeProperties.ContainsKey(pascalPropName))
                    {
                        propName = pascalPropName;
                    }
                }

                reader.Read();

                var propInfo = typeProperties.TryGetValue(propName);
                if (propInfo?.CanWrite ?? true)
                {
                    var propValue = serializer.Deserialize(reader, propInfo?.ValueType.Type ?? typeof(object));
                    propValue = propValue is JToken jtoken ? jtoken.Unwrap() : propValue;

                    expando[propName] = propValue;
                }
                else
                {
                    if (typeContractProperties != null && !typeContractProperties.Contains(serializedPropName))
                    {
                        // ignore property if the serializer ignored it.
                        continue;
                    }

                    var propValue = expando[propName];
                    if (propValue != null && !propInfo.ValueType.Type.IsValueType)
                    {
                        serializer.Populate(reader, propValue);
                    }
                }

                reader.Read();
            }

            return expando;
        }

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

            var valueTypeInfo = this.typeRegistry.GetTypeInfo(value.GetType());
            if (!this.InterfaceType.IsAssignableFrom(valueTypeInfo.Type))
            {
                throw new SerializationException($"Cannot write values of type {valueTypeInfo}. Path: {writer.Path}.");
            }

            writer.WriteStartObject();

            // write type information.
            if (valueTypeInfo.Type != this.DefaultImplementationType
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
            var typeContractProperties = (serializer.ContractResolver.ResolveContract(valueTypeInfo.Type) as JsonDynamicContract)?.Properties;

            var expando = (IExpando)value;
#if NETSTANDARD2_1
            foreach (var (key, propValue) in expando.ToDictionary())
            {
#else
            foreach (var kv in expando.ToDictionary())
            {
                var key = kv.Key;
                var propValue = kv.Value;
#endif
                if (propValue == null && serializer.NullValueHandling == NullValueHandling.Ignore)
                {
                    continue;
                }

                var isClassProperty = typeProperties.ContainsKey(key);
                var propName = casingResolver != null && isClassProperty
                    ? casingResolver.GetSerializedPropertyName(key)
                    : key;

                if (isClassProperty && typeContractProperties != null && !typeContractProperties.Contains(propName))
                {
                    // ignore property if the serializer ignored it.
                    continue;
                }

                writer.WritePropertyName(propName);
                serializer.Serialize(writer, propValue);
            }

            writer.WriteEndObject();
        }
    }
}