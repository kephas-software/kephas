// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service collection extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Kephas.Extensions.DependencyInjection.Resources;
    using Kephas.Injection;
    using Kephas.Injection.Builder;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class InjectionServiceCollectionExtensions
    {
        private static MethodInfo AddServiceFactoriesMethod = ReflectionHelper.GetGenericMethodOf(_ => AddServiceFactoriesOfObject<string, string>(null!, null!, null));

        /// <summary>
        /// Includes the service collection in the composition.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// An IServiceCollection.
        /// </returns>
        public static IServiceCollection UseAmbientServices(this IServiceCollection services, IAmbientServices ambientServices)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            var buildContext = new InjectionBuildContext(ambientServices);
            buildContext.AddAppServices();

            foreach (var appServiceInfo in ambientServices)
            {
                services.AddAppServiceInfo(appServiceInfo);
            }

            return services;
        }

        public static IServiceProvider BuildWithDependencyInjection(this IAmbientServices ambientServices, IServiceCollection services)
        {
            services.UseAmbientServices(ambientServices);

            return services.BuildServiceProvider();
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
            var contractType = appServiceInfo.ContractType ?? throw new InvalidOperationException(Strings.InjectionServiceCollectionExtensions_AddAppServiceInfo_ContractTypeNotSet);
            return appServiceInfo switch
            {
                { InstanceType: not null, Lifetime: AppServiceLifetime.Singleton } =>
                    services.AddSingleton(appServiceInfo.InstanceType)
                        .AddServiceFactories(contractType, appServiceInfo.InstanceType, metadataType, appServiceInfo.Metadata),
                { InstanceType: not null, Lifetime: AppServiceLifetime.Scoped } =>
                    services.AddScoped(appServiceInfo.InstanceType)
                        .AddServiceFactories(contractType, appServiceInfo.InstanceType, metadataType, appServiceInfo.Metadata),
                { InstanceType: not null } =>
                    services.AddTransient(appServiceInfo.InstanceType)
                        .AddServiceFactories(contractType, appServiceInfo.InstanceType, metadataType, appServiceInfo.Metadata),
                { InstanceFactory: not null, Lifetime: AppServiceLifetime.Singleton } =>
                    services.AddSingleton(contractType, appServiceInfo.InstanceFactory)
                        .AddServiceFactories(contractType, appServiceInfo.InstanceFactory, metadataType, appServiceInfo.Metadata),
                { InstanceFactory: not null, Lifetime: AppServiceLifetime.Scoped } =>
                    services.AddScoped(contractType, appServiceInfo.InstanceFactory)
                        .AddServiceFactories(contractType, appServiceInfo.InstanceFactory, metadataType, appServiceInfo.Metadata),
                { InstanceFactory: not null } =>
                    services.AddTransient(contractType, appServiceInfo.InstanceFactory)
                        .AddServiceFactories(contractType, appServiceInfo.InstanceFactory, metadataType, appServiceInfo.Metadata),
                { Instance: not null } =>
                    services.AddSingleton(contractType, appServiceInfo.Instance)
                        .AddServiceFactories(contractType, provider => appServiceInfo.InstancingStrategy!, metadataType, appServiceInfo.Metadata),
                _ => services
            };
        }

        public static IServiceCollection AddSingleton(
            this IServiceCollection services,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type metadataType,
            IDictionary<string, object?>? metadata)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            return services.AddSingleton(serviceType)
                .AddServiceFactories(contractType, serviceType, metadataType, metadata);
        }

        public static IServiceCollection AddSingleton(
            this IServiceCollection services,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type metadataType)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            return services.AddSingleton(serviceType)
                .AddServiceFactories(contractType, serviceType, metadataType);
        }

        public static IServiceCollection AddTransient(
            this IServiceCollection services,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type metadataType,
            IDictionary<string, object?>? metadata)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            return services.AddTransient(serviceType)
                .AddServiceFactories(contractType, serviceType, metadataType, metadata);
        }

        public static IServiceCollection AddTransient(
            this IServiceCollection services,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type metadataType)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            return services.AddTransient(serviceType)
                .AddServiceFactories(contractType, serviceType, metadataType);
        }

        public static IServiceCollection AddScoped(
            this IServiceCollection services,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type metadataType,
            IDictionary<string, object?>? metadata)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            return services.AddScoped(serviceType)
                .AddServiceFactories(contractType, serviceType, metadataType, metadata);
        }

        public static IServiceCollection AddScoped(
            this IServiceCollection services,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type metadataType)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            return services.AddScoped(serviceType)
                .AddServiceFactories(contractType, serviceType, metadataType);
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
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            metadataType = metadataType ?? throw new ArgumentNullException(nameof(metadataType));

            services
                .AddTransient(contractType, provider => provider.GetRequiredService(serviceType))
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
            IDictionary<string, object?>? metadata)
        {
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            metadataType = metadataType ?? throw new ArgumentNullException(nameof(metadataType));

            services
                .AddTransient(contractType, provider => provider.GetRequiredService(serviceType))
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
            var addServiceFactories = AddServiceFactoriesMethod.MakeGenericMethod(contractType, metadataType);
            addServiceFactories.Call(services, serviceFactory, metadata);
            return services;
        }
    }
}