// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediatorService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using Kephas.DependencyInjection;
using Kephas.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kephas.Extensions.Mediator;

/// <summary>
/// The default implementation of the <see cref="IMediatorServiceProvider"/>.
/// </summary>
public class MediatorServiceProvider(
    IServiceProvider serviceProvider,
    ILogger<MediatorServiceProvider>? logger = null) : IMediatorServiceProvider
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
            var itemType = serviceType.TryGetEnumerableItemType();
            return itemType is null
                ? GetSingleRequiredService(serviceType)
                : GetEnumerableRequiredService(serviceType, itemType);
        }
        catch (Exception ex)
        {
            throw new MediatorServiceResolutionException($"Errors occurred while resolving service {serviceType}. Check the inner exception for further information.", ex)
            {
                ServiceType = serviceType,
            };
        }
    }

    private TEnumerable? TryGetEnumerableService<TEnumerable, TItem>()
        where TEnumerable : class, IEnumerable<TItem>
    {
        var itemType = typeof(TItem);
        if (itemType.IsConstructedGenericType)
        {
            var openGenericType = itemType.GetGenericTypeDefinition();
            if (openGenericType == typeof(IPipelineBehavior<,>))
            {
                return TryGetPipelineBehaviors<TEnumerable, TItem>(itemType);
            }

            if (openGenericType == typeof(INotificationHandler<>))
            {
                return TryGetNotificationHandlers<TEnumerable, TItem>(itemType);
            }

            if (openGenericType == typeof(IStreamPipelineBehavior<,>))
            {
                return TryGetStreamPipelineBehaviors<TEnumerable, TItem>(itemType);
            }
        }

        return null;
    }

    private TEnumerable? TryGetNotificationHandlers<TEnumerable, TItem>(Type itemType)
        where TEnumerable : class, IEnumerable<TItem> =>
        TryGet1ArgHandlers<TEnumerable, TItem, INotificationHandlerBase, NotificationMetadata>(
            itemType,
            (m, notificationType) =>
                m.NotificationType?.IsAssignableFrom(notificationType) is true);

    private TEnumerable? TryGetPipelineBehaviors<TEnumerable, TItem>(Type itemType)
        where TEnumerable : class, IEnumerable<TItem> =>
        TryGet2ArgsHandlers<TEnumerable, TItem, IPipelineBehaviorBase, RequestResponseMetadata>(
            itemType,
            (m, requestType, responseType) =>
                m.ResponseType == responseType && m.RequestType?.IsAssignableFrom(requestType) is true);

    private TEnumerable? TryGetStreamPipelineBehaviors<TEnumerable, TItem>(Type itemType)
        where TEnumerable : class, IEnumerable<TItem> =>
        TryGet2ArgsHandlers<TEnumerable, TItem, IStreamPipelineBehaviorBase, RequestResponseMetadata>(
            itemType,
            (m, requestType, responseType) =>
                m.ResponseType == responseType && m.RequestType?.IsAssignableFrom(requestType) is true);

    private TEnumerable? TryGet2ArgsHandlers<TEnumerable, TItem, TDeclaredBase, TMetadata>(Type itemType, Func<TMetadata, Type, Type, bool> filter)
        where TEnumerable : class, IEnumerable<TItem>
    {
        var typeArg1 = itemType.GenericTypeArguments[0];
        var typeArg2 = itemType.GenericTypeArguments[1];
        var factoryEnumeration = serviceProvider.GetRequiredService<IFactoryEnumerable<TDeclaredBase, TMetadata>>();
        var handlers = factoryEnumeration.SelectServices(m => filter(m, typeArg1, typeArg2))
            .Cast<TItem>()
            .ToList();
        if (handlers.Count == 0)
        {
            return null;
        }

        handlers.AddRange(serviceProvider.GetServices<TItem>());
        return (TEnumerable)(object)handlers;
    }

    private TEnumerable? TryGet1ArgHandlers<TEnumerable, TItem, TDeclaredBase, TMetadata>(Type itemType, Func<TMetadata, Type, bool> filter)
        where TEnumerable : class, IEnumerable<TItem>
    {
        var typeArg = itemType.GenericTypeArguments[0];
        var factoryEnumeration = serviceProvider.GetRequiredService<IFactoryEnumerable<TDeclaredBase, TMetadata>>();
        var handlers = factoryEnumeration.SelectServices(m => filter(m, typeArg))
            .Cast<TItem>()
            .ToList();
        if (handlers.Count == 0)
        {
            return null;
        }

        handlers.AddRange(serviceProvider.GetServices<TItem>());
        return (TEnumerable)(object)handlers;
    }

    private object GetEnumerableRequiredService(Type enumerableServiceType, Type itemServiceType)
    {
        var getEnumerableRequiredService = TryGetEnumerableServiceMethod.MakeGenericMethod(enumerableServiceType, itemServiceType);
        return getEnumerableRequiredService.Call(this) ?? serviceProvider.GetService(enumerableServiceType)!;
    }

    private object? TryGetSingleService<TService>()
    {
        var serviceType = typeof(TService);

        // for non handler types redirect to the service provider
        // this might change in the future, if additional functionality is required.
        if (serviceType.IsConstructedGenericType)
        {
            var openServiceType = serviceType.GetGenericTypeDefinition();
            if (openServiceType == typeof(IRequestHandler<,>))
            {
                return TryGetRequestResponseHandler<TService, IRequestResponseHandlerBase>();
            }

            if (openServiceType == typeof(IStreamRequestHandler<,>))
            {
                return TryGetRequestResponseHandler<TService, IStreamRequestHandlerBase>();
            }
            
            if (openServiceType == typeof(IRequestHandler<>))
            {
                return TryGetRequestHandler<TService, IRequestHandlerBase>();
            }
        }

        return null;
    }

    private object GetSingleRequiredService(Type serviceType)
    {
        var tryGetSingleService = TryGetSingleServiceMethod.MakeGenericMethod(serviceType);
        var service = tryGetSingleService.Call(this);
        if (service is not null)
        {
            return service;
        }
        
        // if no handler is found over the service selector, try the classical way,
        // maybe the developers did not use the base classes.
        var fallbackService = ServiceProviderServiceExtensions.GetRequiredService(serviceProvider, serviceType);
        if (fallbackService is IDeclaredHandlerService)
        {
            // if the handler service is now found over the classical way, it means it was filtered out
            // by the service selector - do not return it, instead throw an explicit exception.
            throw new InvalidOperationException($"No valid handler found for '{serviceType}'. Existing handler '{fallbackService.GetType()}' might have been disabled by a behavior.");
        }

        // it is recommended to use the new approach, as it provides advanced control over the service selector pipelines,
        // so make sure to log this fallback case as a warning, but only in the environment where the debug level is enabled.
        if (logger?.IsEnabled(LogLevel.Debug) is true)
        {
            logger.LogWarning(
                "Identified handler '{HandlerType}' for service type '{ServiceType}'. However, it is strongly recommended to use the declared base classes instead of MediatR interfaces, to allow use of the behaviors.",
                fallbackService.GetType(),
                serviceType);
        }

        return fallbackService;
    }

    private object? TryGetRequestHandler<TService, TDeclaredBase>() =>
        TryGet1ArgHandler<TService, TDeclaredBase, RequestResponseMetadata>(
            (m, requestType) => m.RequestType == requestType,
            (m, requestType) => m.RequestType?.IsAssignableFrom(requestType) is true);

    private object? TryGetRequestResponseHandler<TService, TDeclaredBase>() =>
        TryGet2ArgsHandler<TService, TDeclaredBase, RequestResponseMetadata>(
            (m, requestType, responseType) => m.ResponseType == responseType && m.RequestType == requestType,
            (m, requestType, responseType) => m.ResponseType == responseType && m.RequestType?.IsAssignableFrom(requestType) is true);

    private object? TryGet2ArgsHandler<TService, TDeclaredBase, TMetadata>(
        Func<TMetadata, Type, Type, bool> exactMatch,
        Func<TMetadata, Type, Type, bool> polymorphicMatch)
    {
        // identify a handler implementing the TService
        // first, use an exact match, and if none is found, use the polymorphic match
        var serviceType = typeof(TService);
        var typeArg1 = serviceType.GenericTypeArguments[0];
        var typeArg2 = serviceType.GenericTypeArguments[1];

        var factoryEnumerable = serviceProvider.GetRequiredService<IFactoryEnumerable<TDeclaredBase, TMetadata>>();
        var service = factoryEnumerable.GetService(m => exactMatch(m, typeArg1, typeArg2))
                      ?? factoryEnumerable.GetService(m => polymorphicMatch(m, typeArg1, typeArg2));

        return service;
    }

    private object? TryGet1ArgHandler<TService, TDeclaredBase, TMetadata>(
        Func<TMetadata, Type, bool> exactMatch,
        Func<TMetadata, Type, bool> polymorphicMatch)
    {
        // identify a handler implementing the TService
        // first, use an exact match, and if none is found, use the polymorphic match
        var serviceType = typeof(TService);
        var typeArg = serviceType.GenericTypeArguments[0];

        var factoryEnumerable = serviceProvider.GetRequiredService<IFactoryEnumerable<TDeclaredBase, TMetadata>>();
        var service = factoryEnumerable.GetService(m => exactMatch(m, typeArg))
                      ?? factoryEnumerable.GetService(m => polymorphicMatch(m, typeArg));

        return service;
    }
}
