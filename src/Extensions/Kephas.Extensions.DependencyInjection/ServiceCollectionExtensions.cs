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
    /// <returns>The <see cref="IAppRuntime"/>.</returns>
    public static IAppRuntime GetAppRuntime(this IServiceCollection services) =>
        GetSingleton<IAppRuntime>(services, () => new AppRuntime());

    /// <summary>
    /// Gets the <typeparamref name="TService"/> registered in <paramref name="services"/> as singleton instance.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="defaultFactory">The factory used to create the default value, if service not found.</param>
    /// <returns>The <paramref name="services"/>.</returns>
    public static TService GetSingleton<TService>(this IServiceCollection services, Func<TService> defaultFactory)
        where TService : class
    {
        var serviceDescriptor = services.LastOrDefault(d => d.ServiceType == typeof(TService));
        return serviceDescriptor is null
            ? defaultFactory()
            : serviceDescriptor.ImplementationInstance as TService
              ?? throw new InvalidOperationException($"The {typeof(TService).Name} service is not registered as a singleton instance.");
    }

    /// <summary>
    /// Gets the service registered in <paramref name="services"/> as singleton instance.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="defaultFactory">The factory used to create the default value, if service not found.</param>
    /// <returns>The <paramref name="services"/>.</returns>
    public static object GetSingleton(this IServiceCollection services, Type serviceType, Func<object> defaultFactory)
    {
        var serviceDescriptor = services.LastOrDefault(d => d.ServiceType == serviceType);
        return serviceDescriptor is null
            ? defaultFactory()
            : serviceDescriptor.ImplementationInstance
              ?? throw new InvalidOperationException($"The {serviceType} service is not registered as a singleton instance.");
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

        IOpenGenericMetadataProvider openGenericMetadataProvider = new OpenGenericMetadataProvider(); 
        var serviceInfos = new ServiceInfoCollection
        {
            { appRuntime, b => b.Lifetime = AppServiceLifetime.Singleton },
            { openGenericMetadataProvider, b => b.Lifetime = AppServiceLifetime.Singleton },
        };

        var builderSettings = new ServiceInfosBuilderSettings
        {
            GetAppAssemblies = appRuntime.GetAppAssemblies,
            Services = serviceInfos,
        };
        builderConfig?.Invoke(builderSettings);
        var builder = new ServiceInfoCollectionBuilder(builderSettings);
        return services.AddApplicationServicesCore(configuration, builder.Build());
    }
    
    private static IServiceCollection AddApplicationServicesCore(this IServiceCollection services, IConfiguration configuration, IEnumerable<IServiceInfo> serviceInfos)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        if (serviceInfos is null)
        {
            return services;
        }

        services.AddGenericCollections();
        foreach (var serviceInfo in serviceInfos)
        {
            services.AddServiceInfo(configuration, serviceInfo);
        }

        return services;
    }

    /// <summary>
    /// Adds the <see cref="ICollection{T}"/>, <see cref="IList{T}"/>, <see cref="IReadOnlyCollection{T}"/>, and
    /// <see cref="IReadOnlyList{T}"/> as open generic registrations.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddGenericCollections(this IServiceCollection services)
    {
        services.TryAddTransient(typeof(ICollection<>), typeof(List<>));
        services.TryAddTransient(typeof(IReadOnlyCollection<>), typeof(List<>));
        services.TryAddTransient(typeof(IList<>), typeof(List<>));
        services.TryAddTransient(typeof(IReadOnlyList<>), typeof(List<>));

        return services;
    }

    /// <summary>
    /// Adds the application service to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="serviceInfo">The <see cref="IServiceInfo"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddServiceInfo(
        this IServiceCollection services,
        IConfiguration configuration,
        IServiceInfo serviceInfo)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        var metadataType = serviceInfo.MetadataType ?? typeof(ServiceMetadata);
        var serviceType = serviceInfo.ServiceType
                          ?? serviceInfo.ImplementationType
                          ?? throw new InvalidOperationException($"The service type is not set for: {serviceInfo}.");
        var implementationType = serviceInfo.ImplementationType;
        var serviceDeclarationType = serviceInfo.ServiceDeclarationType;
        if (serviceInfo.AsOpenGeneric && implementationType is null)
        {
            throw new InvalidOperationException($"Must provide the implementation type for open generics in: {serviceInfo}.");
        }

        if (IsServiceRegistration(serviceType, implementationType))
        {
            RegisterServices(implementationType, services, configuration);
        }

        return serviceInfo switch
        {
            // InstanceType, including open generics
            { ImplementationType: not null, Lifetime: AppServiceLifetime.Singleton } =>
                services.AddSingleton(serviceType, implementationType!, metadataType, serviceInfo.Metadata, serviceDeclarationType),
            { ImplementationType: not null, Lifetime: AppServiceLifetime.Scoped } =>
                services.AddScoped(serviceType, implementationType!, metadataType, serviceInfo.Metadata, serviceDeclarationType),
            { ImplementationType: not null } =>
                services.AddTransient(serviceType, implementationType!, metadataType, serviceInfo.Metadata, serviceDeclarationType),
            // InstanceFactory
            { Factory: not null, Lifetime: AppServiceLifetime.Singleton } =>
                services.AddSingleton(serviceType, serviceInfo.Factory, metadataType, serviceInfo.Metadata),
            { Factory: not null, Lifetime: AppServiceLifetime.Scoped } =>
                services.AddScoped(serviceType, serviceInfo.Factory, metadataType, serviceInfo.Metadata),
            { Factory: not null } =>
                services.AddTransient(serviceType, serviceInfo.Factory, metadataType, serviceInfo.Metadata),
            // Instance
            { InstancingStrategy: not null } =>
                services.AddSingleton(serviceType, serviceInfo.InstancingStrategy, metadataType, serviceInfo.Metadata),
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
    public static IServiceCollection AddSingleton(
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

        var serviceRegistrationType = implementationType.IsGenericTypeDefinition
            ? GetGenericServiceType(serviceType, serviceDeclarationType, implementationType)
            : serviceType;
        services.AddSingleton(implementationType);
        if (serviceRegistrationType != implementationType)
        {
            // TODO this is probably not correct for open generics, as it will generate different instances for the registration type and for the implementation tyoe.  
            _ = implementationType.IsGenericTypeDefinition
                ? ServiceCollectionServiceExtensions.AddSingleton(services, serviceRegistrationType, implementationType)
                : ServiceCollectionServiceExtensions.AddSingleton(services, serviceType, provider => provider.GetRequiredService(implementationType));
        }

        return metadata is null
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
    public static IServiceCollection AddSingleton<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TImplementation>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata = null)
        where TService : class
        where TImplementation : class, TService =>
        AddSingleton<TService, TImplementation, ServiceMetadata>(services, metadata);

    /// <summary>
    /// Registers a service as singleton.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <typeparamref name="TImplementation"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSingleton<
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
            ServiceCollectionServiceExtensions.AddSingleton(services, typeof(TService), typeof(TImplementation));
        }
        else
        {
            services.AddSingleton(typeof(TImplementation));
            if (typeof(TService) != typeof(TImplementation))
            {
                ServiceCollectionServiceExtensions.AddSingleton(services, typeof(TService), provider => provider.GetRequiredService(typeof(TImplementation)));
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
    public static IServiceCollection AddSingleton(
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

        return ServiceCollectionServiceExtensions
            .AddSingleton(services, serviceType, instance)
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
    public static IServiceCollection AddSingleton<
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
    public static IServiceCollection AddTransient(
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

        var serviceRegistrationType = implementationType.IsGenericTypeDefinition
            ? GetGenericServiceType(serviceType, serviceDeclarationType, implementationType)
            : serviceType;
        services.AddTransient(implementationType);
        if (serviceRegistrationType != implementationType)
        {
            _ = implementationType.IsGenericTypeDefinition
                ? services.AddTransientRaw(serviceRegistrationType, implementationType)
                : services.AddTransientRaw(serviceType, provider => provider.GetRequiredService(implementationType));
        }

        return metadata is null
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
    public static IServiceCollection AddTransient<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TImplementation>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata = null)
        where TService : class
        where TImplementation : class, TService =>
        AddTransient<TService, TImplementation, ServiceMetadata>(services, metadata);

    /// <summary>
    /// Registers a service as transient.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <typeparamref name="TImplementation"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddTransient<
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
            ServiceCollectionServiceExtensions.AddTransient(services, typeof(TService), typeof(TImplementation));
        }
        else
        {
            services.AddTransient(typeof(TImplementation));
            if (typeof(TService) != typeof(TImplementation))
            {
                ServiceCollectionServiceExtensions.AddTransient(services, typeof(TService), provider => provider.GetRequiredService(typeof(TImplementation)));
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
    public static IServiceCollection AddScoped(
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

        var serviceRegistrationType = implementationType.IsGenericTypeDefinition
            ? GetGenericServiceType(serviceType, serviceDeclarationType, implementationType)
            : serviceType;
        services.AddScoped(implementationType);
        if (serviceRegistrationType != implementationType)
        {
            // TODO this is probably not correct for open generics, as it will generate different instances for the registration type and for the implementation tyoe.  
            _ = implementationType.IsGenericTypeDefinition
                ? ServiceCollectionServiceExtensions.AddScoped(services, serviceRegistrationType, implementationType)
                : ServiceCollectionServiceExtensions.AddScoped(services, serviceType, provider => provider.GetRequiredService(implementationType));
        }

        return metadata is null
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
    public static IServiceCollection AddScoped<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TImplementation>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata = null)
        where TService : class
        where TImplementation : class, TService =>
        AddScoped<TService, TImplementation, ServiceMetadata>(services, metadata);

    /// <summary>
    /// Registers a service as scoped.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <typeparamref name="TImplementation"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddScoped<
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
            ServiceCollectionServiceExtensions.AddScoped(services, typeof(TService), typeof(TImplementation));
        }
        else
        {
            services.AddScoped(typeof(TImplementation));
            if (typeof(TService) != typeof(TImplementation))
            {
                ServiceCollectionServiceExtensions.AddScoped(services, typeof(TService), provider => provider.GetRequiredService(typeof(TImplementation)));
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
    public static IServiceCollection AddSingleton(
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

        return ServiceCollectionServiceExtensions
            .AddSingleton(services, serviceType, serviceFactory)
            .AddServiceFactories(serviceType, serviceFactory, metadataType, metadata);
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
    public static IServiceCollection AddSingleton<
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
            .AddSingleton(serviceFactory)
            .AddServiceFactories<TService, TMetadata>(serviceFactory, metadata);
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
    public static IServiceCollection AddTransient(
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

        return ServiceCollectionServiceExtensions
            .AddTransient(services, serviceType, serviceFactory)
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
    public static IServiceCollection AddTransient<
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
    public static IServiceCollection AddScoped(
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

        return ServiceCollectionServiceExtensions
            .AddScoped(services, serviceType, serviceFactory)
            .AddServiceFactories(serviceType, serviceFactory, metadataType, metadata);
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
    public static IServiceCollection AddScoped<
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
            .AddScoped(serviceFactory)
            .AddServiceFactories<TService, TMetadata>(serviceFactory, metadata);
    }

    /// <summary>
    /// Tries to get the service instance from the registered services,
    /// in case the service was registered using an instance.
    /// </summary>
    /// <param name="appServices">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <returns>The service instance or <c>null</c>.</returns>
    public static object? TryGetServiceInstance(this IServiceCollection appServices, Type serviceType)
    {
        appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        return appServices.LastOrDefault(i => i.ServiceType == serviceType && i.ImplementationInstance is not null)?.ImplementationInstance;
    }

    /// <summary>
    /// Tries to get the service instance from the registered services
    /// </summary>
    /// <typeparam name="T">The contract type.</typeparam>
    /// <param name="appServices">The service collection.</param>
    /// <returns>The service instance or <c>null</c>.</returns>
    public static T? TryGetServiceInstance<T>(this IServiceCollection appServices)
        where T : class
        => appServices.TryGetServiceInstance(typeof(T)) as T;

    /// <summary>
    /// Tries to get the service instance from the registered services,
    /// in case the service was registered using an instance.
    /// </summary>
    /// <param name="appServices">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <returns>The service instance or <c>null</c>.</returns>
    public static object GetServiceInstance(this IServiceCollection appServices, Type serviceType)
        => appServices.TryGetServiceInstance(serviceType)
           ?? throw new ArgumentException(string.Format(ServicesAbstractionsStrings.ServiceInstanceNotRegistered, serviceType), nameof(serviceType));

    /// <summary>
    /// Gets the service instance from the registered services
    /// </summary>
    /// <typeparam name="T">The contract type.</typeparam>
    /// <param name="appServices">The service collection.</param>
    /// <returns>The service instance or <c>null</c>.</returns>
    [return: NotNull]
    public static T GetServiceInstance<T>(this IServiceCollection appServices)
        where T : class
        => (T)appServices.GetServiceInstance(typeof(T));

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IServiceCollection AddTransientRaw(this IServiceCollection services, Type serviceType, Type implementationType)
        => ServiceCollectionServiceExtensions.AddTransient(services, serviceType, implementationType);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IServiceCollection AddTransientRaw(this IServiceCollection services, Type serviceType, Func<IServiceProvider, object> factory)
        => ServiceCollectionServiceExtensions.AddTransient(services, serviceType, factory);

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
        var openGenericMetadataProvider = services.GetServiceInstance<IOpenGenericMetadataProvider>();
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
    
    private static bool IsServiceRegistration(Type serviceType, [NotNullWhen(true)] Type? implementationType)
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
}
