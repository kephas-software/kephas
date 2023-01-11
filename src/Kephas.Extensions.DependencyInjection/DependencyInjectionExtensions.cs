// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service collection extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Kephas.Extensions.DependencyInjection;
using Kephas.Extensions.DependencyInjection.Resources;
using Kephas.Logging;
using Kephas.Services;
using Kephas.Services.Builder;
using Kephas.Reflection;
using Kephas.Services;
using Kephas.Services.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class DependencyInjectionExtensions
{
    private static MethodInfo AddServiceFactoriesWithFactoryMethod = ReflectionHelper.GetGenericMethodOf(_ => AddServiceFactoriesOfObject<string, string>(null!, null!, null));

    /// <summary>
    /// Builds the service provider using the service collection.
    /// </summary>
    /// <param name="servicesBuilder">The services builder.</param>
    /// <param name="builderOptions">Optional. The builder configuration.</param>
    /// <returns>The built service provider.</returns>
    public static IServiceProvider BuildWithDependencyInjection(
        this IAppServiceCollectionBuilder servicesBuilder,
        Action<IServiceCollection>? builderOptions = null)
    {
        servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));

        var services = new ServiceCollection();
        builderOptions?.Invoke(services);

        return servicesBuilder.BuildWithDependencyInjection(services);
    }

    /// <summary>
    /// Builds the service provider using the service collection.
    /// </summary>
    /// <param name="servicesBuilder">The services builder.</param>
    /// <param name="services">The service collection.</param>
    /// <returns>The built service provider.</returns>
    public static IServiceProvider BuildWithDependencyInjection(this IAppServiceCollectionBuilder servicesBuilder, IServiceCollection services)
    {
        servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));

        services.UseServicesBuilder(servicesBuilder);

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Includes the service collection in the composition.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="servicesBuilder">The services builder.</param>
    /// <returns>
    /// The <paramref name="services"></paramref>.
    /// </returns>
    public static IServiceCollection UseServicesBuilder(this IServiceCollection services, IAppServiceCollectionBuilder servicesBuilder)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));

        services.AddGenericCollections();

        var appServices = servicesBuilder.Build();

        foreach (var appServiceInfo in appServices)
        {
            services.AddAppServiceInfo(appServiceInfo);
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
    /// <param name="appServiceInfo">The <see cref="IAppServiceInfo"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddAppServiceInfo(
        this IServiceCollection services,
        IAppServiceInfo appServiceInfo)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        var metadataType = appServiceInfo.MetadataType ?? typeof(AppServiceMetadata);
        var contractType = appServiceInfo.ContractType ?? appServiceInfo.InstanceType ?? throw new InvalidOperationException(Strings.InjectionServiceCollectionExtensions_AddAppServiceInfo_ContractTypeNotSet.FormatWith(appServiceInfo));
        var instanceType = appServiceInfo.InstanceType;
        if (appServiceInfo.AsOpenGeneric && instanceType is null)
        {
            throw new InvalidOperationException(Strings.InjectionServiceCollectionExtensions_AddAppServiceInfo_MustProvideServiceTypeForOpenGenerics.FormatWith(appServiceInfo));
        }

        return appServiceInfo switch
        {
            { InstanceType: not null, Lifetime: AppServiceLifetime.Singleton } =>
                services.AddSingleton(contractType, instanceType!, metadataType, appServiceInfo.Metadata),
            { InstanceType: not null, Lifetime: AppServiceLifetime.Scoped } =>
                services.AddScoped(contractType, instanceType!, metadataType, appServiceInfo.Metadata),
            { InstanceType: not null } =>
                services.AddTransient(contractType, instanceType!, metadataType, appServiceInfo.Metadata),
            { InstanceFactory: not null, Lifetime: AppServiceLifetime.Singleton } =>
                services.AddSingleton(contractType, appServiceInfo.InstanceFactory, metadataType, appServiceInfo.Metadata),
            { InstanceFactory: not null, Lifetime: AppServiceLifetime.Scoped } =>
                services.AddScoped(contractType, appServiceInfo.InstanceFactory, metadataType, appServiceInfo.Metadata),
            { InstanceFactory: not null } =>
                services.AddTransient(contractType, appServiceInfo.InstanceFactory, metadataType, appServiceInfo.Metadata),
            { InstancingStrategy: not null } =>
                services.AddSingleton(contractType, appServiceInfo.InstancingStrategy)
                    .AddServiceFactories(contractType, provider => appServiceInfo.InstancingStrategy, metadataType, appServiceInfo.Metadata),
            _ => services
        };
    }

    /// <summary>
    /// Registers a service as singleton.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="contractType">The contract type.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="metadataType">The metadata type. If not provided, it is considered <see cref="AppServiceMetadata"/>.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <paramref name="serviceType"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSingleton(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? metadataType = null,
        IDictionary<string, object?>? metadata = null)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        metadataType ??= typeof(AppServiceMetadata);

        services.AddSingleton(serviceType);
        if (contractType != serviceType)
        {
            services.AddSingleton(contractType, provider => provider.GetRequiredService(serviceType));
        }

        return metadata is null
            ? services.AddServiceFactories(contractType, serviceType, metadataType)
            : services.AddServiceFactories(contractType, serviceType, metadataType, metadata);
    }

    /// <summary>
    /// Registers a service as singleton.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <typeparamref name="TService"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSingleton<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata = null)
        where TContract : class
        where TService : class, TContract =>
        AddSingleton<TContract, TService, AppServiceMetadata>(services, metadata);

    /// <summary>
    /// Registers a service as singleton.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <typeparamref name="TService"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSingleton<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata = null)
        where TContract : class
        where TService : class, TContract
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        services.AddSingleton(typeof(TService));
        if (typeof(TContract) != typeof(TService))
        {
            services.AddSingleton(typeof(TContract), provider => provider.GetRequiredService(typeof(TService)));
        }

        return metadata is null
            ? services.AddServiceFactories<TContract, TService, TMetadata>()
            : services.AddServiceFactories<TContract, TService, TMetadata>(metadata);
    }

    /// <summary>
    /// Registers a service as singleton instance.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="contractType">The contract type.</param>
    /// <param name="instance">The service instance.</param>
    /// <param name="metadataType">The metadata type. If not provided, it is considered <see cref="AppServiceMetadata"/>.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSingleton(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        object instance,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? metadataType = null,
        IDictionary<string, object?>? metadata = null)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        instance = instance ?? throw new ArgumentNullException(nameof(instance));
        metadataType ??= typeof(AppServiceMetadata);

        return services
            .AddSingleton(contractType, instance)
            .AddServiceFactories(contractType, _ => instance, metadataType, metadata);
    }

    /// <summary>
    /// Registers a service as singleton instance.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="instance">The service instance.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSingleton<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(
        this IServiceCollection services,
        TContract instance,
        IDictionary<string, object?>? metadata = null)
        where TContract : class
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        return services
            .AddSingleton(instance)
            .AddServiceFactories<TContract, TMetadata>(_ => instance, metadata);
    }

    /// <summary>
    /// Registers a service as transient.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="contractType">The contract type.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="metadataType">The metadata type. If not provided, it is considered <see cref="AppServiceMetadata"/>.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <paramref name="serviceType"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddTransient(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? metadataType = null,
        IDictionary<string, object?>? metadata = null)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        metadataType ??= typeof(AppServiceMetadata);

        services.AddTransient(serviceType);
        if (contractType != serviceType)
        {
            services.AddTransient(contractType, provider => provider.GetRequiredService(serviceType));
        }

        return metadata is null
            ? services.AddServiceFactories(contractType, serviceType, metadataType)
            : services.AddServiceFactories(contractType, serviceType, metadataType, metadata);
    }

    /// <summary>
    /// Registers a service as transient.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <typeparamref name="TService"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddTransient<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata = null)
        where TContract : class
        where TService : class, TContract =>
        AddTransient<TContract, TService, AppServiceMetadata>(services, metadata);

    /// <summary>
    /// Registers a service as transient.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <typeparamref name="TService"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddTransient<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata = null)
        where TContract : class
        where TService : class, TContract
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        services.AddTransient(typeof(TService));
        if (typeof(TContract) != typeof(TService))
        {
            services.AddTransient(typeof(TContract), provider => provider.GetRequiredService(typeof(TService)));
        }

        return metadata is null
            ? services.AddServiceFactories<TContract, TService, TMetadata>()
            : services.AddServiceFactories<TContract, TService, TMetadata>(metadata);
    }

    /// <summary>
    /// Registers a service as scoped.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="contractType">The contract type.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="metadataType">The metadata type. If not provided, it is considered <see cref="AppServiceMetadata"/>.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <paramref name="serviceType"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddScoped(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? metadataType = null,
        IDictionary<string, object?>? metadata = null)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        metadataType ??= typeof(AppServiceMetadata);

        services.AddScoped(serviceType);
        if (contractType != serviceType)
        {
            services.AddScoped(contractType, provider => provider.GetRequiredService(serviceType));
        }

        return metadata is null
            ? services.AddServiceFactories(contractType, serviceType, metadataType)
            : services.AddServiceFactories(contractType, serviceType, metadataType, metadata);
    }

    /// <summary>
    /// Registers a service as scoped.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <typeparamref name="TService"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddScoped<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata = null)
        where TContract : class
        where TService : class, TContract =>
        AddScoped<TContract, TService, AppServiceMetadata>(services, metadata);

    /// <summary>
    /// Registers a service as scoped.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="metadata">The metadata. If not provided, it will be collected from the <typeparamref name="TService"/>.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddScoped<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata = null)
        where TContract : class
        where TService : class, TContract
    {
        services = services ?? throw new ArgumentNullException(nameof(services));

        services.AddScoped(typeof(TService));
        if (typeof(TContract) != typeof(TService))
        {
            services.AddScoped(typeof(TContract), provider => provider.GetRequiredService(typeof(TService)));
        }

        return metadata is null
            ? services.AddServiceFactories<TContract, TService, TMetadata>()
            : services.AddServiceFactories<TContract, TService, TMetadata>(metadata);
    }

    /// <summary>
    /// Registers a service with factory as singleton.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="contractType">The contract type.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="metadataType">The metadata type. If not provided, it is considered <see cref="AppServiceMetadata"/>.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSingleton(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        Func<IServiceProvider, object> serviceFactory,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? metadataType = null,
        IDictionary<string, object?>? metadata = null)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        metadataType ??= typeof(AppServiceMetadata);

        return services
            .AddSingleton(contractType, serviceFactory)
            .AddServiceFactories(contractType, serviceFactory, metadataType, metadata);
    }

    /// <summary>
    /// Registers a service as singleton.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSingleton<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(
        this IServiceCollection services,
        Func<IServiceProvider, TContract> serviceFactory,
        IDictionary<string, object?>? metadata = null)
        where TContract : class
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return services
            .AddSingleton(serviceFactory)
            .AddServiceFactories<TContract, TMetadata>(serviceFactory, metadata);
    }

    /// <summary>
    /// Registers a service with factory as transient.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="contractType">The contract type.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="metadataType">The metadata type. If not provided, it is considered <see cref="AppServiceMetadata"/>.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddTransient(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        Func<IServiceProvider, object> serviceFactory,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? metadataType = null,
        IDictionary<string, object?>? metadata = null)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        metadataType ??= typeof(AppServiceMetadata);

        return services
            .AddTransient(contractType, serviceFactory)
            .AddServiceFactories(contractType, serviceFactory, metadataType, metadata);
    }

    /// <summary>
    /// Registers a service as transient.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddTransient<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(
        this IServiceCollection services,
        Func<IServiceProvider, TContract> serviceFactory,
        IDictionary<string, object?>? metadata = null)
        where TContract : class
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return services
            .AddTransient(serviceFactory)
            .AddServiceFactories<TContract, TMetadata>(serviceFactory, metadata);
    }

    /// <summary>
    /// Registers a service with factory as scoped.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="contractType">The contract type.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="metadataType">The metadata type. If not provided, it is considered <see cref="AppServiceMetadata"/>.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddScoped(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
        Func<IServiceProvider, object> serviceFactory,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? metadataType = null,
        IDictionary<string, object?>? metadata = null)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        metadataType ??= typeof(AppServiceMetadata);

        return services
            .AddScoped(contractType, serviceFactory)
            .AddServiceFactories(contractType, serviceFactory, metadataType, metadata);
    }

    /// <summary>
    /// Registers a service as scoped.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceFactory">The service factory.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddScoped<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>(
        this IServiceCollection services,
        Func<IServiceProvider, TContract> serviceFactory,
        IDictionary<string, object?>? metadata = null)
        where TContract : class
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

        return services
            .AddScoped(serviceFactory)
            .AddServiceFactories<TContract, TMetadata>(serviceFactory, metadata);
    }

    private static IServiceCollection AddServiceFactories<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]T,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]TMetadata>(this IServiceCollection services)
        where T : class
        where TService : class, T
    {
        services
            .AddTransient<T>(provider => provider.GetRequiredService<TService>())
            .AddTransient<Lazy<T>, LazyService<T, TService>>()
            .AddTransient<Lazy<T, TMetadata>, LazyService<T, TService, TMetadata>>()
            .AddTransient<IExportFactory<T>, FactoryService<T, TService>>()
            .AddTransient<IExportFactory<T, TMetadata>, FactoryService<T, TService, TMetadata>>();

        if (typeof(TMetadata) != typeof(AppServiceMetadata))
        {
            services
                .AddTransient<Lazy<T, AppServiceMetadata>, LazyService<T, TService, AppServiceMetadata>>()
                .AddTransient<IExportFactory<T, AppServiceMetadata>, FactoryService<T, TService, AppServiceMetadata>>();
        }

        return services;
    }

    private static IServiceCollection AddServiceFactories<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]T,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]TMetadata>(
        this IServiceCollection services,
        IDictionary<string, object?>? metadata)
        where T : class
        where TService : class, T
    {
        services
            .AddTransient<T>(provider => provider.GetRequiredService<TService>())
            .AddTransient<Lazy<T>, LazyService<T, TService>>()
            .AddTransient<Lazy<T, TMetadata>>(provider => new LazyService<T, TService, TMetadata>(provider, metadata))
            .AddTransient<IExportFactory<T>, FactoryService<T, TService>>()
            .AddTransient<IExportFactory<T, TMetadata>>(provider => new FactoryService<T, TService, TMetadata>(provider, metadata));

        if (typeof(TMetadata) != typeof(AppServiceMetadata))
        {
            services
                .AddTransient<Lazy<T, AppServiceMetadata>>(provider => new LazyService<T, TService, AppServiceMetadata>(provider, metadata))
                .AddTransient<IExportFactory<T, AppServiceMetadata>>(provider => new FactoryService<T, TService, AppServiceMetadata>(provider, metadata));
        }

        return services;
    }

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
            .AddTransient<Lazy<T>>(provider => new Lazy<T>(() => factory(provider)))
            .AddTransient<Lazy<T, TMetadata>>(provider => new Lazy<T, TMetadata>(() => factory(provider), ServiceHelper.GetServiceMetadata<TMetadata>(metadata)))
            .AddTransient<IExportFactory<T>>(provider => new ExportFactory<T>(() => factory(provider)))
            .AddTransient<IExportFactory<T, TMetadata>>(provider => new ExportFactory<T, TMetadata>(() => factory(provider), metadata));

        if (typeof(TMetadata) != typeof(AppServiceMetadata))
        {
            services
                .AddTransient<Lazy<T, AppServiceMetadata>>(provider => new Lazy<T, AppServiceMetadata>(() => factory(provider), new AppServiceMetadata(metadata)))
                .AddTransient<IExportFactory<T, AppServiceMetadata>>(provider => new ExportFactory<T, AppServiceMetadata>(() => factory(provider), metadata));
        }

        return services;
    }

    private static IServiceCollection AddServiceFactories(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type contractType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type metadataType)
    {
        services
            .AddTransient(typeof(Lazy<>).MakeGenericType(contractType), typeof(LazyService<,>).MakeGenericType(contractType, serviceType))
            .AddTransient(typeof(Lazy<,>).MakeGenericType(contractType, metadataType), typeof(LazyService<,,>).MakeGenericType(contractType, serviceType, metadataType))
            .AddTransient(typeof(IExportFactory<>).MakeGenericType(contractType), typeof(FactoryService<,>).MakeGenericType(contractType, serviceType))
            .AddTransient(typeof(IExportFactory<,>).MakeGenericType(contractType, metadataType), typeof(FactoryService<,,>).MakeGenericType(contractType, serviceType, metadataType));

        if (metadataType != typeof(AppServiceMetadata))
        {
            services
                .AddTransient(typeof(Lazy<,>).MakeGenericType(contractType, typeof(AppServiceMetadata)), typeof(LazyService<,,>).MakeGenericType(contractType, serviceType, typeof(AppServiceMetadata)))
                .AddTransient(typeof(IExportFactory<,>).MakeGenericType(contractType, typeof(AppServiceMetadata)), typeof(FactoryService<,,>).MakeGenericType(contractType, serviceType, typeof(AppServiceMetadata)));
        }

        return services;
    }

    private static IServiceCollection AddServiceFactories(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type contractType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type metadataType,
        IDictionary<string, object?> metadata)
    {
        services
            .AddTransient(typeof(Lazy<>).MakeGenericType(contractType), typeof(LazyService<,>).MakeGenericType(contractType, serviceType))
            .AddTransient(typeof(Lazy<,>).MakeGenericType(contractType, metadataType), provider => Activator.CreateInstance(typeof(LazyService<,,>).MakeGenericType(contractType, serviceType, metadataType), provider, metadata)!)
            .AddTransient(typeof(IExportFactory<>).MakeGenericType(contractType), typeof(FactoryService<,>).MakeGenericType(contractType, serviceType))
            .AddTransient(typeof(IExportFactory<,>).MakeGenericType(contractType, metadataType), provider => Activator.CreateInstance(typeof(FactoryService<,,>).MakeGenericType(contractType, serviceType, metadataType), provider, metadata)!);

        if (metadataType != typeof(AppServiceMetadata))
        {
            services
                .AddTransient(typeof(Lazy<,>).MakeGenericType(contractType, typeof(AppServiceMetadata)), provider => Activator.CreateInstance(typeof(LazyService<,,>).MakeGenericType(contractType, serviceType, typeof(AppServiceMetadata)), provider, metadata)!)
                .AddTransient(typeof(IExportFactory<,>).MakeGenericType(contractType, typeof(AppServiceMetadata)), provider => Activator.CreateInstance(typeof(FactoryService<,,>).MakeGenericType(contractType, serviceType, typeof(AppServiceMetadata)), provider, metadata)!);
        }

        return services;
    }

    private static IServiceCollection AddServiceFactories(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type contractType,
        Func<IServiceProvider, object> serviceFactory,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type metadataType,
        IDictionary<string, object?>? metadata)
    {
        var addServiceFactories = AddServiceFactoriesWithFactoryMethod.MakeGenericMethod(contractType, metadataType);
        addServiceFactories.Call(services, serviceFactory, metadata);
        return services;
    }
}