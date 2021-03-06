// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityServerJwtBearerOptionsConfiguration.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Authentication
{
    using System;
    using System.Threading.Tasks;

    using global::IdentityServer4.Extensions;
    using global::IdentityServer4.Stores;
    using Kephas.AspNetCore.IdentityServer4.Configuration;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;

    internal class IdentityServerJwtBearerOptionsConfiguration : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly string scheme;
        private readonly string apiName;
        private readonly IIdentityServerJwtDescriptor localApiDescriptor;

        public IdentityServerJwtBearerOptionsConfiguration(
            string scheme,
            string apiName,
            IIdentityServerJwtDescriptor localApiDescriptor)
        {
            this.scheme = scheme;
            this.apiName = apiName;
            this.localApiDescriptor = localApiDescriptor;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            var definitions = this.localApiDescriptor.GetResourceSettings();
            if (!definitions.ContainsKey(this.apiName))
            {
                return;
            }

            if (string.Equals(name, this.scheme, StringComparison.Ordinal))
            {
                options.Events ??= new JwtBearerEvents();
                options.Events.OnMessageReceived = ResolveAuthorityAndKeysAsync;
                options.Audience = this.apiName;

                var staticConfiguration = new OpenIdConnectConfiguration
                {
                    Issuer = options.Authority,
                };

                var manager = new StaticConfigurationManager(staticConfiguration);
                options.ConfigurationManager = manager;
                options.TokenValidationParameters.ValidIssuer = options.Authority;
                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.RoleClaimType = "role";
            }
        }

        internal static async Task ResolveAuthorityAndKeysAsync(MessageReceivedContext messageReceivedContext)
        {
            var options = messageReceivedContext.Options;
            if (options.TokenValidationParameters.ValidIssuer == null || options.TokenValidationParameters.IssuerSigningKey == null)
            {
                var store = messageReceivedContext.HttpContext.RequestServices.GetRequiredService<ISigningCredentialStore>();
                var credential = await store.GetSigningCredentialsAsync();
                options.Authority ??= messageReceivedContext.HttpContext.GetIdentityServerIssuerUri();
                options.TokenValidationParameters.IssuerSigningKey = credential.Key;
                options.TokenValidationParameters.ValidIssuer = options.Authority;
            }
        }

        public void Configure(JwtBearerOptions options)
        {
        }
    }
}
