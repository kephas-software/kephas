// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectorExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Diagnostics.Contracts;

    using Kephas.Logging;

    /// <summary>
    /// Extension methods for <see cref="IServiceProvider"/>.
    /// </summary>
    public static class InjectorExtensions
    {
        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="serviceProvider">The injector to act on.</param>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        /// A logger for the provided name.
        /// </returns>
        [Pure]
        public static ILogger GetLogger(this IServiceProvider serviceProvider, string loggerName)
        {
            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            loggerName = loggerName ?? throw new ArgumentNullException(nameof(loggerName));

            return serviceProvider.Resolve<ILogManager>().GetLogger(loggerName);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <param name="serviceProvider">The injector to act on.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        [Pure]
        public static ILogger GetLogger(this IServiceProvider serviceProvider, Type type)
        {
            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            type = type ?? throw new ArgumentNullException(nameof(type));

            return serviceProvider.Resolve<ILogManager>().GetLogger(type);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <typeparam name="T">The type for which a logger should be created.</typeparam>
        /// <param name="serviceProvider">The injector to act on.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        [Pure]
        public static ILogger GetLogger<T>(this IServiceProvider serviceProvider)
        {
            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            return serviceProvider.Resolve<ILogManager>().GetLogger(typeof(T));
        }
    }
}