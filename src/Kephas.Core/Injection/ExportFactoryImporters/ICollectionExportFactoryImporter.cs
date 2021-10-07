// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICollectionExportFactoryImporter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICollectionExportFactoryImporter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.ExportFactoryImporters
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
    /// <typeparam name="TTargetContract">Type of the target service contract.</typeparam>
    [AppServiceContract(AsOpenGeneric = true)]
    public interface ICollectionExportFactoryImporter<out TTargetContract> : ICollectionExportFactoryImporter
    {
        /// <summary>
        /// Gets the export factories.
        /// </summary>
        /// <value>
        /// The export factories.
        /// </value>
        new IEnumerable<IExportFactory<TTargetContract>> ExportFactories { get; }
    }

    /// <summary>
    /// Generic service contract for importers of a collection of export factories with metadata.
    /// </summary>
    /// <typeparam name="TTargetContract">Type of the target service contract.</typeparam>
    /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
    [AppServiceContract(AsOpenGeneric = true)]
    public interface ICollectionExportFactoryImporter<out TTargetContract, out TMetadata> : ICollectionExportFactoryImporter
    {
        /// <summary>
        /// Gets the export factories.
        /// </summary>
        /// <value>
        /// The export factories.
        /// </value>
        new IEnumerable<IExportFactory<TTargetContract, TMetadata>> ExportFactories { get; }
    }
}