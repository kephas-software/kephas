﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExportFactoryImporter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the collection export factory importer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.ExportFactoryImporters
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Service importing a collection of export factories with metadata.
    /// </summary>
    /// <typeparam name="TTargetContract">Type of the target service contract.</typeparam>
    /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
    public class CollectionExportFactoryImporter<TTargetContract, TMetadata> : ICollectionExportFactoryImporter<TTargetContract, TMetadata>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionExportFactoryImporter{TTargetContract,TMetadata}"/> class.
        /// </summary>
        /// <param name="exportFactories">The export factories.</param>
        public CollectionExportFactoryImporter(IEnumerable<IExportFactory<TTargetContract, TMetadata>> exportFactories)
        {
            this.ExportFactories = exportFactories;
        }

        /// <summary>
        /// Gets the export factories.
        /// </summary>
        /// <remarks>
        /// The invocation of ToList() allows importing of <seealso cref="IEnumerable{T}"/>, <seealso cref="ICollection{T}"/>, <seealso cref="IList{T}"/>, and <seealso cref="List{T}"/>.
        /// </remarks>
        IEnumerable ICollectionExportFactoryImporter.ExportFactories => this.ExportFactories.ToList();

        /// <summary>
        /// Gets the export factories.
        /// </summary>
        /// <value>
        /// The export factories.
        /// </value>
        public IEnumerable<IExportFactory<TTargetContract, TMetadata>> ExportFactories { get; }
    }

    /// <summary>
    /// Service importing a collection of export factories.
    /// </summary>
    /// <typeparam name="TTargetContract">Type of the target service contract.</typeparam>
    public class CollectionExportFactoryImporter<TTargetContract> : ICollectionExportFactoryImporter<TTargetContract>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionExportFactoryImporter{TTargetContract}"/> class.
        /// </summary>
        /// <param name="exportFactories">The export factories.</param>
        public CollectionExportFactoryImporter(IEnumerable<IExportFactory<TTargetContract>> exportFactories)
        {
            this.ExportFactories = exportFactories;
        }

        /// <summary>
        /// Gets the export factories.
        /// </summary>
        /// <remarks>
        /// The invocation of ToList() allows importing of <seealso cref="IEnumerable{T}"/>, <seealso cref="ICollection{T}"/>, <seealso cref="IList{T}"/>, and <seealso cref="List{T}"/>.
        /// </remarks>
        IEnumerable ICollectionExportFactoryImporter.ExportFactories => this.ExportFactories.ToList();

        /// <summary>
        /// Gets the export factories.
        /// </summary>
        /// <value>
        /// The export factories.
        /// </value>
        public IEnumerable<IExportFactory<TTargetContract>> ExportFactories { get; }
    }
}