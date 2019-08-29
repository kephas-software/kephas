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

    using Kephas.Composition;
    using Kephas.Composition.Lightweight;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Resources;

    /// <summary>
    /// Extension methods for <see cref="IAmbientServices"/>.
    /// </summary>
    public static class AmbientServicesExtensions
    {
        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="ambientServices">The ambient services to act on.</param>
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
        /// <param name="ambientServices">The ambient services to act on.</param>
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
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        public static ILogger GetLogger<T>(this IAmbientServices ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices.LogManager.GetLogger(typeof(T));
        }

        /// <summary>
        /// Registers the provided service.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <param name="builder">The registration builder.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        public static IAmbientServices Register<TService>(this IAmbientServices ambientServices, Action<IServiceRegistrationBuilder> builder)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(builder, nameof(builder));

            return ambientServices.Register(typeof(TService), builder);
        }

        /// <summary>
        /// Registers the provided service instance.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <param name="service">The service.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        public static IAmbientServices Register<TService>(this IAmbientServices ambientServices, TService service)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(service, nameof(service));

            return ambientServices.Register(typeof(TService), b => b.WithInstance(service));
        }

        /// <summary>
        /// Registers the provided service with implementation type as singleton.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TServiceImplementation">Type of the service implementation.</typeparam>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <returns>
        /// The IAmbientServices.
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
        /// Registers the provided service with implementation type as singleton.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TServiceImplementation">Type of the service implementation.</typeparam>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <returns>
        /// The IAmbientServices.
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
        /// Registers the provided service as singleton factory.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// The IAmbientServices.
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
        /// Registers the provided service as transient factory.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// The IAmbientServices.
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
        /// Registers the provided service as singleton factory.
        /// </summary>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// The IAmbientServices.
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
        /// Registers the provided service as transient factory.
        /// </summary>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// The IAmbientServices.
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
        /// Registers the provided service.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="service">The service.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        public static IAmbientServices Register(
            this IAmbientServices ambientServices,
            Type serviceType,
            object service)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(service, nameof(service));

            ambientServices.Register(serviceType, b => b.WithInstance(service));
            return ambientServices;
        }

        /// <summary>
        /// Registers the provided service as singleton.
        /// </summary>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceImplementationType">The service implementation type.</param>
        /// <returns>
        /// The IAmbientServices.
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
        /// Registers the provided service as transient.
        /// </summary>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceImplementationType">The service implementation type.</param>
        /// <returns>
        /// The IAmbientServices.
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
        /// Gets the service with the provided type.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services to act on.</param>
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
        /// <param name="ambientServices">The ambient services to act on.</param>
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
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <returns>
        /// A service object of type <typeparamref name="TService"/>.-or- <c>null</c> if there is no
        /// service object of type <typeparamref name="TService"/>.
        /// </returns>
        public static TService GetRequiredService<TService>(this IServiceProvider ambientServices)
            where TService : class
        {
            return (TService)GetRequiredService(ambientServices, typeof(TService));
        }
    }
}