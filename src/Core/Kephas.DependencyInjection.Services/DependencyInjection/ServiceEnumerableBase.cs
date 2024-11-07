// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceEnumerableBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.DependencyInjection;

/// <summary>
/// Base class for collection of ordered lazy services.
/// </summary>
/// <typeparam name="TItem">The enumeration item type.</typeparam>
/// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
public abstract class ServiceEnumerableBase<TItem, TMetadata> : IEnumerable<TItem>
{
    private readonly ICollection<TItem> serviceFactories;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceEnumerableBase{TItem,TMetadata}"/> class.
    /// </summary>
    /// <param name="serviceFactories">The service factories.</param>
    protected ServiceEnumerableBase(IEnumerable<TItem>? serviceFactories = null) =>
        this.serviceFactories = ComputeServiceFactories(serviceFactories);

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>
    /// The enumerator.
    /// </returns>
    public IEnumerator<TItem> GetEnumerator() => serviceFactories.GetEnumerator();

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>
    /// The enumerator.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Gets the metadata from the enumeration item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The metadata associated to the item.</returns>
    protected abstract TMetadata GetMetadata(TItem item);

    private ICollection<TItem> ComputeServiceFactories(IEnumerable<TItem>? factories)
    {
        if (factories == null)
        {
            return Array.Empty<TItem>();
        }

        var orderedFactories = factories
            .OrderBy(f => (GetMetadata(f) as IHasPriority)?.Priority ?? Priority.Normal)
            .ToList();

        // get the overridden services which should be eliminated
        var overriddenTypes = orderedFactories
            .Where(f => GetMetadata(f) is IHasImplementationType { OverrideBaseImplementation: true, ImplementationType.BaseType: { } })
            .Select(f => (GetMetadata(f) as IHasImplementationType)?.ImplementationType?.BaseType)
            .ToList();
        if (overriddenTypes.Count == 0)
        {
            return orderedFactories;
        }

        // eliminate the overridden services
        orderedFactories = orderedFactories
            .Where(f => !overriddenTypes.Contains((GetMetadata(f) as IHasImplementationType)?.ImplementationType))
            .ToList();

        return orderedFactories;
    }
}