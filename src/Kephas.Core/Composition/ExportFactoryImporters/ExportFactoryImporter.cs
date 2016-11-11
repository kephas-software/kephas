// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactoryImporter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the export factory importer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.ExportFactoryImporters
{
    using Kephas.Composition;

    /// <summary>
    /// Service importing an export factory with metadata.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
    public class ExportFactoryImporter<TService, TMetadata> : IExportFactoryImporter<TService, TMetadata>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFactoryImporter{TService,TMetadata}"/> class.
        /// </summary>
        /// <param name="exportFactory">The export factory.</param>
        public ExportFactoryImporter(IExportFactory<TService, TMetadata> exportFactory)
        {
            this.ExportFactory = exportFactory;
        }

        /// <summary>
        /// Gets the export factory.
        /// </summary>
        /// <value>
        /// The export factory.
        /// </value>
        public IExportFactory<TService, TMetadata> ExportFactory { get; }

        /// <summary>
        /// Gets the export factory.
        /// </summary>
        object IExportFactoryImporter.ExportFactory => this.ExportFactory;
    }

    /// <summary>
    /// Service importing an export factory.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    public class ExportFactoryImporter<TService> : IExportFactoryImporter<TService>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFactoryImporter{TService}"/> class.
        /// </summary>
        /// <param name="exportFactory">The export factory.</param>
        public ExportFactoryImporter(IExportFactory<TService> exportFactory)
        {
            this.ExportFactory = exportFactory;
        }

        /// <summary>
        /// Gets the export factory.
        /// </summary>
        /// <value>
        /// The export factory.
        /// </value>
        public IExportFactory<TService> ExportFactory { get; }

        /// <summary>
        /// Gets the export factory.
        /// </summary>
        object IExportFactoryImporter.ExportFactory => this.ExportFactory;
    }
}