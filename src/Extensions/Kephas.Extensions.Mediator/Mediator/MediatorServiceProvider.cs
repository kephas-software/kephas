// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediatorService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using Kephas.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Extensions.Mediator;

/// <summary>
/// The default implementation of the <see cref="IMediatorServiceProvider"/>.
/// </summary>
public class MediatorServiceProvider(
    IServiceProvider serviceProvider,
    IMediatorHandlerResolver handlerResolver) : IMediatorServiceProvider
{
    private static readonly MethodInfo TryGetEnumerableServiceMethod = ReflectionHelper.GetGenericMethodOf(_ =>
        ((MediatorServiceProvider) null!).TryGetEnumerableService<IEnumerable<string>, string>());

    private static readonly MethodInfo TryGetSingleServiceMethod = ReflectionHelper.GetGenericMethodOf(_ =>
        ((MediatorServiceProvider)null!).TryGetSingleService<string>());

    /// <summary>Gets the service object of the specified type.</summary>
    /// <param name="serviceType">An object that specifies the type of service object to get.</param>
    /// <returns>A service object of type <paramref name="serviceType" />.
    /// -or-
    /// <see langword="null" /> if there is no service object of type <paramref name="serviceType" />.</returns>
    public object GetService(Type serviceType)
    {
        try
        {
            var handlerType = serviceType.TryGetEnumerableItemType();
            return handlerType is null
                ? GetSingleRequiredService(serviceType)
                : GetEnumerableRequiredService(serviceType, handlerType);
        }
        catch (Exception ex)
        {
            throw new MediatorServiceResolutionException($"Errors occurred while resolving service {serviceType}. Check the inner exception for further information.", ex)
            {
                ServiceType = serviceType,
            };
        }
    }

    private object GetEnumerableRequiredService(Type enumerableServiceType, Type handlerType)
    {
        var getEnumerableRequiredService = TryGetEnumerableServiceMethod.MakeGenericMethod(enumerableServiceType, handlerType);
        return getEnumerableRequiredService.Call(this)
               ?? serviceProvider.GetService(enumerableServiceType)!;
    }

    private TEnumerable? TryGetEnumerableService<TEnumerable, THandler>()
        where TEnumerable : class, IEnumerable<THandler>
        where THandler : notnull
    {
        var handlerType = typeof(THandler);
        return handlerType.IsConstructedGenericType
            ? (TEnumerable?)handlerResolver.TryGetMatchingHandlerEnumeration<THandler>(serviceProvider)
            : null;
    }

    private object GetSingleRequiredService(Type handlerType)
    {
        var tryGetSingleService = TryGetSingleServiceMethod.MakeGenericMethod(handlerType);
        var service = tryGetSingleService.Call(this);
        return service
               ?? ServiceProviderServiceExtensions.GetRequiredService(serviceProvider, handlerType);
    }

    private THandler? TryGetSingleService<THandler>()
        where THandler : class
    {
        var handlerType = typeof(THandler);
        return handlerType.IsConstructedGenericType
            ? handlerResolver.TryGetMatchingHandler<THandler>(serviceProvider)
            : null;
    }
}
