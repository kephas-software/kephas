// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthUIServicesConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.InteractiveTests.ServicesConfigurators
{
    using Kephas;
    using Kephas.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The authentication UI services configurator.
    /// </summary>
    /// <remarks>
    /// Make sure that the UI is registered before the authentication services themselves.
    /// </remarks>
    [ProcessingPriority(Priority.High - 100)]
    public class AuthUIServicesConfigurator : IServicesConfigurator
    {
        /// <summary>
        /// Configure the services.
        /// </summary>
        /// <param name="services">The services to configure.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="ambientServices">The ambient services.</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, IAmbientServices ambientServices)
        {
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true);
        }
    }
}
