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
        /// Deserializes the object from JSON asynchronously.
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
                return default(TRootObject);
            }

            context = CheckOrCreateSerializationContext<TMediaType>(serializationService, context);
            context.RootObjectType = typeof(TRootObject);

            var serializer = serializationService.GetSerializer(context);
            var result = await serializer.DeserializeAsync(serializedObj, context, cancellationToken).PreserveThreadContext();
            return (TRootObject)result;
        }

        /// <summary>
        /// Deserializes the object from JSON asynchronously.
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

            context = CheckOrCreateSerializationContext<TMediaType>(serializationService, context);

            var serializer = serializationService.GetSerializer(context);
            return serializer.DeserializeAsync(serializedObj, context, cancellationToken);
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
        /// Serializes the provided object as JSON asynchronously.
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

            context = CheckOrCreateSerializationContext<TMediaType>(serializationService, context);

            var serializer = serializationService.GetSerializer(context);
            return serializer.SerializeAsync(obj, context, cancellationToken);
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
        /// A Task promising the serialized object as a string.
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
        /// Serializes the provided object as JSON asynchronously.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the <see cref="ISerializationContext.MediaType"/> is not <see cref="XmlMediaType"/> in the provided context.</exception>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="obj">The object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a string.
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
                return writer.GetStringBuilder().ToString();
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
        /// Creates serialization context.
        /// </summary>
        /// <typeparam name="TMediaType">Type of the media type.</typeparam>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The new serialization context.
        /// </returns>
        private static ISerializationContext CheckOrCreateSerializationContext<TMediaType>(ISerializationService serializationService, ISerializationContext context)
            where TMediaType : IMediaType
        {
            if (context == null)
            {
                context = SerializationContext.Create<TMediaType>(serializationService);
            }
            else
            {
                if (context.MediaType != typeof(TMediaType))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            Strings.Serialization_MediaTypeMismatch_Exception,
                            typeof(TMediaType),
                            context.MediaType));
                }
            }

            return context;
        }
    }
}