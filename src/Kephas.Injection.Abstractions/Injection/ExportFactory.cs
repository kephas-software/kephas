// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the export factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection
{
    using System;

    /// <summary>
    /// An export factory.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    public class ExportFactory<TService> : IExportFactory<TService>
    {
        /// <summary>
        /// The factory.
        /// </summary>
        private readonly Func<Tuple<TService, Action>> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFactory{TService}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public ExportFactory(Func<TService> factory)
        {
            this.factory = () => Tuple.Create(factory(), (Action)(() => { }));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFactory{TService}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public ExportFactory(Func<Tuple<TService, Action>> factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>
        /// A handle allowing the created part to be accessed then released.
        /// </returns>
        public IExport<TService> CreateExport()
        {
            return new Export<TService>(this.factory);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"Export factory of {typeof(TService).Name}.";
        }
    }

    /// <summary>
    /// An export factory with metadata.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
    public class ExportFactory<TService, TMetadata> : IExportFactory<TService, TMetadata>
    {
        /// <summary>
        /// The factory.
        /// </summary>
        private readonly Func<Tuple<TService, Action>> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFactory{TService, TMetadata}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="metadata">The metadata.</param>
        public ExportFactory(Func<TService> factory, TMetadata metadata)
        {
            this.factory = () => Tuple.Create(factory(), (Action)(() => { }));
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
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>
        /// A handle allowing the created part to be accessed then released.
        /// </returns>
        public IExport<TService, TMetadata> CreateExport()
        {
            return new Export<TService, TMetadata>(this.factory, this.Metadata);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"Export factory of {typeof(TService).Name} with {this.Metadata}.";
        }
    }
}