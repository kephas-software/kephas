// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the serialization extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Net.Mime;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Extension methods for serialization convenience.
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// Deserializes the object from the provided format asynchronously.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        public static async Task<TRootObject> DeserializeAsync<TMediaType, TRootObject>(
            this ISerializationService serializationService,
            string? serializedObj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
            where TMediaType : IMediaType
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (serializedObj == null)
            {
                return default;
            }

            void Config(ISerializationContext ctx)
            {
                ctx.MediaType = typeof(TMediaType);
                ctx.RootObjectType = typeof(TRootObject);
                optionsConfig?.Invoke(ctx);
            }

            var result = await serializationService.DeserializeAsync(serializedObj, Config, cancellationToken).PreserveThreadContext();
            return (TRootObject)result;
        }

        /// <summary>
        /// Deserializes the object from the provided format.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static TRootObject Deserialize<TMediaType, TRootObject>(
            this ISerializationService serializationService,
            string? serializedObj,
            Action<ISerializationContext>? optionsConfig = null)
            where TMediaType : IMediaType
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (serializedObj == null)
            {
                return default;
            }

            Action<ISerializationContext> config = ctx =>
            {
                ctx.MediaType = typeof(TMediaType);
                ctx.RootObjectType = typeof(TRootObject);
                optionsConfig?.Invoke(ctx);
            };

            var result = serializationService.Deserialize(serializedObj, config);
            return (TRootObject)result;
        }

        /// <summary>
        /// Deserializes the object from the provided format asynchronously.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        public static Task<object?> DeserializeAsync<TMediaType>(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
            where TMediaType : IMediaType
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (serializedObj == null)
            {
                return Task.FromResult((object?)null);
            }

            Action<ISerializationContext> config = ctx =>
            {
                ctx.MediaType = typeof(TMediaType);
                optionsConfig?.Invoke(ctx);
            };

            return serializationService.DeserializeAsync(serializedObj, config, cancellationToken);
        }

        /// <summary>
        /// Deserializes the object from the provided format.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static object? Deserialize<TMediaType>(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null)
            where TMediaType : IMediaType
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (serializedObj == null)
            {
                return null;
            }

            Action<ISerializationContext> config = ctx =>
            {
                ctx.MediaType = typeof(TMediaType);
                optionsConfig?.Invoke(ctx);
            };

            return serializationService.Deserialize(serializedObj, config);
        }

        /// <summary>
        /// Deserializes the object from JSON asynchronously.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        public static Task<object?> JsonDeserializeAsync(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return DeserializeAsync<JsonMediaType>(serializationService, serializedObj, optionsConfig, cancellationToken);
        }

        /// <summary>
        /// Deserializes the object from JSON.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static object? JsonDeserialize(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return Deserialize<JsonMediaType>(serializationService, serializedObj, optionsConfig);
        }

        /// <summary>
        /// Deserializes the object from JSON asynchronously.
        /// </summary>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        public static Task<TRootObject> JsonDeserializeAsync<TRootObject>(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return DeserializeAsync<JsonMediaType, TRootObject>(serializationService, serializedObj, optionsConfig, cancellationToken);
        }

        /// <summary>
        /// Deserializes the object from JSON.
        /// </summary>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static TRootObject JsonDeserialize<TRootObject>(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return Deserialize<JsonMediaType, TRootObject>(serializationService, serializedObj, optionsConfig);
        }


        /// <summary>
        /// Deserializes the object from BSON asynchronously.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        public static Task<object?> BsonDeserializeAsync(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return DeserializeAsync<BsonMediaType>(serializationService, serializedObj, optionsConfig, cancellationToken);
        }

        /// <summary>
        /// Deserializes the object from BSON.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static object? BsonDeserialize(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return Deserialize<BsonMediaType>(serializationService, serializedObj, optionsConfig);
        }

        /// <summary>
        /// Deserializes the object from BSON asynchronously.
        /// </summary>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        public static Task<TRootObject> BsonDeserializeAsync<TRootObject>(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return DeserializeAsync<BsonMediaType, TRootObject>(serializationService, serializedObj, optionsConfig, cancellationToken);
        }

        /// <summary>
        /// Deserializes the object from BSON.
        /// </summary>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static TRootObject BsonDeserialize<TRootObject>(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return Deserialize<BsonMediaType, TRootObject>(serializationService, serializedObj, optionsConfig);
        }

        /// <summary>
        /// Deserializes the object from XML asynchronously.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        public static Task<object?> XmlDeserializeAsync(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return DeserializeAsync<XmlMediaType>(serializationService, serializedObj, optionsConfig, cancellationToken);
        }

        /// <summary>
        /// Deserializes the object from XML.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static object? XmlDeserialize(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return Deserialize<XmlMediaType>(serializationService, serializedObj, optionsConfig);
        }

        /// <summary>
        /// Deserializes the object from XML asynchronously.
        /// </summary>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        public static Task<TRootObject> XmlDeserializeAsync<TRootObject>(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return DeserializeAsync<XmlMediaType, TRootObject>(serializationService, serializedObj, optionsConfig, cancellationToken);
        }

        /// <summary>
        /// Deserializes the object from XML.
        /// </summary>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static TRootObject XmlDeserialize<TRootObject>(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return Deserialize<XmlMediaType, TRootObject>(serializationService, serializedObj, optionsConfig);
        }

        /// <summary>
        /// Serializes the provided object in the specified format.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a string.
        /// </returns>
        public static Task<string?> SerializeAsync<TMediaType>(
            this ISerializationService serializationService,
            object? obj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
            where TMediaType : IMediaType
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (obj == null)
            {
                return Task.FromResult((string?)null);
            }

            Action<ISerializationContext> config = ctx =>
            {
                ctx.MediaType = typeof(TMediaType);
                optionsConfig?.Invoke(ctx);
            };

            return serializationService.SerializeAsync(obj, config, cancellationToken);
        }

        /// <summary>
        /// Serializes the provided object in the specified format.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="obj">The object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The serialized object as a string in the specified format.
        /// </returns>
        public static string? Serialize<TMediaType>(
            this ISerializationService serializationService,
            object? obj,
            Action<ISerializationContext>? optionsConfig = null)
            where TMediaType : IMediaType
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (obj == null)
            {
                return null;
            }

            Action<ISerializationContext> config = ctx =>
            {
                ctx.MediaType = typeof(TMediaType);
                optionsConfig?.Invoke(ctx);
            };

            return serializationService.Serialize(obj, config);
        }

        /// <summary>
        /// Serializes the provided object as JSON asynchronously.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="obj">The object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a JSON string.
        /// </returns>
        public static Task<string?> JsonSerializeAsync(
            this ISerializationService serializationService,
            object? obj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return SerializeAsync<JsonMediaType>(serializationService, obj, optionsConfig, cancellationToken);
        }

        /// <summary>
        /// Serializes the provided object as JSON.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="obj">The object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The serialized object as a JSON string.
        /// </returns>
        public static string? JsonSerialize(
            this ISerializationService serializationService,
            object? obj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return Serialize<JsonMediaType>(serializationService, obj, optionsConfig);
        }

        /// <summary>
        /// Serializes the provided object as BSON asynchronously.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="obj">The object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a BSON string.
        /// </returns>
        public static Task<string?> BsonSerializeAsync(
            this ISerializationService serializationService,
            object? obj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return SerializeAsync<BsonMediaType>(serializationService, obj, optionsConfig, cancellationToken);
        }

        /// <summary>
        /// Serializes the provided object as BSON.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="obj">The object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The serialized object as a BSON string.
        /// </returns>
        public static string? BsonSerialize(
            this ISerializationService serializationService,
            object? obj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return Serialize<BsonMediaType>(serializationService, obj, optionsConfig);
        }

        /// <summary>
        /// Serializes the provided object as XML asynchronously.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a XML string.
        /// </returns>
        public static Task<string?> XmlSerializeAsync(
            this ISerializationService serializationService,
            object? obj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return SerializeAsync<XmlMediaType>(serializationService, obj, optionsConfig, cancellationToken);
        }

        /// <summary>
        /// Serializes the provided object as XML.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="obj">The object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The serialized object as a XML string.
        /// </returns>
        public static string? XmlSerialize(
            this ISerializationService serializationService,
            object? obj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return Serialize<XmlMediaType>(serializationService, obj, optionsConfig);
        }
    }
}