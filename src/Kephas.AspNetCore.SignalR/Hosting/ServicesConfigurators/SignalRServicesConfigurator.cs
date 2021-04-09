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
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Services configurator for SignalR.
    /// </summary>
    [ProcessingPriority(Priority.High + 300)]
    public class SignalRServicesConfigurator : IServicesConfigurator
    {
        /// <summary>
        /// Configure the services.
        /// </summary>
        /// <param name="services">The services to configure.</param>
        /// <param name="ambientServices">The ambient services.</param>
        public virtual void ConfigureServices(IServiceCollection services, IAmbientServices ambientServices)
        {
            services.AddSignalR();
        }
    }
}
