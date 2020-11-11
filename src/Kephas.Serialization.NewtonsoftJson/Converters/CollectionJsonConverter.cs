// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionJsonConverter.cs" company="Kephas Software SRL">
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
    using Kephas.Services;
    using Newtonsoft.Json;

    /// <summary>
    /// JSON converter for collections.
    /// </summary>
    /// <remarks>
    /// Generic collections should be processed after specific collections, that's why the lower priority.
    /// </remarks>
    [ProcessingPriority(Priority.BelowNormal)]
    public class CollectionJsonConverter : JsonConverterBase
    {
        private static readonly MethodInfo WriteJsonMethod =
            ReflectionHelper.GetGenericMethodOf(_ =>
                WriteJson<int>(null!, null!, null!));

        private static readonly MethodInfo ReadJsonMethod =
            ReflectionHelper.GetGenericMethodOf(_ =>
                ReadJson<int>(null!, null!, null!));

        private readonly IRuntimeTypeRegistry typeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionJsonConverter"/> class.
        /// </summary>
        /// <param name="typeRegistry">The runtime type registry.</param>
        public CollectionJsonConverter(IRuntimeTypeRegistry typeRegistry)
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
        public override bool CanConvert(Type objectType) => objectType.IsReadOnlyCollection();

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var valueType = existingValue?.GetType() ?? objectType;
            if (valueType.IsConstructedGenericOf(typeof(IReadOnlyCollection<>))
                || valueType.IsConstructedGenericOf(typeof(ICollection<>))
                || valueType.IsConstructedGenericOf(typeof(IList<>)))
            {
                valueType = typeof(List<>).MakeGenericType(valueType.TryGetCollectionItemType());
            }

            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new SerializationException($"Cannot read values of type {valueType}. Expected an object at {reader.Path}.");
            }

            var valueTypeInfo = this.typeRegistry.GetTypeInfo(valueType);
            var itemType = valueType.TryGetCollectionItemType();
            if (itemType == null)
            {
                throw new SerializationException($"Cannot read values of type {valueTypeInfo}. Reader path: {reader.Path}.");
            }

            var createInstance = existingValue == null || !valueType.IsCollection();
            var value = (createInstance ? valueTypeInfo.CreateInstance() : existingValue)!;

            var readJson = ReadJsonMethod.MakeGenericMethod(itemType);
            return readJson.Call(null, reader, value, serializer);
        }

        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var objectType = value.GetType();
            var itemType = objectType.TryGetCollectionItemType();
            if (itemType == null)
            {
                throw new SerializationException($"Cannot write values of type {objectType}.");
            }

            var writeJson = WriteJsonMethod.MakeGenericMethod(itemType);
            writeJson.Call(null, writer, value, serializer);
        }

        private static ICollection<TItem> ReadJson<TItem>(JsonReader reader, ICollection<TItem> value, JsonSerializer serializer)
        {
            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
            {
                var item = serializer.Deserialize(reader, typeof(TItem));
                value.Add((TItem)item);
            }

            return value;
        }

        private static JsonWriter WriteJson<TItem>(JsonWriter writer, IEnumerable<TItem> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (var item in value)
            {
                serializer.Serialize(writer, item);
            }

            writer.WriteEndArray();
            return writer;
        }
    }
}