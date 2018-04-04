// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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

    /// <summary>
    /// Base contract for serializers.
    /// </summary>
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
            CancellationToken cancellationToken = default);

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
            CancellationToken cancellationToken = default);
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
}