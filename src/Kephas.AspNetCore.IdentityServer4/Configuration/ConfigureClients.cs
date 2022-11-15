// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureClients.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    using System;
    using System.Collections.Generic;

    using global::IdentityServer4.Models;
    using Kephas.AspNetCore.IdentityServer4.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    internal class ConfigureClients : IConfigureOptions<ApiAuthorizationOptions>
    {
        private const string DefaultLocalSPARelativeRedirectUri = "/authentication/login-callback";
        private const string DefaultLocalSPARelativePostLogoutRedirectUri = "/authentication/logout-callback";

        private readonly IdentityServerSettings _configuration;
        private readonly ILogger<ConfigureClients> _logger;

        public ConfigureClients(
            IdentityServerSettings configuration,
            ILogger<ConfigureClients> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void Configure(ApiAuthorizationOptions options)
        {
            foreach (var client in GetClients())
            {
                options.Clients.Add(client);
            }
        }

        internal IEnumerable<Client> GetClients()
        {
            var data = _configuration.Clients;
            if (data != null)
            {
                foreach (var kvp in data)
                {
                    _logger.LogInformation($"Configuring client '{kvp.Key}'.");
                    var name = kvp.Key;
                    var definition = kvp.Value;

                    switch (definition.Profile)
                    {
                        case ApplicationProfiles.SPA:
                            yield return GetSPA(name, definition);
                            break;
                        case ApplicationProfiles.IdentityServerSPA:
                            yield return GetLocalSPA(name, definition);
                            break;
                        case ApplicationProfiles.NativeApp:
                            yield return GetNativeApp(name, definition);
                            break;
                        default:
                            throw new InvalidOperationException($"Type '{definition.Profile}' is not supported.");
                    }
                }
            }
        }

        private Client GetSPA(string name, ClientSettings definition)
        {
            if (definition.RedirectUri == null ||
                !Uri.TryCreate(definition.RedirectUri, UriKind.Absolute, out var redirectUri))
            {
                throw new InvalidOperationException($"The redirect uri " +
                    $"'{definition.RedirectUri}' for '{name}' is invalid. " +
                    $"The redirect URI must be an absolute url.");
            }

            if (definition.LogoutUri == null ||
                !Uri.TryCreate(definition.LogoutUri, UriKind.Absolute, out var postLogouturi))
            {
                throw new InvalidOperationException($"The logout uri " +
                    $"'{definition.LogoutUri}' for '{name}' is invalid. " +
                    $"The logout URI must be an absolute url.");
            }

            if (!string.Equals(
                redirectUri.GetLeftPart(UriPartial.Authority),
                postLogouturi.GetLeftPart(UriPartial.Authority),
                StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"The redirect uri and the logout uri " +
                    $"for '{name}' have a different scheme, host or port.");
            }

            var client = ClientBuilder.SPA(name)
                .WithRedirectUri(definition.RedirectUri)
                .WithLogoutRedirectUri(definition.LogoutUri)
                .WithAllowedOrigins(redirectUri.GetLeftPart(UriPartial.Authority))
                .FromConfiguration();

            return client.Build();
        }

        private Client GetNativeApp(string name, ClientSettings definition)
        {
            var client = ClientBuilder.NativeApp(name)
                .FromConfiguration();
            return client.Build();
        }

        private Client GetLocalSPA(string name, ClientSettings definition)
        {
            var client = ClientBuilder
                .IdentityServerSPA(name)
                .WithRedirectUri(definition.RedirectUri ?? DefaultLocalSPARelativeRedirectUri)
                .WithLogoutRedirectUri(definition.LogoutUri ?? DefaultLocalSPARelativePostLogoutRedirectUri)
                .WithAllowedOrigins()
                .FromConfiguration();

            return client.Build();
        }
    }
}
