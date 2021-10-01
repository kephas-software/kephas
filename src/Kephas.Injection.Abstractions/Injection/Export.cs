// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Export.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the export class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection
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
        /// Gets the value.
        /// </summary>
        public TService Value => this.lazyValue.Value;

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