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
        /// <param name="service">The service.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        public static IAmbientServices RegisterService<TService>(this IAmbientServices ambientServices, TService service)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(service, nameof(service));

            return ambientServices.RegisterService(typeof(TService), service);
        }

        /// <summary>
        /// Registers the provided service.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TServiceImplementation">Type of the service implementation.</typeparam>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <param name="isSingleton">Indicates whether the function should be evaluated only once, or each
        ///                           time it is called.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        public static IAmbientServices RegisterService<TService, TServiceImplementation>(this IAmbientServices ambientServices, bool isSingleton = false)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices.RegisterService(typeof(TService), typeof(TServiceImplementation), isSingleton);
        }

        /// <summary>
        /// Registers the provided service.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="isSingleton">Optional. Indicates whether the function should be evaluated only
        ///                           once, or each time it is called.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        public static IAmbientServices RegisterService<TService>(
            this IAmbientServices ambientServices,
            Func<TService> serviceFactory,
            bool isSingleton = false)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.RegisterService(typeof(TService), ctx => serviceFactory(), isSingleton);
        }

        /// <summary>
        /// Registers the provided service.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="isSingleton">Optional. Indicates whether the function should be evaluated only
        ///                           once, or each time it is called.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        public static IAmbientServices RegisterService<TService>(
            this IAmbientServices ambientServices,
            Func<ICompositionContext, TService> serviceFactory,
            bool isSingleton = false)
            where TService : class
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.RegisterService(typeof(TService), serviceFactory, isSingleton);
        }

        /// <summary>
        /// Registers the provided service factory.
        /// </summary>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="isSingleton">Optional. Indicates whether the function should be evaluated only
        ///                           once, or each time it is called.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        public static IAmbientServices RegisterService(
            this IAmbientServices ambientServices,
            Type serviceType,
            Func<object> serviceFactory,
            bool isSingleton = false)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            return ambientServices.RegisterService(serviceType, ctx => serviceFactory(), isSingleton);
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