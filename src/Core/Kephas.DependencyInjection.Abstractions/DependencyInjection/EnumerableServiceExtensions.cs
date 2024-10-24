// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableServiceExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection;

using Kephas.DependencyInjection.Resources;

/// <summary>
/// Extension methods for enumerable services.
/// </summary>
public static class EnumerableServiceExtensions
{
    /// <summary>
    /// Gets the services in the appropriate order.
    /// </summary>
    /// <param name="lazyServices">The lazy services.</param>
    /// <param name="predicate">Optional. Specifies a predicate.</param>
    /// <returns>
    /// The ordered services.
    /// </returns>
    public static IEnumerable<TService> SelectServices<TService, TMetadata>(this IEnumerable<Lazy<TService, TMetadata>> lazyServices, Func<Lazy<TService, TMetadata>, bool>? predicate = null)
    {
        _ = lazyServices  ?? throw new ArgumentNullException(nameof(lazyServices));

        return (predicate == null ? lazyServices : lazyServices.Where(predicate))
            .Select(e => e.Value);
    }

    /// <summary>
    /// Tries to get the service based on the provided predicate.
    /// </summary>
    /// <param name="lazyServices">The lazy services.</param>
    /// <param name="predicate">The predicate function.</param>
    /// <returns>The requested service or <c>null</c>.</returns>
    public static TService? TryGetService<TService, TMetadata>(this IEnumerable<Lazy<TService, TMetadata>> lazyServices, Func<TMetadata, bool> predicate)
    {
        _ = lazyServices  ?? throw new ArgumentNullException(nameof(lazyServices));

        return lazyServices.SelectServices(l => predicate(l.Metadata)).LastOrDefault();
    }

    /// <summary>
    /// Gets the service based on the provided predicate.
    /// </summary>
    /// <param name="lazyServices">The lazy services.</param>
    /// <param name="predicate">the predicate function.</param>
    /// <returns>If found, the requested service, otherwise an exception occurs.</returns>
    public static TService GetService<TService, TMetadata>(this IEnumerable<Lazy<TService, TMetadata>> lazyServices, Func<TMetadata, bool> predicate)
    {
        _ = lazyServices  ?? throw new ArgumentNullException(nameof(lazyServices));

        var service = lazyServices.TryGetService(predicate);
        return service ?? throw new InvalidOperationException(ServicesAbstractionsStrings.OrderedServiceFactoryCollection_service_with_requested_criteria_not_found);
    }

    /// <summary>
    /// Gets the services in the appropriate order.
    /// </summary>
    /// <param name="factoryServices">The factory services.</param>
    /// <param name="predicate">Optional. Specifies a predicate.</param>
    /// <returns>
    /// The ordered services.
    /// </returns>
    public static IEnumerable<TService> SelectServices<TService, TMetadata>(this IEnumerable<IFactory<TService, TMetadata>> factoryServices, Func<IFactory<TService, TMetadata>, bool>? predicate = null)
    {
        _ = factoryServices  ?? throw new ArgumentNullException(nameof(factoryServices));

        return (predicate == null ? factoryServices : factoryServices.Where(predicate))
            .Select(e => e.Create());
    }

    /// <summary>
    /// Tries to get the service based on the provided predicate.
    /// </summary>
    /// <param name="factoryServices">The factory services.</param>
    /// <param name="predicate">The predicate function.</param>
    /// <returns>The requested service or <c>null</c>.</returns>
    public static TService? TryGetService<TService, TMetadata>(this IEnumerable<IFactory<TService, TMetadata>> factoryServices, Func<TMetadata, bool> predicate)
    {
        _ = factoryServices  ?? throw new ArgumentNullException(nameof(factoryServices));

        return factoryServices.SelectServices(l => predicate(l.Metadata)).LastOrDefault();
    }

    /// <summary>
    /// Gets the service based on the provided predicate.
    /// </summary>
    /// <param name="factoryServices">The factory services.</param>
    /// <param name="predicate">the predicate function.</param>
    /// <returns>If found, the requested service, otherwise an exception occurs.</returns>
    public static TService GetService<TService, TMetadata>(this IEnumerable<IFactory<TService, TMetadata>> factoryServices, Func<TMetadata, bool> predicate)
    {
        _ = factoryServices  ?? throw new ArgumentNullException(nameof(factoryServices));

        var service = factoryServices.TryGetService(predicate);
        return service ?? throw new InvalidOperationException(ServicesAbstractionsStrings.OrderedServiceFactoryCollection_service_with_requested_criteria_not_found);
    }
}
