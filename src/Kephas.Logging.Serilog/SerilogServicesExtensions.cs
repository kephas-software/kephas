// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerilogServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using global::Serilog;

    using Kephas.Logging;
    using Kephas.Logging.Serilog;
    using Kephas.Services.Builder;
    using Serilog.Events;

    /// <summary>
    /// Serilog related extension methods for the <see cref="IAppServiceCollectionBuilder"/>.
    /// </summary>
    public static class SerilogServicesExtensions
    {
        /// <summary>
        /// Sets the Serilog log manager to The application services.
        /// </summary>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <param name="configuration">Optional. The logger configuration.</param>
        /// <param name="minimumLevel">Optional. The minimum logging level.</param>
        /// <param name="dynamicMinimumLevel">
        /// Optional. Indicates whether to allow changing the minimum level at runtime
        /// (true) or leave it controlled by the configuration (false). If not provided (null),
        /// a dynamic minimum level will be used only if no configuration is provided.
        /// </param>
        /// <param name="replaceDefault">Optional. True to replace <see cref="LoggingHelper.DefaultLogManager"/>.</param>
        /// <returns>
        /// This <paramref name="servicesBuilder"/>.
        /// </returns>
        public static IAppServiceCollectionBuilder WithSerilogManager(this IAppServiceCollectionBuilder servicesBuilder, LoggerConfiguration? configuration = null, LogEventLevel? minimumLevel = null, bool? dynamicMinimumLevel = null, bool replaceDefault = true)
        {
            servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));

            return servicesBuilder.WithLogManager(new SerilogManager(configuration, minimumLevel, dynamicMinimumLevel), replaceDefault);
        }
    }
}