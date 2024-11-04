// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryEnumerableBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.DependencyInjection
{
    /// <summary>
    /// Collection of ordered services.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
    public abstract class FactoryEnumerableBase<TService, TMetadata> : IEnumerable<IFactory<TService, TMetadata>>
        where TService : notnull
    {
        private readonly ICollection<IFactory<TService, TMetadata>> serviceFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryEnumerable{TService,TMetadata}"/> class.
        /// </summary>
        /// <param name="serviceFactories">The service factories.</param>
        protected FactoryEnumerableBase(IEnumerable<IFactory<TService, TMetadata>>? serviceFactories = null) =>
            this.serviceFactories = ComputeServiceFactories(serviceFactories);

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public IEnumerator<IFactory<TService, TMetadata>> GetEnumerator() =>
            serviceFactories.GetEnumerator();

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private ICollection<IFactory<TService, TMetadata>> ComputeServiceFactories(IEnumerable<IFactory<TService, TMetadata>>? serviceFactories)
        {
            if (serviceFactories == null)
            {
                return Array.Empty<IFactory<TService, TMetadata>>();
            }

            var orderedFactories = serviceFactories
                .OrderBy(f => f.Metadata.GetPriority())
                .ToList();

            // get the overridden services which should be eliminated
            var overriddenTypes = orderedFactories
                .Where(f => f.Metadata is IHasImplementationType { OverrideBaseImplementation: true, ImplementationType.BaseType: not null })
                .Select(f => (f.Metadata as IHasImplementationType)?.ImplementationType?.BaseType)
                .ToList();
            if (overriddenTypes.Count == 0)
            {
                return orderedFactories;
            }

            // eliminate the overridden services
            orderedFactories = orderedFactories
                .Where(f => !overriddenTypes.Contains((f.Metadata as IHasImplementationType)?.ImplementationType))
                .ToList();

            return orderedFactories;
        }
    }
}