// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExportFactoryImporter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IExportFactoryImporter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.ExportFactoryImporters
{
    using Kephas.Services;

    /// <summary>
    /// Interface for importers of an export factory.
    /// </summary>
    public interface IExportFactoryImporter
    {
        /// <summary>
        /// Gets the export factory.
        /// </summary>
        /// <value>
        /// The export factory.
        /// </value>
        object ExportFactory { get; }
    }

    /// <summary>
    /// Generic service contract for importers of an export factory with metadata.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    [AppServiceContract(AsOpenGeneric = true)]
    public interface IExportFactoryImporter<TService> : IExportFactoryImporter
    {
        /// <summary>
        /// Gets the export factory.
        /// </summary>
        /// <value>
        /// The export factory.
        /// </value>
        new IExportFactory<TService> ExportFactory { get; }
    }

    /// <summary>
    /// Generic service contract for importers of an export factory with metadata.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
    [AppServiceContract(AsOpenGeneric = true)]
    public interface IExportFactoryImporter<TService, TMetadata> : IExportFactoryImporter
    {
        /// <summary>
        /// Gets the export factory.
        /// </summary>
        /// <value>
        /// The export factory.
        /// </value>
        new IExportFactory<TService, TMetadata> ExportFactory { get; }
    }
}