// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderedServiceFactoryCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ordered service collection class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Collection of ordered services.
    /// </summary>
    /// <typeparam name="TTargetContract">Type of the target service contract.</typeparam>
    /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
    [OverridePriority(Priority.Low)]
    public class OrderedServiceFactoryCollection<TTargetContract, TMetadata> : IOrderedServiceFactoryCollection<TTargetContract, TMetadata>
        where TMetadata : AppServiceMetadata
    {
        private readonly ICollection<IExportFactory<TTargetContract, TMetadata>> serviceFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedServiceFactoryCollection{TTargetContract, TMetadata}"/> class.
        /// </summary>
        /// <param name="serviceFactories">The service factories.</param>
        public OrderedServiceFactoryCollection(IEnumerable<IExportFactory<TTargetContract, TMetadata>>? serviceFactories = null)
        {
            this.serviceFactories = this.ComputeServiceFactories(serviceFactories);
        }

        /// <summary>
        /// Gets the service factories in the appropriate order.
        /// </summary>
        /// <param name="filter">Optional. Specifies a filter.</param>
        /// <returns>
        /// The ordered service factories.
        /// </returns>
        public IEnumerable<IExportFactory<TTargetContract, TMetadata>> GetServiceFactories(
            Func<IExportFactory<TTargetContract, TMetadata>, bool>? filter = null)
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
        public IEnumerable<TTargetContract> GetServices(
            Func<IExportFactory<TTargetContract, TMetadata>, bool>? filter = null)
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
        public IEnumerator<IExportFactory<TTargetContract, TMetadata>> GetEnumerator()
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

        private ICollection<IExportFactory<TTargetContract, TMetadata>> ComputeServiceFactories(IEnumerable<IExportFactory<TTargetContract, TMetadata>>? serviceFactories)
        {
            if (serviceFactories == null)
            {
                return Array.Empty<IExportFactory<TTargetContract, TMetadata>>();
            }

            var orderedFactories = serviceFactories
                       .OrderBy(f => f.Metadata.OverridePriority)
                       .ThenBy(f => f.Metadata.ProcessingPriority)
                       .ToList();

            // get the overridden services which should be eliminated
            var overriddenTypes = orderedFactories
                .Where(f => f.Metadata.IsOverride && f.Metadata.ServiceType?.BaseType != null)
                .Select(f => f.Metadata.ServiceType?.BaseType)
                .ToList();
            if (overriddenTypes.Count == 0)
            {
                return orderedFactories;
            }

            // eliminate the overridden services
            orderedFactories = orderedFactories
                .Where(f => !overriddenTypes.Contains(f.Metadata.ServiceType))
                .ToList();

            return orderedFactories;
        }
    }
}