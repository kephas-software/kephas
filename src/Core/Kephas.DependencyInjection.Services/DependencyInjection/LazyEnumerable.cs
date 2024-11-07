// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LazyEnumerable.cs" company="Kephas Software SRL">
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
/// Collection of ordered lazy services.
/// </summary>
/// <typeparam name="TService">Type of the service.</typeparam>
[Priority(Priority.Low)]
public class LazyEnumerable<TService> 
    : LazyEnumerableBase<TService, ServiceMetadata>, ILazyEnumerable<TService>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LazyEnumerable{TService}"/> class.
    /// </summary>
    /// <param name="serviceFactories">The service factories.</param>
    public LazyEnumerable(IEnumerable<Lazy<TService, ServiceMetadata>>? serviceFactories = null)
        : base(serviceFactories)
    {
    }
}

/// <summary>
/// Collection of ordered lazy services.
/// </summary>
/// <typeparam name="TService">Type of the service.</typeparam>
/// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
[Priority(Priority.Low)]
public class LazyEnumerable<TService, TMetadata>
    : LazyEnumerableBase<TService, TMetadata>, ILazyEnumerable<TService, TMetadata>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LazyEnumerable{TService,TMetadata}"/> class.
    /// </summary>
    /// <param name="serviceFactories">The service factories.</param>
    public LazyEnumerable(IEnumerable<Lazy<TService, TMetadata>>? serviceFactories = null)
        : base(serviceFactories)
    {
    }
}