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
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Base contract for serializers.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes the provided object asynchronously.
        /// </summary>
        /// <param name="obj">              The object.</param>
        /// <param name="context">          The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a string.
        /// </returns>
        Task<string> SerializeAsync(
            object obj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deserialize an object asynchronously.
        /// </summary>
        /// <param name="serializedObj">    The serialized object.</param>
        /// <param name="context">          The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        Task<object> DeserializeAsync(
            string serializedObj,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Contract for a serializer with for a specific contract.
    /// </summary>
    /// <typeparam name="TFormat">Type of the format.</typeparam>
    [AppServiceContract(ContractType = typeof(ISerializer))]
    public interface ISerializer<TFormat> : ISerializer
        where TFormat : IFormat
    {
    }
}