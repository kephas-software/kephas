// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Services.Builder;
    using Kephas.Services.Configuration;
    using Kephas.Services.Reflection;
    using Kephas.Services.Resources;

    /// <summary>
    /// Extensions for <see cref="IAppServiceCollection"/> for the injection subsystem.
    /// </summary>
    public static class AppServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the provided service instance as singleton, if not previously registered.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="appServices">The application services.</param>
        /// <param name="serviceInstance">The service instance.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="appServices"/>.
        /// </returns>
        public static IAppServiceCollection TryAdd<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(this IAppServiceCollection appServices, TContract serviceInstance, Action<IAppServiceInfoBuilder>? builder = null)
            where TContract : class
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

            return appServices.TryAddService(
                typeof(TContract),
                serviceInstance,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service instance as singleton.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="appServices">The application services.</param>
        /// <param name="serviceInstance">The service instance.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="appServices"/>.
        /// </returns>
        public static IAppServiceCollection Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(this IAppServiceCollection appServices, TContract serviceInstance, Action<IAppServiceInfoBuilder>? builder = null)
            where TContract : class
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

            return appServices.AddService(
                typeof(TContract),
                serviceInstance,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Replaces the provided service instance, registering it as singleton.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="appServices">The application services.</param>
        /// <param name="serviceInstance">The service instance.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="appServices"/>.
        /// </returns>
        public static IAppServiceCollection Replace<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(this IAppServiceCollection appServices, TContract serviceInstance, Action<IAppServiceInfoBuilder>? builder = null)
            where TContract : class
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

            return appServices.ReplaceService(
                typeof(TContract),
                serviceInstance,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service with implementation type, by default as singleton, if not previously registered.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <typeparam name="TService">Type of the service implementation.</typeparam>
        /// <param name="appServices">The application services.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="appServices"/>.
        /// </returns>
        public static IAppServiceCollection TryAdd<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(this IAppServiceCollection appServices, Action<IAppServiceInfoBuilder>? builder = null)
            where TContract : class
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));

            return appServices.TryAddService(
                typeof(TContract),
                typeof(TService),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service with implementation type, by default as singleton.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <typeparam name="TService">Type of the service implementation.</typeparam>
        /// <param name="appServices">The application services.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="appServices"/>.
        /// </returns>
        public static IAppServiceCollection Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(this IAppServiceCollection appServices, Action<IAppServiceInfoBuilder>? builder = null)
            where TContract : class
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));

            return appServices.AddService(
                typeof(TContract),
                typeof(TService),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Replaces the provided service with implementation type, registering it by default as singleton.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <typeparam name="TService">Type of the service implementation.</typeparam>
        /// <param name="appServices">The application services.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="appServices"/>.
        /// </returns>
        public static IAppServiceCollection Replace<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(this IAppServiceCollection appServices, Action<IAppServiceInfoBuilder>? builder = null)
            where TContract : class
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));

            return appServices.ReplaceService(
                typeof(TContract),
                typeof(TService),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service with implementation type, by default as singleton, if not previously registered.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="appServices">The application services.</param>
        /// <param name="serviceType">The service type.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="appServices"/>.
        /// </returns>
        public static IAppServiceCollection TryAdd<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(this IAppServiceCollection appServices, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType, Action<IAppServiceInfoBuilder>? builder = null)
            where TContract : class
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

            return appServices.TryAddService(
                typeof(TContract),
                serviceType,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service with implementation type, by default as singleton.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="appServices">The application services.</param>
        /// <param name="serviceType">The service type.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="appServices"/>.
        /// </returns>
        public static IAppServiceCollection Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(this IAppServiceCollection appServices, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType, Action<IAppServiceInfoBuilder>? builder = null)
            where TContract : class
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

            return appServices.AddService(
                typeof(TContract),
                serviceType,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Replaces the provided service with implementation type, registering it by default as singleton.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="appServices">The application services.</param>
        /// <param name="serviceType">The service type.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="appServices"/>.
        /// </returns>
        public static IAppServiceCollection Replace<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(this IAppServiceCollection appServices, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType, Action<IAppServiceInfoBuilder>? builder = null)
            where TContract : class
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

            return appServices.ReplaceService(
                typeof(TContract),
                serviceType,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service, by default as singleton factory, if not previously registered.
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
            Action<IAppServiceInfoBuilder>? builder = null)
            where TContract : class
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return appServices.TryAddService(
                typeof(TContract),
                (Func<IServiceProvider, object>)(_ => (object)serviceFactory()),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service, by default as singleton factory.
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
            Action<IAppServiceInfoBuilder>? builder = null)
            where TContract : class
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return appServices.AddService(
                typeof(TContract),
                (Func<IServiceProvider, object>)(_ => (object)serviceFactory()),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Replaces the provided service, registering it by default as singleton factory.
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
            Action<IAppServiceInfoBuilder>? builder = null)
            where TContract : class
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return appServices.ReplaceService(
                typeof(TContract),
                (Func<IServiceProvider, object>)(_ => (object)serviceFactory()),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service, by default as singleton factory, if not previously registered.
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
            Action<IAppServiceInfoBuilder>? builder = null)
            where TContract : class
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return appServices.TryAddService(
                typeof(TContract),
                (Func<IServiceProvider, object>)(injector => (object)serviceFactory(injector)),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service, by default as singleton factory.
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
            Action<IAppServiceInfoBuilder>? builder = null)
            where TContract : class
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return appServices.AddService(
                typeof(TContract),
                (Func<IServiceProvider, object>)(injector => (object)serviceFactory(injector)),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Replaces the provided service, registering it by default as singleton factory.
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
            Action<IAppServiceInfoBuilder>? builder = null)
            where TContract : class
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return appServices.ReplaceService(
                typeof(TContract),
                (Func<IServiceProvider, object>)(injector => (object)serviceFactory(injector)),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service, by default as singleton factory, if not previously registered.
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
            Action<IAppServiceInfoBuilder>? builder = null)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return appServices.TryAddService(
                contractType,
                (Func<IServiceProvider, object>)(_ => (object)serviceFactory()),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service, by default as singleton factory.
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
            Action<IAppServiceInfoBuilder>? builder = null)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return appServices.AddService(
                contractType,
                (Func<IServiceProvider, object>)(_ => (object)serviceFactory()),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Replaces the provided service, registering it by default as singleton factory.
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
            Action<IAppServiceInfoBuilder>? builder = null)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return appServices.ReplaceService(
                contractType,
                (Func<IServiceProvider, object>)(_ => (object)serviceFactory()),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service, by default as singleton factory, if not previously registered.
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
            Action<IAppServiceInfoBuilder>? builder = null)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return appServices.TryAddService(
                contractType,
                (Func<IServiceProvider, object>)(injector => (object)serviceFactory(injector)),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service, by default as singleton factory.
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
            Action<IAppServiceInfoBuilder>? builder = null)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return appServices.AddService(
                contractType,
                (Func<IServiceProvider, object>)(injector => (object)serviceFactory(injector)),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Replaces the provided service, registering it by default as singleton factory.
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
            Action<IAppServiceInfoBuilder>? builder = null)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return appServices.ReplaceService(
                contractType,
                (Func<IServiceProvider, object>)(injector => (object)serviceFactory(injector)),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service, by default as singleton, if not previously registered.
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
            Action<IAppServiceInfoBuilder>? builder = null)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

            appServices.TryAddService(
                contractType,
                serviceType,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
            return appServices;
        }

        /// <summary>
        /// Registers the provided service, by default as singleton.
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
            Action<IAppServiceInfoBuilder>? builder = null)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

            return appServices.AddService(
                contractType,
                serviceType,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Replaces the provided service, registering it by default as singleton.
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
            Action<IAppServiceInfoBuilder>? builder = null)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

            appServices.ReplaceService(
                contractType,
                serviceType,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
            return appServices;
        }

        /// <summary>
        /// Registers the provided service instance, by default as singleton, if not previously registered.
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
            Action<IAppServiceInfoBuilder>? builder = null)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

            appServices.TryAddService(
                contractType,
                serviceInstance,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
            return appServices;
        }

        /// <summary>
        /// Registers the provided service instance, by default as singleton.
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
            Action<IAppServiceInfoBuilder>? builder = null)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

            return appServices.AddService(
                contractType,
                serviceInstance,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Replaces the provided service instance, registering it by default as singleton.
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
            Action<IAppServiceInfoBuilder>? builder = null)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

            appServices.ReplaceService(
                contractType,
                serviceInstance,
                b =>
                {
                    b.Singleton();
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
               ?? throw new ArgumentException(string.Format(Strings.ServiceInstanceNotRegistered, contractType), nameof(contractType));

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
        /// Gets the application runtime.
        /// </summary>
        /// <param name="appServices">The application services.</param>
        /// <returns>
        /// The application runtime.
        /// </returns>
        public static IAppRuntime GetAppRuntime(this IAppServiceCollection appServices)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));

            return appServices.TryGetServiceInstance<IAppRuntime>()
                   ?? throw new InvalidOperationException("The application runtime is not registered.");
        }

        /// <summary>
        /// Adds the application services from the provided <see cref="IAppServiceInfoProvider"/>s.
        /// </summary>
        /// <param name="appServices">The application services.</param>
        /// <param name="contracts">The contract declarations.</param>
        /// <param name="services">The service declarations.</param>
        /// <param name="resolutionStrategy">The resolution strategy for ambiguous registrations.</param>
        /// <param name="logger">Optional. The logger.</param>
        /// <returns>The provided app services.</returns>
        public static IAppServiceCollection AddAppServices(
            this IAppServiceCollection appServices,
            IEnumerable<ContractDeclaration> contracts,
            IEnumerable<ServiceDeclaration> services,
            AmbiguousServiceResolutionStrategy resolutionStrategy = AmbiguousServiceResolutionStrategy.ForcePriority,
            ILogger? logger = null)
        {
            var existingContracts = appServices.TryGetServiceInstance<IContractDeclarationCollection>();
            contracts = existingContracts is null
                ? new ContractDeclarationCollection(contracts)
                : new ContractDeclarationCollection(
                    ((ICollection<ContractDeclaration>)new List<ContractDeclaration>(existingContracts)).AddRange(contracts));
            appServices.Replace((IContractDeclarationCollection)contracts);

            if (logger.IsDebugEnabled())
            {
                logger.Debug("Building the service map...");
            }

            var serviceMap = BuildServiceMap(
                contracts,
                services,
                logger);

            if (logger.IsDebugEnabled())
            {
                logger.Debug("Service map built.");
            }

            foreach (var (contractDeclarationType, serviceEntry) in serviceMap)
            {
                // first: split the contract info and the rest of the registrations.
                var registrations = serviceEntry.Registrations;
                var appContractDefinition = serviceEntry.ContractAppServiceInfo;

                if (registrations.Count == 0)
                {
                    if (logger.IsDebugEnabled())
                    {
                        logger.Warn("No service types registered for {contractDeclarationType}.", contractDeclarationType);
                    }

                    continue;
                }

                if (registrations.Count == 1)
                {
                    // register one service, no matter if multiple or single.
                    var appServiceInfo = registrations[0];
                    appServices.AddAppService(appServiceInfo, logger);
                    continue;
                }

                // order the services by override and processing priority, resolve overrides
                // and with the rest of the services:
                // 1. if multiple are registered, register them in the computed order.
                // 2. for single mode, pick the first one in the order and register it.
                var (sortedRegistrations, overriddenTypes) = SortRegistrations(registrations, logger);
                if (!appContractDefinition.AllowMultiple)
                {
                    var appServiceInfo = ResolveAmbiguousRegistration(
                        contractDeclarationType,
                        sortedRegistrations,
                        resolutionStrategy,
                        logger);
                    appServices.AddAppService(appServiceInfo, logger);
                }
                else
                {
                    var filteredServiceInfos = overriddenTypes.Count == 0
                        ? registrations
                        : registrations.Where(i => !overriddenTypes.Contains(((IAppServiceInfo)i).InstanceType!));
                    foreach (var appServiceInfo in filteredServiceInfos)
                    {
                        appServices.AddAppService(appServiceInfo, logger);
                    }
                }
            }

            return appServices;
        }

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
            Action<IAppServiceInfoBuilder>? builder = null)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            contractDeclarationType = contractDeclarationType ?? throw new ArgumentNullException(nameof(contractDeclarationType));
            instancingStrategy = instancingStrategy ?? throw new ArgumentNullException(nameof(instancingStrategy));

            var serviceBuilder = new AppServiceInfoBuilder(contractDeclarationType, instancingStrategy);
            builder?.Invoke(serviceBuilder);
            var appServiceInfo = serviceBuilder.Build();

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
            Action<IAppServiceInfoBuilder>? builder = null)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            contractDeclarationType = contractDeclarationType ?? throw new ArgumentNullException(nameof(contractDeclarationType));
            instancingStrategy = instancingStrategy ?? throw new ArgumentNullException(nameof(instancingStrategy));

            var serviceBuilder = new AppServiceInfoBuilder(contractDeclarationType, instancingStrategy);
            builder?.Invoke(serviceBuilder);
            appServices.Add(serviceBuilder.Build());

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
            Action<IAppServiceInfoBuilder>? builder = null)
        {
            appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
            contractDeclarationType = contractDeclarationType ?? throw new ArgumentNullException(nameof(contractDeclarationType));
            instancingStrategy = instancingStrategy ?? throw new ArgumentNullException(nameof(instancingStrategy));

            var serviceBuilder = new AppServiceInfoBuilder(contractDeclarationType, instancingStrategy);
            builder?.Invoke(serviceBuilder);
            appServices.Replace(serviceBuilder.Build());

            return appServices;
        }

        private static void AddAppService(
            this IAppServiceCollection appServices,
            IAppServiceInfo appServiceInfo,
            ILogger? logger)
        {
            CheckContractType(appServiceInfo, logger);

            if (logger.IsDebugEnabled())
            {
                logger.Debug(ToJsonString(appServiceInfo));
            }

            appServices.Add(appServiceInfo);
        }

        private static string ToJsonString(IAppServiceInfo appServiceInfo)
        {
            return $"{{ '{(appServiceInfo.ContractDeclarationType ?? appServiceInfo.ContractType)?.Name}': {appServiceInfo.ToJsonString()} }}";
        }

        private static void CheckContractType(IAppServiceInfo appServiceInfo, ILogger? logger)
        {
            var contractDeclarationType = appServiceInfo.ContractDeclarationType;
            var contractType = appServiceInfo.ContractType;
            if (contractDeclarationType is null || contractType is null)
            {
                return;
            }

            if (contractType.IsGenericTypeDefinition)
            {
                // TODO check to see if any of the interfaces have as generic definition the exported contract.
            }
            else if (!contractType.IsAssignableFrom(contractDeclarationType))
            {
                var contractValidationMessage = string.Format(
                    Strings.AppServiceContractTypeDoesNotMatchServiceContract,
                    contractType,
                    contractDeclarationType);
                logger.Error(contractValidationMessage);
                throw new InjectionException(contractValidationMessage);
            }
        }


        private static AppServiceInfo ResolveAmbiguousRegistration(
            Type contractDeclarationType,
            IList<(AppServiceInfo appServiceInfo, Priority overridePriority)> sortedRegistrations,
            AmbiguousServiceResolutionStrategy serviceResolutionStrategy,
            ILogger? logger)
        {
            if (sortedRegistrations.Count == 1)
            {
                return sortedRegistrations[0].appServiceInfo;
            }

            if (logger.IsDebugEnabled())
            {
                logger.Debug(
                    Strings.MultipleRegistrationsForAppServiceContract,
                    contractDeclarationType,
                    serviceResolutionStrategy,
                    sortedRegistrations.Select(item => $"{item.appServiceInfo}:{item.overridePriority}"));
            }

            var priority = sortedRegistrations[0].overridePriority;
            return serviceResolutionStrategy switch
            {
                AmbiguousServiceResolutionStrategy.UseFirst =>
                    sortedRegistrations[0].appServiceInfo,
                AmbiguousServiceResolutionStrategy.UseLast =>
                    sortedRegistrations.Last(s => s.overridePriority == priority).appServiceInfo,
                AmbiguousServiceResolutionStrategy.ForcePriority when priority == sortedRegistrations[1].overridePriority =>
                    throw new AmbiguousServiceResolutionException(string.Format(
                        Strings.AmbiguousOverrideForAppServiceContract,
                        contractDeclarationType,
                        string.Join(", ", sortedRegistrations.Select(item => $"{item.appServiceInfo}:{item.overridePriority}")))),
                _ => sortedRegistrations[0].appServiceInfo
            };
        }

        private static (IList<(AppServiceInfo appServiceInfo, Priority overridePriority)> sortedRegistrations, IList<Type> overriddenTypes) SortRegistrations(IEnumerable<AppServiceInfo> appServiceInfos, ILogger? logger)
        {
            // leave the implementation with the runtime type info
            // so that it may be possible to use runtime added attributes
            var overrideChain = appServiceInfos
                .Select(si => (appServiceInfo: si, overridePriority: (Priority)(si.Metadata?.TryGetValue(nameof(AppServiceMetadata.OverridePriority)) ?? Priority.Normal)))
                .OrderBy(item => item.overridePriority)
                .ToList();

            // get the overridden services which should be eliminated
            var overriddenTypes = overrideChain
                .Where(kv => (bool)(kv.appServiceInfo.Metadata?.TryGetValue(nameof(AppServiceMetadata.IsOverride)) ?? false) && ((IAppServiceInfo)kv.appServiceInfo).InstanceType?.BaseType != null)
                .Select(kv => ((IAppServiceInfo)kv.appServiceInfo).InstanceType?.BaseType)
                .ToList();

            if (overriddenTypes.Count > 0)
            {
                if (logger.IsDebugEnabled())
                {
                    logger.Debug("Excluding the following overridden services: {serviceInfos}", appServiceInfos.Where(i => overriddenTypes.Contains(((IAppServiceInfo)i).InstanceType)).ToList());
                }

                // eliminate the overridden services
                overrideChain = overrideChain
                    .Where(kv => !overriddenTypes.Contains(((IAppServiceInfo)kv.appServiceInfo).InstanceType))
                    .ToList();
            }

            return (overrideChain, overriddenTypes!);
        }

        private static IDictionary<Type, ServiceEntry> BuildServiceMap(
            IEnumerable<ContractDeclaration> contractDeclarations,
            IEnumerable<ServiceDeclaration> serviceDeclarations,
            ILogger? logger)
        {
            var serviceMap = new Dictionary<Type, ServiceEntry>();

            if (logger.IsDebugEnabled())
            {
                logger.Debug("Entering {operation}...", nameof(BuildServiceMap));
            }

            foreach (var (contractDeclarationType, appServiceInfo) in contractDeclarations)
            {
                if (!serviceMap.TryGetValue(contractDeclarationType, out var serviceEntry))
                {
                    serviceMap.Add(contractDeclarationType, new ServiceEntry(contractDeclarationType, appServiceInfo));
                }
                else
                {
                    logger.Warn(
                        Strings.AppServiceCollectionMultipleContractDeclarationsWithSameType,
                        contractDeclarationType,
                        serviceEntry.ContractAppServiceInfo,
                        appServiceInfo);
                    serviceEntry.ContractAppServiceInfo = appServiceInfo;
                }
            }

            serviceDeclarations.ForEach(si => AddServiceType(serviceMap, si.ContractDeclarationType, si.ServiceType, logger));

            if (logger.IsDebugEnabled())
            {
                logger.Debug("Exiting {operation}...", nameof(BuildServiceMap));
            }

            return serviceMap;
        }


        private static void AddServiceType(
            IDictionary<Type, ServiceEntry> serviceMap,
            Type contractDeclarationType,
            Type serviceType,
            ILogger? logger)
        {
            if (logger.IsDebugEnabled())
            {
                logger.Debug("Adding service type {serviceType} for {contractDeclarationType}", serviceType, contractDeclarationType);
            }

            if (!serviceMap.TryGetValue(contractDeclarationType, out var serviceEntry))
            {
                // if the contract declaration type is not found in the map,
                // it may be because it is a constructed generic type and the
                // registration contains the generic type definition.
                if (contractDeclarationType.IsConstructedGenericType)
                {
                    var contractDeclarationTypeGenericDefinition = contractDeclarationType.GetGenericTypeDefinition();
                    if (serviceMap.TryGetValue(contractDeclarationTypeGenericDefinition, out serviceEntry))
                    {
                        // if the contract declaration based on the generic type definition is found,
                        // build a new contract declaration based on the constructed generic type
                        // and add a new entry in the map.
                        var appServiceInfoGenericDefinition = serviceEntry.Registrations.First();
                        var appServiceInfoDeclaration = new AppServiceInfo(appServiceInfoGenericDefinition, contractDeclarationType, contractDeclarationType);
                        var appServiceInfo = new AppServiceInfo(appServiceInfoDeclaration, contractDeclarationType, contractDeclarationType, serviceType);
                        ((IAppServiceInfo)appServiceInfo).AddMetadata(ServiceHelper.GetServiceMetadata(serviceType, contractDeclarationType));

                        // add to the list of service infos on the first place the declaration.
                        serviceMap[contractDeclarationType] = new ServiceEntry(contractDeclarationType, appServiceInfoDeclaration) { Registrations = { appServiceInfo } };
                        return;
                    }
                }
            }
            else
            {
                var appServiceInfoDeclaration = serviceEntry.ContractAppServiceInfo;
                var appServiceInfo = new AppServiceInfo(appServiceInfoDeclaration, appServiceInfoDeclaration.ContractType ?? contractDeclarationType, contractDeclarationType, serviceType);
                ((IAppServiceInfo)appServiceInfo).AddMetadata(ServiceHelper.GetServiceMetadata(serviceType, contractDeclarationType));
                serviceEntry.Registrations.Add(appServiceInfo);
                return;
            }

            logger.Warn(
                "Service type {contractType} declares a contract of {contractDeclarationType}, but the contract is not registered as an application service contract.",
                serviceType,
                contractDeclarationType);
        }

        private record ServiceEntry(Type ContractDeclarationType, IAppServiceInfo ContractAppServiceInfo)
        {
            public IList<AppServiceInfo> Registrations { get; } = new List<AppServiceInfo>();

            public IList<Type> OverriddenTypes { get; } = new List<Type>();

            public IAppServiceInfo ContractAppServiceInfo { get; set; } = ContractAppServiceInfo;
        }
    }
}