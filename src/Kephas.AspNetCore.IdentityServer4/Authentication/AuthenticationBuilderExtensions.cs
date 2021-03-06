// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationBuilderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Authentication
{
    using System;

    using Kephas.AspNetCore.IdentityServer4.Configuration;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Extension methods to configure authentication for existing APIs coexisting with an Authorization Server.
    /// </summary>
    public static class AuthenticationBuilderExtensions
    {
        private const string IdentityServerJwtNameSuffix = "API";

        private static readonly PathString DefaultIdentityUIPathPrefix =
            new PathString("/Identity");

        /// <summary>
        /// Adds an authentication handler for an API that coexists with an Authorization Server.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <returns>The provided <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddIdentityServerJwt(this AuthenticationBuilder builder)
        {
            var services = builder.Services;
            services.TryAddSingleton<IIdentityServerJwtDescriptor, IdentityServerJwtDescriptor>();
            services.TryAddEnumerable(ServiceDescriptor
                .Transient<IConfigureOptions<JwtBearerOptions>, IdentityServerJwtBearerOptionsConfiguration>(JwtBearerOptionsFactory));

            services.AddAuthentication(IdentityServerJwtConstants.IdentityServerJwtScheme)
                .AddPolicyScheme(IdentityServerJwtConstants.IdentityServerJwtScheme, null, options =>
                {
                    options.ForwardDefaultSelector = new IdentityServerJwtPolicySchemeForwardSelector(
                        DefaultIdentityUIPathPrefix,
                        IdentityServerJwtConstants.IdentityServerJwtBearerScheme).SelectScheme;
                })
                .AddJwtBearer(IdentityServerJwtConstants.IdentityServerJwtBearerScheme, null, o => { });

            return builder;

            IdentityServerJwtBearerOptionsConfiguration JwtBearerOptionsFactory(IServiceProvider sp)
            {
                var schemeName = IdentityServerJwtConstants.IdentityServerJwtBearerScheme;

                var localApiDescriptor = sp.GetRequiredService<IIdentityServerJwtDescriptor>();
                var hostingEnvironment = sp.GetRequiredService<IWebHostEnvironment>();
                var apiName = hostingEnvironment.ApplicationName + IdentityServerJwtNameSuffix;

                return new IdentityServerJwtBearerOptionsConfiguration(schemeName, apiName, localApiDescriptor);
            }
        }
    }
}
