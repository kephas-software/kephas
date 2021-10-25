// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectorExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection
{
    using System;
    using System.Diagnostics.Contracts;

    using Kephas.Injection.Internal;
    using Kephas.Logging;

    /// <summary>
    /// Extension methods for <see cref="IInjector"/>.
    /// </summary>
    public static class InjectorExtensions
    {
        internal const string LiteInjectionKey = "__LiteInjection";

        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="injector">The injector to act on.</param>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>
        /// A logger for the provided name.
        /// </returns>
        [Pure]
        public static ILogger GetLogger(this IInjector injector, string loggerName)
        {
            injector = injector ?? throw new ArgumentNullException(nameof(injector));
            loggerName = loggerName ?? throw new ArgumentNullException(nameof(loggerName));

            return injector.Resolve<ILogManager>().GetLogger(loggerName);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <param name="injector">The injector to act on.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        [Pure]
        public static ILogger GetLogger(this IInjector injector, Type type)
        {
            injector = injector ?? throw new ArgumentNullException(nameof(injector));
            type = type ?? throw new ArgumentNullException(nameof(type));

            return injector.Resolve<ILogManager>().GetLogger(type);
        }

        /// <summary>
        /// Gets the logger for the provided type.
        /// </summary>
        /// <typeparam name="T">The type for which a logger should be created.</typeparam>
        /// <param name="injector">The injector to act on.</param>
        /// <returns>
        /// A logger for the provided type.
        /// </returns>
        [Pure]
        public static ILogger GetLogger<T>(this IInjector injector)
        {
            injector = injector ?? throw new ArgumentNullException(nameof(injector));

            return injector.Resolve<ILogManager>().GetLogger(typeof(T));
        }

        /// <summary>
        /// Converts a <see cref="IInjector"/> to a <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="injector">The injector to act on.</param>
        /// <returns>
        /// The <see cref="IInjector"/> as an <see cref="IServiceProvider"/>.
        /// </returns>
        public static IServiceProvider ToServiceProvider(this IInjector injector)
        {
            injector = injector ?? throw new ArgumentNullException(nameof(injector));

            return injector as IServiceProvider
                   ?? new ServiceProviderAdapter(injector);
        }

        /// <summary>
        /// Converts a <see cref="IServiceProvider"/> to a <see cref="IInjector"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider to act on.</param>
        /// <returns>
        /// The service provider as an <see cref="IInjector"/>.
        /// </returns>
        public static IInjector ToInjector(this IServiceProvider serviceProvider)
        {
            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            return serviceProvider as IInjector
                   ?? new InjectorAdapter(serviceProvider);
        }
    }
}