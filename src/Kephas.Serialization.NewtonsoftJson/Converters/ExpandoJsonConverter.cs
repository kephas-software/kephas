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
    /// JSON converter for <see cref="IExpandoBase"/> based instances.
    /// </summary>
    public class ExpandoJsonConverter : JsonConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandoJsonConverter"/> class.
        /// </summary>
        /// <param name="typeRegistry">The runtime type registry.</param>
        /// <param name="typeResolver">The type resolver.</param>
        public ExpandoJsonConverter(IRuntimeTypeRegistry typeRegistry, ITypeResolver typeResolver)
            : this(typeRegistry, typeResolver, typeof(IExpandoBase), typeof(Expando))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandoJsonConverter"/> class.
        /// </summary>
        /// <param name="typeRegistry">The runtime type registry.</param>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="expandoBaseType">The expando base type.</param>
        /// <param name="defaultImplementationType">The expando default implementation type.</param>
        protected ExpandoJsonConverter(IRuntimeTypeRegistry typeRegistry, ITypeResolver typeResolver, Type expandoBaseType, Type defaultImplementationType)
        {
            Requires.NotNull(expandoBaseType, nameof(expandoBaseType));
            Requires.NotNull(defaultImplementationType, nameof(defaultImplementationType));

            this.TypeRegistry = typeRegistry;
            this.TypeResolver = typeResolver;

            if (!typeof(IExpandoBase).IsAssignableFrom(expandoBaseType))
            {
                throw new SerializationException($"The expando base type {expandoBaseType} must be convertible to {typeof(IExpandoBase)}.");
            }

            this.ExpandoBaseType = expandoBaseType;
            this.DefaultImplementationType = defaultImplementationType;
        }

        /// <summary>
        /// Gets the expando base type.
        /// </summary>
        protected Type ExpandoBaseType { get; }

        /// <summary>
        /// Gets the expando default implementation type.
        /// </summary>
        protected Type DefaultImplementationType { get; }

        /// <summary>
        /// Gets the runtime type registry.
        /// </summary>
        protected IRuntimeTypeRegistry TypeRegistry { get; }

        /// <summary>
        /// Gets the type resolver.
        /// </summary>
        protected ITypeResolver TypeResolver { get; }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType) =>
            this.ExpandoBaseType.IsAssignableFrom(objectType);

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

            var valueTypeInfo = this.TypeRegistry.GetTypeInfo(existingValue?.GetType() ?? objectType);
            if (valueTypeInfo.Type == this.ExpandoBaseType || valueTypeInfo.Type.IsInterface)
            {
                valueTypeInfo = this.TypeRegistry.GetTypeInfo(this.DefaultImplementationType);
            }

            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new SerializationException($"Cannot read values of type {valueTypeInfo}. Expected an object at {reader.Path}.");
            }

            var createInstance = existingValue == null;

            // first read type information, if applicable
            reader.Read();
            valueTypeInfo = JsonHelper.EnsureProperValueType(reader, this.TypeResolver, this.TypeRegistry, valueTypeInfo, ref createInstance);

            if (!this.ExpandoBaseType.IsAssignableFrom(valueTypeInfo.Type))
            {
                throw new SerializationException($"Cannot read values of type {valueTypeInfo}. Path: {reader.Path}.");
            }

            var expandoCollector = this.CreateExpandoCollector(valueTypeInfo, existingValue);

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
                if (this.CanWriteProperty(propInfo, existingValue))
                {
                    var propValue = serializer.Deserialize(reader, propInfo?.ValueType.Type ?? typeof(object));
                    propValue = propValue is JToken jtoken ? jtoken.Unwrap() : propValue;

                    expandoCollector[propName] = propValue;
                }
                else
                {
                    if (typeContractProperties != null && !typeContractProperties.Contains(serializedPropName))
                    {
                        // ignore property if the serializer ignored it.
                        continue;
                    }

                    var propValue = expandoCollector[propName];
                    if (propValue != null && !propInfo.ValueType.Type.IsValueType)
                    {
                        serializer.Populate(reader, propValue);
                    }
                }

                reader.Read();
            }

            return this.GetReadReturnValue(valueTypeInfo, expandoCollector, existingValue);
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

            var valueTypeInfo = this.TypeRegistry.GetTypeInfo(value.GetType());
            if (!this.ExpandoBaseType.IsAssignableFrom(valueTypeInfo.Type))
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

            var valueDictionary = value.ToDictionary();
#if NETSTANDARD2_0
            foreach (var kv in valueDictionary)
            {
                var key = kv.Key;
                var propValue = kv.Value;
#else
            foreach (var (key, propValue) in valueDictionary)
            {
#endif
                if (propValue == null && serializer.NullValueHandling == NullValueHandling.Ignore)
                {
                    continue;
                }

                var isClassProperty = typeProperties.TryGetValue(key, out var typeProperty);
                var propName = casingResolver != null && isClassProperty
                    ? casingResolver.GetSerializedPropertyName(key)
                    : key;

                if (isClassProperty &&
                    ((typeContractProperties != null && !typeContractProperties.Contains(propName))
                    || typeProperty.ExcludeFromSerialization()))
                {
                    // ignore property if the serializer ignored it or if explicitly removed from serialization.
                    continue;
                }

                writer.WritePropertyName(propName);
                serializer.Serialize(writer, propValue);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Gets the return value of the <see cref="ReadJson"/> operation.
        /// </summary>
        /// <param name="expandoTypeInfo">The return value type information.</param>
        /// <param name="expandoCollector">The expando value collecting the properties.</param>
        /// <param name="existingValue">The existing value.</param>
        /// <returns>The read operation's return value.</returns>
        protected virtual object? GetReadReturnValue(IRuntimeTypeInfo expandoTypeInfo, IExpandoBase expandoCollector, object? existingValue)
        {
            return expandoCollector;
        }

        /// <summary>
        /// Creates the expando value which should collect the JSON values.
        /// </summary>
        /// <param name="expandoTypeInfo">The type information of the target expando value.</param>
        /// <param name="existingValue">The existing value.</param>
        /// <returns>The newly created expando collector.</returns>
        protected virtual IExpandoBase CreateExpandoCollector(IRuntimeTypeInfo expandoTypeInfo, object? existingValue)
        {
            var createInstance = existingValue == null;
            var expando = (IExpandoBase)(createInstance ? expandoTypeInfo.CreateInstance() : existingValue)!;
            return expando;
        }

        /// <summary>
        /// Gets a value indicating whether the property can be written.
        /// </summary>
        /// <param name="propInfo">The property information.</param>
        /// <param name="existingValue">The existing value.</param>
        /// <returns>A value indicating whether the property can be written.</returns>
        protected virtual bool CanWriteProperty(IRuntimePropertyInfo propInfo, object? existingValue)
        {
            return propInfo?.CanWrite ?? true;
        }
    }
}