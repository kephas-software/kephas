// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthServicesConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Hosting.ServicesConfigurators
{
    using Kephas.AspNetCore.IdentityServer4.Authentication;
    using Kephas.Configuration;
    using Kephas.Cryptography;
    using Kephas.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Configurator for authentication services.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class AuthServicesConfigurator : IServicesConfigurator
    {
        /// <summary>
        /// Configure the services.
        /// </summary>
        /// <param name="services">The services to configure.</param>
        /// <param name="ambientServices">The ambient services.</param>
        public virtual void ConfigureServices(IServiceCollection services, IAmbientServices ambientServices)
            => this.ConfigureServices<IdentityUser, IdentityRole>(services, ambientServices);

        /// <summary>
        /// Configure the services.
        /// </summary>
        /// <typeparam name="TUser">The user type.</typeparam>
        /// <typeparam name="TRole">The role type.</typeparam>
        /// <param name="services">The services to configure.</param>
        /// <param name="ambientServices">The ambient services.</param>
        protected virtual void ConfigureServices<TUser, TRole>(IServiceCollection services, IAmbientServices ambientServices)
            where TUser : class
            where TRole : class
        {
            services.AddSingleton<IPasswordHasher<TUser>>(
                serviceProvider => new PasswordDoubleHasher<TUser>(
                    serviceProvider,
                    serviceProvider.GetService<IConfiguration<CryptographySettings>>()));

            services.AddIdentityServer()
                .AddApiAuthorization<TUser, TRole>();
            services.AddAuthentication()
                .AddIdentityServerJwt();
        }
    }
}