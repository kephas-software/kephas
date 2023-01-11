// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionAppExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.InteractiveTests6.Extensions
{
    using Kephas.Logging.Serilog;
    using Kephas.Services.Builder;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using Serilog.Events;

    public static class AppServiceCollectionAppExtensions
    {
        /// <summary>
        /// Configures the Serilog logging infrastructure from the configuration.
        /// </summary>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The <paramref name="servicesBuilder"/>.
        /// </returns>
        internal static IAppServiceCollectionBuilder WithSerilogManager(this IAppServiceCollectionBuilder servicesBuilder, IConfiguration? configuration)
        {
            var loggerConfig = new LoggerConfiguration();
            loggerConfig
                .ReadFrom.Configuration(configuration)
                .Enrich.With(new AppLogEventEnricher(servicesBuilder.AppServices.GetAppRuntime()!));

            var minimumLevel = configuration.GetValue<LogEventLevel?>("Serilog:MinimumLevel") ?? LogEventLevel.Information;

            return servicesBuilder.WithSerilogManager(loggerConfig, minimumLevel, dynamicMinimumLevel: true);
        }
    }
}