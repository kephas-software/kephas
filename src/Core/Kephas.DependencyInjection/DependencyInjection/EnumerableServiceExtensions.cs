// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableServiceExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.CompilerServices;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Resources;

namespace Kephas.DependencyInjection;

/// <summary>
/// Extension methods for enumerable services.
/// </summary>
public static class EnumerableServiceExtensions
{
    /// <summary>
    /// Converts the enumeration of lazy services to a <see cref="ILazyEnumerable{TService,TMetadata}"/>.
    /// </summary>
    /// <param name="lazyServices">The services.</param>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <returns>An instance implementing <see cref="ILazyEnumerable{TService,TMetadata}"/>.</returns>
    public static ILazyEnumerable<TService, TMetadata> ToLazyEnumerable<TService, TMetadata>(
        this IEnumerable<Lazy<TService, TMetadata>> lazyServices)
    {
        return lazyServices as ILazyEnumerable<TService, TMetadata>
               ?? new LazyEnumerable<TService, TMetadata>(lazyServices ?? throw new ArgumentNullException(nameof(lazyServices)));
    }

    /// <summary>
    /// Gets the services in the appropriate order.
    /// </summary>
    /// <param name="lazyServices">The lazy services.</param>
    /// <param name="predicate">Optional. Specifies a predicate.</param>
    /// <returns>
    /// The ordered services.
    /// </returns>
    public static IEnumerable<TService> SelectServices<TService, TMetadata>(
        this ILazyEnumerable<TService, TMetadata> lazyServices,
        Func<TMetadata, bool>? predicate = null)
    {
        _ = lazyServices  ?? throw new ArgumentNullException(nameof(lazyServices));

        return (predicate == null ? lazyServices : lazyServices.Where(l => predicate(l.Metadata)))
            .Select(e => e.Value);
    }

    /// <summary>
    /// Tries to get the service based on the provided predicate.
    /// </summary>
    /// <remarks>
    /// If multiple are found with the same criteria, the last one with the highest priority is chosen,
    /// as the services are assumed to be ordered by their priority.
    /// </remarks>
    /// <param name="lazyServices">The lazy services.</param>
    /// <param name="predicate">The predicate function.</param>
    /// <returns>The requested service or <c>null</c>.</returns>
    public static TService? GetService<TService, TMetadata>(
        this ILazyEnumerable<TService, TMetadata> lazyServices,
        Func<TMetadata, bool>? predicate = null)
    {
        _ = lazyServices  ?? throw new ArgumentNullException(nameof(lazyServices));

        var filteredLazyServices = predicate == null ? lazyServices : lazyServices.Where(l => predicate(l.Metadata));
        using var enumerator = filteredLazyServices.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return default;
        }
        
        var lazyService = enumerator.Current;
        var priority = lazyService.Metadata.GetPriority();
        while (enumerator.MoveNext() && enumerator.Current.Metadata.GetPriority() == priority)
        {
            lazyService = enumerator.Current;
        }

