// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A handle allowing the graph of parts associated with an exported instance
//   to be released.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition
{
    using System;
    using System.Composition;

    /// <summary>
    /// A handle allowing the graph of parts associated with an exported instance
    /// to be released.
    /// </summary>
    /// <typeparam name="T">The export type.</typeparam>
    internal class ExportAdapter<T>
    {
        private Export<T> export;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportAdapter{T}" /> class.
        /// </summary>
        /// <param name="factory">The inner export.</param>
        internal ExportAdapter(Func<Export<T>> factory)
        {
            this.Lazy = new DisposableLazy<T>(() => (this.export = factory()).Value, () => this.export?.Dispose());
        }

        /// <summary>
        /// Gets the lazy output.
        /// </summary>
        public DisposableLazy<T> Lazy { get; }
    }

    /// <summary>
    /// A handle allowing the graph of parts associated with an exported instance
    /// to be released.
    /// </summary>
    /// <typeparam name="T">The export type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    internal class ExportAdapter<T, TMetadata>
    {
        private Export<T>? export;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportAdapter{T,TMetadata}" /> class.
        /// </summary>
        /// <param name="factory">The inner export.</param>
        /// <param name="metadata">The metadata.</param>
        internal ExportAdapter(Func<Export<T>> factory, TMetadata metadata)
        {
            this.Lazy = new DisposableLazy<T, TMetadata>(() => (this.export = factory()).Value, metadata, () => this.export?.Dispose());
        }

        /// <summary>
        /// Gets the lazy output.
        /// </summary>
        public DisposableLazy<T, TMetadata> Lazy { get; }
    }}