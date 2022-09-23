﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionAmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Injection;
    using Kephas.Injection.Builder;
    using Kephas.Injection.Configuration;
    using Kephas.Injection.Resources;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Extensions for <see cref="IAmbientServices"/> for the injection subsystem.
    /// </summary>
    public static class InjectionAmbientServicesExtensions
    {
        internal const string AppServiceInfosKey = "__" + nameof(AppServiceInfosKey);

        /// <summary>
        /// Registers the provided service instance.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceInstance">The service instance.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(this IAmbientServices ambientServices, TContract serviceInstance, Action<IRegistrationBuilder>? builder = null)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

            return ambientServices.AddService(typeof(TContract), serviceInstance, builder);
        }

        /// <summary>
        /// Replaces the provided service instance.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceInstance">The service instance.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Replace<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(this IAmbientServices ambientServices, TContract serviceInstance, Action<IRegistrationBuilder>? builder = null)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

            return ambientServices.ReplaceService(typeof(TContract), serviceInstance, builder);
        }

        /// <summary>
        /// Registers the provided service with implementation type as singleton.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <typeparam name="TService">Type of the service implementation.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(this IAmbientServices ambientServices, Action<IRegistrationBuilder>? builder = null)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            return ambientServices.AddService(
                typeof(TContract),
                typeof(TService),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Replaces the provided service with implementation type as singleton.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <typeparam name="TService">Type of the service implementation.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Replace<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TService>(this IAmbientServices ambientServices, Action<IRegistrationBuilder>? builder = null)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            return ambientServices.ReplaceService(
                typeof(TContract),
                typeof(TService),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service with implementation type as singleton.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceType">The service type.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(this IAmbientServices ambientServices, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType, Action<IRegistrationBuilder>? builder = null)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

            return ambientServices.AddService(
                typeof(TContract),
                serviceType,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Replaces the provided service with implementation type as singleton.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceType">The service type.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Replace<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(this IAmbientServices ambientServices, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType, Action<IRegistrationBuilder>? builder = null)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

            return ambientServices.ReplaceService(
                typeof(TContract),
                serviceType,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service as singleton factory.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(
            this IAmbientServices ambientServices,
            Func<TContract> serviceFactory,
            Action<IRegistrationBuilder>? builder = null)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return ambientServices.AddService(
                typeof(TContract),
                (Func<IInjector, object>)(_ => (object)serviceFactory()),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Replaces the provided service as singleton factory.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Replace<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(
            this IAmbientServices ambientServices,
            Func<TContract> serviceFactory,
            Action<IRegistrationBuilder>? builder = null)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return ambientServices.ReplaceService(
                typeof(TContract),
                (Func<IInjector, object>)(_ => (object)serviceFactory()),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service as singleton factory.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(
            this IAmbientServices ambientServices,
            Func<IInjector, TContract> serviceFactory,
            Action<IRegistrationBuilder>? builder = null)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return ambientServices.AddService(
                typeof(TContract),
                (Func<IInjector, object>)(injector => (object)serviceFactory(injector)),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Replaces the provided service as singleton factory.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Replace<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TContract>(
            this IAmbientServices ambientServices,
            Func<IInjector, TContract> serviceFactory,
            Action<IRegistrationBuilder>? builder = null)
            where TContract : class
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return ambientServices.ReplaceService(
                typeof(TContract),
                (Func<IInjector, object>)(injector => (object)serviceFactory(injector)),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service as singleton factory.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Add(
            this IAmbientServices ambientServices,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
            Func<object> serviceFactory,
            Action<IRegistrationBuilder>? builder = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return ambientServices.AddService(
                contractType,
                (Func<IInjector, object>)(_ => (object)serviceFactory()),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Replaces the provided service as singleton factory.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Replace(
            this IAmbientServices ambientServices,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
            Func<object> serviceFactory,
            Action<IRegistrationBuilder>? builder = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return ambientServices.ReplaceService(
                contractType,
                (Func<IInjector, object>)(_ => (object)serviceFactory()),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service as singleton factory.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Add(
            this IAmbientServices ambientServices,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
            Func<IInjector, object> serviceFactory,
            Action<IRegistrationBuilder>? builder = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return ambientServices.AddService(
                contractType,
                (Func<IInjector, object>)(injector => (object)serviceFactory(injector)),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Replaces the provided service as singleton factory.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Replace(
            this IAmbientServices ambientServices,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
            Func<IInjector, object> serviceFactory,
            Action<IRegistrationBuilder>? builder = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            return ambientServices.ReplaceService(
                contractType,
                (Func<IInjector, object>)(injector => (object)serviceFactory(injector)),
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
        }

        /// <summary>
        /// Registers the provided service as singleton.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="serviceType">The service implementation type.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Add(
            this IAmbientServices ambientServices,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
            Action<IRegistrationBuilder>? builder = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

            ambientServices.AddService(
                contractType,
                serviceType,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
            return ambientServices;
        }

        /// <summary>
        /// Replaces the provided service as singleton.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="serviceType">The service implementation type.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Replace(
            this IAmbientServices ambientServices,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType,
            Action<IRegistrationBuilder>? builder = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

            ambientServices.ReplaceService(
                contractType,
                serviceType,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
            return ambientServices;
        }

        /// <summary>
        /// Registers the provided service instance as singleton.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="serviceInstance">The service instance.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Add(
            this IAmbientServices ambientServices,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
            object serviceInstance,
            Action<IRegistrationBuilder>? builder = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

            ambientServices.AddService(
                contractType,
                serviceInstance,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
            return ambientServices;
        }

        /// <summary>
        /// Replaces the provided service instance as singleton.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="serviceInstance">The service instance.</param>
        /// <param name="builder">Optional. The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Replace(
            this IAmbientServices ambientServices,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contractType,
            object serviceInstance,
            Action<IRegistrationBuilder>? builder = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

            ambientServices.ReplaceService(
                contractType,
                serviceInstance,
                b =>
                {
                    b.Singleton();
                    builder?.Invoke(b);
                });
            return ambientServices;
        }

        /// <summary>
        /// Registers the provided service using a registration builder.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractDeclarationType">The contract declaration type.</param>
        /// <param name="instancingStrategy">The instancing strategy.</param>
        /// <param name="builder">The builder.</param>
        /// <returns>
        /// This <see cref="IAmbientServices"/>.
        /// </returns>
        internal static IAmbientServices AddService(
            this IAmbientServices ambientServices,
            Type contractDeclarationType,
            object instancingStrategy,
            Action<IRegistrationBuilder>? builder = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractDeclarationType = contractDeclarationType ?? throw new ArgumentNullException(nameof(contractDeclarationType));
            instancingStrategy = instancingStrategy ?? throw new ArgumentNullException(nameof(instancingStrategy));

            var serviceBuilder = new AppServiceInfoBuilder(contractDeclarationType, instancingStrategy);
            builder?.Invoke(serviceBuilder);
            ambientServices.Add(serviceBuilder.Build());

            return ambientServices;
        }

        /// <summary>
        /// Replaces the service with the same contract type, adding the provided service.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractDeclarationType">The contract declaration type.</param>
        /// <param name="instancingStrategy">The instancing strategy.</param>
        /// <param name="builder">The registration builder.</param>
        /// <returns>
        /// This <see cref="IAmbientServices"/>.
        /// </returns>
        internal static IAmbientServices ReplaceService(
            this IAmbientServices ambientServices,
            Type contractDeclarationType,
            object instancingStrategy,
            Action<IRegistrationBuilder>? builder = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractDeclarationType = contractDeclarationType ?? throw new ArgumentNullException(nameof(contractDeclarationType));
            instancingStrategy = instancingStrategy ?? throw new ArgumentNullException(nameof(instancingStrategy));

            var serviceBuilder = new AppServiceInfoBuilder(contractDeclarationType, instancingStrategy);
            builder?.Invoke(serviceBuilder);
            ambientServices.Replace(serviceBuilder.Build());

            return ambientServices;
        }

        /// <summary>
        /// Tries to get the service instance from the registered services,
        /// in case the service was registered using an instance.
        /// </summary>
        /// <param name="ambientServices">The service collection.</param>
        /// <param name="contractType">The contract type.</param>
        /// <returns>The service instance or <c>null</c>.</returns>
        public static object? TryGetServiceInstance(this IAmbientServices ambientServices, Type contractType)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));

            return ambientServices.LastOrDefault(i => i.ContractType == contractType && i.Instance is not null)?.Instance;
        }

        /// <summary>
        /// Tries to get the service instance from the registered services
        /// </summary>
        /// <typeparam name="T">The contract type.</typeparam>
        /// <param name="ambientServices">The service collection.</param>
        /// <returns>The service instance or <c>null</c>.</returns>
        public static T? TryGetServiceInstance<T>(this IAmbientServices ambientServices)
            where T : class
            => ambientServices.TryGetServiceInstance(typeof(T)) as T;

        /// <summary>
        /// Tries to get the service instance from the registered services,
        /// in case the service was registered using an instance.
        /// </summary>
        /// <param name="ambientServices">The service collection.</param>
        /// <param name="contractType">The contract type.</param>
        /// <returns>The service instance or <c>null</c>.</returns>
        public static object GetServiceInstance(this IAmbientServices ambientServices, Type contractType)
            => ambientServices.TryGetServiceInstance(contractType)
               ?? throw new ArgumentException(string.Format(Strings.ServiceInstanceNotRegistered, contractType), nameof(contractType));

        /// <summary>
        /// Gets the service instance from the registered services
        /// </summary>
        /// <typeparam name="T">The contract type.</typeparam>
        /// <param name="ambientServices">The service collection.</param>
        /// <returns>The service instance or <c>null</c>.</returns>
        [return: NotNull]
        public static T GetServiceInstance<T>(this IAmbientServices ambientServices)
            where T : class
            => (T)ambientServices.GetServiceInstance(typeof(T));

        /// <summary>
        /// Sets the injector to the ambient services.
        /// </summary>
        /// <typeparam name="TInjectorBuilder">Type of the injector builder.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="builderOptions">The injector builder configuration.</param>
        /// <remarks>The container builder type must provide a constructor with one parameter of type <see cref="IContext" />.</remarks>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IInjector BuildWith<TInjectorBuilder>(this IAmbientServices ambientServices, Action<TInjectorBuilder>? builderOptions = null)
            where TInjectorBuilder : IInjectorBuilder
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            var context = new InjectionBuildContext(ambientServices.GetAppRuntime().GetAppAssemblies());

            var containerBuilder = (TInjectorBuilder)Activator.CreateInstance(typeof(TInjectorBuilder), context)!;

            builderOptions?.Invoke(containerBuilder);

            return containerBuilder.Build();
        }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// The application runtime.
        /// </returns>
        public static IAppRuntime GetAppRuntime(this IAmbientServices ambientServices)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            return ambientServices.TryGetServiceInstance<IAppRuntime>()
                   ?? throw new InvalidOperationException("The application runtime is not registered.");
        }

        /// <summary>
        /// Adds the application services from the provided <see cref="IAppServiceInfosProvider"/>s.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="appServiceInfoProviders">The providers.</param>
        /// <param name="resolutionStrategy">The resolution strategy for ambiguous registrations.</param>
        /// <returns>The provided ambient services.</returns>
        public static IAmbientServices AddAppServices(
            this IAmbientServices ambientServices,
            IEnumerable<IAppServiceInfosProvider> appServiceInfoProviders,
            AmbiguousServiceResolutionStrategy resolutionStrategy = AmbiguousServiceResolutionStrategy.ForcePriority,
            ILogger logger)
        {
            // get all type infos from the injection assemblies
            var appServiceInfoList = appServiceInfoProviders
                .SelectMany(p => p.GetAppServiceContracts())
                .Select(t => t with
                {
                    ContractDeclarationType = t.ContractDeclarationType.ToNormalizedType()
                })
                .ToList();

            ambientServices.Replace<IContractDeclarationCollection>(new ContractDeclarationCollection(appServiceInfoList));

            if (logger.IsDebugEnabled())
            {
                logger.Debug("Aggregating the service types...");
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            IEnumerable<ServiceDeclaration> GetAppServices(IAppServiceInfosProvider appServiceInfosProvider)
            {
                if (logger.IsDebugEnabled())
                {
                    logger.Debug("Getting the app services from provider {provider}...", appServiceInfosProvider);
                }

                var appServices = appServiceInfosProvider.GetAppServices();

                if (logger.IsTraceEnabled())
                {
                    logger.Trace("Getting the app services from provider {provider} succeeded.", appServiceInfosProvider);
                }

                return appServices;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ServiceDeclaration NormalizeAppService(ServiceDeclaration serviceDeclaration)
            {
                var (serviceType, contractDeclarationType) = serviceDeclaration;

                if (logger.IsTraceEnabled())
                {
                    logger.Trace("Normalizing the service declaration for {serviceType}/{contractDeclarationType}.", serviceType, contractDeclarationType);
                }

                return new ServiceDeclaration(serviceType.ToNormalizedType(), contractDeclarationType.ToNormalizedType());
            }

            var serviceTypes = appServiceInfoProviders
                .SelectMany(GetAppServices)
                .Select(NormalizeAppService)
                .ToList();

            if (logger.IsDebugEnabled())
            {
                logger.Debug("Building the service map...");
            }

            var serviceMap = BuildServiceMap(
                appServiceInfoList,
                serviceTypes,
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

                var contractType = appContractDefinition.ContractType ?? contractDeclarationType;
                if (registrations.Count == 1)
                {
                    // register one service, no matter if multiple or single.
                    var registration = registrations[0];
                    ambientServices.Add(new AppServiceInfo(registration, contractType) with {})
                    this.RegisterService(builder, contractDeclarationType, contractType, registrations[0], logger);
                    continue;
                }

                // order the services by override and processing priority, resolve overrides
                // and with the rest of the services:
                // 1. if multiple are registered, register them in the computed order.
                // 2. for single mode, pick the first one in the order and register it.
                var (sortedRegistrations, overriddenTypes) = this.SortRegistrations(registrations, logger);
                if (!appContractDefinition.AllowMultiple)
                {
                    var appServiceInfo = ResolveAmbiguousRegistration(
                        contractDeclarationType,
                        sortedRegistrations,
                        resolutionStrategy,
                        logger);
                    this.RegisterService(builder, contractDeclarationType, contractType, appServiceInfo, logger);
                }
                else
                {
                    var filteredServiceInfos = overriddenTypes.Count == 0
                        ? registrations
                        : registrations.Where(i => !overriddenTypes.Contains(i.InstanceType!));
                    foreach (var appServiceInfo in filteredServiceInfos)
                    {
                        this.RegisterService(builder, contractDeclarationType, contractType, appServiceInfo, logger);
                    }
                }
            }
            // TODO add code from AppServiceInfoInjectionRegistrar.RegisterServices

            return ambientServices;
        }

        private static IDictionary<Type, ServiceEntry> BuildServiceMap(
            IEnumerable<ContractDeclaration> contractDeclarations,
            IEnumerable<ServiceDeclaration> serviceDeclarations,
            ILogger logger)
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
            ILogger logger)
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
                        var appServiceInfoDeclaration = new AppServiceInfo(appServiceInfoGenericDefinition, contractDeclarationType);
                        var appServiceInfo = new AppServiceInfo(appServiceInfoDeclaration, contractDeclarationType, serviceType);
                        ((IAppServiceInfo)appServiceInfo).AddMetadata(ServiceHelper.GetServiceMetadata(serviceType, contractDeclarationType));

                        // add to the list of service infos on the first place the declaration.
                        serviceMap[contractDeclarationType] = new ServiceEntry(contractDeclarationType, appServiceInfoDeclaration) { Registrations = { appServiceInfo } };
                        return;
                    }
                }
            }
            else
            {
                // The first app service info in the list must be the contract declaration.
                var appServiceInfoDeclaration = serviceEntry.Registrations.First();
                var appServiceInfo = new AppServiceInfo(appServiceInfoDeclaration, appServiceInfoDeclaration.ContractType ?? contractDeclarationType, serviceType);
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