        return lazyService.Value;
    }

    /// <summary>
    /// Gets the service based on the provided predicate.
    /// </summary>
    /// <remarks>
    /// If multiple are found with the same criteria, the last one with the highest priority is chosen,
    /// as the services are assumed to be ordered by their priority.
    /// </remarks>
    /// <param name="lazyServices">The lazy services.</param>
    /// <param name="predicate">the predicate function.</param>
    /// <returns>If found, the requested service, otherwise an exception occurs.</returns>
    public static TService GetRequiredService<TService, TMetadata>(
        this ILazyEnumerable<TService, TMetadata> lazyServices,
        Func<TMetadata, bool> predicate)
    {
        _ = lazyServices  ?? throw new ArgumentNullException(nameof(lazyServices));

        var service = lazyServices.GetService(predicate);
        return service ?? throw new InvalidOperationException(ServicesAbstractionsStrings.OrderedServiceFactoryCollection_service_with_requested_criteria_not_found);
    }
    
    /// <summary>
    /// Converts the enumeration of factory services to a <see cref="IFactoryEnumerable{TService,TMetadata}"/>.
    /// </summary>
    /// <param name="factoryServices">The services.</param>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <returns>An instance implementing <see cref="IFactoryEnumerable{TService,TMetadata}"/>.</returns>
    public static IFactoryEnumerable<TService, TMetadata> ToFactoryEnumerable<TService, TMetadata>(
        this IEnumerable<IFactory<TService, TMetadata>> factoryServices)
        where TService : notnull
    {
        return factoryServices as IFactoryEnumerable<TService, TMetadata>
               ?? new FactoryEnumerable<TService, TMetadata>(factoryServices ?? throw new ArgumentNullException(nameof(factoryServices)));
    }

    /// <summary>
    /// Gets the services in the appropriate order.
    /// </summary>
    /// <param name="factoryServices">The factory services.</param>
    /// <param name="predicate">Optional. Specifies a predicate.</param>
    /// <returns>
    /// The ordered services.
    /// </returns>
    public static IEnumerable<TService> SelectServices<TService, TMetadata>(
        this IFactoryEnumerable<TService, TMetadata> factoryServices,
        Func<TMetadata, bool>? predicate = null)
        where TService : notnull
    {
        _ = factoryServices  ?? throw new ArgumentNullException(nameof(factoryServices));

        return (predicate == null ? factoryServices : factoryServices.Where(f => predicate(f.Metadata)))
            .Select(e => e.GetService());
    }

    /// <summary>
    /// Tries to get the service based on the provided predicate.
    /// </summary>
    /// <remarks>
    /// If multiple are found with the same criteria, the last one with the highest priority is chosen,
    /// as the services are assumed to be ordered by their priority.
    /// </remarks>
    /// <param name="factoryServices">The factory services.</param>
    /// <param name="predicate">The predicate function.</param>
    /// <returns>The requested service or <c>null</c>.</returns>
    public static TService? GetService<TService, TMetadata>(
        this IFactoryEnumerable<TService, TMetadata> factoryServices,
        Func<TMetadata, bool>? predicate = null)
        where TService : notnull
    {
        _ = factoryServices  ?? throw new ArgumentNullException(nameof(factoryServices));

        var filteredFactoryServices = predicate == null ? factoryServices : factoryServices.Where(l => predicate(l.Metadata));
        using var enumerator = filteredFactoryServices.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return default;
        }
        
        var factoryService = enumerator.Current;
        var priority = factoryService.Metadata.GetPriority();
        while (enumerator.MoveNext() && enumerator.Current.Metadata.GetPriority() == priority)
        {
            factoryService = enumerator.Current;
        }

        return factoryService.GetService();
    }

    /// <summary>
    /// Gets the service based on the provided predicate.
    /// </summary>
    /// <remarks>
    /// If multiple are found with the same criteria, the last one with the highest priority is chosen,
    /// as the services are assumed to be ordered by their priority.
    /// </remarks>
    /// <param name="factoryServices">The factory services.</param>
    /// <param name="predicate">the predicate function.</param>
    /// <returns>If found, the requested service, otherwise an exception occurs.</returns>
    public static TService GetRequiredService<TService, TMetadata>(
        this IFactoryEnumerable<TService, TMetadata> factoryServices,
        Func<TMetadata, bool>? predicate = null)
        where TService : notnull
    {
        _ = factoryServices  ?? throw new ArgumentNullException(nameof(factoryServices));

        var service = factoryServices.GetService(predicate);
        return service ?? throw new InvalidOperationException(ServicesAbstractionsStrings.OrderedServiceFactoryCollection_service_with_requested_criteria_not_found);
    }

    /// <summary>
    /// Gets the priority from the provided metadata.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The priority value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Priority GetPriority(this object? metadata) => (metadata as IHasPriority)?.Priority ?? Priority.Normal;
}
