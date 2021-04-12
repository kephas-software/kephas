// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthServicesConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Hosting.ServicesConfigurators
{
    using System;

    using global::IdentityServer4.Configuration;
    using Kephas.AspNetCore.IdentityServer4.Authentication;
    using Kephas.Configuration;
    using Kephas.Cryptography;
    using Kephas.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Configurator for authentication services.
    /// </summary>
    [ProcessingPriority(ProcessingPriority)]
    public class AuthServicesConfigurator : IServicesConfigurator
    {
        /// <summary>
        /// The processing priority of <see cref="AuthServicesConfigurator"/>.
        /// </summary>
        public const Priority ProcessingPriority = Priority.High;

        /// <summary>
        /// Configure the services.
        /// </summary>
        /// <param name="services">The services to configure.</param>
        /// <param name="ambientServices">The ambient services.</param>
        public virtual void ConfigureServices(IServiceCollection services, IAmbientServices ambientServices)
            => this.ConfigureServices<IdentityUser, IdentityRole>(services, ambientServices, null);

        /// <summary>
        /// Configure the services.
        /// </summary>
        /// <typeparam name="TUser">The user type.</typeparam>
        /// <typeparam name="TRole">The role type.</typeparam>
        /// <param name="services">The services to configure.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="setupAction">The setup action.</param>
        protected virtual void ConfigureServices<TUser, TRole>(IServiceCollection services, IAmbientServices ambientServices, Action<IdentityServerOptions>? setupAction)
            where TUser : class
            where TRole : class
        {
            services.AddSingleton<IPasswordHasher<TUser>>(
                serviceProvider => new PasswordDoubleHasher<TUser>(
                    serviceProvider.GetRequiredService<IHashingService>(),
                    serviceProvider.GetService<IConfiguration<CryptographySettings>>()));

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            this.ConfigureIdentityServer<TUser, TRole>(services.AddIdentityServer());
            this.ConfigureAuthentication(services.AddAuthentication());
        }

        /// <summary>
        /// Configures the identity server.
        /// </summary>
        /// <param name="builder">The identity server builder.</param>
        /// <typeparam name="TUser">The user type.</typeparam>
        /// <typeparam name="TRole">The role type.</typeparam>
        /// <returns>The provided identity server builder.</returns>
        protected virtual IIdentityServerBuilder ConfigureIdentityServer<TUser, TRole>(IIdentityServerBuilder builder)
            where TUser : class
            where TRole : class
        {
            return builder.AddApiAuthorization<TUser, TRole>();
        }

        /// <summary>
        /// Configures the identity server.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <returns>The provided authentication builder.</returns>
        protected virtual AuthenticationBuilder ConfigureAuthentication(AuthenticationBuilder builder)
        {
            return builder.AddIdentityServerJwt();
        }
    }
}