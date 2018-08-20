// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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

    using Kephas.Diagnostics.Contracts;
    using Kephas.Net.Mime;
    using Kephas.Resources;
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
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        public static async Task<TRootObject> DeserializeAsync<TMediaType, TRootObject>(
            this ISerializationService serializationService,
            string serializedObj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
            where TMediaType : IMediaType
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (serializedObj == null)
            {
                return default;
            }

            context = serializationService.CreateOrUpdateSerializationContext<TMediaType>(context);
            context.RootObjectType = typeof(TRootObject);

            var serializer = serializationService.GetSerializer(context);
            var result = await serializer.DeserializeAsync(serializedObj, context, cancellationToken).PreserveThreadContext();
            return (TRootObject)result;
        }

        /// <summary>
        /// Deserializes the object from the provided format.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static TRootObject Deserialize<TMediaType, TRootObject>(
            this ISerializationService serializationService,
            string serializedObj,
            ISerializationContext context = null)
            where TMediaType : IMediaType
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (serializedObj == null)
            {
                return default;
            }

            context = serializationService.CreateOrUpdateSerializationContext<TMediaType>(context);
            context.RootObjectType = typeof(TRootObject);

            var serializer = serializationService.GetSerializer(context);
            var result = serializer.Deserialize(serializedObj, context);
            return (TRootObject)result;
        }

        /// <summary>
        /// Deserializes the object from the provided format asynchronously.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        public static Task<object> DeserializeAsync<TMediaType>(
            this ISerializationService serializationService,
            string serializedObj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
            where TMediaType : IMediaType
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (serializedObj == null)
            {
                return Task.FromResult((object)null);
            }

            context = serializationService.CreateOrUpdateSerializationContext<TMediaType>(context);

            var serializer = serializationService.GetSerializer(context);
            return serializer.DeserializeAsync(serializedObj, context, cancellationToken);
        }

        /// <summary>
        /// Deserializes the object from the provided format.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static object Deserialize<TMediaType>(
            this ISerializationService serializationService,
            string serializedObj,
            ISerializationContext context = null)
            where TMediaType : IMediaType
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (serializedObj == null)
            {
                return Task.FromResult((object)null);
            }

            context = serializationService.CreateOrUpdateSerializationContext<TMediaType>(context);

            var serializer = serializationService.GetSerializer(context);
            return serializer.Deserialize(serializedObj, context);
        }

        /// <summary>
        /// Deserializes the object from JSON asynchronously.
        /// </summary>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        public static Task<object> JsonDeserializeAsync(
            this ISerializationService serializationService,
            string serializedObj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            return DeserializeAsync<JsonMediaType>(serializationService, serializedObj, context, cancellationToken);
        }

        /// <summary>
        /// Deserializes the object from JSON.
        /// </summary>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static object JsonDeserialize(
            this ISerializationService serializationService,
            string serializedObj,
            ISerializationContext context = null)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            return Deserialize<JsonMediaType>(serializationService, serializedObj, context);
        }

        /// <summary>
        /// Deserializes the object from JSON asynchronously.
        /// </summary>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        public static Task<TRootObject> JsonDeserializeAsync<TRootObject>(
            this ISerializationService serializationService,
            string serializedObj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            return DeserializeAsync<JsonMediaType, TRootObject>(serializationService, serializedObj, context, cancellationToken);
        }

        /// <summary>
        /// Deserializes the object from JSON.
        /// </summary>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static TRootObject JsonDeserialize<TRootObject>(
            this ISerializationService serializationService,
            string serializedObj,
            ISerializationContext context = null)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            return Deserialize<JsonMediaType, TRootObject>(serializationService, serializedObj, context);
        }

        /// <summary>
        /// Deserializes the object from XML asynchronously.
        /// </summary>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        public static Task<object> XmlDeserializeAsync(
            this ISerializationService serializationService,
            string serializedObj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            return DeserializeAsync<XmlMediaType>(serializationService, serializedObj, context, cancellationToken);
        }

        /// <summary>
        /// Deserializes the object from XML.
        /// </summary>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static object XmlDeserialize(
            this ISerializationService serializationService,
            string serializedObj,
            ISerializationContext context = null)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            return Deserialize<XmlMediaType>(serializationService, serializedObj, context);
        }

        /// <summary>
        /// Deserializes the object from XML asynchronously.
        /// </summary>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        public static Task<TRootObject> XmlDeserializeAsync<TRootObject>(
            this ISerializationService serializationService,
            string serializedObj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            return DeserializeAsync<XmlMediaType, TRootObject>(serializationService, serializedObj, context, cancellationToken);
        }

        /// <summary>
        /// Deserializes the object from XML.
        /// </summary>
        /// <typeparam name="TRootObject">Type of the root object.</typeparam>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static TRootObject XmlDeserialize<TRootObject>(
            this ISerializationService serializationService,
            string serializedObj,
            ISerializationContext context = null)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            return Deserialize<XmlMediaType, TRootObject>(serializationService, serializedObj, context);
        }

        /// <summary>
        /// Serializes the provided object in the specified format.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the
        ///                                             <see cref="ISerializationContext.MediaType"/> is
        ///                                             mismatched in the provided context.</exception>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="obj">The object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a string.
        /// </returns>
        public static Task<string> SerializeAsync<TMediaType>(
            this ISerializationService serializationService,
            object obj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
            where TMediaType : IMediaType
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (obj == null)
            {
                return Task.FromResult((string)null);
            }

            context = serializationService.CreateOrUpdateSerializationContext<TMediaType>(context);

            var serializer = serializationService.GetSerializer(context);
            return serializer.SerializeAsync(obj, context, cancellationToken);
        }

        /// <summary>
        /// Serializes the provided object in the specified format.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the
        ///                                             <see cref="ISerializationContext.MediaType"/> is
        ///                                             mismatched in the provided context.</exception>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="obj">The object.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The serialized object as a string in the specified format.
        /// </returns>
        public static string Serialize<TMediaType>(
            this ISerializationService serializationService,
            object obj,
            ISerializationContext context = null)
            where TMediaType : IMediaType
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            if (obj == null)
            {
                return null;
            }

            context = serializationService.CreateOrUpdateSerializationContext<TMediaType>(context);

            var serializer = serializationService.GetSerializer(context);
            return serializer.Serialize(obj, context);
        }

        /// <summary>
        /// Serializes the provided object as JSON asynchronously.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the <see cref="ISerializationContext.MediaType"/> is not <see cref="JsonMediaType"/> in the provided context.</exception>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="obj">The object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a JSON string.
        /// </returns>
        public static Task<string> JsonSerializeAsync(
            this ISerializationService serializationService,
            object obj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            return SerializeAsync<JsonMediaType>(serializationService, obj, context, cancellationToken);
        }

        /// <summary>
        /// Serializes the provided object as JSON.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the <see cref="ISerializationContext.MediaType"/> is not <see cref="JsonMediaType"/> in the provided context.</exception>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="obj">The object.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The serialized object as a JSON string.
        /// </returns>
        public static string JsonSerialize(
            this ISerializationService serializationService,
            object obj,
            ISerializationContext context = null)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            return Serialize<JsonMediaType>(serializationService, obj, context);
        }

        /// <summary>
        /// Serializes the provided object as XML asynchronously.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the <see cref="ISerializationContext.MediaType"/> is not <see cref="XmlMediaType"/> in the provided context.</exception>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="obj">The object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a XML string.
        /// </returns>
        public static Task<string> XmlSerializeAsync(
            this ISerializationService serializationService,
            object obj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            return SerializeAsync<XmlMediaType>(serializationService, obj, context, cancellationToken);
        }

        /// <summary>
        /// Serializes the provided object as XML.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the <see cref="ISerializationContext.MediaType"/> is not <see cref="XmlMediaType"/> in the provided context.</exception>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="obj">The object.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The serialized object as a XML string.
        /// </returns>
        public static string XmlSerialize(
            this ISerializationService serializationService,
            object obj,
            ISerializationContext context = null)
        {
            Requires.NotNull(serializationService, nameof(serializationService));

            return Serialize<XmlMediaType>(serializationService, obj, context);
        }

        /// <summary>
        /// Serializes the provided object asynchronously.
        /// </summary>
        /// <param name="serializer">The serializer to act on.</param>
        /// <param name="obj">The object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a string.
        /// </returns>
        public static async Task<string> SerializeAsync(
            this ISerializer serializer,
            object obj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(serializer, nameof(serializer));

            var writer = new StringWriter();
            try
            {
                await serializer.SerializeAsync(obj, writer, context, cancellationToken).PreserveThreadContext();
                var stringBuilder = writer.GetStringBuilder();
                return stringBuilder.ToString();
            }
            finally
            {
                writer.Dispose();
            }
        }

        /// <summary>
        /// Serializes the provided object.
        /// </summary>
        /// <param name="serializer">The serializer to act on.</param>
        /// <param name="obj">The object.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The serialized object as a string.
        /// </returns>
        public static string Serialize(
            this ISerializer serializer,
            object obj,
            ISerializationContext context = null)
        {
            Requires.NotNull(serializer, nameof(serializer));

            var writer = new StringWriter();
            try
            {
                serializer.Serialize(obj, writer, context);
                var stringBuilder = writer.GetStringBuilder();
                return stringBuilder.ToString();
            }
            finally
            {
                writer.Dispose();
            }
        }

        /// <summary>
        /// Deserialize an object asynchronously.
        /// </summary>
        /// <param name="serializer">The serializer to act on.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        public static async Task<object> DeserializeAsync(
            this ISerializer serializer,
            string serializedObj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(serializer, nameof(serializer));

            var reader = new StringReader(serializedObj);
            try
            {
                var result = await serializer.DeserializeAsync(reader, context, cancellationToken).PreserveThreadContext();
                return result;
            }
            finally
            {
                reader.Dispose();
            }
        }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="serializer">The serializer to act on.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static object Deserialize(
            this ISerializer serializer,
            string serializedObj,
            ISerializationContext context = null)
        {
            Requires.NotNull(serializer, nameof(serializer));

            var reader = new StringReader(serializedObj);
            try
            {
                var result = serializer.Deserialize(reader, context);
                return result;
            }
            finally
            {
                reader.Dispose();
            }
        }

        /// <summary>
        /// Creates the serialization context or updates it with the serialization service and media type.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="context">The serialization context (optional).</param>
        /// <param name="contextConfig">The context configuration (optional).</param>
        /// <returns>
        /// The new serialization context.
        /// </returns>
        public static ISerializationContext CreateOrUpdateSerializationContext<TMediaType>(this ISerializationService serializationService, ISerializationContext context = null, Action<ISerializationContext> contextConfig = null)
            where TMediaType : IMediaType
        {
            if (context == null)
            {
                context = SerializationContext.Create<TMediaType>(serializationService);
            }
            else
            {
                if (context.SerializationService == null)
                {
                    context.SerializationService = serializationService;
                }

                if (context.MediaType == null)
                {
                    context.MediaType = typeof(TMediaType);
                }
                else if (context.MediaType != typeof(TMediaType))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            Strings.Serialization_MediaTypeMismatch_Exception,
                            typeof(TMediaType),
                            context.MediaType));
                }
            }

            contextConfig?.Invoke(context);
            return context;
        }

        /// <summary>
        /// Serializes the provided object.
        /// </summary>
        /// <param name="serializer">The serializer to act on.</param>
        /// <param name="obj">The object.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> used to write the object content.</param>
        /// <param name="context">Optional. The context.</param>
        public static void Serialize(
            this ISerializer serializer,
            object obj,
            TextWriter textWriter,
            ISerializationContext context = null)
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
        /// Deserializes an object.
        /// </summary>
        /// <param name="serializer">The serializer to act on.</param>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static object Deserialize(
            this ISerializer serializer,
            TextReader textReader,
            ISerializationContext context = null)
        {
            Requires.NotNull(serializer, nameof(serializer));

            if (serializer is ISyncSerializer syncSerializer)
            {
                return syncSerializer.Deserialize(textReader, context);
            }

            return serializer.DeserializeAsync(textReader, context).GetResultNonLocking();
        }
    }
}