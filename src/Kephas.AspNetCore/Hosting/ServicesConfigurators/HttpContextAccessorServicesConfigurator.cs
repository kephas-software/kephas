// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpContextAccessorServicesConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore.Hosting.ServicesConfigurators
{
    using System;

    using Kephas.Logging;
    using Kephas.Resources;
    using Kephas.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// Services configurator
    /// </summary>
    [ProcessingPriority(Priority.Lowest)]
    public class HttpContextAccessorServicesConfigurator : Loggable, IServicesConfigurator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContextAccessorServicesConfigurator"/> class.
        /// </summary>
        /// <param name="logManager">Optional. The log manager.</param>
        public HttpContextAccessorServicesConfigurator(ILogManager? logManager = null)
            : base(logManager)
        {
        }

        /// <summary>
        /// Configure the services.
        /// </summary>
        /// <param name="services">The services to configure.</param>
        /// <param name="ambientServices">The ambient services.</param>
        public void ConfigureServices(IServiceCollection services, IAmbientServices ambientServices)
        {
            try
            {
                services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(ex, Strings.App_BootstrapAsync_ErrorDuringConfiguration_Exception);
                throw;
            }
        }
    }
}