// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportAdapter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A handle allowing the graph of parts associated with an exported instance
//   to be released.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef
{
    /// <summary>
    /// A handle allowing the graph of parts associated with an exported instance
    /// to be released.
    /// </summary>
    /// <typeparam name="T">The export type.</typeparam>
    public class ExportAdapter<T> : IExport<T>
    {
        /// <summary>
        /// The inner export.
        /// </summary>
        private readonly System.Composition.Export<T> innerExport;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportAdapter{T}" /> class.
        /// </summary>
        /// <param name="innerExport">The inner export.</param>
        protected internal ExportAdapter(System.Composition.Export<T> innerExport)
        {
            this.innerExport = innerExport;
        }

        /// <summary>
        /// Gets the exported value.
        /// </summary>
        public T Value => this.innerExport.Value;

        /// <summary>
        /// Release the parts associated with the exported value.
        /// </summary>
        public void Dispose()
        {
            this.innerExport.Dispose();
        }
    }

    /// <summary>
    /// A handle allowing the graph of parts associated with an exported instance to be released.
    /// </summary>
    /// <typeparam name="T">The export type.</typeparam>
    /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
    public class ExportAdapter<T, TMetadata> : ExportAdapter<T>, IExport<T, TMetadata>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportAdapter{T,TMetadata}" /> class.
        /// </summary>
        /// <param name="innerExport">The inner export.</param>
        /// <param name="metadata">The metadata.</param>
        protected internal ExportAdapter(System.Composition.Export<T> innerExport, TMetadata metadata)
            : base(innerExport)
        {
            this.Metadata = metadata;
        }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public TMetadata Metadata { get; }
    }
}