// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenGenericServiceHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Extensions.DependencyInjection;

internal static class EnumerableServiceHelper
{
    public static IEnumerable<IFactory<TService, TMetadata>> EnumerateFactories<TService, TMetadata>(this IServiceProvider serviceProvider)
        where TService : notnull
    {
        var factories = serviceProvider.GetRequiredService<IEnumerable<IFactory<TService, TMetadata>>>();
        return typeof(TService).IsGenericType
            ? factories.Union(GetOpenGenericFactories<TService, TMetadata>(serviceProvider))
            : factories;
    }

    public static IEnumerable<IFactory<TService, TMetadata>> GetOpenGenericFactories<TService, TMetadata>(IServiceProvider serviceProvider)
        where TService : notnull =>
        GetOpenGenericServiceFactories<TService, TMetadata, IFactory<TService, TMetadata>>(
            serviceProvider,
            (factory, metadata) => new Factory<TService, TMetadata>(factory, metadata));

    public static IEnumerable<Lazy<TService, TMetadata>> EnumerateLazies<TService, TMetadata>(this IServiceProvider serviceProvider)
        where TService : notnull
    {
        var lazies = serviceProvider.GetRequiredService<IEnumerable<Lazy<TService, TMetadata>>>();
        return typeof(TService).IsGenericType
            ? lazies.Union(GetOpenGenericLazies<TService, TMetadata>(serviceProvider))
            : lazies;
    }

    private static IEnumerable<Lazy<TService, TMetadata>> GetOpenGenericLazies<TService, TMetadata>(IServiceProvider serviceProvider)
        where TService : notnull =>
        GetOpenGenericServiceFactories<TService, TMetadata, Lazy<TService, TMetadata>>(
            serviceProvider,
            (factory, metadata) => new Lazy<TService, TMetadata>(factory, metadata));

    private static IEnumerable<TItem> GetOpenGenericServiceFactories<TService, TMetadata, TItem>(
        IServiceProvider serviceProvider,
        Func<Func<TService>, TMetadata, TItem> itemFactory)
        where TService : notnull
    {
        var genericServiceType = typeof(TService);
        if (!genericServiceType.IsGenericType || genericServiceType.IsGenericTypeDefinition)
        {
            yield break;
        }
        
        var openGenericMetadataProvider = serviceProvider.GetRequiredService<IOpenGenericMetadataProvider>();
        var genericServiceTypeDefinition = genericServiceType.GetGenericTypeDefinition();
        if (!openGenericMetadataProvider.TryGetValue(genericServiceTypeDefinition, out var registrations))
        {
            yield break;
        }

        foreach (var registration in registrations)
        {
            var (implementationType, metadata) = registration;
            if (!implementationType.IsGenericTypeDefinition)
            {
                continue;
            }

            var closedImplementationType = TryGetClosedImplementationType(implementationType, genericServiceType);
            if (closedImplementationType is null)
            {
                continue;
            }

            // do a copy of the metadata to avoid modifying the generic definition one
            var genericEnrichedMetadata = new Dictionary<string, object?>(metadata);
            ServiceHelper.AddGenericServiceMetadata(genericEnrichedMetadata, closedImplementationType, genericServiceType);
            yield return itemFactory(
                () => (TService)serviceProvider.GetRequiredService(closedImplementationType),
                ServiceHelper.GetServiceMetadata<TMetadata>(genericEnrichedMetadata));
        }
    }

    private static Type? TryGetClosedImplementationType(Type implementationType, Type genericServiceType)
    {
        try
        {
            // can crash if the generic arguments are not compatible with the generic service type
            // like not matching constraints
            return implementationType.MakeGenericType(genericServiceType.GenericTypeArguments);
        }
        catch
        {
            return null;
        }
    }
}