// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactoryAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Component used to import parts that wish to dynamically create instances of other parts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition
{
    using System;
    using System.Composition;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Injection;

    /// <summary>
    /// Component used to import parts that wish to dynamically create instances of other parts.
    /// </summary>
    /// <typeparam name="T">The contract type of the created parts.</typeparam>
    public class ExportFactoryAdapter<T> : IExportFactory<T>
    {
        private readonly ExportFactory<T> innerExportFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFactoryAdapter{T}"/> class.
        /// </summary>
        /// <param name="exportCreator">The export creator.</param>
        public ExportFactoryAdapter(Func<Tuple<T, Action>> exportCreator)
        {
            this.innerExportFactory = new ExportFactory<T>(exportCreator);
        }

        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>A handle allowing the created part to be accessed then released.</returns>
        public virtual Lazy<T> CreateExport() => new ExportAdapter<T>(() => this.innerExportFactory.CreateExport()).Lazy;
    }

    /// <summary>
    /// An ExportFactory that provides metadata describing the created exports.
    /// </summary>
    /// <typeparam name="T">The contract type being created.</typeparam>
    /// <typeparam name="TMetadata">The metadata required from the export.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class ExportFactoryAdapter<T, TMetadata> : IExportFactory<T, TMetadata>
    {
        private readonly ExportFactory<T> innerExportFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFactoryAdapter{T,TMetadata}"/> class.
        /// </summary>
        /// <param name="exportCreator">The export creator.</param>
        /// <param name="metadata">The metadata.</param>
        public ExportFactoryAdapter(Func<Tuple<T, Action>> exportCreator, TMetadata metadata)
        {
            this.innerExportFactory = new ExportFactory<T, TMetadata>(exportCreator, metadata);

            this.Metadata = metadata;
        }

        /// <summary>
        /// Gets the metadata associated with the export.
        /// </summary>
        public TMetadata Metadata { get; }

        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>A handle allowing the created part to be accessed then released.</returns>
        public Lazy<T, TMetadata> CreateExport()
            => new ExportAdapter<T, TMetadata>(() => this.innerExportFactory.CreateExport(), this.Metadata).Lazy;
    }
}