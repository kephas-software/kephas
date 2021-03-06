// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base contract for serializers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Net.Mime;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base contract for serializers.
    /// </summary>
    /// <remarks>
    /// Serializers handle a single media type and are aggregated by the <see cref="ISerializationService"/>.
    /// </remarks>
    public interface ISerializer
    {
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
        Task SerializeAsync(
            object? obj,
            TextWriter textWriter,
            ISerializationContext context,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Serializes the provided object asynchronously.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the serialized object.
        /// </returns>
        Task<string?> SerializeAsync(
            object? obj,
            ISerializationContext context,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deserializes an object asynchronously.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        Task<object?> DeserializeAsync(
            TextReader textReader,
            ISerializationContext context,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deserializes an object asynchronously.
        /// </summary>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        Task<object?> DeserializeAsync(
            string? serializedObj,
            ISerializationContext context,
            CancellationToken cancellationToken = default);

#if NETSTANDARD2_0
#else
        /// <summary>
        /// Serializes the provided object.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> used to write the object content.</param>
        /// <param name="context">The context containing serialization options.</param>
        void Serialize(
            object? obj,
            TextWriter textWriter,
            ISerializationContext context)
        {
            this.SerializeAsync(obj, textWriter, context).WaitNonLocking();
        }

        /// <summary>
        /// Serializes the provided object.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The serialized object.
        /// </returns>
        string? Serialize(
            object? obj,
            ISerializationContext context)
        {
            return this.SerializeAsync(obj, context).GetResultNonLocking();
        }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        object? Deserialize(
            TextReader textReader,
            ISerializationContext context)
        {
            return this.DeserializeAsync(textReader, context).GetResultNonLocking();
        }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        object? Deserialize(
            string? serializedObj,
            ISerializationContext context)
        {
            return this.DeserializeAsync(serializedObj, context).GetResultNonLocking();
        }
#endif
    }

    /// <summary>
    /// Application service contract for a serializer based on the indicated media type.
    /// </summary>
    /// <remarks>
    /// Serializers handle a single media type and are aggregated by the <see cref="ISerializationService"/>.
    /// </remarks>
    /// <typeparam name="TMedia">The media type.</typeparam>
    [AppServiceContract(ContractType = typeof(ISerializer), AllowMultiple = true)]
    public interface ISerializer<TMedia> : ISerializer
        where TMedia : IMediaType
    {
    }

#if NETSTANDARD2_0
    /// <summary>
    /// Interface for a synchronous serializer.
    /// </summary>
    /// <remarks>
    /// Typically, a serializer supporting synchronous serialization
    /// will implement this interface too.
    /// </remarks>
    public interface ISyncSerializer
    {
        /// <summary>
        /// Serializes the provided object.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> used to write the object content.</param>
        /// <param name="context">The context containing serialization options.</param>
        void Serialize(
            object? obj,
            TextWriter textWriter,
            ISerializationContext context);

        /// <summary>
        /// Serializes the provided object.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The serialized object.
        /// </returns>
        string? Serialize(
            object? obj,
            ISerializationContext context);

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        object? Deserialize(
            TextReader textReader,
            ISerializationContext context);

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="context">The context containing serialization options.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        object? Deserialize(
            string? serializedObj,
            ISerializationContext context);
    }
#endif
}