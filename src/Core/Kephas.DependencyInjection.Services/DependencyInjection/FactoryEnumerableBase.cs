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
    public abstract class FactoryEnumerableBase<TService, TMetadata> : ServiceEnumerableBase<IFactory<TService, TMetadata>, TMetadata>
        where TService : notnull
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryEnumerable{TService,TMetadata}"/> class.
        /// </summary>
        /// <param name="serviceFactories">The service factories.</param>
        protected FactoryEnumerableBase(IEnumerable<IFactory<TService, TMetadata>>? serviceFactories = null)
            : base(serviceFactories)
        {
        }

        /// <summary>
        /// Gets the metadata from the enumeration item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The metadata associated to the item.</returns>
        protected override TMetadata GetMetadata(IFactory<TService, TMetadata> item) =>
            item.Metadata;
    }
}