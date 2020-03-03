// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacExportFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac export factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac
{
    using System;

    using Kephas.Composition.ExportFactories;

    /// <summary>
    /// An Autofac export factory.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    public class AutofacExportFactory<TService> : IExportFactory<TService>
    {
        private readonly Lazy<TService> servicePromise;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacExportFactory{TService}"/> class.
        /// class.
        /// </summary>
        /// <param name="servicePromise">The service promise.</param>
        public AutofacExportFactory(Lazy<TService> servicePromise)
        {
            this.servicePromise = servicePromise;
        }

        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>
        /// A handle allowing the created part to be accessed then released.
        /// </returns>
        public virtual IExport<TService> CreateExport()
        {
            return new Export<TService>(() => Tuple.Create(this.servicePromise.Value, (Action)(() => { })));
        }

        /// <summary>
        /// Creates the export.
        /// </summary>
        /// <returns>
        /// The new export.
        /// </returns>
        IExport IExportFactory.CreateExport()
        {
            return this.CreateExport();
        }
    }

    /// <summary>
    /// An Autofac export factory.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
    public class AutofacExportFactory<TService, TMetadata> : AutofacExportFactory<TService>, IExportFactory<TService, TMetadata>
    {
        private readonly Lazy<TService, TMetadata> servicePromise;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacExportFactory{TService,TMetadata}"/> class.
        /// class.
        /// </summary>
        /// <param name="servicePromise">The service promise.</param>
        public AutofacExportFactory(Lazy<TService, TMetadata> servicePromise)
            : base(servicePromise)
        {
            this.servicePromise = servicePromise;
        }

        /// <summary>
        /// The metadata.
        /// </summary>
        public TMetadata Metadata { get; }

        /// <summary>
        /// Create an instance of the exported part.
        /// </summary>
        /// <returns>
        /// A handle allowing the created part to be accessed then released.
        /// </returns>
        public new IExport<TService, TMetadata> CreateExport()
        {
            return new Export<TService, TMetadata>(() => Tuple.Create(this.servicePromise.Value, (Action)(() => { })), this.servicePromise.Metadata);
        }
    }
}
