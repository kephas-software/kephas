// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the ambient services extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Diagnostics.Contracts;

    using Kephas.Logging;

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
            Contract.Requires(ambientServices != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(loggerName));

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
            Contract.Requires(ambientServices != null);
            Contract.Requires(type != null);

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
        public static ILogger<T> GetLogger<T>(this IAmbientServices ambientServices)
        {
            Contract.Requires(ambientServices != null);

            return new TypedLogger<T>(ambientServices.LogManager);
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
            Contract.Requires(ambientServices != null);
            Contract.Requires(service != null);
            Contract.Ensures(Contract.Result<IAmbientServices>() != null);

            return ambientServices.RegisterService(typeof(TService), service);
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
            Contract.Requires(ambientServices != null);

            return (TService)ambientServices.GetService(typeof(TService));
        }
    }
}