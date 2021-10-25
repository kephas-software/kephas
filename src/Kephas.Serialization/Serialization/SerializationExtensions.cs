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
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Net.Mime;

    /// <summary>
    /// Extension methods for serialization convenience.
    /// </summary>
    public static class SerializationExtensions
    {
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
            return serializationService.DeserializeAsync<BsonMediaType>(serializedObj, optionsConfig, cancellationToken);
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
            return serializationService.Deserialize<BsonMediaType>(serializedObj, optionsConfig);
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
        public static Task<TRootObject?> BsonDeserializeAsync<TRootObject>(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return serializationService.DeserializeAsync<BsonMediaType, TRootObject>(serializedObj, optionsConfig, cancellationToken);
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
        public static TRootObject? BsonDeserialize<TRootObject>(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return serializationService.Deserialize<BsonMediaType, TRootObject>(serializedObj, optionsConfig);
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
            return serializationService.DeserializeAsync<XmlMediaType>(serializedObj, optionsConfig, cancellationToken);
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
            return serializationService.Deserialize<XmlMediaType>(serializedObj, optionsConfig);
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
        public static Task<TRootObject?> XmlDeserializeAsync<TRootObject>(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return serializationService.DeserializeAsync<XmlMediaType, TRootObject>(serializedObj, optionsConfig, cancellationToken);
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
        public static TRootObject? XmlDeserialize<TRootObject>(
            this ISerializationService serializationService,
            string serializedObj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return serializationService.Deserialize<XmlMediaType, TRootObject>(serializedObj, optionsConfig);
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
        public static Task<string> BsonSerializeAsync(
            this ISerializationService serializationService,
            object? obj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return serializationService.SerializeAsync<BsonMediaType>(obj, optionsConfig, cancellationToken);
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
        public static string BsonSerialize(
            this ISerializationService serializationService,
            object? obj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return serializationService.Serialize<BsonMediaType>(obj, optionsConfig);
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
        public static Task<string> XmlSerializeAsync(
            this ISerializationService serializationService,
            object? obj,
            Action<ISerializationContext>? optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            return serializationService.SerializeAsync<XmlMediaType>(obj, optionsConfig, cancellationToken);
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
            object? obj,
            Action<ISerializationContext>? optionsConfig = null)
        {
            return serializationService.Serialize<XmlMediaType>(obj, optionsConfig);
        }
    }
}