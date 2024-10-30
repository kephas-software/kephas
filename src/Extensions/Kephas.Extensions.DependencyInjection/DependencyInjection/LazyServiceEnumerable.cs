// --------------------------------------------------------------------------------------------------------------------
// <copyright file="C.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.Extensions.DependencyInjection;

/// <summary>
/// Collection of ordered lazy services.
/// </summary>
/// <typeparam name="TService">Type of the service.</typeparam>
[Priority(Priority.BelowNormal)]
[OverrideBaseImplementation]
public class LazyServiceEnumerable<TService> : LazyEnumerable<TService>
    where TService : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LazyServiceEnumerable{TService}"/> class.
    /// </summary>
    public LazyServiceEnumerable(IServiceProvider serviceProvider)
        : base(serviceProvider.EnumerateLazies<TService, ServiceMetadata>())
    {
    }
}

/// <summary>
/// Collection of ordered lazy services.
/// </summary>
/// <typeparam name="TService">Type of the service.</typeparam>
/// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
[Priority(Priority.BelowNormal)]
[OverrideBaseImplementation]
public class LazyServiceEnumerable<TService, TMetadata> : LazyEnumerable<TService, TMetadata>
    where TService : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LazyServiceEnumerable{TService,TMetadata}"/> class.
    /// </summary>
    public LazyServiceEnumerable(IServiceProvider serviceProvider)
        : base(serviceProvider.EnumerateLazies<TService, TMetadata>())
    {
    }
}