// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactoryImporter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the export factory importer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.ExportFactoryImporters
{
    /// <summary>
    /// Service importing an export factory with metadata.
    /// </summary>
    /// <typeparam name="TTargetContract">Type of the target service contract.</typeparam>
    /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
    public class ExportFactoryImporter<TTargetContract, TMetadata> : IExportFactoryImporter<TTargetContract, TMetadata>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFactoryImporter{TTargetContract,TMetadata}"/> class.
        /// </summary>
        /// <param name="exportFactory">The export factory.</param>
        public ExportFactoryImporter(IExportFactory<TTargetContract, TMetadata> exportFactory)
        {
            this.ExportFactory = exportFactory;
        }

        /// <summary>
        /// Gets the export factory.
        /// </summary>
        /// <value>
        /// The export factory.
        /// </value>
        public IExportFactory<TTargetContract, TMetadata> ExportFactory { get; }
    }

    /// <summary>
    /// Service importing an export factory.
    /// </summary>
    /// <typeparam name="TTargetContract">Type of the target service contract.</typeparam>
    public class ExportFactoryImporter<TTargetContract> : IExportFactoryImporter<TTargetContract>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFactoryImporter{TTargetContract}"/> class.
        /// </summary>
        /// <param name="exportFactory">The export factory.</param>
        public ExportFactoryImporter(IExportFactory<TTargetContract> exportFactory)
        {
            this.ExportFactory = exportFactory;
        }

        /// <summary>
        /// Gets the export factory.
        /// </summary>
        /// <value>
        /// The export factory.
        /// </value>
        public IExportFactory<TTargetContract> ExportFactory { get; }
    }
}