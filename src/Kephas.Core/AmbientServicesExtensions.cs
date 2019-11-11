// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ambient services extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.Hosting;
    using Kephas.Composition.Lite;
    using Kephas.Composition.Lite.Hosting;
    using Kephas.Configuration;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Services;

    /// <summary>
    /// Extension methods for <see cref="IAmbientServices"/>.
    /// </summary>
    public static class AmbientServicesExtensions
    {
        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        /// A logger for the provided name.
        /// </returns>
        public static ILogger GetLogger(this IAmbientServices ambientServices, string loggerName)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNullOrEmpty(loggerName, nameof(loggerName));

            return ambientServices.LogManager.GetLogger(loggerName);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        public static ILogger GetLogger(this IAmbientServices ambientServices, Type type)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(type, nameof(type));

            return ambientServices.LogManager.GetLogger(type);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <typeparam name="T">The type for which a logger should be created.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        public static ILogger GetLogger<T>(this IAmbientServices ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices.LogManager.GetLogger(typeof(T));
        }

        /// <summary>
        /// Configures the settings.
        /// </summary>
        /// <typeparam name="TSettings">Type of the settings.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="optionsConfig">The options configuration.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Configure<TSettings>(this IAmbientServices ambientServices, Action<TSettings> optionsConfig)
            where TSettings : class, new()
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            ambientServices.ConfigurationStore.Configure(optionsConfig);
            return ambientServices;
        }

        /// <summary>
        /// Registers the provided service.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="builder">The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register<TService>(this IAmbientServices ambientServices, Action<IServiceRegistrationBuilder> builder)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(builder, nameof(builder));

            return ambientServices.Register(typeof(TService), builder);
        }

        /// <summary>
        /// Registers the provided service, allowing also multiple registrations.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="builder">The registration builder.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterMultiple<TService>(this IAmbientServices ambientServices, Action<IServiceRegistrationBuilder> builder)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(builder, nameof(builder));

            return ambientServices.Register(
                typeof(TService),
                b =>
                    {
                        builder(b);
                        b.AllowMultiple();
                    });
        }

        /// <summary>
        /// Registers the provided service instance.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="service">The service.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register<TService>(this IAmbientServices ambientServices, TService service)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(service, nameof(service));

            return ambientServices.Register(typeof(TService), b => b.WithInstance(service));
        }

        /// <summary>
        /// Registers the provided service instance, allowing also multiple registrations.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="service">The service.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterMultiple<TService>(this IAmbientServices ambientServices, TService service)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(service, nameof(service));

            return ambientServices.Register(typeof(TService), b => b.WithInstance(service).AllowMultiple());
        }

        /// <summary>
        /// Registers the provided service with implementation type as singleton.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TServiceImplementation">Type of the service implementation.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register<TService, TServiceImplementation>(this IAmbientServices ambientServices)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices.Register(
                typeof(TService),
                b => b.WithType(typeof(TServiceImplementation))
                            .AsSingleton());
        }

        /// <summary>
        /// Registers the provided service with implementation type as singleton, allowing also multiple registrations.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TServiceImplementation">Type of the service implementation.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterMultiple<TService, TServiceImplementation>(this IAmbientServices ambientServices)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices.Register(
                typeof(TService),
                b => b.WithType(typeof(TServiceImplementation))
                      .AsSingleton()
                      .AllowMultiple());
        }

        /// <summary>
        /// Registers the provided service with implementation type as transient.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TServiceImplementation">Type of the service implementation.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterTransient<TService, TServiceImplementation>(this IAmbientServices ambientServices)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices.Register(
                typeof(TService),
                b => b.WithType(typeof(TServiceImplementation))
                            .AsTransient());
        }

        /// <summary>
        /// Registers the provided service with implementation type as transient, allowing also multiple registrations.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TServiceImplementation">Type of the service implementation.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterTransientMultiple<TService, TServiceImplementation>(this IAmbientServices ambientServices)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices.Register(
                typeof(TService),
                b => b.WithType(typeof(TServiceImplementation))
                      .AsTransient()
                      .AllowMultiple());
        }

        /// <summary>
        /// Registers the provided service as singleton factory.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register<TService>(
            this IAmbientServices ambientServices,
            Func<TService> serviceFactory)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.Register(
                typeof(TService),
                b => b.WithFactory(ctx => serviceFactory())
                      .AsSingleton());
        }

        /// <summary>
        /// Registers the provided service as singleton factory, allowing also multiple registrations.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterMultiple<TService>(
            this IAmbientServices ambientServices,
            Func<TService> serviceFactory)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.Register(
                typeof(TService),
                b => b.WithFactory(ctx => serviceFactory())
                      .AsSingleton()
                      .AllowMultiple());
        }

        /// <summary>
        /// Registers the provided service as transient factory.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterTransient<TService>(
            this IAmbientServices ambientServices,
            Func<TService> serviceFactory)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.Register(
                typeof(TService),
                b => b.WithFactory(ctx => serviceFactory())
                      .AsTransient());
        }

        /// <summary>
        /// Registers the provided service as transient factory, allowing also multiple registrations.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterTransientMultiple<TService>(
            this IAmbientServices ambientServices,
            Func<TService> serviceFactory)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.Register(
                typeof(TService),
                b => b.WithFactory(ctx => serviceFactory())
                      .AsTransient()
                      .AllowMultiple());
        }

        /// <summary>
        /// Registers the provided service as singleton factory.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register(
            this IAmbientServices ambientServices,
            Type serviceType,
            Func<object> serviceFactory)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.Register(
                serviceType,
                b => b.WithFactory(ctx => serviceFactory())
                      .AsSingleton());
        }

        /// <summary>
        /// Registers the provided service as singleton factory, allowing also multiple registrations.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterMultiple(
            this IAmbientServices ambientServices,
            Type serviceType,
            Func<object> serviceFactory)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.Register(
                serviceType,
                b => b.WithFactory(ctx => serviceFactory())
                      .AsSingleton()
                      .AllowMultiple());
        }

        /// <summary>
        /// Registers the provided service as transient factory.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterTransient(
            this IAmbientServices ambientServices,
            Type serviceType,
            Func<object> serviceFactory)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.Register(
                serviceType,
                b => b.WithFactory(ctx => serviceFactory())
                      .AsTransient());
        }

        /// <summary>
        /// Registers the provided service as transient factory, allowing also multiple registrations.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterTransientMultiple(
            this IAmbientServices ambientServices,
            Type serviceType,
            Func<object> serviceFactory)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.Register(
                serviceType,
                b => b.WithFactory(ctx => serviceFactory())
                      .AsTransient()
                      .AllowMultiple());
        }

        /// <summary>
        /// Registers the provided service.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="service">The service.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register(
            this IAmbientServices ambientServices,
            Type serviceType,
            object service)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(service, nameof(service));

            return ambientServices.Register(serviceType, b => b.WithInstance(service));
        }

        /// <summary>
        /// Registers the provided service, allowing also multiple registrations.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="service">The service.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterMultiple(
            this IAmbientServices ambientServices,
            Type serviceType,
            object service)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(service, nameof(service));

            return ambientServices.Register(serviceType, b => b.WithInstance(service).AllowMultiple());
        }

        /// <summary>
        /// Registers the provided service as singleton.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceImplementationType">The service implementation type.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices Register(
            this IAmbientServices ambientServices,
            Type serviceType,
            Type serviceImplementationType)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(serviceImplementationType, nameof(serviceImplementationType));

            ambientServices.Register(
                serviceType,
                b => b.WithType(serviceImplementationType).AsSingleton());
            return ambientServices;
        }

        /// <summary>
        /// Registers the provided service as singleton, allowing also multiple registrations.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceImplementationType">The service implementation type.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterMultiple(
            this IAmbientServices ambientServices,
            Type serviceType,
            Type serviceImplementationType)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(serviceImplementationType, nameof(serviceImplementationType));

            ambientServices.Register(
                serviceType,
                b => b.WithType(serviceImplementationType).AsSingleton().AllowMultiple());
            return ambientServices;
        }

        /// <summary>
        /// Registers the provided service as transient.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceImplementationType">The service implementation type.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterTransient(
            this IAmbientServices ambientServices,
            Type serviceType,
            Type serviceImplementationType)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(serviceImplementationType, nameof(serviceImplementationType));

            ambientServices.Register(
                serviceType,
                b => b.WithType(serviceImplementationType).AsTransient());
            return ambientServices;
        }

        /// <summary>
        /// Registers the provided service as transient, allowing also multiple registrations.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceImplementationType">The service implementation type.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices RegisterTransientMultiple(
            this IAmbientServices ambientServices,
            Type serviceType,
            Type serviceImplementationType)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(serviceImplementationType, nameof(serviceImplementationType));

            ambientServices.Register(
                serviceType,
                b => b.WithType(serviceImplementationType).AsTransient().AllowMultiple());
            return ambientServices;
        }

        /// <summary>
        /// Gets the service with the provided type.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// A service object of type <typeparamref name="TService"/>.-or- <c>null</c> if there is no
        /// service object of type <typeparamref name="TService"/>.
        /// </returns>
        public static TService GetService<TService>(this IServiceProvider ambientServices)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return (TService)ambientServices.GetService(typeof(TService));
        }

        /// <summary>
        /// Gets the service with the provided type.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.
        /// </returns>
        public static object GetRequiredService(this IServiceProvider ambientServices, Type serviceType)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceType, nameof(serviceType));

            var service = ambientServices.GetService(serviceType);
            if (service == null)
            {
                throw new CompositionException(
                    string.Format(
                        Strings.AmbientServices_RequiredServiceNotRegistered_Exception,
                        serviceType));
            }

            return service;
        }

        /// <summary>
        /// Gets the service with the provided type.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// A service object of type <typeparamref name="TService"/>.-or- <c>null</c> if there is no
        /// service object of type <typeparamref name="TService"/>.
        /// </returns>
        public static TService GetRequiredService<TService>(this IServiceProvider ambientServices)
            where TService : class
        {
            return (TService)GetRequiredService(ambientServices, typeof(TService));
        }

        /// <summary>
        /// Sets the configuration store to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="configurationStore">The configuration store.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithConfigurationStore(this IAmbientServices ambientServices, IConfigurationStore configurationStore)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(configurationStore, nameof(configurationStore));

            ambientServices.Register(configurationStore);

            return ambientServices;
        }

        /// <summary>
        /// Sets the log manager to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="logManager">The log manager.</param>
        /// <param name="replaceDefault">Optional. True to replace the <see cref="Loggable.DefaultLogManager"/>.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithLogManager(this IAmbientServices ambientServices, ILogManager logManager, bool replaceDefault = true)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(logManager, nameof(logManager));

            if (replaceDefault)
            {
                Loggable.DefaultLogManager = logManager;
            }

            ambientServices.Register(logManager);

            return ambientServices;
        }

        /// <summary>
        /// Sets the application runtime to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithAppRuntime(this IAmbientServices ambientServices, IAppRuntime appRuntime)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(appRuntime, nameof(appRuntime));

            ambientServices.Register(appRuntime);

            return ambientServices;
        }

        /// <summary>
        /// Sets the composition container to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="compositionContainer">The composition container.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithCompositionContainer(this IAmbientServices ambientServices, ICompositionContext compositionContainer)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(compositionContainer, nameof(compositionContainer));

            ambientServices.Register(compositionContainer);

            return ambientServices;
        }

        /// <summary>
        /// Sets the composition container to the ambient services.
        /// </summary>
        /// <typeparam name="TContainerBuilder">Type of the composition container builder.</typeparam>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// <remarks>The container builder type must provide a constructor with one parameter of type <see cref="IContext" />.</remarks>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithCompositionContainer<TContainerBuilder>(this IAmbientServices ambientServices, Action<TContainerBuilder> containerBuilderConfig = null)
            where TContainerBuilder : ICompositionContainerBuilder
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var builderType = typeof(TContainerBuilder).AsRuntimeTypeInfo();
            var context = new CompositionRegistrationContext(ambientServices);

            var containerBuilder = (TContainerBuilder)builderType.CreateInstance(new[] { context });

            containerBuilderConfig?.Invoke(containerBuilder);

            return ambientServices.WithCompositionContainer(containerBuilder.CreateContainer());
        }

        /// <summary>
        /// Sets the Lite composition container to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// This <paramref name="ambientServices"/>.
        public static IAmbientServices WithLiteCompositionContainer(this IAmbientServices ambientServices, Action<LiteCompositionContainerBuilder> containerBuilderConfig = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var containerBuilder = new LiteCompositionContainerBuilder(new CompositionRegistrationContext(ambientServices));

            containerBuilderConfig?.Invoke(containerBuilder);

            var container = containerBuilder.CreateContainer();
            return ambientServices.WithCompositionContainer(container);
        }
    }
}