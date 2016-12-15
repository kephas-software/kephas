// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestExportFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A test export factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Core.Composition
{
    using System;

    using Kephas.Composition;

    /// <summary>
    /// A test export factory.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    public class TestExportFactory<TService> : IExportFactory<TService>
    {
        /// <summary>
        /// The factory.
        /// </summary>
        private readonly Func<Tuple<TService, Action>> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestExportFactory{TService}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public TestExportFactory(Func<TService> factory)
        {
            this.factory = () => Tuple.Create(factory(), (Action)(() => { }));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestExportFactory{TService}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public TestExportFactory(Func<Tuple<TService, Action>> factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>
        /// A handle allowing the created part to be accessed then released.
        /// </returns>
        IExport<TService> IExportFactory<TService>.CreateExport()
        {
            return this.CreateExport();
        }

        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>
        /// A handle allowing the created part to be accessed then released.
        /// </returns>
        public IExport<TService> CreateExport()
        {
            return new TestExport<TService>(this.factory);
        }

        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>A handle allowing the created part to be accessed then released.</returns>
        IExport IExportFactory.CreateExport() => this.CreateExport();
    }

    /// <summary>
    /// A test export factory with metadata.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
    public class TestExportFactory<TService, TMetadata> : IExportFactory<TService, TMetadata>
    {
        /// <summary>
        /// The factory.
        /// </summary>
        private readonly Func<Tuple<TService, Action>> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestExportFactory{TService, TMetadata}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="metadata">The metadata.</param>
        public TestExportFactory(Func<TService> factory, TMetadata metadata)
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
        /// Creates the export.
        /// </summary>
        /// <returns>
        /// The new export.
        /// </returns>
        IExport<TService> IExportFactory<TService>.CreateExport()
        {
            return this.CreateExport();
        }

        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>
        /// A handle allowing the created part to be accessed then released.
        /// </returns>
        public IExport<TService, TMetadata> CreateExport()
        {
            return new TestExport<TService, TMetadata>(this.factory, this.Metadata);
        }

        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>A handle allowing the created part to be accessed then released.</returns>
        IExport IExportFactory.CreateExport() => this.CreateExport();
    }
}