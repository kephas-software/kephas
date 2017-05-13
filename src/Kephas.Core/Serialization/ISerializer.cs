// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base contract for serializers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Net.Mime;
    using Kephas.Services;

    /// <summary>
    /// Base contract for serializers.
    /// </summary>
    [ContractClass(typeof(SerializerContractClass))]
    public interface ISerializer
    {
        /// <summary>
        /// Serializes the provided object asynchronously.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> used to write the object content.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a string.
        /// </returns>
        Task SerializeAsync(
            object obj,
            TextWriter textWriter,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deserialize an object asynchronously.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        Task<object> DeserializeAsync(
            TextReader textReader,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Application service contract for a serializer based on the indicated media type.
    /// </summary>
    /// <typeparam name="TMedia">The media type.</typeparam>
    [AppServiceContract(ContractType = typeof(ISerializer), AllowMultiple = true)]
    public interface ISerializer<TMedia> : ISerializer
        where TMedia : IMediaType
    {
    }

    /// <summary>
    /// Contract class for <see cref="ISerializer"/>.
    /// </summary>
    [ContractClassFor(typeof(ISerializer))]
    internal abstract class SerializerContractClass : ISerializer
    {
        /// <summary>
        /// Serializes the provided object asynchronously.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> used to write the object content.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a string.
        /// </returns>
        public Task SerializeAsync(
            object obj,
            TextWriter textWriter,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(textWriter != null);
            Contract.Ensures(Contract.Result<Task>() != null);

            return Contract.Result<Task>();
        }

        /// <summary>
        /// Deserialize an object asynchronously.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        public Task<object> DeserializeAsync(
            TextReader textReader,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(textReader != null);
            Contract.Ensures(Contract.Result<Task>() != null);

            return Contract.Result<Task<object>>();
        }
    }
}