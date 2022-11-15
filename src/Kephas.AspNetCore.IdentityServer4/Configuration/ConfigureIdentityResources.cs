// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureIdentityResources.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    using System;

    using global::IdentityServer4;
    using Kephas.AspNetCore.IdentityServer4.Options;
    using Kephas.Configuration;
    using Kephas.Logging;
    using Microsoft.Extensions.Options;

    internal class ConfigureIdentityResources : Loggable, IConfigureOptions<ApiAuthorizationOptions>
    {
        private readonly Lazy<IdentityResourceSettings?> lazyConfiguration;

        public ConfigureIdentityResources(Lazy<IConfiguration<IdentityServerSettings>> lazyConfiguration, ILogManager? logManager = null)
            : this(new Lazy<IdentityResourceSettings?>(() => lazyConfiguration.Value.GetSettings().Identity), logManager)
        {
        }

        internal ConfigureIdentityResources(IdentityResourceSettings? configuration, ILogManager? logManager = null)
            : this(new Lazy<IdentityResourceSettings?>(() => configuration), logManager)
        {
        }

        internal ConfigureIdentityResources(Lazy<IdentityResourceSettings?> lazyConfiguration, ILogManager? logManager = null)
            : base(logManager)
        {
            this.lazyConfiguration = lazyConfiguration;
        }

        public void Configure(ApiAuthorizationOptions options)
        {
            var data = this.lazyConfiguration.Value;
            var scopes = data?.Scopes;
            if (scopes == null)
            {
                return;
            }

            if (scopes.Length > 0)
            {
                ClearDefaultIdentityResources(options);
            }

            foreach (var scope in scopes)
            {
                switch (scope)
                {
                    case IdentityServerConstants.StandardScopes.OpenId:
                        options.IdentityResources.Add(IdentityResourceBuilder.OpenId()
                            .AllowAllClients()
                            .FromConfiguration()
                            .Build());
                        break;
                    case IdentityServerConstants.StandardScopes.Profile:
                        options.IdentityResources.Add(IdentityResourceBuilder.Profile()
                            .AllowAllClients()
                            .FromConfiguration()
                            .Build());
                        break;
                    case IdentityServerConstants.StandardScopes.Address:
                        options.IdentityResources.Add(IdentityResourceBuilder.Address()
                            .AllowAllClients()
                            .FromConfiguration()
                            .Build());
                        break;
                    case IdentityServerConstants.StandardScopes.Email:
                        options.IdentityResources.Add(IdentityResourceBuilder.Email()
                            .AllowAllClients()
                            .FromConfiguration()
                            .Build());
                        break;
                    case IdentityServerConstants.StandardScopes.Phone:
                        options.IdentityResources.Add(IdentityResourceBuilder.Phone()
                            .AllowAllClients()
                            .FromConfiguration()
                            .Build());
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid identity resource name '{scope}'");
                }
            }
        }

        private static void ClearDefaultIdentityResources(ApiAuthorizationOptions options)
        {
            var allDefault = true;
            foreach (var resource in options.IdentityResources)
            {
                if (!resource.Properties.TryGetValue(ApplicationProfilesPropertyNames.Source, out var source) ||
                    !string.Equals(ApplicationProfilesPropertyValues.Default, source, StringComparison.OrdinalIgnoreCase))
                {
                    allDefault = false;
                    break;
                }
            }

            if (allDefault)
            {
                options.IdentityResources.Clear();
            }
        }
    }
}
