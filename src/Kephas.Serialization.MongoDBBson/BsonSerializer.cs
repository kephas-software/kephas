// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonSerializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the bson serializer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Bson
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Net.Mime;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using MongoDB.Bson.IO;

    using MongoBsonSerializer = MongoDB.Bson.Serialization.BsonSerializer;

    /// <summary>
    /// A BSON serializer.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class BsonSerializer : ISerializer<BsonMediaType>
    {
        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public object? Deserialize(TextReader textReader, ISerializationContext context)
        {
            if (context.RootObjectType == null)
            {
                throw new InvalidOperationException($"The root object type must be specified in the context.");
            }

            using var jsonReader = new JsonReader(textReader);
            return MongoBsonSerializer.Deserialize(jsonReader, context.RootObjectType, context.DeserializeConfigurator());
        }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public object? Deserialize(string? serializedObj, ISerializationContext context)
        {
            if (serializedObj == null)
            {
                return null;
            }

            if (context.RootObjectType == null)
            {
                throw new InvalidOperationException($"The root object type must be specified in the context.");
            }

            using var jsonReader = new JsonReader(serializedObj);
            return MongoBsonSerializer.Deserialize(jsonReader, context.RootObjectType, context.DeserializeConfigurator());
        }

        /// <summary>
        /// Deserializes an object asynchronously.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        public async Task<object?> DeserializeAsync(TextReader textReader, ISerializationContext context, CancellationToken cancellationToken = default)
        {
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
        public async Task<object?> DeserializeAsync(string? serializedObj, ISerializationContext context, CancellationToken cancellationToken = default)
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
        public void Serialize(object? obj, TextWriter textWriter, ISerializationContext context)
        {
            if (obj == null)
            {
                return;
            }

            using var jsonWriter = new JsonWriter(textWriter);
            MongoBsonSerializer.Serialize(jsonWriter, context.RootObjectType ?? obj.GetType(), obj, context.SerializeConfigurator());
        }

        /// <summary>
        /// Serializes the provided object.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The serialized object.
        /// </returns>
        public string? Serialize(object? obj, ISerializationContext context)
        {
            if (obj == null)
            {
                return null;
            }

            using var sbWriter = new StringWriter();
            using var jsonWriter = new JsonWriter(sbWriter);
            MongoBsonSerializer.Serialize(jsonWriter, context.RootObjectType ?? obj.GetType(), obj, context.SerializeConfigurator());
            return sbWriter.ToString();
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
        public async Task SerializeAsync(object? obj, TextWriter textWriter, ISerializationContext context, CancellationToken cancellationToken = default)
        {
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
        public async Task<string?> SerializeAsync(object? obj, ISerializationContext context, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            cancellationToken.ThrowIfCancellationRequested();

            return this.Serialize(obj, context);
        }
    }
}
