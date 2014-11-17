// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMetadataValue.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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