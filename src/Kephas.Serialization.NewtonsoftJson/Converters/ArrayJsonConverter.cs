// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayJsonConverter.cs" company="Kephas Software SRL">
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
    using Kephas.Services;
    using Newtonsoft.Json;

    /// <summary>
    /// JSON converter for arrays.
    /// </summary>
    /// <remarks>
    /// Arrays should be processed before collections, that's why the higher priority.
    /// </remarks>
    [ProcessingPriority(Priority.AboveNormal)]
    public class ArrayJsonConverter : JsonConverterBase
    {
        private static readonly MethodInfo WriteJsonMethod =
            ReflectionHelper.GetGenericMethodOf(_ =>
                WriteJson<int>(null!, null!, null!));

        private static readonly MethodInfo ReadJsonMethod =
            ReflectionHelper.GetGenericMethodOf(_ =>
                ReadJson<int>(null!, null!));

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType) =>
            objectType.IsArray && objectType.GetArrayRank() == 1;

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object? ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            objectType ??= existingValue?.GetType();
            var itemType = objectType?.GetElementType();
            if (itemType == null)
            {
                throw new SerializationException($"Cannot read values of type {objectType}. Reader path: {reader.Path}.");
            }

            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new SerializationException($"Cannot read values of type {objectType}. Expected an object at {reader.Path}.");
            }

            var readJson = ReadJsonMethod.MakeGenericMethod(itemType);
            return readJson.Call(null, reader, serializer);
        }

        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var objectType = value.GetType();
            var itemType = objectType.GetElementType();
            if (itemType == null)
            {
                throw new SerializationException($"Cannot write values of type {objectType}. Writer path: {writer.Path}.");
            }

            var writeJson = WriteJsonMethod.MakeGenericMethod(itemType);
            writeJson.Call(null, writer, value, serializer);
        }

        private static TItem[] ReadJson<TItem>(JsonReader reader, JsonSerializer serializer)
        {
            var list = new List<TItem>();
            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
            {
                var item = serializer.Deserialize(reader, typeof(TItem));
                list.Add((TItem)item);
            }

            return list.ToArray();
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