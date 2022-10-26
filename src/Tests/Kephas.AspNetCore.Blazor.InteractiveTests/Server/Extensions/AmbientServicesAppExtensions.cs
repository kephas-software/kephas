// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesAppExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Server.Extensions
{
    using System;

    using Kephas.Cryptography;
    using Kephas.Logging.Serilog;
    using Kephas.Services.Builder;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using Serilog.Events;

    public static class AmbientServicesAppExtensions
    {
        public static IAppServiceCollectionBuilder SetupAmbientServices(
            this IAppServiceCollectionBuilder servicesBuilder,
            Func<IAppServiceCollectionBuilder, IEncryptionService> encryptionServiceFactory,
            IConfiguration? configuration)
        {
            return servicesBuilder
                .WithDefaultLicensingManager(encryptionServiceFactory(servicesBuilder))
                .WithDynamicAppRuntime()
                .WithSerilogManager(configuration);
        }

        /// <summary>
        /// Configures the Serilog logging infrastructure from the configuration.
        /// </summary>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The provided ambient services.
        /// </returns>
        internal static IAppServiceCollectionBuilder WithSerilogManager(this IAppServiceCollectionBuilder servicesBuilder, IConfiguration? configuration)
        {
            var loggerConfig = new LoggerConfiguration();
            loggerConfig
                .ReadFrom.Configuration(configuration)
                .Enrich.With(new AppLogEventEnricher(servicesBuilder.AmbientServices.GetAppRuntime()!));

            var minimumLevel = configuration.GetValue<LogEventLevel?>("Serilog:MinimumLevel") ?? LogEventLevel.Information;

            return servicesBuilder.WithSerilogManager(loggerConfig, minimumLevel, dynamicMinimumLevel: true);
        }
    }
}