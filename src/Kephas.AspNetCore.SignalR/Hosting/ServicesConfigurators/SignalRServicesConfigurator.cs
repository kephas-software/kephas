// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignalRServicesConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.SignalR.Hosting.ServicesConfigurators
{
    using Kephas;
    using Kephas.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Services configurator for SignalR.
    /// </summary>
    [ProcessingPriority(ProcessingPriority)]
    public class SignalRServicesConfigurator : IServicesConfigurator
    {
        /// <summary>
        /// The processing priority of <see cref="SignalRServicesConfigurator"/>.
        /// </summary>
        public const Priority ProcessingPriority = Priority.High + 300;

        /// <summary>
        /// Configure the services.
        /// </summary>
        /// <param name="services">The services to configure.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="appServices">The application services.</param>
        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration, IAppServiceCollection appServices)
        {
            services.AddSignalR();
        }
    }
}
