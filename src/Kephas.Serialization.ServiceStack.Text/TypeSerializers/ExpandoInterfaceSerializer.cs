// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoInterfaceSerializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the expando interface serializer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text.TypeSerializers
{
    using Kephas.Dynamic;

    /// <summary>
    /// A <see cref="IExpando"/> serializer.
    /// </summary>
    public class ExpandoInterfaceSerializer : ITypeJsonSerializer<IExpando>
    {
        /// <summary>
        /// Raw implementation of value serialization.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A string.
        /// </returns>
        public string RawSerialize(IExpando value) => global::ServiceStack.Text.JsonSerializer.SerializeToString(value.ToDictionaryDeep());

        /// <summary>
        /// Raw implementation of value deserialization.
        /// </summary>
        /// <param name="serializedValue">The serialized value.</param>
        /// <returns>
        /// A TValue.
        /// </returns>
        public IExpando RawDeserialize(string serializedValue) => new JsonExpando(serializedValue);
    }
}