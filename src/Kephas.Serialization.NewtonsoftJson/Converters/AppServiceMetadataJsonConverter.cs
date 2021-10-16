// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceMetadataJsonConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Converters
{
    using System;
    using System.Collections.Generic;

    using Kephas.Data.Formatting;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using Newtonsoft.Json;

    using JsonSerializer = Newtonsoft.Json.JsonSerializer;

    /// <summary>
    /// JSON converter for <see cref="AppServiceMetadata"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="AppServiceMetadata"/> should be processed before expandos, that's why the higher priority.
    /// </remarks>
    [ProcessingPriority(Priority.AboveNormal)]
    public class AppServiceMetadataJsonConverter : DictionaryJsonConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceMetadataJsonConverter"/> class.
        /// </summary>
        /// <param name="typeRegistry">The runtime type registry.</param>
        /// <param name="typeResolver">The type resolver.</param>
        public AppServiceMetadataJsonConverter(IRuntimeTypeRegistry typeRegistry, ITypeResolver typeResolver)
            : base(typeRegistry, typeResolver)
        {
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType) =>
            typeof(AppServiceMetadata).IsAssignableFrom(objectType);

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

            var dictionary = ((IDataFormattable)value).ToData();
            base.WriteJson(writer, dictionary, serializer);
        }

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var dictionary = (IDictionary<string, object?>?)base.ReadJson(
                reader,
                typeof(Dictionary<string, object?>),
                new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase),
                serializer);
            if (existingValue == null)
            {
                return Activator.CreateInstance(objectType, dictionary);
            }

            if (dictionary == null)
            {
                return null;
            }

            var metadata = (AppServiceMetadata)existingValue;
            foreach (var (key, value) in dictionary)
            {
                metadata[key] = value;
            }

            return metadata;
        }
    }
}