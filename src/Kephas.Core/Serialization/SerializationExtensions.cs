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
    using Kephas.Resources;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Extension methods for serialization convenience.
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// Serializes the object with the provided options.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="textWriter">The text writer where the serialized object should be written.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        public static void Serialize(
            this ISerializationService serializationService,
            object obj,
            TextWriter textWriter,
            Action<ISerializationContext> optionsConfig = null)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (obj == null)
            {
                return;
            }

            if (serializationService is ISyncSerializationService syncService)
            {
                syncService.Serialize(obj, textWriter, optionsConfig);
            }
            else
            {
                serializationService.SerializeAsync(obj, textWriter, optionsConfig).WaitNonLocking();
            }
        }

        /// <summary>
        /// Serializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The serialized object.
        /// </returns>
        public static string Serialize(
            this ISerializationService serializationService,
            object obj,
            Action<ISerializationContext> optionsConfig = null)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (obj == null)
            {
                return null;
            }

            if (serializationService is ISyncSerializationService syncService)
            {
                return syncService.Serialize(obj, optionsConfig);
            }
            else
            {
                return serializationService.SerializeAsync(obj, optionsConfig).GetResultNonLocking();
            }
        }

        /// <summary>
        /// Deserializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="textReader">The text reader where from the serialized object should be read.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The serialized object.
        /// </returns>
        public static object Deserialize(
            this ISerializationService serializationService,
            TextReader textReader,
            Action<ISerializationContext> optionsConfig = null)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (serializationService is ISyncSerializationService syncService)
            {
                return syncService.Deserialize(textReader, optionsConfig);
            }
            else
            {
                return serializationService.DeserializeAsync(textReader, optionsConfig).GetResultNonLocking();
            }
        }

        /// <summary>
        /// Deserializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <returns>
        /// The serialized object.
        /// </returns>
        public static object Deserialize(
            this ISerializationService serializationService,
            string obj,
            Action<ISerializationContext> optionsConfig = null)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (serializationService is ISyncSerializationService syncService)
            {
                return syncService.Deserialize(obj, optionsConfig);
            }
            else
            {
                return serializationService.DeserializeAsync(obj, optionsConfig).GetResultNonLocking();
            }
        }

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
            string serializedObj,
            Action<ISerializationContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
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

            var result = await serializationService.DeserializeAsync(serializedObj, config, cancellationToken).PreserveThreadContext();
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
            string serializedObj,
            Action<ISerializationContext> optionsConfig = null)
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
        public static Task<object> DeserializeAsync<TMediaType>(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
            where TMediaType : IMediaType
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (serializedObj == null)
            {
                return Task.FromResult((object)null);
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
        public static object Deserialize<TMediaType>(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext> optionsConfig = null)
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
        public static Task<object> JsonDeserializeAsync(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext> optionsConfig = null,
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
        public static object JsonDeserialize(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext> optionsConfig = null)
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
            Action<ISerializationContext> optionsConfig = null,
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
            Action<ISerializationContext> optionsConfig = null)
        {
            return Deserialize<JsonMediaType, TRootObject>(serializationService, serializedObj, optionsConfig);
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
        public static Task<object> XmlDeserializeAsync(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext> optionsConfig = null,
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
        public static object XmlDeserialize(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext> optionsConfig = null)
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
            Action<ISerializationContext> optionsConfig = null,
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
            Action<ISerializationContext> optionsConfig = null)
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
        public static Task<string> SerializeAsync<TMediaType>(
            this ISerializationService serializationService,
            object obj,
            Action<ISerializationContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
            where TMediaType : IMediaType
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (obj == null)
            {
                return Task.FromResult((string)null);
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
        public static string Serialize<TMediaType>(
            this ISerializationService serializationService,
            object obj,
            Action<ISerializationContext> optionsConfig = null)
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
        public static Task<string> JsonSerializeAsync(
            this ISerializationService serializationService,
            object obj,
            Action<ISerializationContext> optionsConfig = null,
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
        public static string JsonSerialize(
            this ISerializationService serializationService,
            object obj,
            Action<ISerializationContext> optionsConfig = null)
        {
            return Serialize<JsonMediaType>(serializationService, obj, optionsConfig);
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
        /// <example>
        /// .
        /// </example>
        public static Task<string> XmlSerializeAsync(
            this ISerializationService serializationService,
            object obj,
            Action<ISerializationContext> optionsConfig = null,
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
        public static string XmlSerialize(
            this ISerializationService serializationService,
            object obj,
            Action<ISerializationContext> optionsConfig = null)
        {
            return Serialize<XmlMediaType>(serializationService, obj, optionsConfig);
        }

        /// <summary>
        /// Serializes the provided object.
        /// </summary>
        /// <param name="serializer">The serializer to act on.</param>
        /// <param name="obj">The object.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> used to write the object content.</param>
        /// <param name="context">The context containing serialization options.</param>
        public static void Serialize(
            this ISerializer serializer,
            object obj,
            TextWriter textWriter,
            ISerializationContext context)
        {
            Requires.NotNull(serializer, nameof(serializer));

            if (serializer is ISyncSerializer syncSerializer)
            {
                syncSerializer.Serialize(obj, textWriter, context);
            }
            else
            {
                serializer.SerializeAsync(obj, textWriter, context).WaitNonLocking();
            }
        }

        /// <summary>
        /// Serializes the provided object.
        /// </summary>
        /// <param name="serializer">The serializer to act on.</param>
        /// <param name="obj">The object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The serialized object.
        /// </returns>
        public static string Serialize(
            this ISerializer serializer,
            object obj,
            ISerializationContext context)
        {
            Requires.NotNull(serializer, nameof(serializer));

            if (serializer is ISyncSerializer syncSerializer)
            {
                return syncSerializer.Serialize(obj, context);
            }
            else
            {
                return serializer.SerializeAsync(obj, context).GetResultNonLocking();
            }
        }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="serializer">The serializer to act on.</param>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static object Deserialize(
            this ISerializer serializer,
            TextReader textReader,
            ISerializationContext context)
        {
            Requires.NotNull(serializer, nameof(serializer));

            if (serializer is ISyncSerializer syncSerializer)
            {
                return syncSerializer.Deserialize(textReader, context);
            }

            return serializer.DeserializeAsync(textReader, context).GetResultNonLocking();
        }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="serializer">The serializer to act on.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="context">Optional. The context containing serialization options.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static object Deserialize(
            this ISerializer serializer,
            string serializedObject,
            ISerializationContext context)
        {
            Requires.NotNull(serializer, nameof(serializer));

            if (serializer is ISyncSerializer syncSerializer)
            {
                return syncSerializer.Deserialize(serializedObject, context);
            }

            return serializer.DeserializeAsync(serializedObject, context).GetResultNonLocking();
        }
    }
}