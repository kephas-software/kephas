// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultSerializationService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A default serialization service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Net.Mime;
    using Kephas.Resources;
    using Kephas.Serialization.Composition;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default serialization service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultSerializationService : ISerializationService
#if NETSTANDARD2_1
#else
        , ISyncSerializationService
#endif
    {
        private readonly IDictionary<Type, IExportFactory<ISerializer, SerializerMetadata>> serializerFactories;
        private readonly IContextFactory contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSerializationService"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="serializerFactories">The serializer factories.</param>
        public DefaultSerializationService(IContextFactory contextFactory, ICollection<IExportFactory<ISerializer, SerializerMetadata>> serializerFactories)
        {
            Requires.NotNull(contextFactory, nameof(contextFactory));
            Requires.NotNull(serializerFactories, nameof(serializerFactories));

            this.serializerFactories = serializerFactories.ToPrioritizedDictionary(f => f.Metadata.MediaType);
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// Serializes the object with the provided options.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="textWriter">The text writer where the serialized object should be written.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public async Task SerializeAsync(
            object? obj,
            TextWriter textWriter,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            if (obj == null)
            {
                return;
            }

            using var context = this.CreateSerializationContext(optionsConfig);
            var serializer = this.GetSerializer(context);
            await serializer.SerializeAsync(obj, textWriter, context, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Serializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the serialized object.
        /// </returns>
        public async Task<string?> SerializeAsync(
            object? obj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            if (obj == null)
            {
                return null;
            }

            using var context = this.CreateSerializationContext(optionsConfig);
            var serializer = this.GetSerializer(context);
            return await serializer.SerializeAsync(obj, context, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Serializes the object with the provided options.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="textWriter">The text writer where the serialized object should be written.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        public void Serialize(
            object? obj,
            TextWriter textWriter,
            Action<ISerializationContext>? optionsConfig = null)
        {
            if (obj == null)
            {
                return;
            }

            using var context = this.CreateSerializationContext(optionsConfig);
            var serializer = this.GetSerializer(context);
            serializer.Serialize(obj, textWriter, context);
        }

        /// <summary>
        /// Serializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The serialized object.
        /// </returns>
        public string? Serialize(object? obj, Action<ISerializationContext>? optionsConfig = null)
        {
            if (obj == null)
            {
                return null;
            }

            using var context = this.CreateSerializationContext(optionsConfig);
            var serializer = this.GetSerializer(context);
            return serializer.Serialize(obj, context);
        }

        /// <summary>
        /// Deserializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="textReader">The text reader where from the serialized object should be read.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        public async Task<object?> DeserializeAsync(
            TextReader textReader,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            using var context = this.CreateSerializationContext(optionsConfig);
            var serializer = this.GetSerializer(context);
            return await serializer.DeserializeAsync(textReader, context, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Deserializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        public async Task<object?> DeserializeAsync(
            string? serializedObj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            if (serializedObj == null)
            {
                return null;
            }

            using var context = this.CreateSerializationContext(optionsConfig);
            var serializer = this.GetSerializer(context);
            return await serializer.DeserializeAsync(serializedObj, context, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Deserializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="textReader">The text reader where from the serialized object should be read.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public object? Deserialize(
            TextReader textReader,
            Action<ISerializationContext>? optionsConfig = null)
        {
            using var context = this.CreateSerializationContext(optionsConfig);
            var serializer = this.GetSerializer(context);
            return serializer.Deserialize(textReader, context);
        }

        /// <summary>
        /// Deserializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public object? Deserialize(
            string? serializedObj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            if (serializedObj == null)
            {
                return null;
            }

            using var context = this.CreateSerializationContext(optionsConfig);
            var serializer = this.GetSerializer(context);
            return serializer.Deserialize(serializedObj, context);
        }

        /// <summary>
        /// Creates serialization context.
        /// </summary>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The new serialization context.
        /// </returns>
        protected virtual ISerializationContext CreateSerializationContext(Action<ISerializationContext>? optionsConfig = null)
        {
            var context = this.contextFactory.CreateContext<SerializationContext>(this);
            optionsConfig?.Invoke(context);
            if (context.MediaType == null)
            {
                context.MediaType = typeof(JsonMediaType);
            }

            return context;
        }

        /// <summary>
        /// Gets a serializer for the provided context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The serializer.
        /// </returns>
        protected virtual ISerializer GetSerializer(ISerializationContext context)
        {
            context ??= this.contextFactory.CreateContext<SerializationContext>(this, typeof(JsonMediaType));
            var mediaType = context.MediaType ?? typeof(JsonMediaType);

            var serializer = this.serializerFactories.TryGetValue(mediaType);
            if (serializer == null)
            {
                throw new KeyNotFoundException(string.Format(Strings.DefaultSerializationService_SerializerNotFound_Exception, mediaType));
            }

            return serializer.CreateExport().Value;
        }
    }
}