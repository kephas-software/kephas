// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSpanJsonConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A time span JSON converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Converters
{
    using System;
    using System.Xml;

    using Newtonsoft.Json;

    using JsonSerializer = Newtonsoft.Json.JsonSerializer;

    /// <summary>
    /// A time span JSON converter.
    /// </summary>
    public class TimeSpanJsonConverter : JsonConverterBase
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param><param name="value">The value.</param><param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var str = ((TimeSpan)value).ToString("c");
            serializer.Serialize(writer, str);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param><param name="objectType">Type of the object.</param><param name="existingValue">The existing value of object being read.</param><param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object? ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var timeSpanString = serializer.Deserialize<string>(reader);
            if (TimeSpan.TryParse(timeSpanString, out var timeSpan))
            {
                return timeSpan;
            }

            return XmlConvert.ToTimeSpan(timeSpanString);
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(TimeSpan) || objectType == typeof(TimeSpan?);
    }
}