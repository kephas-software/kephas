// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the JSON helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Reflection;

namespace Kephas.Serialization.Json
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Runtime;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A JSON helper.
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Gets the property name containing the type.
        /// </summary>
        internal static readonly string TypePropertyName = "$type";

        private const string JsonOptionsKey = "__JsonOptions";

        /// <summary>
        /// Configures the serialization/deserialization with the provided options.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="options">The custom options.</param>
        /// <returns>The <paramref name="context"/>.</returns>
        public static ISerializationContext Configure(
            this ISerializationContext context,
            Action<JsonSerializerSettings> options)
        {
            Requires.NotNull(context, nameof(context));

            context[JsonOptionsKey] = options;

            return context;
        }

        /// <summary>
        /// Configures the settings with the options stored in the provided context.
        /// </summary>
        /// <param name="settings">The settings to be configured.</param>
        /// <param name="context">The serialization context.</param>
        /// <returns>The configured <paramref name="settings"/>.</returns>
        public static JsonSerializerSettings Configure(
            this JsonSerializerSettings settings,
            ISerializationContext? context)
        {
            Requires.NotNull(settings, nameof(settings));

            if (context == null)
            {
                return settings;
            }

            if (context.Indent.HasValue)
            {
                settings.Formatting = context.Indent.Value ? Formatting.Indented : Formatting.None;
            }

            if (context.IncludeTypeInfo.HasValue)
            {
                settings.TypeNameHandling = context.IncludeTypeInfo.Value
                    ? TypeNameHandling.Objects
                    : TypeNameHandling.None;
            }

            if (context.IncludeNullValues.HasValue)
            {
                settings.NullValueHandling = context.IncludeNullValues.Value
                    ? NullValueHandling.Include
                    : NullValueHandling.Ignore;
            }

            var options = context[JsonOptionsKey] as Action<JsonSerializerSettings>;
            options?.Invoke(settings);

            return settings;
        }

        /// <summary>
        /// A JToken extension method that unwraps the given value.
        /// </summary>
        /// <param name="value">The value to act on.</param>
        /// <returns>
        /// An object.
        /// </returns>
        internal static object? Unwrap(this JToken value)
        {
            return value switch
            {
                JObject jobj => new JObjectDictionary(jobj),
                JValue jval => jval.Value,
                JArray jarr => new JObjectList(jarr),
                _ => value
            };
        }

        /// <summary>
        /// An object extension method that wraps the given object.
        /// </summary>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>
        /// A JToken.
        /// </returns>
        internal static JToken Wrap(this object? obj)
        {
            return obj is JToken token ? token : JToken.FromObject(obj);
        }

        /// <summary>
        /// Ensures that the inferred value type matches the one set in the reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="valueTypeInfo">[ref] The inferred value type.</param>
        /// <param name="createInstance">[ref] Indicates whether a new instance should be created.</param>
        /// <returns>The proper value type information.</returns>
        internal static IRuntimeTypeInfo EnsureProperValueType(
            JsonReader reader,
            ITypeResolver typeResolver,
            IRuntimeTypeRegistry typeRegistry,
            IRuntimeTypeInfo valueTypeInfo,
            ref bool createInstance)
        {
            var propName = (string)reader.Value;
            if (propName == JsonHelper.TypePropertyName)
            {
                reader.Read();
                var valueTypeName = reader.Value?.ToString();
                var valueType = typeResolver.ResolveType(valueTypeName)!;
                if (valueType != valueTypeInfo.Type)
                {
                    valueTypeInfo = typeRegistry.GetTypeInfo(valueType);
                    createInstance = true;
                }

                // advance the reader
                reader.Read();
            }

            return valueTypeInfo;
        }
    }
}
