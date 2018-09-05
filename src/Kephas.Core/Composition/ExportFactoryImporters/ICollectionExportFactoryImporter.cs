// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICollectionExportFactoryImporter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICollectionExportFactoryImporter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.ExportFactoryImporters
{
    using System.Collections;
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// Interface for importers of a collection of export factories.
    /// </summary>
    public interface ICollectionExportFactoryImporter
    {
        /// <summary>
        /// Gets the export factories.
        /// </summary>
        /// <value>
        /// The export factories.
        /// </value>
        IEnumerable ExportFactories { get; }
    }

    /// <summary>
    /// Generic service contract for importers of a collection of export factories.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    [AppServiceContract(AsOpenGeneric = true)]
    public interface ICollectionExportFactoryImporter<out TService> : ICollectionExportFactoryImporter
    {
        /// <summary>
        /// Gets the export factories.
        /// </summary>
        /// <value>
        /// The export factories.
        /// </value>
        new IEnumerable<IExportFactory<TService>> ExportFactories { get; }
    }

    /// <summary>
    /// Generic service contract for importers of a collection of export factories with metadata.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
    [AppServiceContract(AsOpenGeneric = true)]
    public interface ICollectionExportFactoryImporter<out TService, out TMetadata> : ICollectionExportFactoryImporter
    {
        /// <summary>
        /// Gets the export factories.
        /// </summary>
        /// <value>
        /// The export factories.
        /// </value>
        new IEnumerable<IExportFactory<TService, TMetadata>> ExportFactories { get; }
    }
}