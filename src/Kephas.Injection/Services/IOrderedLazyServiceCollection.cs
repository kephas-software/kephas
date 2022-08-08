// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrderedLazyServiceCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOrderedLazyServiceCollection interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Kephas.Injection.Resources;

/// <summary>
/// Interface for ordered lazy service collection.
/// </summary>
/// <typeparam name="TContract">Type of the service contract.</typeparam>
/// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
[AppServiceContract(AsOpenGeneric = true)]
public interface IOrderedLazyServiceCollection<TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata> : IEnumerable<Lazy<TContract, TMetadata>>
{
    /// <summary>
    /// Gets the service factories in the appropriate order.
    /// </summary>
    /// <param name="filter">Optional. Specifies a filter.</param>
    /// <returns>
    /// The ordered service factories.
    /// </returns>
    IEnumerable<Lazy<TContract, TMetadata>> GetServiceFactories(
        Func<Lazy<TContract, TMetadata>, bool>? filter = null);

    /// <summary>
    /// Gets the services in the appropriate order.
    /// </summary>
    /// <param name="filter">Optional. Specifies a filter.</param>
    /// <returns>
    /// The ordered services.
    /// </returns>
    IEnumerable<TContract> GetServices(
        Func<Lazy<TContract, TMetadata>, bool>? filter = null);

#if NETSTANDARD2_1
#else
    /// <summary>
    /// Tries to get the service based on the provided criteria.
    /// </summary>
    /// <param name="criteria">The criteria function.</param>
    /// <returns>The requested service or <c>null</c>.</returns>
    TContract? TryGetService(Func<TMetadata, bool> criteria) =>
        this.GetServices(l => criteria(l.Metadata)).FirstOrDefault();

    /// <summary>
    /// Tries to get the service based on the provided criteria.
    /// </summary>
    /// <param name="criteria">The criteria function.</param>
    /// <returns>The requested service or <c>null</c>.</returns>
    TContract? TryGetService(Func<Lazy<TContract, TMetadata>, bool> criteria) =>
        this.GetServices(criteria).FirstOrDefault();

    /// <summary>
    /// Gets the service based on the provided criteria.
    /// </summary>
    /// <param name="criteria">the criteria function.</param>
    /// <returns>If found, the requested service, otherwise an exception occurs.</returns>
    TContract GetService(Func<TMetadata, bool> criteria)
    {
        var service = this.TryGetService(criteria);
        return service ?? throw new ArgumentException(Strings.OrderedServiceFactoryCollection_service_with_requested_criteria_not_found, nameof(criteria));
    }

    /// <summary>
    /// Gets the service based on the provided criteria.
    /// </summary>
    /// <param name="criteria">the criteria function.</param>
    /// <returns>If found, the requested service, otherwise an exception occurs.</returns>
    TContract GetService(Func<Lazy<TContract, TMetadata>, bool> criteria)
    {
        var service = this.TryGetService(criteria);
        return service ?? throw new ArgumentException(Strings.OrderedServiceFactoryCollection_service_with_requested_criteria_not_found, nameof(criteria));
    }
#endif
}

#if NETSTANDARD2_1
/// <summary>
/// Extension methods for <see cref="IOrderedLazyServiceCollection{TContract,TMetadata}"/>.
/// </summary>
public static class OrderedLazyServiceCollectionExtensions
{
    /// <summary>
    /// Tries to get the service based on the provided criteria.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
    /// <param name="orderedCollection">The ordered collection.</param>
    /// <param name="criteria">The criteria function.</param>
    /// <returns>The requested service or <c>null</c>.</returns>
    public static TContract? TryGetService<TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(this IOrderedLazyServiceCollection<TContract, TMetadata> orderedCollection, Func<TMetadata, bool> criteria) =>
        orderedCollection.GetServices(l => criteria(l.Metadata)).FirstOrDefault();

    /// <summary>
    /// Tries to get the service based on the provided criteria.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
    /// <param name="orderedCollection">The ordered collection.</param>
    /// <param name="criteria">The criteria function.</param>
    /// <returns>The requested service or <c>null</c>.</returns>
    public static TContract? TryGetService<TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(this IOrderedLazyServiceCollection<TContract, TMetadata> orderedCollection, Func<Lazy<TContract, TMetadata>, bool> criteria) =>
        orderedCollection.GetServices(criteria).FirstOrDefault();

    /// <summary>
    /// Gets the service based on the provided criteria.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
    /// <param name="orderedCollection">The ordered collection.</param>
    /// <param name="criteria">the criteria function.</param>
    /// <returns>If found, the requested service, otherwise an exception occurs.</returns>
    public static TContract GetService<TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(this IOrderedLazyServiceCollection<TContract, TMetadata> orderedCollection, Func<TMetadata, bool> criteria)
    {
        var service = orderedCollection.TryGetService(criteria);
        return service ?? throw new ArgumentException(Strings.OrderedServiceFactoryCollection_service_with_requested_criteria_not_found, nameof(criteria));
    }

    /// <summary>
    /// Gets the service based on the provided criteria.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
    /// <param name="orderedCollection">The ordered collection.</param>
    /// <param name="criteria">the criteria function.</param>
    /// <returns>If found, the requested service, otherwise an exception occurs.</returns>
    public static TContract GetService<TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(this IOrderedLazyServiceCollection<TContract, TMetadata> orderedCollection, Func<Lazy<TContract, TMetadata>, bool> criteria)
    {
        var service = orderedCollection.TryGetService(criteria);
        return service ?? throw new ArgumentException(Strings.OrderedServiceFactoryCollection_service_with_requested_criteria_not_found, nameof(criteria));
    }
}
#endif