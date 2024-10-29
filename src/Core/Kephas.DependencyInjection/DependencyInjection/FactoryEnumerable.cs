// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryEnumerable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ordered service collection class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.DependencyInjection;

/// <summary>
/// Collection of ordered services.
/// </summary>
/// <typeparam name="TTargetContract">Type of the target service contract.</typeparam>
/// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
[Priority(Priority.Low)]
public class FactoryEnumerable<TTargetContract, TMetadata> : IFactoryEnumerable<TTargetContract, TMetadata>
    where TTargetContract : notnull
{
    private readonly ICollection<IFactory<TTargetContract, TMetadata>> serviceFactories;

    /// <summary>
    /// Initializes a new instance of the <see cref="FactoryEnumerable{TTargetContract,TMetadata}"/> class.
    /// </summary>
    /// <param name="serviceFactories">The service factories.</param>
    public FactoryEnumerable(IEnumerable<IFactory<TTargetContract, TMetadata>>? serviceFactories = null) =>
        this.serviceFactories = ComputeServiceFactories(serviceFactories);

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>
    /// The enumerator.
    /// </returns>
    public IEnumerator<IFactory<TTargetContract, TMetadata>> GetEnumerator() =>
        serviceFactories.GetEnumerator();

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>
    /// The enumerator.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private ICollection<IFactory<TTargetContract, TMetadata>> ComputeServiceFactories(IEnumerable<IFactory<TTargetContract, TMetadata>>? serviceFactories)
    {
        if (serviceFactories == null)
        {
            return Array.Empty<IFactory<TTargetContract, TMetadata>>();
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
