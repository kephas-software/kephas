// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the logging service collection extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Logging
{
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Logging extensions.
    /// </summary>
    public static class LoggingServiceCollectionExtensions
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
        /// WARNING: Do not use both <see cref="WithExtensionsLogManager"/> and <see cref="ConfigureExtensionsLogging"/> methods
        /// as this might generate StackOverflow exception.
        /// </para>
        /// </remarks>
        /// <param name="ambientServices">The ambientServices to act on.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices ConfigureExtensionsLogging(this IAmbientServices ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var serviceCollection = ambientServices.GetRequiredService<IServiceCollection>();

            serviceCollection.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, LoggerProvider>(_ => new LoggerProvider(ambientServices.LogManager)));

            return ambientServices;
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
        /// WARNING: Do not use both <see cref="WithExtensionsLogManager"/> and <see cref="ConfigureExtensionsLogging"/> methods
        /// as this might generate StackOverflow exception.
        /// </para>
        /// </remarks>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="replaceDefault">Optional. True to replace <see cref="LoggingHelper.DefaultLogManager"/>.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithExtensionsLogManager(this IAmbientServices ambientServices, bool replaceDefault = true)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var serviceCollection = ambientServices.GetRequiredService<IServiceCollection>();
            var logFactory = new LoggerFactory();
            serviceCollection.Replace(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>(_ => logFactory));

            return ambientServices.WithLogManager(new ExtensionsLogManager(logFactory), replaceDefault);
        }
    }
}