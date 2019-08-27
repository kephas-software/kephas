// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderedServiceCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ordered service collection class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Services.Composition;

    /// <summary>
    /// Collection of ordered services.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    /// <typeparam name="TServiceMetadata">Type of the service metadata.</typeparam>
    [OverridePriority(Priority.Low)]
    public class OrderedServiceCollection<TService, TServiceMetadata> : IOrderedServiceCollection<TService, TServiceMetadata>
        where TServiceMetadata : AppServiceMetadata
    {
        private readonly ICollection<IExportFactory<TService, TServiceMetadata>> serviceFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedServiceCollection{TService, TServiceMetadata}"/> class.
        /// </summary>
        /// <param name="serviceFactories">The service factories.</param>
        public OrderedServiceCollection(IEnumerable<IExportFactory<TService, TServiceMetadata>> serviceFactories = null)
        {
            this.serviceFactories = serviceFactories
                ?.OrderBy(f => f.Metadata.OverridePriority)
                .ThenBy(f => f.Metadata.ProcessingPriority)
                .ToList()
                ?? new List<IExportFactory<TService, TServiceMetadata>>();
        }

        /// <summary>
        /// Gets the service factories in the appropriate order.
        /// </summary>
        /// <param name="filter">Optional. Specifies a filter.</param>
        /// <returns>
        /// The ordered service factories.
        /// </returns>
        public IEnumerable<IExportFactory<TService, TServiceMetadata>> GetServiceFactories(
            Func<IExportFactory<TService, TServiceMetadata>, bool> filter = null)
        {
            return filter == null ? this.serviceFactories : this.serviceFactories.Where(filter);
        }

        /// <summary>
        /// Gets the services in the appropriate order.
        /// </summary>
        /// <param name="filter">Optional. Specifies a filter.</param>
        /// <returns>
        /// The ordered services.
        /// </returns>
        public IEnumerable<TService> GetServices(
            Func<IExportFactory<TService, TServiceMetadata>, bool> filter = null)
        {
            var factories = filter == null ? this.serviceFactories : this.serviceFactories.Where(filter);
            foreach (var factory in factories)
            {
                yield return factory.CreateExportedValue();
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public IEnumerator<IExportFactory<TService, TServiceMetadata>> GetEnumerator()
        {
            return this.serviceFactories.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}