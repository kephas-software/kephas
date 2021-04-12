// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesAppExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.InteractiveTests.Extensions
{
    using System;

    using Kephas.Application;
    using Kephas.Cryptography;
    using Kephas.Logging.Serilog;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using Serilog.Events;

    public static class AmbientServicesAppExtensions
    {
        public static void SetupAmbientServices(
            this IAmbientServices ambientServices,
            Func<IAmbientServices, IEncryptionService> encryptionServiceFactory,
            IConfiguration? configuration)
        {
            ambientServices
                .WithDefaultLicensingManager(encryptionServiceFactory(ambientServices))
                .WithDynamicAppRuntime()
                .WithSerilogManager(configuration);
        }

        /// <summary>
        /// Configures the Serilog logging infrastructure from the configuration.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The provided ambient services.
        /// </returns>
        internal static IAmbientServices WithSerilogManager(this IAmbientServices ambientServices, IConfiguration? configuration)
        {
            var loggerConfig = new LoggerConfiguration();
            loggerConfig
                .ReadFrom.Configuration(configuration)
                .Enrich.With(new AppLogEventEnricher(ambientServices.AppRuntime));

            var minimumLevel = configuration.GetValue<LogEventLevel?>("Serilog:MinimumLevel") ?? LogEventLevel.Information;

            return ambientServices.WithSerilogManager(loggerConfig, minimumLevel, dynamicMinimumLevel: true);
        }
    }
}