// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMetadataValue.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for metadata values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Metadata
{
    /// <summary>
    /// Non-typed contract for metadata values.
    /// </summary>
    public interface IMetadataValue
    {
        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        object Value { get; }
    }

    /// <summary>
    /// Contract for metadata values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface IMetadataValue<out TValue> : IMetadataValue
    {
        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        new TValue Value { get; }
    }
}