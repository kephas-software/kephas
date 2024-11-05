// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediatorHandlerResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Concurrent;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Extensions.Mediator;

/// <summary>
/// The default implementation of <see cref="IMediatorHandlerResolver"/>.
/// </summary>
public class MediatorHandlerResolver(IServiceInfoCollection serviceInfoCollection) : IMediatorHandlerResolver
{
    private readonly ConcurrentDictionary<Type, Func<IServiceProvider, IEnumerable<object>?>> enumerationCache = new();
    private readonly ConcurrentDictionary<Type, Func<IServiceProvider, object?>> singleCache = new();

    /// <summary>
    /// Gets an ordered list of matching handlers, matching the requested handler type.
    /// </summary>
    /// <returns>An ordered list of matching handlers.</returns>
    public IEnumerable<THandler>? TryGetMatchingHandlerEnumeration<THandler>(IServiceProvider serviceProvider)
        where THandler : notnull
    {
        var items = enumerationCache.GetOrAdd(typeof(THandler), ComputeEnumerationResolver)(serviceProvider);
        return items?.Cast<THandler>();
    }

    /// <summary>
    /// Tries to get a handler matching the requested handler type.
    /// </summary>
    /// <returns>An matching handler or null, if the handler type is not handled.</returns>
    public THandler? TryGetMatchingHandler<THandler>(IServiceProvider serviceProvider)
        where THandler : notnull =>
        (THandler?)singleCache.GetOrAdd(typeof(THandler), ComputeSingleResolver)(serviceProvider);

    private Func<IServiceProvider, object?> ComputeSingleResolver(Type handlerType)
    {
        var matchingTypes = GetMatchingTypes(handlerType);
        var exactMatchingTypes = matchingTypes.Where(t => t.type == handlerType).ToList();
        if (exactMatchingTypes.Count > 0)
        {
            // for single resolvers, if the handler type can be matched exactly, consider only that handler type
            matchingTypes = exactMatchingTypes;
        }

        return matchingTypes.Count switch
        {
            0 => _ => null,
            1 => sp => GetExactMatchingHandlers(sp, matchingTypes[0].type).GetRequiredService(),
            _ => GetCombinedSingleResolver(matchingTypes),
        };
    }

    private static IFactoryEnumerable<object, ServiceMetadata> GetExactMatchingHandlers(IServiceProvider sp, Type handlerType)
    {
        var factoryEnumerableType = typeof(IFactoryEnumerable<,>).MakeGenericType(handlerType, typeof(ServiceMetadata));
        return (IFactoryEnumerable<object, ServiceMetadata>)sp.GetRequiredService(factoryEnumerableType);
    }

    private Func<IServiceProvider, IEnumerable<object>?> ComputeEnumerationResolver(Type handlerType)
    {
        var matchingTypes = GetMatchingTypes(handlerType);
        return matchingTypes.Count switch
        {
            0 => _ => null,
            1 => sp => GetExactMatchingHandlers(sp, matchingTypes[0].type).Select(f => f.GetService()),
            _ => GetCombinedEnumerationResolver(matchingTypes),
        };
    }

    private Func<IServiceProvider,IEnumerable<object>?> GetCombinedEnumerationResolver(IList<(Type type, bool isFromOpenGeneric, ServiceMetadata? metadata)> matchingTypes)
    {
        var resolvers = GetHandlerResolvers(matchingTypes);
        return sp => new FactoryEnumerable<object, ServiceMetadata>(resolvers.SelectMany(r => r(sp))).SelectServices();
    }

    private Func<IServiceProvider, object?> GetCombinedSingleResolver(IList<(Type type, bool isFromOpenGeneric, ServiceMetadata? metadata)> matchingTypes)
    {
        var resolvers = GetHandlerResolvers(matchingTypes);
        return sp => new FactoryEnumerable<object, ServiceMetadata>(resolvers.SelectMany(r => r(sp))).GetRequiredService();
    }

    private static List<Func<IServiceProvider, IEnumerable<IFactory<object, ServiceMetadata>>>> GetHandlerResolvers(IList<(Type type, bool isFromOpenGeneric, ServiceMetadata? metadata)> matchingTypes)
    {
        var resolvers = new List<Func<IServiceProvider, IEnumerable<IFactory<object, ServiceMetadata>>>>();
        foreach (var (matchingType, isFromOpenGeneric, metadata) in matchingTypes)
        {
            if (isFromOpenGeneric)
            {
                resolvers.Add(sp => [new Factory<object, ServiceMetadata>(() => sp.GetRequiredService(matchingType), metadata!)]);
            }
            else
            {
                var factoryEnumerableType = typeof(IEnumerable<>).MakeGenericType(typeof(IFactory<,>).MakeGenericType(matchingType, typeof(ServiceMetadata)));
                resolvers.Add(sp => (IEnumerable<IFactory<object, ServiceMetadata>>)sp.GetRequiredService(factoryEnumerableType));
            }
        }

        return resolvers;
    }

    private IList<(Type type, bool isFromOpenGeneric, ServiceMetadata? metadata)> GetMatchingTypes(Type handlerType)
    {
        // TODO how to handle open generics?
        
        if (handlerType.IsConstructedGenericType)
        {
            // a match on the message type (request or notification) should suffice.
            var messageType = handlerType.GenericTypeArguments[0];
            var openGenericHandlerType = handlerType.GetGenericTypeDefinition();
            return serviceInfoCollection
                .Where(si => si.ServiceType?.IsConstructedGenericType is true &&
                             si.ServiceType.GetGenericTypeDefinition() == openGenericHandlerType &&
                             si.ServiceType.GenericTypeArguments[0].IsAssignableFrom(messageType))
                .Select(si => (si.ServiceType!, false, (ServiceMetadata?)null))
                .Union(serviceInfoCollection
                    .Where(si => si.ServiceType == openGenericHandlerType)
                    .Select(si => (si.ImplementationType!.MakeGenericType(handlerType.GenericTypeArguments), true, (ServiceMetadata?)new ServiceMetadata(si.Metadata))))
                .ToList();
        }

        return [];
    }
}