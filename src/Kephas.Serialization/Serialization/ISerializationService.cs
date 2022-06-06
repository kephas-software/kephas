// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializationService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for serialization services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Net.Mime;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Contract for singleton serialization services.
    /// </summary>
    [SingletonAppServiceContract]
    public interface ISerializationService
    {
        /// <summary>
        /// Deserializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="textReader">The text reader where from the serialized object should be read.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        Task<object?> DeserializeAsync(
            TextReader textReader,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deserializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        Task<object?> DeserializeAsync(
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deserializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="textReader">The text reader where from the serialized object should be read.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        object? Deserialize(
            TextReader textReader,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return this.DeserializeAsync(textReader, optionsConfig).GetResultNonLocking();
        }

        /// <summary>
        /// Deserializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        object? Deserialize(string serializedObj, Action<ISerializationContext>? optionsConfig = null)
        {
            return this.DeserializeAsync(serializedObj, optionsConfig).GetResultNonLocking();
        }

        /// <summary>
        /// Deserializes the object from the provided format asynchronously.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        public async Task<TRootObject?> DeserializeAsync<TMediaType, TRootObject>(
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
            where TMediaType : IMediaType
        {
            serializedObj = serializedObj ?? throw new ArgumentNullException(nameof(serializedObj));

            void Config(ISerializationContext ctx)
            {
                ctx.MediaType = typeof(TMediaType);
                ctx.RootObjectType = typeof(TRootObject);
                optionsConfig?.Invoke(ctx);
            }

            var result = await this.DeserializeAsync(serializedObj, Config, cancellationToken).PreserveThreadContext();
            return (TRootObject?)result;
        }

        /// <summary>
        /// Deserializes the object from the provided format.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public TRootObject? Deserialize<TMediaType, TRootObject>(
            string? serializedObj,
            Action<ISerializationContext>? optionsConfig = null)
            where TMediaType : IMediaType
        {
            serializedObj = serializedObj ?? throw new ArgumentNullException(nameof(serializedObj));

            void Config(ISerializationContext ctx)
            {
                ctx.MediaType = typeof(TMediaType);
                ctx.RootObjectType = typeof(TRootObject);
                optionsConfig?.Invoke(ctx);
            }

            var result = this.Deserialize(serializedObj, (Action<ISerializationContext>)Config);
            return (TRootObject?)result;
        }

        /// <summary>
        /// Deserializes the object from the provided format asynchronously.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        public Task<object?> DeserializeAsync<TMediaType>(
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
            where TMediaType : IMediaType
        {
            serializedObj = serializedObj ?? throw new ArgumentNullException(nameof(serializedObj));

            void Config(ISerializationContext ctx)
            {
                ctx.MediaType = typeof(TMediaType);
                optionsConfig?.Invoke(ctx);
            }

            return this.DeserializeAsync(serializedObj, (Action<ISerializationContext>)Config, cancellationToken);
        }

        /// <summary>
        /// Deserializes the object from the provided format.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public object? Deserialize<TMediaType>(
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null)
            where TMediaType : IMediaType
        {
            serializedObj = serializedObj ?? throw new ArgumentNullException(nameof(serializedObj));

            void Config(ISerializationContext ctx)
            {
                ctx.MediaType = typeof(TMediaType);
                optionsConfig?.Invoke(ctx);
            }

            return this.Deserialize(serializedObj, Config);
        }

        /// <summary>
        /// Deserializes the object from JSON asynchronously.
        /// </summary>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        public Task<object?> JsonDeserializeAsync(
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return this.DeserializeAsync<JsonMediaType>(serializedObj, optionsConfig, cancellationToken);
        }

        /// <summary>
        /// Deserializes the object from JSON.
        /// </summary>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public object? JsonDeserialize(
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return this.Deserialize<JsonMediaType>(serializedObj, optionsConfig);
        }

        /// <summary>
        /// Deserializes the object from JSON asynchronously.
        /// </summary>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        public Task<TRootObject?> JsonDeserializeAsync<TRootObject>(
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return this.DeserializeAsync<JsonMediaType, TRootObject>(serializedObj, optionsConfig, cancellationToken);
        }

        /// <summary>
        /// Deserializes the object from JSON.
        /// </summary>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public TRootObject? JsonDeserialize<TRootObject>(
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return this.Deserialize<JsonMediaType, TRootObject>(serializedObj, optionsConfig);
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
        Task SerializeAsync(
            object? obj,
            TextWriter textWriter,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Serializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the serialized object.
        /// </returns>
        Task<string> SerializeAsync(
            object? obj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Serializes the object with the provided options.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="textWriter">The text writer where the serialized object should be written.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        void Serialize(
            object? obj,
            TextWriter textWriter,
            Action<ISerializationContext>? optionsConfig = null)
        {
            this.SerializeAsync(obj ?? throw new ArgumentNullException(nameof(obj)), textWriter, optionsConfig).WaitNonLocking();
        }

        /// <summary>
        /// Serializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The serialized object.
        /// </returns>
        string Serialize(object? obj, Action<ISerializationContext>? optionsConfig = null)
        {
            return this.SerializeAsync(obj ?? throw new ArgumentNullException(nameof(obj)), optionsConfig).GetResultNonLocking();
        }

        /// <summary>
        /// Serializes the provided object in the specified format.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a string.
        /// </returns>
        public Task<string> SerializeAsync<TMediaType>(
            object? obj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
            where TMediaType : IMediaType
        {
            void Config(ISerializationContext ctx)
            {
                ctx.MediaType = typeof(TMediaType);
                optionsConfig?.Invoke(ctx);
            }

            return this.SerializeAsync(obj, Config, cancellationToken);
        }

        /// <summary>
        /// Serializes the provided object in the specified format.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The serialized object as a string in the specified format.
        /// </returns>
        public string Serialize<TMediaType>(
            object? obj,
            Action<ISerializationContext>? optionsConfig = null)
            where TMediaType : IMediaType
        {
            void Config(ISerializationContext ctx)
            {
                ctx.MediaType = typeof(TMediaType);
                optionsConfig?.Invoke(ctx);
            }

            return this.Serialize(obj, (Action<ISerializationContext>)Config);
        }

        /// <summary>
        /// Serializes the provided object as JSON asynchronously.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a JSON string.
        /// </returns>
        public Task<string> JsonSerializeAsync(
            object? obj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return this.SerializeAsync<JsonMediaType>(obj, optionsConfig, cancellationToken);
        }

        /// <summary>
        /// Serializes the provided object as JSON.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The serialized object as a JSON string.
        /// </returns>
        public string? JsonSerialize(
            object? obj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return this.Serialize<JsonMediaType>(obj, optionsConfig);
        }
    }
}