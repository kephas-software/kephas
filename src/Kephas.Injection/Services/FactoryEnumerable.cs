// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryEnumerable.cs" company="Kephas Software SRL">
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

    using Kephas.Injection;

    /// <summary>
    /// Collection of ordered services.
    /// </summary>
    /// <typeparam name="TTargetContract">Type of the target service contract.</typeparam>
    /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
    [OverridePriority(Priority.Low)]
    public class FactoryEnumerable<TTargetContract, TMetadata> : IFactoryEnumerable<TTargetContract, TMetadata>
    {
        private readonly ICollection<IExportFactory<TTargetContract, TMetadata>> serviceFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryEnumerable{TTargetContract,TMetadata}"/> class.
        /// </summary>
        /// <param name="serviceFactories">The service factories.</param>
        public FactoryEnumerable(IEnumerable<IExportFactory<TTargetContract, TMetadata>>? serviceFactories = null)
        {
            this.serviceFactories = this.ComputeServiceFactories(serviceFactories);
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
                       .OrderBy(f => (f.Metadata as IHasOverridePriority)?.OverridePriority ?? Priority.Normal)
                       .ThenBy(f => (f.Metadata as IHasProcessingPriority)?.ProcessingPriority ?? Priority.Normal)
                       .ToList();

            // get the overridden services which should be eliminated
            var overriddenTypes = orderedFactories
                .Where(f => f.Metadata is IAppServiceMetadata { IsOverride: true, ServiceType.BaseType: { } })
                .Select(f => (f.Metadata as IAppServiceMetadata)?.ServiceType?.BaseType)
                .ToList();
            if (overriddenTypes.Count == 0)
            {
                return orderedFactories;
            }

            // eliminate the overridden services
            orderedFactories = orderedFactories
                .Where(f => !overriddenTypes.Contains((f.Metadata as IAppServiceMetadata)?.ServiceType))
                .ToList();

            return orderedFactories;
        }
    }
}