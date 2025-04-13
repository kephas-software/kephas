// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service collection extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Builder;
using Kephas.DependencyInjection.Builder.Configuration;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.Resources;
using Kephas.Extensions.DependencyInjection;
using Kephas.Reflection;
using Kephas.Runtime;
using Kephas.Runtime.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kephas.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    private static readonly MethodInfo AddServiceFactoriesWithFactoryMethod = ReflectionHelper.GetGenericMethodOf(_ => AddServiceFactoriesOfObject<string, string>(null!, null!, null));
    private static readonly MethodInfo AddServiceFactoriesWithoutMetadataMethod = ReflectionHelper.GetGenericMethodOf(_ => AddServiceFactories<string, string, string>(null!));
    private static readonly MethodInfo AddServiceFactoriesWithMetadataMethod = ReflectionHelper.GetGenericMethodOf(_ => AddServiceFactories<string, string, string>(null!, null!));

    /// <summary>
    /// Gets the <see cref="IAppRuntime"/> registered in <paramref name="services"/> as singleton instance.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="defaultIfNotFound">
    /// When <c>true</c>, a new instance of <see cref="AppRuntime"/> will be created if the service has not been previously registered.
    /// If <c>false</c> and the service is not found, an exception will be thrown.
    /// </param>
    /// <returns>The <see cref="IAppRuntime"/>.</returns>
    public static IAppRuntime GetAppRuntime(this IServiceCollection services, bool defaultIfNotFound = false) =>
        GetSingletonService<IAppRuntime>(
            services,
            defaultIfNotFound
                ? (() => new AppRuntime())
                : (() => throw new ServiceResolutionException<IAppRuntime>($"The {nameof(IAppRuntime)} must be registered before trying to access it.")));

    /// <summary>
    /// Gets the <typeparamref name="TService"/> registered in <paramref name="services"/> as singleton instance.
    /// </summary>
    /// <remarks>
    /// If the singleton instance is not found, an exception is thrown.
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <returns>The <paramref name="services"/>.</returns>
    public static TService GetRequiredSingletonService<TService>(this IServiceCollection services)
        where TService : class => GetSingletonService<TService>(services, () => throw new ServiceResolutionException<TService>());

    /// <summary>
    /// Gets the <typeparamref name="TService"/> registered in <paramref name="services"/> as singleton instance.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="defaultFactory">The factory used to create the default value, if service not found.</param>
    /// <returns>The <paramref name="services"/>.</returns>
    public static TService GetSingletonService<TService>(this IServiceCollection services, Func<TService> defaultFactory)
        where TService : class
    {
        var serviceDescriptor = services.LastOrDefault(d => d.ServiceType == typeof(TService));
        return serviceDescriptor is null
            ? defaultFactory()
            : serviceDescriptor.ImplementationInstance as TService
              ?? throw new ServiceResolutionException<TService>($"The {typeof(TService).Name} service is not registered as a singleton instance.");
    }

    /// <summary>
    /// Gets the service registered in <paramref name="services"/> as singleton instance.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="defaultFactory">The factory used to create the default value, if service not found.</param>
    /// <returns>The <paramref name="services"/>.</returns>
    public static object GetSingletonService(this IServiceCollection services, Type serviceType, Func<object> defaultFactory)
    {
        var serviceDescriptor = services.LastOrDefault(d => d.ServiceType == serviceType);
        return serviceDescriptor is null
            ? defaultFactory()
            : serviceDescriptor.ImplementationInstance
              ?? throw new ServiceResolutionException($"The {serviceType} service is not registered as a singleton instance.") { ServiceType = serviceType };
    }

    /// <summary>
    /// Adds the collected application services to the <paramref name="services"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="appRuntimeConfig">Optional application runtime configurator.</param>
    /// <param name="builderConfig">Optional service builder configurator.</param>
    /// <returns>
    /// The <paramref name="services"></paramref>.
    /// </returns>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AppRuntimeSettings>? appRuntimeConfig = null,
        Action<ServiceInfosBuilderSettings>? builderConfig = null)
    {
        var appRuntimeSettings = new AppRuntimeSettings();
        appRuntimeConfig?.Invoke(appRuntimeSettings);
        var appRuntime = new AppRuntime(appRuntimeSettings);
        return services.AddApplicationServices(configuration, appRuntime, builderConfig);
    }

    /// <summary>
    /// Adds the collected application services to the <paramref name="services"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="appRuntime">The application runtime.</param>
    /// <param name="builderConfig">Optional service builder configurator.</param>
    /// <returns>
    /// The <paramref name="services"></paramref>.
    /// </returns>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IAppRuntime appRuntime,
        Action<ServiceInfosBuilderSettings>? builderConfig = null)
    {
        ArgumentNullException.ThrowIfNull(nameof(services));
        ArgumentNullException.ThrowIfNull(nameof(configuration));
        ArgumentNullException.ThrowIfNull(nameof(appRuntime));

        services.AddInitialServices(appRuntime);

        var builderSettings = new ServiceInfosBuilderSettings
        {
            GetAppAssemblies = appRuntime.GetAppAssemblies,
            Services = new ServiceInfoCollection(),
        };
        builderConfig?.Invoke(builderSettings);
        var builder = new ServiceInfoCollectionBuilder(builderSettings);

        var serviceInfos = builder.Build();
        foreach (var serviceInfo in serviceInfos)
        {
            services.AddServiceInfo(serviceInfo);
        }

        foreach (var serviceInfo in serviceInfos
                     .Where(si => si.ServiceType?.IsServiceRegistration(si.ImplementationType) is true))
        {
            RegisterServices(serviceInfo.ImplementationType!, services, configuration);
        }

        return services;
    }

    internal static IServiceCollection AddInitialServices(this IServiceCollection services, IAppRuntime appRuntime)
    {
        services.TryAddTransient(typeof(ICollection<>), typeof(List<>));
        services.TryAddTransient(typeof(IReadOnlyCollection<>), typeof(List<>));
        services.TryAddTransient(typeof(IList<>), typeof(List<>));
        services.TryAddTransient(typeof(IReadOnlyList<>), typeof(List<>));
        services.AddSingleton<IOpenGenericMetadataProvider>(new OpenGenericMetadataProvider());
        services.AddSingleton<IAppRuntime>(appRuntime);

        return services;
    }

    /// <summary>
    /// Adds the application service to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceInfo">The <see cref="IServiceInfo"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddServiceInfo(
        this IServiceCollection services,
        IServiceInfo serviceInfo)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        var metadataType = serviceInfo.MetadataType ?? typeof(ServiceMetadata);
        var serviceType = serviceInfo.ServiceType
                          ?? serviceInfo.ImplementationType
                          ?? throw new InvalidOperationException($"The service type is not set for: {serviceInfo}.");
        var implementationType = serviceInfo.ImplementationType;
        var serviceDeclarationType = serviceInfo.ServiceDeclarationType;
        if (serviceInfo.AsOpenGeneric is true && implementationType is null)
        {
            throw new InvalidOperationException($"Must provide the implementation type for open generics in: {serviceInfo}.");
        }

        return serviceInfo switch
        {
            // InstanceType, including open generics
            { ImplementationType: not null, Lifetime: AppServiceLifetime.Singleton } =>
                services.AddSingletonWithMetadata(serviceType, implementationType!, metadataType, serviceInfo.Metadata, serviceDeclarationType),
            { ImplementationType: not null, Lifetime: AppServiceLifetime.Scoped } =>
                services.AddScopedWithMetadata(serviceType, implementationType!, metadataType, serviceInfo.Metadata, serviceDeclarationType),
            { ImplementationType: not null } =>
                services.AddTransientWithMetadata(serviceType, implementationType!, metadataType, serviceInfo.Metadata, serviceDeclarationType),
            // InstanceFactory
            { Factory: not null, Lifetime: AppServiceLifetime.Singleton } =>
                services.AddSingletonWithMetadata(serviceType, serviceInfo.Factory, metadataType, serviceInfo.Metadata),
            { Factory: not null, Lifetime: AppServiceLifetime.Scoped } =>
                services.AddScopedWithMetadata(serviceType, serviceInfo.Factory, metadataType, serviceInfo.Metadata),
            { Factory: not null } =>
                services.AddTransientWithMetadata(serviceType, serviceInfo.Factory, metadataType, serviceInfo.Metadata),
            // Instance
            { InstancingStrategy: not null } =>
                services.AddSingletonWithMetadata(serviceType, serviceInfo.InstancingStrategy, metadataType, serviceInfo.Metadata),
            _ => services
        };
    }

    /// <summary>
    /// Registers a service as singleton.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="implementationType">The service implementation type.</param>
    /// <param name="metadataType">The metadata type. If not provided, it is considered <see cref="ServiceMetadata"/>.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <paramref name="implementationType"/>.</param>
    /// <param name="serviceDeclarationType">The service declaration type. Reserved internally by the framework.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSingletonWithMetadata(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type implementationType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? metadataType = null,
        IDictionary<string, object?>? metadata = null,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? serviceDeclarationType = null)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        implementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
        metadataType ??= typeof(ServiceMetadata);
        serviceDeclarationType ??= serviceType;

        services.AddSingleton(implementationType);

        return implementationType.IsGenericTypeDefinition
            // TODO this is probably not correct for open generics, as it will generate different instances
            // for the registration type and for the implementation type.
            // However, there is no good solution for this right now.
            ? AddOpenGenericServiceFactories(
                services, serviceType, serviceDeclarationType, implementationType,
                openGenericServiceType => services.AddSingleton(openGenericServiceType, implementationType))
            : metadata is null
                ? services.AddServiceFactories(serviceType, serviceDeclarationType, implementationType, metadataType)
                : services.AddServiceFactories(serviceType, serviceDeclarationType, implementationType, metadataType, metadata);
    }

    /// <summary>
    /// Registers a service as singleton.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The service type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <typeparamref name="TImplementation"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSingletonWithMetadata<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TImplementation>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata = null)
        where TService : class
        where TImplementation : class, TService =>
        AddSingletonWithMetadata<TService, TImplementation, ServiceMetadata>(services, metadata);

    /// <summary>
    /// Registers a service as singleton.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <typeparamref name="TImplementation"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSingletonWithMetadata<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TImplementation,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata = null)
        where TService : class
        where TImplementation : class, TService
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        if (typeof(TService).IsGenericTypeDefinition)
        {
            services.AddSingleton(typeof(TService), typeof(TImplementation));
        }
        else
        {
            services.AddSingleton(typeof(TImplementation));
            if (typeof(TService) != typeof(TImplementation))
            {
                services.AddSingleton(typeof(TService), provider => provider.GetRequiredService(typeof(TImplementation)));
            }
        }

        return metadata is null
            ? services.AddServiceFactories<TService, TImplementation, TMetadata>()
            : services.AddServiceFactories<TService, TImplementation, TMetadata>(metadata);
    }

    /// <summary>
    /// Registers a service as singleton instance.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="instance">The service instance.</param>
    /// <param name="metadataType">The metadata type. If not provided, it is considered <see cref="ServiceMetadata"/>.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSingletonWithMetadata(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        object instance,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? metadataType = null,
        IDictionary<string, object?>? metadata = null)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        instance = instance ?? throw new ArgumentNullException(nameof(instance));
        metadataType ??= typeof(ServiceMetadata);

        return services
            .AddSingleton(serviceType, instance)
            .AddServiceFactories(serviceType, _ => instance, metadataType, metadata);
    }

    /// <summary>
    /// Registers a service as singleton instance.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="instance">The service instance.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSingletonWithMetadata<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(
        this IServiceCollection services,
        TService instance,
        IDictionary<string, object?>? metadata = null)
        where TService : class
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        return services
            .AddSingleton(instance)
            .AddServiceFactories<TService, TMetadata>(_ => instance, metadata);
    }

    /// <summary>
    /// Registers a service as transient.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="implementationType">The service implementation type.</param>
    /// <param name="metadataType">The metadata type. If not provided, it is considered <see cref="ServiceMetadata"/>.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <paramref name="implementationType"/>.</param>
    /// <param name="serviceDeclarationType">The service declaration type. Reserved internally by the framework.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddTransientWithMetadata(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type implementationType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? metadataType = null,
        IDictionary<string, object?>? metadata = null,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? serviceDeclarationType = null)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        implementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
        metadataType ??= typeof(ServiceMetadata);
        serviceDeclarationType ??= serviceType;

        services.AddTransient(implementationType);

        return implementationType.IsGenericTypeDefinition
            ? AddOpenGenericServiceFactories(
                services, serviceType, serviceDeclarationType, implementationType,
                openGenericServiceType => services.AddTransient(openGenericServiceType, implementationType))
            : metadata is null
                ? services.AddServiceFactories(serviceType, serviceDeclarationType, implementationType, metadataType)
                : services.AddServiceFactories(serviceType, serviceDeclarationType, implementationType, metadataType, metadata);
    }

    /// <summary>
    /// Registers a service as transient.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The service type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <typeparamref name="TImplementation"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddTransientWithMetadata<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TImplementation>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata = null)
        where TService : class
        where TImplementation : class, TService =>
        AddTransientWithMetadata<TService, TImplementation, ServiceMetadata>(services, metadata);

    /// <summary>
    /// Registers a service as transient.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <typeparamref name="TImplementation"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddTransientWithMetadata<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TImplementation,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata = null)
        where TService : class
        where TImplementation : class, TService
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        if (typeof(TService).IsGenericTypeDefinition)
        {
            services.AddTransient(typeof(TService), typeof(TImplementation));
        }
        else
        {
            services.AddTransient(typeof(TImplementation));
            if (typeof(TService) != typeof(TImplementation))
            {
                services.AddTransient(typeof(TService), provider => provider.GetRequiredService(typeof(TImplementation)));
            }
        }

        return metadata is null
            ? services.AddServiceFactories<TService, TImplementation, TMetadata>()
            : services.AddServiceFactories<TService, TImplementation, TMetadata>(metadata);
    }

    /// <summary>
    /// Registers a service as scoped.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="implementationType">The service implementation type.</param>
    /// <param name="metadataType">The metadata type. If not provided, it is considered <see cref="ServiceMetadata"/>.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <paramref name="implementationType"/>.</param>
    /// <param name="serviceDeclarationType">The service declaration type. Reserved internally by the framework.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddScopedWithMetadata(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type implementationType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? metadataType = null,
        IDictionary<string, object?>? metadata = null,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? serviceDeclarationType = null)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        implementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
        metadataType ??= typeof(ServiceMetadata);
        serviceDeclarationType ??= serviceType;

        services.AddScoped(implementationType);

        return implementationType.IsGenericTypeDefinition
            // TODO this is probably not correct for open generics, as it will generate different instances
            // for the registration type and for the implementation type.  
            // However, there is no good solution for this right now.
            ? AddOpenGenericServiceFactories(
                services, serviceType, serviceDeclarationType, implementationType,
                openGenericServiceType => services.AddScoped(openGenericServiceType, implementationType))
            : metadata is null
                ? services.AddServiceFactories(serviceType, serviceDeclarationType, implementationType, metadataType)
                : services.AddServiceFactories(serviceType, serviceDeclarationType, implementationType, metadataType, metadata);
    }

    /// <summary>
    /// Registers a service as scoped.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The service type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <typeparamref name="TImplementation"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddScopedWithMetadata<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TImplementation>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata = null)
        where TService : class
        where TImplementation : class, TService =>
        AddScopedWithMetadata<TService, TImplementation, ServiceMetadata>(services, metadata);

    /// <summary>
    /// Registers a service as scoped.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <typeparamref name="TImplementation"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddScopedWithMetadata<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TImplementation,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata = null)
        where TService : class
        where TImplementation : class, TService
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        if (typeof(TService).IsGenericTypeDefinition)
        {
            services.AddScoped(typeof(TService), typeof(TImplementation));
        }
        else
        {
            services.AddScoped(typeof(TImplementation));
            if (typeof(TService) != typeof(TImplementation))
            {
                services.AddScoped(typeof(TService), provider => provider.GetRequiredService(typeof(TImplementation)));
            }
        }

        return metadata is null
            ? services.AddServiceFactories<TService, TImplementation, TMetadata>()
            : services.AddServiceFactories<TService, TImplementation, TMetadata>(metadata);
    }

    /// <summary>
    /// Registers a service with factory as singleton.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="metadataType">The metadata type. If not provided, it is considered <see cref="ServiceMetadata"/>.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSingletonWithMetadata(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        Func<IServiceProvider, object> serviceFactory,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? metadataType = null,
        IDictionary<string, object?>? metadata = null)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        metadataType ??= typeof(ServiceMetadata);

        var key = Guid.NewGuid();
        return services
            .AddKeyedSingleton(serviceType, key, (sp, _) => serviceFactory(sp))
            .AddSingleton(serviceType, sp => sp.GetRequiredKeyedService(serviceType, key))
            .AddServiceFactories(serviceType, sp => sp.GetRequiredKeyedService(serviceType, key), metadataType, metadata);
    }

    /// <summary>
    /// Registers a service as singleton.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSingletonWithMetadata<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> serviceFactory,
        IDictionary<string, object?>? metadata = null)
        where TService : class
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        var key = Guid.NewGuid();
        return services
            .AddKeyedSingleton<TService>(key, (sp, _) => serviceFactory(sp))
            .AddSingleton<TService>(sp => sp.GetRequiredKeyedService<TService>(key))
            .AddServiceFactories(typeof(TService), sp => sp.GetRequiredKeyedService<TService>(key), typeof(TMetadata), metadata);
    }

    /// <summary>
    /// Registers a service as scoped.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddScopedWithMetadata<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> serviceFactory,
        IDictionary<string, object?>? metadata = null)
        where TService : class
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        var key = Guid.NewGuid();
        return services
            .AddKeyedScoped<TService>(key, (sp, _) => serviceFactory(sp))
            .AddScoped<TService>(sp => sp.GetRequiredKeyedService<TService>(key))
            .AddServiceFactories(typeof(TService), sp => sp.GetRequiredKeyedService<TService>(key), typeof(TMetadata), metadata);
    }

    /// <summary>
    /// Registers a service with factory as transient.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="metadataType">The metadata type. If not provided, it is considered <see cref="ServiceMetadata"/>.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddTransientWithMetadata(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        Func<IServiceProvider, object> serviceFactory,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? metadataType = null,
        IDictionary<string, object?>? metadata = null)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        metadataType ??= typeof(ServiceMetadata);

        return services
            .AddTransient(serviceType, serviceFactory)
            .AddServiceFactories(serviceType, serviceFactory, metadataType, metadata);
    }

    /// <summary>
    /// Registers a service as transient.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddTransientWithMetadata<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> serviceFactory,
        IDictionary<string, object?>? metadata = null)
        where TService : class
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return services
            .AddTransient(serviceFactory)
            .AddServiceFactories<TService, TMetadata>(serviceFactory, metadata);
    }

    /// <summary>
    /// Registers a service with factory as scoped.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="metadataType">The metadata type. If not provided, it is considered <see cref="ServiceMetadata"/>.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddScopedWithMetadata(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        Func<IServiceProvider, object> serviceFactory,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? metadataType = null,
        IDictionary<string, object?>? metadata = null)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        metadataType ??= typeof(ServiceMetadata);

        var key = Guid.NewGuid();
        return services
            .AddKeyedScoped(serviceType, key, (sp, _) => serviceFactory(sp))
            .AddScoped(serviceType, sp => sp.GetRequiredKeyedService(serviceType, key))
            .AddServiceFactories(serviceType, sp => sp.GetRequiredKeyedService(serviceType, key), metadataType, metadata);
    }

    /// <summary>
    /// Tries to get the service instance from the registered services,
    /// in case the service was registered using an instance.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <returns>The service instance or <c>null</c>.</returns>
    public static object? GetServiceInstance(this IServiceCollection services, Type serviceType)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        return services.LastOrDefault(i => i.ServiceType == serviceType && i.ImplementationInstance is not null)?.ImplementationInstance;
    }

    /// <summary>
    /// Tries to get the service instance from the registered services
    /// </summary>
    /// <typeparam name="T">The contract type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service instance or <c>null</c>.</returns>
    public static T? GetServiceInstance<T>(this IServiceCollection services)
        where T : class
        => services.GetServiceInstance(typeof(T)) as T;

    /// <summary>
    /// Tries to get the service instance from the registered services,
    /// in case the service was registered using an instance.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <returns>The service instance or <c>null</c>.</returns>
    public static object GetRequiredServiceInstance(this IServiceCollection services, Type serviceType)
        => services.GetServiceInstance(serviceType)
           ?? throw new ArgumentException(string.Format(ServicesAbstractionsStrings.ServiceInstanceNotRegistered, serviceType), nameof(serviceType));

    /// <summary>
    /// Gets the service instance from the registered services
    /// </summary>
    /// <typeparam name="T">The contract type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service instance or <c>null</c>.</returns>
    [return: NotNull]
    public static T GetRequiredServiceInstance<T>(this IServiceCollection services)
        where T : class
        => (T)services.GetRequiredServiceInstance(typeof(T));

    private static IServiceCollection AddServiceFactories<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]T,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]TImplementation,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]TMetadata>(this IServiceCollection services)
        where T : class
        where TImplementation : class, T
    {
        if (typeof(T) != typeof(TImplementation))
        {
            services.AddTransientRaw<T>(provider => provider.GetRequiredService<TImplementation>());
        }

        services
            .AddTransientRaw<Lazy<T>, LazyService<T, TImplementation>>()
            .AddTransientRaw<Lazy<T, TMetadata>, LazyService<T, TImplementation, TMetadata>>()
            .AddTransientRaw<IFactory<T>, FactoryService<T, TImplementation>>()
            .AddTransientRaw<IFactory<T, TMetadata>, FactoryService<T, TImplementation, TMetadata>>();

        if (typeof(TMetadata) != typeof(ServiceMetadata))
        {
            services
                .AddTransientRaw<Lazy<T, ServiceMetadata>, LazyService<T, TImplementation, ServiceMetadata>>()
                .AddTransientRaw<IFactory<T, ServiceMetadata>, FactoryService<T, TImplementation, ServiceMetadata>>();
        }

        return services;
    }

    private static IServiceCollection AddServiceFactories<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]T,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]TImplementation,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]TMetadata>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata)
        where T : class
        where TImplementation : class, T
    {
        if (typeof(T) != typeof(TImplementation))
        {
            services.AddTransientRaw<T>(provider => provider.GetRequiredService<TImplementation>());
        }

        services
            .AddTransientRaw<Lazy<T>, LazyService<T, TImplementation>>()
            .AddTransientRaw<Lazy<T, TMetadata>>(provider => new LazyService<T, TImplementation, TMetadata>(provider, metadata))
            .AddTransientRaw<IFactory<T>, FactoryService<T, TImplementation>>()
            .AddTransientRaw<IFactory<T, TMetadata>>(provider => new FactoryService<T, TImplementation, TMetadata>(provider, metadata));

        if (typeof(TMetadata) != typeof(ServiceMetadata))
        {
            services
                .AddTransientRaw<Lazy<T, ServiceMetadata>>(provider => new LazyService<T, TImplementation, ServiceMetadata>(provider, metadata))
                .AddTransientRaw<IFactory<T, ServiceMetadata>>(provider => new FactoryService<T, TImplementation, ServiceMetadata>(provider, metadata));
        }

        return services;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IServiceCollection AddTransientRaw<T, TImplementation>(this IServiceCollection services)
        where TImplementation : class, T
        where T : class
        => ServiceCollectionServiceExtensions.AddTransient<T, TImplementation>(services);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IServiceCollection AddTransientRaw<T>(this IServiceCollection services, Func<IServiceProvider, T> factory)
        where T : class
        => services.AddTransient(factory);

    private static IServiceCollection AddServiceFactoriesOfObject<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]T,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]TMetadata>(
        this IServiceCollection services,
        Func<IServiceProvider, object> factory,
        IDictionary<string, object?>? metadata)
        where T : class =>
        AddServiceFactories<T, TMetadata>(services, provider => (T)factory(provider), metadata);

    private static IServiceCollection AddServiceFactories<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]T,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]TMetadata>(
        this IServiceCollection services,
        Func<IServiceProvider, T> factory,
        IDictionary<string, object?>? metadata)
        where T : class
    {
        services
            .AddTransientRaw<Lazy<T>>(provider => new Lazy<T>(() => factory(provider)))
            .AddTransientRaw<Lazy<T, TMetadata>>(provider => new Lazy<T, TMetadata>(() => factory(provider), ServiceHelper.GetServiceMetadata<TMetadata>(metadata)))
            .AddTransientRaw<IFactory<T>>(provider => new Factory<T>(() => factory(provider)))
            .AddTransientRaw<IFactory<T, TMetadata>>(provider => new Factory<T, TMetadata>(() => factory(provider), metadata));

        if (typeof(TMetadata) != typeof(ServiceMetadata))
        {
            services
                .AddTransientRaw<Lazy<T, ServiceMetadata>>(provider => new Lazy<T, ServiceMetadata>(() => factory(provider), new ServiceMetadata(metadata)))
                .AddTransientRaw<IFactory<T, ServiceMetadata>>(provider => new Factory<T, ServiceMetadata>(() => factory(provider), metadata));
        }

        return services;
    }

    private static IServiceCollection AddServiceFactories(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type serviceDeclarationType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type implementationType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type metadataType)
    {
        if (implementationType.IsGenericTypeDefinition)
        {
            // do not do here anything, as the open generics for lazy and factory have already been added.
            // do not do here anything except for registering the metadata, as the open generics for lazy and factory have already been added.
            AddOpenGenericServiceMetadata(services, serviceType, serviceDeclarationType, implementationType);
            return services;
        }

        var addServiceFactories = AddServiceFactoriesWithoutMetadataMethod.MakeGenericMethod(serviceType, implementationType, metadataType);
        return addServiceFactories.Call<IServiceCollection>(null, services);
    }

    private static IServiceCollection AddServiceFactories(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type serviceDeclarationType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type implementationType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type metadataType,
        IDictionary<string, object?> metadata)
    {
        if (implementationType.IsGenericTypeDefinition)
        {
            // do not do here anything except for registering the metadata, as the open generics for lazy and factory have already been added.
            AddOpenGenericServiceMetadata(services, serviceType, serviceDeclarationType, implementationType);
            return services;
        }

        var addServiceFactories = AddServiceFactoriesWithMetadataMethod.MakeGenericMethod(serviceType, implementationType, metadataType);
        return addServiceFactories.Call<IServiceCollection>(null, services, metadata);
    }

    private static void AddOpenGenericServiceMetadata(
        IServiceCollection services,
        Type serviceType,
        Type serviceDeclarationType,
        Type implementationType)
    {
        var genericServiceType = GetGenericServiceType(serviceType, serviceDeclarationType, implementationType);
        var openGenericMetadataProvider = services.GetRequiredServiceInstance<IOpenGenericMetadataProvider>();
        if (!openGenericMetadataProvider.TryGetValue(genericServiceType, out var openGenericMetadata))
        {
            openGenericMetadata = [];
            openGenericMetadataProvider.Add(genericServiceType, openGenericMetadata);
        }

        openGenericMetadata.Add(KeyValuePair.Create(implementationType, ServiceHelper.GetServiceMetadata(implementationType, genericServiceType)));
    }

    private static IServiceCollection AddServiceFactories(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type serviceType,
        Func<IServiceProvider, object> serviceFactory,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type metadataType,
        IDictionary<string, object?>? metadata)
    {
        var addServiceFactories = AddServiceFactoriesWithFactoryMethod.MakeGenericMethod(serviceType, metadataType);
        addServiceFactories.Call(null, services, serviceFactory, metadata);
        return services;
    }
    
    private static bool IsServiceRegistration(this Type serviceType, [NotNullWhen(true)] Type? implementationType)
        => serviceType == typeof(IServiceRegistration) && implementationType?.IsAbstract is false;
    
    private static void RegisterServices(Type implementationType, IServiceCollection services, IConfiguration configuration)
    {
        IServiceRegistration? registration;
        try
        {
            registration = Activator.CreateInstance(implementationType) as IServiceRegistration
                           ?? throw new InvalidOperationException($"Cannot create instance of {implementationType}, probably it does not implement {nameof(IServiceRegistration)}.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Cannot create instance of {implementationType}, probably a default constructor is missing.", ex);
        }

        registration.RegisterServices(services, configuration);
    }

    private static Type GetGenericServiceType(Type serviceType, Type serviceDeclarationType, Type implementationType)
    {
        var genericServiceType = serviceType.IsGenericTypeDefinition
            ? serviceType
            : serviceDeclarationType.IsGenericTypeDefinition
                ? serviceDeclarationType
                : null;
        if (genericServiceType is null)
        {
            throw new InvalidOperationException($"To register the open generic implementation type {implementationType}, either the service type {serviceType} or the service declaration type {serviceDeclarationType} must be an open generic type.");
        }

        return genericServiceType;
    }

    private static IServiceCollection AddOpenGenericServiceFactories(
        this IServiceCollection services,
        Type serviceType, Type serviceDeclarationType, Type implementationType,
        Func<Type, IServiceCollection> addService)
    {
        var openGenericServiceType = GetGenericServiceType(serviceType, serviceDeclarationType, implementationType);
        if (openGenericServiceType != implementationType)
        {
            // if the open generic service type is the same with the implementation type,
            // do not register again, as it was already registered.
            addService(openGenericServiceType);
        }

        // do not do anything anymore, as the open generics for lazy and factory have already been added.
        // do not do here anything except for registering the metadata, as the open generics for lazy and factory have already been added.
        AddOpenGenericServiceMetadata(services, serviceType, serviceDeclarationType, implementationType);
        return services;
    }
}
