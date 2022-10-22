// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the export factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using Kephas.Services;

    /// <summary>
    /// An export factory.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    public class ExportFactory<TContract> : IExportFactory<TContract>
    {
        /// <summary>
        /// The factory.
        /// </summary>
        private readonly Func<TContract> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFactory{TContract}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public ExportFactory(Func<TContract> factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        /// Convenience method that creates the exported value.
        /// </summary>
        /// <returns>
        /// The exported value.
        /// </returns>
        public TContract CreateExportedValue() => this.factory();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"Export factory of {typeof(TContract).Name}.";
        }
    }

    /// <summary>
    /// An export factory with metadata.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
    public class ExportFactory<TContract, TMetadata> : ExportFactory<TContract>, IExportFactory<TContract, TMetadata>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFactory{TContract, TMetadata}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="metadata">The metadata.</param>
        public ExportFactory(Func<TContract> factory, IDictionary<string, object?>? metadata)
            : this(factory, ServiceHelper.GetServiceMetadata<TMetadata>(metadata))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFactory{TContract, TMetadata}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="metadata">The metadata.</param>
        public ExportFactory(Func<TContract> factory, TMetadata metadata)
            : base(factory)
        {
            this.Metadata = metadata;
        }

        /// <summary>
        /// Gets or sets the metadata associated with the export.
        /// </summary>
        /// <value>
        /// The metadata associated with the export.
        /// </value>
        public TMetadata Metadata { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"Export factory of {typeof(TContract).Name} with {this.Metadata}.";
        }
    }
}