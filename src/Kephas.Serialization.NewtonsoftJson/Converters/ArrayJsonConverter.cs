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
    /// JSON converter for array and lists.
    /// </summary>
    /// <remarks>
    /// Dictionaries should be processed first, that's why the low priority.
    /// </remarks>
    [ProcessingPriority(Priority.Low)]
    public class ArrayJsonConverter : JsonConverterBase
    {
        private static readonly MethodInfo WriteJsonMethod =
            ReflectionHelper.GetGenericMethodOf(_ =>
                ArrayJsonConverter.WriteJson<int>(null!, null!, null!));

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
            if (objectType.IsArray || objectType.IsCollection())
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
            var itemType = objectType.IsArray ? objectType.GetElementType() : objectType.TryGetCollectionItemType();
            if (itemType == null)
            {
                throw new SerializationException($"Cannot write values of type {objectType}.");
            }

            var writeJson = WriteJsonMethod.MakeGenericMethod(itemType);
            writeJson.Call(null, writer, value, serializer);
        }

        /// <summary>Writes the JSON representation of the object.</summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
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