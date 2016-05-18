﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the serialization extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Resources;
    using Kephas.Serialization.Json;
    using Kephas.Serialization.Xml;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Extension methods for serialization convenience.
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// Deserializes the object from JSON asynchronously.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the
        ///                                             <see cref="ISerializationContext.FormatType"/> is
        ///                                             mismatched in the provided context.</exception>
        /// <typeparam name="TFormatType">Type of the serialization format.</typeparam>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        public static Task<object> DeserializeAsync<TFormatType>(
            this ISerializationService serializationService,
            string serializedObj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TFormatType : IFormat
        {
            Contract.Requires(serializationService != null);

            if (serializedObj == null)
            {
                return Task.FromResult((object)null);
            }

            if (context == null)
            {
                context = SerializationContext.Create<TFormatType>();
            }
            else
            {
                if (context.FormatType != typeof(TFormatType))
                {
                    throw new InvalidOperationException(string.Format(Strings.Serialization_FormatTypeMismatch_Exception, typeof(TFormatType), context.FormatType));
                }
            }

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
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(serializationService != null);

            return DeserializeAsync<JsonFormat>(serializationService, serializedObj, context, cancellationToken);
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
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(serializationService != null);

            return DeserializeAsync<XmlFormat>(serializationService, serializedObj, context, cancellationToken);
        }

        /// <summary>
        /// Serializes the provided object as JSON asynchronously.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the
        ///                                             <see cref="ISerializationContext.FormatType"/> is
        ///                                             mismatched in the provided context.</exception>
        /// <typeparam name="TFormatType">Type of the serialization format.</typeparam>
        /// <param name="serializationService">The serializationService to act on.</param>
        /// <param name="obj">The object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a string.
        /// </returns>
        public static Task<string> SerializeAsync<TFormatType>(
            this ISerializationService serializationService,
            object obj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TFormatType : IFormat
        {
            Contract.Requires(serializationService != null);

            if (obj == null)
            {
                return Task.FromResult((string)null);
            }

            if (context == null)
            {
                context = SerializationContext.Create<TFormatType>();
            }
            else
            {
                if (context.FormatType != typeof(TFormatType))
                {
                    throw new InvalidOperationException(string.Format(Strings.Serialization_FormatTypeMismatch_Exception, typeof(TFormatType), context.FormatType));
                }
            }

            var serializer = serializationService.GetSerializer(context);
            return serializer.SerializeAsync(obj, context, cancellationToken);
        }

        /// <summary>
        /// Serializes the provided object as JSON asynchronously.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the <see cref="ISerializationContext.FormatType"/> is not <see cref="JsonFormat"/> in the provided context.</exception>
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
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(serializationService != null);

            return SerializeAsync<JsonFormat>(serializationService, obj, context, cancellationToken);
        }

        /// <summary>
        /// Serializes the provided object as JSON asynchronously.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the <see cref="ISerializationContext.FormatType"/> is not <see cref="XmlFormat"/> in the provided context.</exception>
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
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(serializationService != null);

            return SerializeAsync<XmlFormat>(serializationService, obj, context, cancellationToken);
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
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(serializer != null);

            var writer = new StringWriter();
            try
            {
                await serializer.SerializeAsync(obj, writer, context, cancellationToken).WithServerThreadingContext();
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
        public static Task<object> DeserializeAsync(
            this ISerializer serializer,
            string serializedObj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(serializer != null);

            var reader = new StringReader(serializedObj);
            try
            {
                return serializer.DeserializeAsync(reader, context, cancellationToken);
            }
            finally
            {
                reader.Dispose();
            }
        }
    }
}