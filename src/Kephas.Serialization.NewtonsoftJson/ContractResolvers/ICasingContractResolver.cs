// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICasingContractResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.ContractResolvers
{
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Contract resolver interface handling name casing.
    /// </summary>
    public interface ICasingContractResolver : IContractResolver
    {
        /// <summary>
        /// Gets the deserialized property name.
        /// </summary>
        /// <param name="propertyName">The serialized property name.</param>
        /// <returns>The deserialized property name.</returns>
        string GetDeserializedPropertyName(string propertyName);

        /// <summary>
        /// Gets the serialized property name.
        /// </summary>
        /// <param name="propertyName">The deserialized property name.</param>
        /// <returns>The serialized property name.</returns>
        string GetSerializedPropertyName(string propertyName);
    }
}