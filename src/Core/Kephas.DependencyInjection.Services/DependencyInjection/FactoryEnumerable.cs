// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryEnumerable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ordered service collection class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.DependencyInjection;

/// <summary>
/// Collection of ordered services.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
[Priority(Priority.Low)]
public class FactoryEnumerable<TService> : FactoryEnumerableBase<TService, ServiceMetadata>, IFactoryEnumerable<TService>
    where TService : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FactoryEnumerable{TService}"/> class.
    /// </summary>
    /// <param name="serviceFactories">The service factories.</param>
    public FactoryEnumerable(IEnumerable<IFactory<TService, ServiceMetadata>>? serviceFactories = null)
        : base(serviceFactories)
    {
    }
}
    
/// <summary>
/// Collection of ordered services.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
/// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
[Priority(Priority.Low)]
public class FactoryEnumerable<TService, TMetadata> : FactoryEnumerableBase<TService, TMetadata>, IFactoryEnumerable<TService, TMetadata>
    where TService : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FactoryEnumerable{TService,TMetadata}"/> class.
    /// </summary>
    /// <param name="serviceFactories">The service factories.</param>
    public FactoryEnumerable(IEnumerable<IFactory<TService, TMetadata>>? serviceFactories = null)
        : base(serviceFactories)
    {
    }
}