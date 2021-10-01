// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExport.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for a handle allowing the graph of parts associated with an exported instance
//   to be released.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection
{
    using System;

    /// <summary>
    /// Contract for a handle allowing the graph of parts associated with an exported instance
    /// to be released.
    /// </summary>
    /// <typeparam name="T">The contract type of the created parts.</typeparam>
    public interface IExport<out T> : IDisposable
    {
        /// <summary>
        /// Gets the exported value.
        /// </summary>
        /// <value>
        /// The exported value.
        /// </value>
        T Value { get; }
    }

    /// <summary>
    /// Contract for a handle allowing the graph of parts associated with an exported instance to be
    /// released.
    /// </summary>
    /// <typeparam name="T">The contract type of the created parts.</typeparam>
    /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
    public interface IExport<out T, out TMetadata> : IExport<T>
    {
        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        TMetadata Metadata { get; }
    }
}