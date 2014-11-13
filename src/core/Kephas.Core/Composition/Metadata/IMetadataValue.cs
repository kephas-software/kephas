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
    /// Contract for metadata values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface IMetadataValue<out TValue>
    {
        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        TValue Value { get; }
    }
}