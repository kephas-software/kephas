﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   JSON serializer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Net.Mime;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// JSON serializer.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class JsonSerializer : ISerializer<JsonMediaType>
#if NETSTANDARD2_0
        , ISyncSerializer
#endif
    {
        /// <summary>
        /// The settings provider.
        /// </summary>
        private readonly IJsonSerializerSettingsProvider settingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer"/>
        /// class.
        /// </summary>
        /// <param name="settingsProvider">The settings provider.</param>
        public JsonSerializer(IJsonSerializerSettingsProvider? settingsProvider = null)
        {
            this.settingsProvider = settingsProvider ?? DefaultJsonSerializerSettingsProvider.Instance;
        }

        /// <summary>
        /// Serializes the provided object asynchronously.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> used to write the object content.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public async Task SerializeAsync(
            object? obj,
            TextWriter textWriter,
            ISerializationContext? context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(textWriter, nameof(textWriter));

            await Task.Yield();

            cancellationToken.ThrowIfCancellationRequested();

            this.Serialize(obj, textWriter, context);
        }

        /// <summary>
        /// Serializes the provided object asynchronously.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the serialized object.
        /// </returns>
        public async Task<string?> SerializeAsync(
            object? obj,
            ISerializationContext? context = null,
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            cancellationToken.ThrowIfCancellationRequested();

            return this.Serialize(obj, context);
        }

        /// <summary>
        /// Deserialize an object asynchronously.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        public async Task<object?> DeserializeAsync(
            TextReader textReader,
            ISerializationContext? context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(textReader, nameof(textReader));

            await Task.Yield();

            cancellationToken.ThrowIfCancellationRequested();

            return this.Deserialize(textReader, context);
        }

        /// <summary>
        /// Deserializes an object asynchronously.
        /// </summary>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        public async Task<object?> DeserializeAsync(
            string? serializedObj,
            ISerializationContext? context = null,
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            cancellationToken.ThrowIfCancellationRequested();

            return this.Deserialize(serializedObj, context);
        }

        /// <summary>
        /// Serializes the provided object.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> used to write the object content.</param>
        /// <param name="context">The context containing serialization options.</param>
        public void Serialize(object? obj, TextWriter textWriter, ISerializationContext? context = null)
        {
            Requires.NotNull(textWriter, nameof(textWriter));

            var settings = this.GetJsonSerializerSettings(context);
            var serializer = Newtonsoft.Json.JsonSerializer.Create(settings);
            serializer.Serialize(textWriter, obj);
        }

        /// <summary>
        /// Serializes the provided object.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The serialized object.
        /// </returns>
        public string? Serialize(object? obj, ISerializationContext? context = null)
        {
            if (obj == null)
            {
                return null;
            }

            var settings = this.GetJsonSerializerSettings(context);
            var serializer = Newtonsoft.Json.JsonSerializer.Create(settings);
            using var writer = new StringWriter();
            serializer.Serialize(writer, obj);
            var stringBuilder = writer.GetStringBuilder();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public object Deserialize(TextReader textReader, ISerializationContext? context = null)
        {
            Requires.NotNull(textReader, nameof(textReader));

            var settings = this.GetJsonSerializerSettings(context);
            var serializer = Newtonsoft.Json.JsonSerializer.Create(settings);

            using var jsonReader = new JsonTextReader(textReader);
            var result = context?.RootObjectFactory?.Invoke();
            var rootObjectType = context?.RootObjectType ?? typeof(object);
            if (result != null)
            {
                serializer.Populate(jsonReader, result);
            }
            else
            {
                result = serializer.Deserialize(jsonReader, rootObjectType);
            }

            return this.PostDeserialize(result, rootObjectType);
        }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public object? Deserialize(string? serializedObj, ISerializationContext? context = null)
        {
            if (serializedObj == null)
            {
                return null;
            }

            using var reader = new StringReader(serializedObj);
            return this.Deserialize(reader, context);
        }

        private JsonSerializerSettings GetJsonSerializerSettings(ISerializationContext? context)
        {
            var settings = this.settingsProvider.GetJsonSerializerSettings();
            if (context != null)
            {
                settings.Configure(context);
            }

            return settings;
        }

        private object PostDeserialize(object obj, Type rootObjectType)
        {
            if (rootObjectType != typeof(object))
            {
                return obj;
            }

            return obj is JToken jtoken ? jtoken.Unwrap() : obj;
        }
    }
}