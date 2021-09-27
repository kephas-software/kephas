// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceInfoJsonConverter.cs" company="Kephas Software SRL">
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
    using Kephas.Services;
    using Kephas.Services.Reflection;
    using Newtonsoft.Json;

    /// <summary>
    /// JSON converter for <see cref="IAppServiceInfo"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="AppServiceInfo"/> should be processed before expandos, that's why the higher priority.
    /// </remarks>
    [ProcessingPriority(Priority.AboveNormal)]
    public class AppServiceInfoJsonConverter : JsonConverterBase
    {
        private readonly IRuntimeTypeRegistry typeRegistry;
        private readonly ITypeResolver typeResolver;
        private readonly Type interfaceType = typeof(IAppServiceInfo);

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceInfoJsonConverter"/> class.
        /// </summary>
        /// <param name="typeRegistry">The runtime type registry.</param>
        /// <param name="typeResolver">The type resolver.</param>
        public AppServiceInfoJsonConverter(IRuntimeTypeRegistry typeRegistry, ITypeResolver typeResolver)
        {
            this.typeRegistry = typeRegistry;
            this.typeResolver = typeResolver;
        }

        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object? value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var appServiceInfo = (IAppServiceInfo)value;

            writer.WriteStartObject();

            writer.WritePropertyName(nameof(IAppServiceInfo.ContractType));
            serializer.Serialize(writer, appServiceInfo.ContractType);

            if (appServiceInfo.InstanceType != null)
            {
                writer.WritePropertyName(nameof(IAppServiceInfo.InstanceType));
                serializer.Serialize(writer, appServiceInfo.InstanceType);
            }

            writer.WritePropertyName(nameof(IAppServiceInfo.Lifetime));
            serializer.Serialize(writer, appServiceInfo.Lifetime);

            writer.WritePropertyName(nameof(IAppServiceInfo.AllowMultiple));
            serializer.Serialize(writer, appServiceInfo.AllowMultiple);

            writer.WritePropertyName(nameof(IAppServiceInfo.AsOpenGeneric));
            serializer.Serialize(writer, appServiceInfo.AsOpenGeneric);

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
            if (reader.TokenType == JsonToken.Null || reader.TokenType == JsonToken.Undefined)
            {
                return null;
            }

            var valueTypeInfo = this.typeRegistry.GetTypeInfo(existingValue?.GetType() ?? objectType);
            if (valueTypeInfo.Type == this.interfaceType)
            {
                valueTypeInfo = this.typeRegistry.GetTypeInfo(typeof(AppServiceInfo));
            }

            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new SerializationException($"Cannot read values of type {valueTypeInfo}. Expected an object at {reader.Path}.");
            }

            var createInstance = existingValue == null;

            // check the first property, if it is the type name metadata.
            reader.Read();
            valueTypeInfo = JsonHelper.EnsureProperValueType(reader, this.typeResolver, this.typeRegistry, valueTypeInfo, ref createInstance);

            var expando = new Expando();
            while (reader.TokenType != JsonToken.EndObject)
            {
                var propName = ((string?)reader.Value)?.ToPascalCase();
                var propInfo = valueTypeInfo.Properties[propName];

                reader.Read();
                var value = serializer.Deserialize(reader, propInfo.ValueType.Type);
                expando[propName] = value;

                reader.Read();
            }

            // advance the reader past the EndObject.
            reader.Read();

            var contractType = (Type?)expando[nameof(IAppServiceInfo.ContractType)];
            var instanceType = (Type?)expando[nameof(IAppServiceInfo.InstanceType)];
            var lifetime = (AppServiceLifetime?)expando[nameof(IAppServiceInfo.Lifetime)];
            var allowMultiple = (bool?)expando[nameof(IAppServiceInfo.AllowMultiple)];
            var asOpenGeneric = (bool?)expando[nameof(IAppServiceInfo.AsOpenGeneric)];

            if (createInstance)
            {
                if (contractType == null)
                {
                    throw new SerializationException($"Must specify the contract type when deserializing a value of type {valueTypeInfo}.");
                }

                var appServiceInfo = instanceType == null
                    ? new AppServiceInfo(contractType, lifetime ?? AppServiceLifetime.Singleton, asOpenGeneric ?? false)
                    : new AppServiceInfo(contractType, instanceType, lifetime ?? AppServiceLifetime.Singleton, asOpenGeneric ?? false);

                if (allowMultiple != null)
                {
                    appServiceInfo.AllowMultiple = allowMultiple.Value;
                }

                existingValue = appServiceInfo;
            }

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
            return this.interfaceType.IsAssignableFrom(objectType);
        }
    }
}