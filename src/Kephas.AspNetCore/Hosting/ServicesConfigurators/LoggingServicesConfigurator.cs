// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingServicesConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore.Hosting.ServicesConfigurators
{
    using Kephas.Extensions.Logging;
    using Kephas.Logging;
    using Kephas.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Services configurator for logging.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class LoggingServicesConfigurator : IServicesConfigurator
    {
        /// <summary>
        /// Configure the services.
        /// </summary>
        /// <param name="services">The services to configure.</param>
        /// <param name="configuration">The configuration.</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.UseKephasLogging(configuration.GetServiceInstance<ILogManager>());
        }
    }
}