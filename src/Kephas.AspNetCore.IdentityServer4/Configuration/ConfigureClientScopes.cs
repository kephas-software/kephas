// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureClientScopes.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    using System;
    using System.Linq;

    using global::IdentityServer4.Models;
    using Kephas.AspNetCore.IdentityServer4.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    internal class ConfigureClientScopes : IPostConfigureOptions<ApiAuthorizationOptions>
    {
        private static readonly char[] DefaultClientListSeparator = new char[] { ' ' };
        private readonly ILogger<ConfigureClientScopes> logger;

        public ConfigureClientScopes(ILogger<ConfigureClientScopes> logger)
        {
            this.logger = logger;
        }

        public void PostConfigure(string name, ApiAuthorizationOptions options)
        {
            this.AddApiResourceScopesToClients(options);
            this.AddIdentityResourceScopesToClients(options);
        }

        private void AddIdentityResourceScopesToClients(ApiAuthorizationOptions options)
        {
            foreach (var identityResource in options.IdentityResources)
            {
                if (!identityResource.Properties.TryGetValue(ApplicationProfilesPropertyNames.Clients, out var clientList))
                {
                    this.logger.LogInformation($"Identity resource '{identityResource.Name}' doesn't define a list of allowed applications.");
                    continue;
                }

                var resourceClients = clientList.Split(DefaultClientListSeparator, StringSplitOptions.RemoveEmptyEntries);
                if (resourceClients.Length == 0)
                {
                    this.logger.LogInformation($"Identity resource '{identityResource.Name}' doesn't define a list of allowed applications.");
                    continue;
                }

                if (resourceClients.Length == 1 && resourceClients[0] == ApplicationProfilesPropertyValues.AllowAllApplications)
                {
                    this.logger.LogInformation($"Identity resource '{identityResource.Name}' allows all applications.");
                }
                else
                {
                    this.logger.LogInformation($"Identity resource '{identityResource.Name}' allows applications '{string.Join(" ", resourceClients)}'.");
                }

                foreach (var client in options.Clients)
                {
                    if ((resourceClients.Length == 1 && resourceClients[0] == ApplicationProfilesPropertyValues.AllowAllApplications) ||
                        resourceClients.Contains(client.ClientId))
                    {
                        client.AllowedScopes.Add(identityResource.Name);
                    }
                }
            }
        }

        private void AddApiResourceScopesToClients(ApiAuthorizationOptions options)
        {
            foreach (var resource in options.ApiResources)
            {
                if (!resource.Properties.TryGetValue(ApplicationProfilesPropertyNames.Clients, out var clientList))
                {
                    this.logger.LogInformation($"Resource '{resource.Name}' doesn't define a list of allowed applications.");
                    continue;
                }

                var resourceClients = clientList.Split(DefaultClientListSeparator, StringSplitOptions.RemoveEmptyEntries);
                if (resourceClients.Length == 0)
                {
                    this.logger.LogInformation($"Resource '{resource.Name}' doesn't define a list of allowed applications.");
                    continue;
                }

                if (resourceClients.Length == 1 && resourceClients[0] == ApplicationProfilesPropertyValues.AllowAllApplications)
                {
                    this.logger.LogInformation($"Resource '{resource.Name}' allows all applications.");
                }
                else
                {
                    this.logger.LogInformation($"Resource '{resource.Name}' allows applications '{string.Join(" ", resourceClients)}'.");
                }

                foreach (var client in options.Clients)
                {
                    if ((resourceClients.Length == 1 && resourceClients[0] == ApplicationProfilesPropertyValues.AllowAllApplications) ||
                        resourceClients.Contains(client.ClientId))
                    {
                        AddScopes(resource, client);
                    }
                }
            }
        }

        private static void AddScopes(ApiResource resource, Client client)
        {
            foreach (var scope in resource.Scopes)
            {
                client.AllowedScopes.Add(scope);
            }
        }
    }
}
