// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsLogging.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the logging service collection extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Logging
{
    using System;

    using Kephas.Logging;
    using Kephas.Services.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Logging extensions.
    /// </summary>
    public static class ExtensionsLogging
    {
        /// <summary>
        /// Adds the Kephas logging provider to the service collection.
        /// </summary>
        /// <remarks>
        /// Use this method if you plan to configure the logging in Kephas and add the Kephas logger provider
        /// to the service collection.
        /// <para>
        /// Advantage: The loggers are available also before the container is built.
        /// </para>
        /// <para>
        /// Disadvantage: Kephas will log only to its own configured logger.
        /// </para>
        /// <para>
        /// WARNING: Do not use both <see cref="WithExtensionsLogManager"/> and <see cref="UseKephasLogging"/> methods
        /// as this might generate StackOverflow exception.
        /// </para>
        /// </remarks>
        /// <param name="services">The service collection.</param>
        /// <param name="logManager">The log manager.</param>
        /// <returns>
        /// This <paramref name="services"/>.
        /// </returns>
        public static IServiceCollection UseKephasLogging(this IServiceCollection services, ILogManager logManager)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            logManager = logManager ?? throw new ArgumentNullException(nameof(logManager));

            services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, LoggerProvider>(_ => new LoggerProvider(logManager)));

            return services;
        }

        /// <summary>
        /// Sets the Extensions log manager to the ambient services.
        /// </summary>
        /// <remarks>
        /// Use this method if you plan to configure the logging outside of Kephas.
        /// <para>
        /// Advantage: Kephas will log to all configured loggers.
        /// </para>
        /// <para>
        /// Disadvantage: the configured loggers will be available only after the container is built.
        /// </para>
        /// <para>
        /// WARNING: Do not use both <see cref="WithExtensionsLogManager"/> and <see cref="UseKephasLogging"/> methods
        /// as this might generate StackOverflow exception.
        /// </para>
        /// </remarks>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <param name="services">The service collection.</param>
        /// <param name="replaceDefault">Optional. True to replace <see cref="LoggingHelper.DefaultLogManager"/>.</param>
        /// <returns>
        /// This <paramref name="servicesBuilder"/>.
        /// </returns>
        public static IAppServiceCollectionBuilder WithExtensionsLogManager(this IAppServiceCollectionBuilder servicesBuilder, IServiceCollection services, bool replaceDefault = true)
        {
            servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));

            var logFactory = new LoggerFactory();
            services.Replace(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>(_ => logFactory));

            return servicesBuilder.WithLogManager(new ExtensionsLogManager(logFactory), replaceDefault);
        }
    }
}