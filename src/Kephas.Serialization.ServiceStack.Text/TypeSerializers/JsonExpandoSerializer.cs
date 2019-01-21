// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonExpandoSerializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the JSON expando serializer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text.TypeSerializers
{
    /// <summary>
    /// A <see cref="JsonExpando"/> serializer.
    /// </summary>
    public class JsonExpandoSerializer : ITypeJsonSerializer<JsonExpando>
    {
        /// <summary>
        /// Raw implementation of value serialization.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A string.
        /// </returns>
        public string RawSerialize(JsonExpando value) => global::ServiceStack.Text.JsonSerializer.SerializeToString(value.ToDictionaryDeep());

        /// <summary>
        /// Raw implementation of value deserialization.
        /// </summary>
        /// <param name="serializedValue">The serialized value.</param>
        /// <returns>
        /// A TValue.
        /// </returns>
        public JsonExpando RawDeserialize(string serializedValue) => new JsonExpando(serializedValue);
    }
}
