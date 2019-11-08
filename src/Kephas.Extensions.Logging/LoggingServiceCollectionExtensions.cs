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
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Logging extensions.
    /// </summary>
    public static class LoggingServiceCollectionExtensions
    {
        /// <summary>
        /// Uses the Kephas logging.
        /// </summary>
        /// <param name="ambientServices">The ambientServices to act on.</param>
        /// <returns>
        /// The provided service collection.
        /// </returns>
        public static IAmbientServices ConfigureLoggingExtensions(this IAmbientServices ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var serviceCollection = ambientServices.GetRequiredService<IServiceCollection>();

            serviceCollection.Replace(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>());

            return ambientServices;
        }
    }
}