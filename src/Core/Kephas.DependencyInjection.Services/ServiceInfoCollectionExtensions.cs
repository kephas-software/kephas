// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.Resources;

namespace Kephas;

/// <summary>
/// Extensions for collections of services.
/// </summary>
public static class ServiceInfoCollectionExtensions
{
    /// <summary>
    /// Registers the provided service instance as transient.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="services">The application services.</param>
    /// <param name="serviceInstance">The service instance.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// The provided <paramref name="services"/>.
    /// </returns>
    public static IServiceInfoCollection Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(
        this IServiceInfoCollection services,
        TService serviceInstance,
        Action<ServiceInfo>? builder = null)
        where TService : class
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

        return services.AddService(
            typeof(TService),
            serviceInstance,
            b =>
            {
                builder?.Invoke(b);
            });
    }

    internal static IServiceInfoCollection Replace<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(
        this IServiceInfoCollection services,
        TService serviceInstance,
        Action<ServiceInfo>? builder = null)
        where TService : class
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

        return services.ReplaceService(
            typeof(TService),
            serviceInstance,
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service with implementation type, by default as transient.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">Type of the service implementation.</typeparam>
    /// <param name="services">The application services.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// The provided <paramref name="services"/>.
    /// </returns>
    public static IServiceInfoCollection Add<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TImplementation>(
        this IServiceInfoCollection services,
        Action<ServiceInfo>? builder = null)
        where TService : class
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        return services.AddService(
            typeof(TService),
            typeof(TImplementation),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service with implementation type, by default as transient.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="services">The application services.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// The provided <paramref name="services"/>.
    /// </returns>
    public static IServiceInfoCollection Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(this IServiceInfoCollection services, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType, Action<ServiceInfo>? builder = null)
        where TService : class
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        return services.AddService(
            typeof(TService),
            serviceType,
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service, by default as transient factory.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="services">The application services.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// The provided <paramref name="services"/>.
    /// </returns>
    public static IServiceInfoCollection Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(
        this IServiceInfoCollection services,
        Func<TService> serviceFactory,
        Action<ServiceInfo>? builder = null)
        where TService : class
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return services.AddService(
            typeof(TService),
            (Func<IServiceProvider, object>)(_ => serviceFactory()),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service, by default as transient factory.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="services">The application services.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// The provided <paramref name="services"/>.
    /// </returns>
    public static IServiceInfoCollection Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(
        this IServiceInfoCollection services,
        Func<IServiceProvider, TService> serviceFactory,
        Action<ServiceInfo>? builder = null)
        where TService : class
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return services.AddService(
            typeof(TService),
            (Func<IServiceProvider, object>)(sp => serviceFactory(sp)),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service, by default as transient factory.
    /// </summary>
    /// <param name="services">The application services.</param>
    /// <param name="contractType">Type of the service contract.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// The provided <paramref name="services"/>.
    /// </returns>
    public static IServiceInfoCollection Add(
        this IServiceInfoCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        Func<object> serviceFactory,
        Action<ServiceInfo>? builder = null)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return services.AddService(
            contractType,
            (Func<IServiceProvider, object>)(_ => serviceFactory()),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service, by default as transient factory.
    /// </summary>
    /// <param name="services">The application services.</param>
    /// <param name="contractType">Type of the service contract.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// The provided <paramref name="services"/>.
    /// </returns>
    public static IServiceInfoCollection Add(
        this IServiceInfoCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        Func<IServiceProvider, object> serviceFactory,
        Action<ServiceInfo>? builder = null)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return services.AddService(
            contractType,
            (Func<IServiceProvider, object>)(sp => serviceFactory(sp)),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service, by default as transient.
    /// </summary>
    /// <param name="services">The application services.</param>
    /// <param name="contractType">Type of the service contract.</param>
    /// <param name="serviceType">The service implementation type.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// The provided <paramref name="services"/>.
    /// </returns>
    public static IServiceInfoCollection Add(
        this IServiceInfoCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        Action<ServiceInfo>? builder = null)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        return services.AddService(
            contractType,
            serviceType,
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service instance, by default as transient.
    /// </summary>
    /// <param name="services">The application services.</param>
    /// <param name="contractType">Type of the service contract.</param>
    /// <param name="serviceInstance">The service instance.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// The provided <paramref name="services"/>.
    /// </returns>
    public static IServiceInfoCollection Add(
        this IServiceInfoCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        object serviceInstance,
        Action<ServiceInfo>? builder = null)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

        return services.AddService(
            contractType,
            serviceInstance,
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Tries to get the service instance from the registered services,
    /// in case the service was registered using an instance.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="contractType">The contract type.</param>
    /// <returns>The service instance or <c>null</c>.</returns>
    public static object? TryGetServiceInstance(this IServiceInfoCollection services, Type contractType)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));

        return services.LastOrDefault(i => i.ServiceType == contractType && i.Instance is not null)?.Instance;
    }

    /// <summary>
    /// Tries to get the service instance from the registered services
    /// </summary>
    /// <typeparam name="T">The contract type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service instance or <c>null</c>.</returns>
    public static T? TryGetServiceInstance<T>(this IServiceInfoCollection services)
        where T : class
        => services.TryGetServiceInstance(typeof(T)) as T;

    /// <summary>
    /// Tries to get the service instance from the registered services,
    /// in case the service was registered using an instance.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="contractType">The contract type.</param>
    /// <returns>The service instance or <c>null</c>.</returns>
    public static object GetServiceInstance(this IServiceInfoCollection services, Type contractType)
        => services.TryGetServiceInstance(contractType)
           ?? throw new ArgumentException(string.Format(ServicesAbstractionsStrings.ServiceInstanceNotRegistered, contractType), nameof(contractType));

    /// <summary>
    /// Gets the service instance from the registered services
    /// </summary>
    /// <typeparam name="T">The contract type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service instance or <c>null</c>.</returns>
    [return: NotNull]
    public static T GetServiceInstance<T>(this IServiceInfoCollection services)
        where T : class
        => (T)services.GetServiceInstance(typeof(T));

    /// <summary>
    /// Registers the provided service using a registration builder.
    /// </summary>
    /// <param name="services">The application services.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="instancingStrategy">The instancing strategy.</param>
    /// <param name="builder">The builder.</param>
    /// <returns>
    /// The provided <paramref name="services"/>.
    /// </returns>
    internal static IServiceInfoCollection AddService(
        this IServiceInfoCollection services,
        Type serviceType,
        object instancingStrategy,
        Action<ServiceInfo>? builder = null)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        instancingStrategy = instancingStrategy ?? throw new ArgumentNullException(nameof(instancingStrategy));

        var serviceInfo = new ServiceInfo(serviceType, instancingStrategy);
        builder?.Invoke(serviceInfo);
        services.Add(serviceInfo);

        return services;
    }

    internal static IServiceInfoCollection ReplaceService(
        this IServiceInfoCollection services,
        Type serviceType,
        object instancingStrategy,
        Action<ServiceInfo>? builder = null)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        instancingStrategy = instancingStrategy ?? throw new ArgumentNullException(nameof(instancingStrategy));

        var serviceInfo = new ServiceInfo(serviceType, instancingStrategy);
        builder?.Invoke(serviceInfo);
        var toRemoveItems = services.Where(s => s.ServiceType == serviceInfo.ServiceType).ToList();
        toRemoveItems.ForEach(item => services.Remove(item));
        services.Add(serviceInfo);

        return services;
    }
}
