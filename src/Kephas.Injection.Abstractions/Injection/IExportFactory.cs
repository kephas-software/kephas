// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExportFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract used to import parts that wish to dynamically create instances of other parts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection
{
    /// <summary>
    /// Contract used to import parts that wish to dynamically create instances of other parts.
    /// </summary>
    /// <typeparam name="T">The contract type of the created parts.</typeparam>
    public interface IExportFactory<out T>
    {
        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>A handle allowing the created part to be accessed then released.</returns>
        IExport<T> CreateExport();
    }

    /// <summary>
    /// A contract for an export factory that provides metadata describing the created exports.
    /// </summary>
    /// <typeparam name="T">The contract type being created.</typeparam>
    /// <typeparam name="TMetadata">The metadata required from the export.</typeparam>
    public interface IExportFactory<out T, out TMetadata> : IExportFactory<T>
    {
        /// <summary>
        /// Gets the metadata associated with the export.
        /// </summary>
        /// <value>
        /// The metadata associated with the export.
        /// </value>
        TMetadata Metadata { get; }

        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>A handle allowing the created part to be accessed then released.</returns>
        new IExport<T, TMetadata> CreateExport();

        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>A handle allowing the created part to be accessed then released.</returns>
        IExport<T> IExportFactory<T>.CreateExport() => this.CreateExport();
    }
}