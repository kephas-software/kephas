// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectableConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Converters;

using Kephas.Collections;
using Kephas.Services;
using Kephas.Reflection;
using Kephas.Runtime;
using Kephas.Serialization.Json.ContractResolvers;
using Kephas.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Converter for instances implementing
/// </summary>
[ProcessingPriority(Priority.Low)]
public class InjectableConverter : JsonConverterBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InjectableConverter"/> class.
    /// </summary>
    /// <param name="typeRegistry">The runtime type registry.</param>
    /// <param name="typeResolver">The type resolver.</param>
    /// <param name="injectableFactory">The injectable factory.</param>
    public InjectableConverter(IRuntimeTypeRegistry typeRegistry, ITypeResolver typeResolver, IInjectableFactory injectableFactory)
    {
        this.InjectableFactory = injectableFactory ?? throw new ArgumentNullException(nameof(injectableFactory));
        this.TypeRegistry = typeRegistry ?? throw new ArgumentNullException(nameof(typeRegistry));
        this.TypeResolver = typeResolver ?? throw new ArgumentNullException(nameof(typeResolver));
    }

    /// <summary>
    /// Gets the injectable factory.
    /// </summary>
    protected IInjectableFactory InjectableFactory { get; }

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
        objectType.IsInjectable();

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

        if (reader.TokenType != JsonToken.StartObject)
        {
            throw new SerializationException($"Cannot read values of type {valueTypeInfo}. Expected an object at {reader.Path}.");
        }

        var createInstance = existingValue == null;

        // first read type information, if applicable
        reader.Read();
        valueTypeInfo = JsonHelper.EnsureProperValueType(reader, this.TypeResolver, this.TypeRegistry, valueTypeInfo, ref createInstance);

        if (!valueTypeInfo.Type.IsInjectable())
        {
            throw new SerializationException($"Cannot read values of type {valueTypeInfo}. Path: {reader.Path}.");
        }

        var injectable = this.CreateInjectable(valueTypeInfo, existingValue);

        // then other properties
        var casingResolver = serializer.ContractResolver as ICasingContractResolver;
        var typeProperties = valueTypeInfo.Properties;
        var typeContractProperties = serializer.ContractResolver.ResolveContract(valueTypeInfo.Type).GetProperties()!;

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
            if (propInfo == null)
            {
                // TODO - do not ignore if instructed to throw
                // property not found, ignore at this time.
            }
            else if (this.CanWriteProperty(propInfo, existingValue))
            {
                var propValue = serializer.Deserialize(reader, propInfo?.ValueType.Type ?? typeof(object));
                propValue = propValue is JToken jtoken ? jtoken.Unwrap() : propValue;

                propInfo!.SetValue(injectable, propValue);
            }
            else
            {
                if (typeContractProperties != null && !typeContractProperties.Contains(serializedPropName))
                {
                    // ignore property if the serializer ignored it.
                    continue;
                }

                var propValue = propInfo.GetValue(injectable);
                if (propValue != null && !propInfo.ValueType.Type.IsValueType)
                {
                    serializer.Populate(reader, propValue);
                }
            }

            reader.Read();
        }

        return injectable;
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
        if (!valueTypeInfo.Type.IsInjectable())
        {
            throw new SerializationException($"Cannot write values of type {valueTypeInfo}. Path: {writer.Path}.");
        }

        writer.WriteStartObject();

        // write type information.
        if (serializer.TypeNameHandling.HasFlag(TypeNameHandling.Objects)
            || serializer.TypeNameHandling.HasFlag(TypeNameHandling.Auto))
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
        var typeContractProperties = serializer.ContractResolver.ResolveContract(valueTypeInfo.Type).GetProperties()!;

        foreach (var (key, typeProperty) in typeProperties)
        {
            var propName = casingResolver != null
                ? casingResolver.GetSerializedPropertyName(key)
                : key;

            if ((typeContractProperties != null && !typeContractProperties.Contains(propName))
                 || typeProperty.ExcludeFromSerialization())
            {
                // ignore property if the serializer ignored it or if explicitly removed from serialization.
                continue;
            }

            var propValue = typeProperty.GetValue(value);

            if (propValue == null && serializer.NullValueHandling == NullValueHandling.Ignore)
            {
                continue;
            }

            writer.WritePropertyName(propName);
            serializer.Serialize(writer, propValue);
        }

        writer.WriteEndObject();
    }

    /// <summary>
    /// Creates the injectable value which should collect the JSON values.
    /// </summary>
    /// <param name="injectableTypeInfo">The type information of the target injectable value.</param>
    /// <param name="existingValue">The existing value.</param>
    /// <returns>The newly created injectable.</returns>
    protected virtual IInjectable CreateInjectable(IRuntimeTypeInfo injectableTypeInfo, object? existingValue)
    {
        var createInstance = existingValue == null;
        var injectable = (IInjectable)(createInstance ? this.InjectableFactory.Create(injectableTypeInfo.Type) : existingValue)!;
        return injectable;
    }

    /// <summary>
    /// Gets a value indicating whether the property can be written.
    /// </summary>
    /// <param name="propInfo">The property information.</param>
    /// <param name="existingValue">The existing value.</param>
    /// <returns>A value indicating whether the property can be written.</returns>
    protected virtual bool CanWriteProperty(IRuntimePropertyInfo propInfo, object? existingValue)
    {
        return propInfo.CanWrite;
    }
}