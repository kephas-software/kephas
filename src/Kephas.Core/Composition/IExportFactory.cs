// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExportFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract used to import parts that wish to dynamically create instances of other parts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition
{
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Non-generic contract used to import parts that wish to dynamically create instances of other parts.
    /// </summary>
    public interface IExportFactory
    {
        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>A handle allowing the created part to be accessed then released.</returns>
        IExport CreateExport();
    }

    /// <summary>
    /// Contract used to import parts that wish to dynamically create instances of other parts.
    /// </summary>
    /// <typeparam name="T">The contract type of the created parts.</typeparam>
    public interface IExportFactory<out T> : IExportFactory
    {
        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>A handle allowing the created part to be accessed then released.</returns>
        new IExport<T> CreateExport();
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
    }

    /// <summary>
    /// Extension methods for <see cref="IExportFactory{T}"/>.
    /// </summary>
    public static class ExportFactoryExtensions
    {
        /// <summary>
        /// Convenience method that creates the exported value.
        /// </summary>
        /// <typeparam name="T">The exported value type.</typeparam>
        /// <param name="exportFactory">The exportFactory to act on.</param>
        /// <returns>
        /// The exported value.
        /// </returns>
        public static T CreateExportedValue<T>(this IExportFactory<T> exportFactory)
        {
            Requires.NotNull(exportFactory, nameof(exportFactory));

            return exportFactory.CreateExport().Value;
        }
    }
}