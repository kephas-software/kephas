// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpContextAccessorServicesConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.InteractiveTests.ServicesConfigurators
{
    using Kephas.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// Services configurator
    /// </summary>
    [ProcessingPriority(ProcessingPriority)]
    public class HttpContextAccessorServicesConfigurator : IServicesConfigurator
    {
        /// <summary>
        /// The processing priority of <see cref="HttpContextAccessorServicesConfigurator"/>.
        /// </summary>
        public const Priority ProcessingPriority = Priority.Lowest;

        /// <summary>
        /// Configure the services.
        /// </summary>
        /// <param name="services">The services to configure.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="ambientServices">The ambient services.</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, IAmbientServices ambientServices)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}