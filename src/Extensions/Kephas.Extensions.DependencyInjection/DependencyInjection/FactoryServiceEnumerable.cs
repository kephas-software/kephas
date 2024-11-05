// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryServiceEnumerable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.Extensions.DependencyInjection;

/// <summary>
/// Collection of ordered services.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
[Priority(Priority.BelowNormal)]
[OverrideBaseImplementation]
public class FactoryServiceEnumerable<TService> : FactoryEnumerable<TService>
    where TService : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FactoryServiceEnumerable{TService}"/> class.
    /// </summary>
    public FactoryServiceEnumerable(IServiceProvider serviceProvider)
        : base(serviceProvider.EnumerateFactories<TService, ServiceMetadata>())
    {
    }
}

/// <summary>
/// Collection of ordered services.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
/// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
[Priority(Priority.BelowNormal)]
[OverrideBaseImplementation]
public class FactoryServiceEnumerable<TService, TMetadata> : FactoryEnumerable<TService, TMetadata>
    where TService : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FactoryServiceEnumerable{TService}"/> class.
    /// </summary>
    public FactoryServiceEnumerable(IServiceProvider serviceProvider)
        : base(serviceProvider.EnumerateFactories<TService, TMetadata>())
    {
    }
}