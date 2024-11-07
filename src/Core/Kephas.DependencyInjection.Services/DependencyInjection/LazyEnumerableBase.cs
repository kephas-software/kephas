// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LazyEnumerableBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection;

/// <summary>
/// Base class for collection of ordered lazy services.
/// </summary>
/// <typeparam name="TService">Type of the service.</typeparam>
/// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
public abstract class LazyEnumerableBase<TService, TMetadata> : ServiceEnumerableBase<Lazy<TService, TMetadata>, TMetadata>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LazyEnumerable{TService,TMetadata}"/> class.
    /// </summary>
    /// <param name="serviceFactories">The service factories.</param>
    protected LazyEnumerableBase(IEnumerable<Lazy<TService, TMetadata>>? serviceFactories = null)
        : base(serviceFactories)
    {
    }

    /// <summary>
    /// Gets the metadata from the enumeration item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The metadata associated to the item.</returns>
    protected override TMetadata GetMetadata(Lazy<TService, TMetadata> item) =>
        item.Metadata;
}