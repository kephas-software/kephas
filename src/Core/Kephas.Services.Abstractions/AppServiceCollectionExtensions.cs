// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Kephas.Services.Reflection;
using Kephas.Services.Resources;

namespace Kephas;

/// <summary>
/// Extensions for <see cref="IAppServiceCollection"/>.
/// </summary>
public static class AppServiceCollectionExtensions
{
    /// <summary>
    /// Registers the provided service instance as transient, if not previously registered.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <param name="appServices">The application services.</param>
    /// <param name="serviceInstance">The service instance.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection TryAdd<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(
        this IAppServiceCollection appServices,
        TContract serviceInstance,
        Action<AppServiceInfo>? builder = null)
        where TContract : class
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

        return appServices.TryAddService(
            typeof(TContract),
            serviceInstance,
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service instance as transient.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <param name="appServices">The application services.</param>
    /// <param name="serviceInstance">The service instance.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(
        this IAppServiceCollection appServices,
        TContract serviceInstance,
        Action<AppServiceInfo>? builder = null)
        where TContract : class
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

        return appServices.AddService(
            typeof(TContract),
            serviceInstance,
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Replaces the provided service instance, registering it as transient.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <param name="appServices">The application services.</param>
    /// <param name="serviceInstance">The service instance.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Replace<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(
        this IAppServiceCollection appServices,
        TContract serviceInstance,
        Action<AppServiceInfo>? builder = null)
        where TContract : class
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

        return appServices.ReplaceService(
            typeof(TContract),
            serviceInstance,
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service with implementation type, by default as transient, if not previously registered.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <typeparam name="TService">Type of the service implementation.</typeparam>
    /// <param name="appServices">The application services.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection TryAdd<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(
        this IAppServiceCollection appServices,
        Action<AppServiceInfo>? builder = null)
        where TContract : class
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));

        return appServices.TryAddService(
            typeof(TContract),
            typeof(TService),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service with implementation type, by default as transient.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <typeparam name="TService">Type of the service implementation.</typeparam>
    /// <param name="appServices">The application services.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Add<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(
        this IAppServiceCollection appServices,
        Action<AppServiceInfo>? builder = null)
        where TContract : class
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));

        return appServices.AddService(
            typeof(TContract),
            typeof(TService),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Replaces the provided service with implementation type, registering it by default as transient.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <typeparam name="TService">Type of the service implementation.</typeparam>
    /// <param name="appServices">The application services.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Replace<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(this IAppServiceCollection appServices, Action<AppServiceInfo>? builder = null)
        where TContract : class
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));

        return appServices.ReplaceService(
            typeof(TContract),
            typeof(TService),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service with implementation type, by default as transient, if not previously registered.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <param name="appServices">The application services.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection TryAdd<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(this IAppServiceCollection appServices, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType, Action<AppServiceInfo>? builder = null)
        where TContract : class
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        return appServices.TryAddService(
            typeof(TContract),
            serviceType,
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service with implementation type, by default as transient.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <param name="appServices">The application services.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(this IAppServiceCollection appServices, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType, Action<AppServiceInfo>? builder = null)
        where TContract : class
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        return appServices.AddService(
            typeof(TContract),
            serviceType,
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Replaces the provided service with implementation type, registering it by default as transient.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <param name="appServices">The application services.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Replace<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(this IAppServiceCollection appServices, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType, Action<AppServiceInfo>? builder = null)
        where TContract : class
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        return appServices.ReplaceService(
            typeof(TContract),
            serviceType,
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service, by default as transient factory, if not previously registered.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <param name="appServices">The application services.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection TryAdd<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(
        this IAppServiceCollection appServices,
        Func<TContract> serviceFactory,
        Action<AppServiceInfo>? builder = null)
        where TContract : class
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return appServices.TryAddService(
            typeof(TContract),
            (Func<IServiceProvider, object>)(_ => serviceFactory()),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service, by default as transient factory.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <param name="appServices">The application services.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(
        this IAppServiceCollection appServices,
        Func<TContract> serviceFactory,
        Action<AppServiceInfo>? builder = null)
        where TContract : class
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return appServices.AddService(
            typeof(TContract),
            (Func<IServiceProvider, object>)(_ => serviceFactory()),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Replaces the provided service, registering it by default as transient factory.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <param name="appServices">The application services.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Replace<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(
        this IAppServiceCollection appServices,
        Func<TContract> serviceFactory,
        Action<AppServiceInfo>? builder = null)
        where TContract : class
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return appServices.ReplaceService(
            typeof(TContract),
            (Func<IServiceProvider, object>)(_ => serviceFactory()),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service, by default as transient factory, if not previously registered.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <param name="appServices">The application services.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection TryAdd<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(
        this IAppServiceCollection appServices,
        Func<IServiceProvider, TContract> serviceFactory,
        Action<AppServiceInfo>? builder = null)
        where TContract : class
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return appServices.TryAddService(
            typeof(TContract),
            (Func<IServiceProvider, object>)(injector => serviceFactory(injector)),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service, by default as transient factory.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <param name="appServices">The application services.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(
        this IAppServiceCollection appServices,
        Func<IServiceProvider, TContract> serviceFactory,
        Action<AppServiceInfo>? builder = null)
        where TContract : class
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return appServices.AddService(
            typeof(TContract),
            (Func<IServiceProvider, object>)(injector => serviceFactory(injector)),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Replaces the provided service, registering it by default as transient factory.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <param name="appServices">The application services.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Replace<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(
        this IAppServiceCollection appServices,
        Func<IServiceProvider, TContract> serviceFactory,
        Action<AppServiceInfo>? builder = null)
        where TContract : class
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return appServices.ReplaceService(
            typeof(TContract),
            (Func<IServiceProvider, object>)(injector => serviceFactory(injector)),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service, by default as transient factory, if not previously registered.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <param name="contractType">Type of the service contract.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection TryAdd(
        this IAppServiceCollection appServices,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        Func<object> serviceFactory,
        Action<AppServiceInfo>? builder = null)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return appServices.TryAddService(
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
    /// <param name="appServices">The application services.</param>
    /// <param name="contractType">Type of the service contract.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Add(
        this IAppServiceCollection appServices,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        Func<object> serviceFactory,
        Action<AppServiceInfo>? builder = null)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return appServices.AddService(
            contractType,
            (Func<IServiceProvider, object>)(_ => (object)serviceFactory()),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Replaces the provided service, registering it by default as transient factory.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <param name="contractType">Type of the service contract.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Replace(
        this IAppServiceCollection appServices,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        Func<object> serviceFactory,
        Action<AppServiceInfo>? builder = null)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return appServices.ReplaceService(
            contractType,
            (Func<IServiceProvider, object>)(_ => (object)serviceFactory()),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service, by default as transient factory, if not previously registered.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <param name="contractType">Type of the service contract.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection TryAdd(
        this IAppServiceCollection appServices,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        Func<IServiceProvider, object> serviceFactory,
        Action<AppServiceInfo>? builder = null)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return appServices.TryAddService(
            contractType,
            (Func<IServiceProvider, object>)(injector => (object)serviceFactory(injector)),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service, by default as transient factory.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <param name="contractType">Type of the service contract.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Add(
        this IAppServiceCollection appServices,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        Func<IServiceProvider, object> serviceFactory,
        Action<AppServiceInfo>? builder = null)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return appServices.AddService(
            contractType,
            (Func<IServiceProvider, object>)(injector => (object)serviceFactory(injector)),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Replaces the provided service, registering it by default as transient factory.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <param name="contractType">Type of the service contract.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Replace(
        this IAppServiceCollection appServices,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        Func<IServiceProvider, object> serviceFactory,
        Action<AppServiceInfo>? builder = null)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return appServices.ReplaceService(
            contractType,
            (Func<IServiceProvider, object>)(injector => (object)serviceFactory(injector)),
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Registers the provided service, by default as transient, if not previously registered.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <param name="contractType">Type of the service contract.</param>
    /// <param name="serviceType">The service implementation type.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection TryAdd(
        this IAppServiceCollection appServices,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        Action<AppServiceInfo>? builder = null)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        appServices.TryAddService(
            contractType,
            serviceType,
            b =>
            {
                builder?.Invoke(b);
            });
        return appServices;
    }

    /// <summary>
    /// Registers the provided service, by default as transient.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <param name="contractType">Type of the service contract.</param>
    /// <param name="serviceType">The service implementation type.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Add(
        this IAppServiceCollection appServices,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        Action<AppServiceInfo>? builder = null)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        return appServices.AddService(
            contractType,
            serviceType,
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Replaces the provided service, registering it by default as transient.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <param name="contractType">Type of the service contract.</param>
    /// <param name="serviceType">The service implementation type.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Replace(
        this IAppServiceCollection appServices,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        Action<AppServiceInfo>? builder = null)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        appServices.ReplaceService(
            contractType,
            serviceType,
            b =>
            {
                builder?.Invoke(b);
            });
        return appServices;
    }

    /// <summary>
    /// Registers the provided service instance, by default as transient, if not previously registered.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <param name="contractType">Type of the service contract.</param>
    /// <param name="serviceInstance">The service instance.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection TryAdd(
        this IAppServiceCollection appServices,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        object serviceInstance,
        Action<AppServiceInfo>? builder = null)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

        appServices.TryAddService(
            contractType,
            serviceInstance,
            b =>
            {
                builder?.Invoke(b);
            });
        return appServices;
    }

    /// <summary>
    /// Registers the provided service instance, by default as transient.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <param name="contractType">Type of the service contract.</param>
    /// <param name="serviceInstance">The service instance.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Add(
        this IAppServiceCollection appServices,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        object serviceInstance,
        Action<AppServiceInfo>? builder = null)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

        return appServices.AddService(
            contractType,
            serviceInstance,
            b =>
            {
                builder?.Invoke(b);
            });
    }

    /// <summary>
    /// Replaces the provided service instance, registering it by default as transient.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <param name="contractType">Type of the service contract.</param>
    /// <param name="serviceInstance">The service instance.</param>
    /// <param name="builder">Optional. The registration builder.</param>
    /// <returns>
    /// This <paramref name="appServices"/>.
    /// </returns>
    public static IAppServiceCollection Replace(
        this IAppServiceCollection appServices,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        object serviceInstance,
        Action<AppServiceInfo>? builder = null)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

        appServices.ReplaceService(
            contractType,
            serviceInstance,
            b =>
            {
                builder?.Invoke(b);
            });
        return appServices;
    }

    /// <summary>
    /// Tries to get the service instance from the registered services,
    /// in case the service was registered using an instance.
    /// </summary>
    /// <param name="appServices">The service collection.</param>
    /// <param name="contractType">The contract type.</param>
    /// <returns>The service instance or <c>null</c>.</returns>
    public static object? TryGetServiceInstance(this IAppServiceCollection appServices, Type contractType)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));

        return appServices.LastOrDefault(i => i.ContractType == contractType && i.Instance is not null)?.Instance;
    }

    /// <summary>
    /// Tries to get the service instance from the registered services
    /// </summary>
    /// <typeparam name="T">The contract type.</typeparam>
    /// <param name="appServices">The service collection.</param>
    /// <returns>The service instance or <c>null</c>.</returns>
    public static T? TryGetServiceInstance<T>(this IAppServiceCollection appServices)
        where T : class
        => appServices.TryGetServiceInstance(typeof(T)) as T;

    /// <summary>
    /// Tries to get the service instance from the registered services,
    /// in case the service was registered using an instance.
    /// </summary>
    /// <param name="appServices">The service collection.</param>
    /// <param name="contractType">The contract type.</param>
    /// <returns>The service instance or <c>null</c>.</returns>
    public static object GetServiceInstance(this IAppServiceCollection appServices, Type contractType)
        => appServices.TryGetServiceInstance(contractType)
           ?? throw new ArgumentException(string.Format(ServicesAbstractionsStrings.ServiceInstanceNotRegistered, contractType), nameof(contractType));

    /// <summary>
    /// Gets the service instance from the registered services
    /// </summary>
    /// <typeparam name="T">The contract type.</typeparam>
    /// <param name="appServices">The service collection.</param>
    /// <returns>The service instance or <c>null</c>.</returns>
    [return: NotNull]
    public static T GetServiceInstance<T>(this IAppServiceCollection appServices)
        where T : class
        => (T)appServices.GetServiceInstance(typeof(T));

    /// <summary>
    /// Registers the provided service using a registration builder, if not previously registered.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <param name="contractDeclarationType">The contract declaration type.</param>
    /// <param name="instancingStrategy">The instancing strategy.</param>
    /// <param name="builder">The builder.</param>
    /// <returns>
    /// This <see cref="IAppServiceCollection"/>.
    /// </returns>
    internal static IAppServiceCollection TryAddService(
        this IAppServiceCollection appServices,
        Type contractDeclarationType,
        object instancingStrategy,
        Action<AppServiceInfo>? builder = null)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        contractDeclarationType = contractDeclarationType ?? throw new ArgumentNullException(nameof(contractDeclarationType));
        instancingStrategy = instancingStrategy ?? throw new ArgumentNullException(nameof(instancingStrategy));

        var serviceBuilder = new AppServiceInfo(contractDeclarationType, instancingStrategy);
        builder?.Invoke(serviceBuilder);
        var appServiceInfo = serviceBuilder;

        if (appServiceInfo.ContractType is not null && !appServices.Contains(appServiceInfo.ContractType))
        {
            appServices.Add(appServiceInfo);
        }

        return appServices;
    }

    /// <summary>
    /// Registers the provided service using a registration builder.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <param name="contractDeclarationType">The contract declaration type.</param>
    /// <param name="instancingStrategy">The instancing strategy.</param>
    /// <param name="builder">The builder.</param>
    /// <returns>
    /// This <see cref="IAppServiceCollection"/>.
    /// </returns>
    internal static IAppServiceCollection AddService(
        this IAppServiceCollection appServices,
        Type contractDeclarationType,
        object instancingStrategy,
        Action<AppServiceInfo>? builder = null)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        contractDeclarationType = contractDeclarationType ?? throw new ArgumentNullException(nameof(contractDeclarationType));
        instancingStrategy = instancingStrategy ?? throw new ArgumentNullException(nameof(instancingStrategy));

        var serviceBuilder = new AppServiceInfo(contractDeclarationType, instancingStrategy);
        builder?.Invoke(serviceBuilder);
        appServices.Add(serviceBuilder);

        return appServices;
    }

    /// <summary>
    /// Replaces the service with the same contract type, adding the provided service.
    /// </summary>
    /// <param name="appServices">The application services.</param>
    /// <param name="contractDeclarationType">The contract declaration type.</param>
    /// <param name="instancingStrategy">The instancing strategy.</param>
    /// <param name="builder">The registration builder.</param>
    /// <returns>
    /// This <see cref="IAppServiceCollection"/>.
    /// </returns>
    internal static IAppServiceCollection ReplaceService(
        this IAppServiceCollection appServices,
        Type contractDeclarationType,
        object instancingStrategy,
        Action<AppServiceInfo>? builder = null)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        contractDeclarationType = contractDeclarationType ?? throw new ArgumentNullException(nameof(contractDeclarationType));
        instancingStrategy = instancingStrategy ?? throw new ArgumentNullException(nameof(instancingStrategy));

        var serviceBuilder = new AppServiceInfo(contractDeclarationType, instancingStrategy);
        builder?.Invoke(serviceBuilder);
        appServices.Replace(serviceBuilder);

        return appServices;
    }
}
