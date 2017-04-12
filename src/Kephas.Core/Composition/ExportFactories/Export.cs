// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Export.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the export class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.ExportFactories
{
    using System;

    /// <summary>
    /// An export.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    public class Export<TService> : IExport<TService>
    {
        /// <summary>
        /// The lazy value.
        /// </summary>
        private readonly Lazy<TService> lazyValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Export{TService}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public Export(Func<Tuple<TService, Action>> factory)
        {
            this.lazyValue = new Lazy<TService>(() => factory().Item1);
        }

        /// <summary>
        /// The value.
        /// </summary>
        public TService Value => this.lazyValue.Value;

        /// <summary>
        /// Gets the exported value.
        /// </summary>
        /// <value>
        /// The exported value.
        /// </value>
        object IExport.Value => this.Value;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public virtual void Dispose()
        {
        }
    }

    /// <summary>
    /// An export with metadata.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
    public class Export<TService, TMetadata> : Export<TService>, IExport<TService, TMetadata>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Export{TService,TMetadata}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="metadata">The metadata.</param>
        public Export(Func<Tuple<TService, Action>> factory, TMetadata metadata)
            : base(factory)
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