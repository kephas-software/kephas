// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactoryAdapter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Component used to import parts that wish to dynamically create instances of other parts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Component used to import parts that wish to dynamically create instances of other parts.
    /// </summary>
    /// <typeparam name="T">The contract type of the created parts.</typeparam>
    public class ExportFactoryAdapter<T> : IExportFactory<T>
    {
        /// <summary>
        /// The inner export factory.
        /// </summary>
        private readonly System.Composition.ExportFactory<T> innerExportFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFactoryAdapter{T}"/> class.
        /// </summary>
        /// <param name="exportCreator">The export creator.</param>
        public ExportFactoryAdapter(Func<Tuple<T, Action>> exportCreator)
        {
            this.innerExportFactory = new System.Composition.ExportFactory<T>(exportCreator);
        }

        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>A handle allowing the created part to be accessed then released.</returns>
        public IExport<T> CreateExport()
        {
            return new ExportAdapter<T>(this.innerExportFactory.CreateExport());
        }
    }

    /// <summary>
    /// An ExportFactory that provides metadata describing the created exports.
    /// </summary>
    /// <typeparam name="T">The contract type being created.</typeparam>
    /// <typeparam name="TMetadata">The metadata required from the export.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class ExportFactoryAdapter<T, TMetadata> : ExportFactoryAdapter<T>, IExportFactory<T, TMetadata>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFactoryAdapter{T,TMetadata}"/> class.
        /// </summary>
        /// <param name="exportCreator">The export creator.</param>
        /// <param name="metadata">The metadata.</param>
        public ExportFactoryAdapter(Func<Tuple<T, Action>> exportCreator, TMetadata metadata)
            : base(exportCreator)
        {
            this.Metadata = metadata;
        }

        /// <summary>
        /// Gets the metadata associated with the export.
        /// </summary>
        public TMetadata Metadata { get; private set; }
    }
}