// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeJsonSerializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ITypeJsonSerializer interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text
{
    using Kephas.Services;

    /// <summary>
    /// Interface for type JSON serializer.
    /// </summary>
    public interface ITypeJsonSerializer
    {
    }

    /// <summary>
    /// Generic interface for JSON serializer of a given value type.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    [SharedAppServiceContract(ContractType = typeof(ITypeJsonSerializer))]
    public interface ITypeJsonSerializer<TValue> : ITypeJsonSerializer
    {
        /// <summary>
        /// Raw implementation of value serialization.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A string.
        /// </returns>
        string RawSerialize(TValue value);

        /// <summary>
        /// Raw implementation of value deserialization.
        /// </summary>
        /// <param name="serializedValue">The serialized value.</param>
        /// <returns>
        /// A TValue.
        /// </returns>
        TValue RawDeserialize(string serializedValue);
    }
}