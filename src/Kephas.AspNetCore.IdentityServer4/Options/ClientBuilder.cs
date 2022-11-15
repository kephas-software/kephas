// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Options
{
    using System;
    using System.Collections.Generic;

    using global::IdentityServer4;
    using global::IdentityServer4.Models;
    using Kephas.AspNetCore.IdentityServer4.Configuration;

    /// <summary>
    /// A builder for Clients.
    /// </summary>
    public class ClientBuilder
    {
        private const string NativeAppClientRedirectUri = "urn:ietf:wg:oauth:2.0:oob";

        Client client;
        private bool isBuilt = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientBuilder"/> class.
        /// </summary>
        public ClientBuilder()
            : this(new Client())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientBuilder"/> class.
        /// </summary>
        /// <param name="client">A preconfigured client.</param>
        public ClientBuilder(Client client)
        {
            this.client = client;
        }

        /// <summary>
        /// Creates a new builder for a single page application that coexists with an authorization server.
        /// </summary>
        /// <param name="clientId">The client id for the single page application.</param>
        /// <returns>A <see cref="ClientBuilder"/>.</returns>
        public static ClientBuilder IdentityServerSPA(string clientId)
        {
            var client = CreateClient(clientId);
            return new ClientBuilder(client)
                .WithApplicationProfile(ApplicationProfiles.IdentityServerSPA)
                .WithAllowedGrants(GrantTypes.Code)
                .WithoutClientSecrets()
                .WithPkce()
                .WithAllowedOrigins(Array.Empty<string>())
                .AllowAccessTokensViaBrowser();
        }

        /// <summary>
        /// Creates a new builder for an externally registered single page application.
        /// </summary>
        /// <param name="clientId">The client id for the single page application.</param>
        /// <returns>A <see cref="ClientBuilder"/>.</returns>
        public static ClientBuilder SPA(string clientId)
        {
            var client = CreateClient(clientId);
            return new ClientBuilder(client)
                .WithApplicationProfile(ApplicationProfiles.SPA)
                .WithAllowedGrants(GrantTypes.Code)
                .WithoutClientSecrets()
                .WithPkce()
                .AllowAccessTokensViaBrowser();
        }

        /// <summary>
        /// Creates a new builder for an externally registered native application.
        /// </summary>
        /// <param name="clientId">The client id for the native application.</param>
        /// <returns>A <see cref="ClientBuilder"/>.</returns>
        public static ClientBuilder NativeApp(string clientId)
        {
            var client = CreateClient(clientId);
            return new ClientBuilder(client)
                .WithApplicationProfile(ApplicationProfiles.NativeApp)
                .WithAllowedGrants(GrantTypes.Code)
                .WithRedirectUri(NativeAppClientRedirectUri)
                .WithLogoutRedirectUri(NativeAppClientRedirectUri)
                .WithPkce()
                .WithoutClientSecrets()
                .WithScopes(IdentityServerConstants.StandardScopes.OfflineAccess);
        }

        /// <summary>
        /// Updates the client id (and name) of the client.
        /// </summary>
        /// <param name="clientId">The new client id.</param>
        /// <returns>The <see cref="ClientBuilder"/>.</returns>
        public ClientBuilder WithClientId(string clientId)
        {
            this.client.ClientId = clientId;
            this.client.ClientName = clientId;

            return this;
        }

        /// <summary>
        /// Sets the application profile for the client.
        /// </summary>
        /// <param name="profile">The the profile for the application from <see cref="ApplicationProfiles"/>.</param>
        /// <returns>The <see cref="ClientBuilder"/>.</returns>
        public ClientBuilder WithApplicationProfile(string profile)
        {
            this.client.Properties.Add(ApplicationProfilesPropertyNames.Profile, profile);
            return this;
        }

        /// <summary>
        /// Adds the <paramref name="scopes"/> to the list of allowed scopes for the client.
        /// </summary>
        /// <param name="scopes">The list of scopes.</param>
        /// <returns>The <see cref="ClientBuilder"/>.</returns>
        public ClientBuilder WithScopes(params string[] scopes)
        {
            foreach (var scope in scopes)
            {
                this.client.AllowedScopes.Add(scope);
            }

            return this;
        }

        /// <summary>
        /// Adds the <paramref name="redirectUri"/> to the list of valid redirect uris for the client.
        /// </summary>
        /// <param name="redirectUri">The redirect uri to add.</param>
        /// <returns>The <see cref="ClientBuilder"/>.</returns>
        public ClientBuilder WithRedirectUri(string redirectUri)
        {
            this.client.RedirectUris.Add(redirectUri);
            return this;
        }

        /// <summary>
        /// Adds the <paramref name="logoutUri"/> to the list of valid logout redirect uris for the client.
        /// </summary>
        /// <param name="logoutUri">The logout uri to add.</param>
        /// <returns>The <see cref="ClientBuilder"/>.</returns>
        public ClientBuilder WithLogoutRedirectUri(string logoutUri)
        {
            this.client.PostLogoutRedirectUris.Add(logoutUri);
            return this;
        }

        /// <summary>
        /// Removes any configured client secret from the client and configures it to not require a client secret for getting tokens
        /// from the token endpoint.
        /// </summary>
        /// <returns>The <see cref="ClientBuilder"/>.</returns>
        public ClientBuilder WithoutClientSecrets()
        {
            this.client.RequireClientSecret = false;
            this.client.ClientSecrets.Clear();

            return this;
        }

        /// <summary>
        /// Builds the client.
        /// </summary>
        /// <returns>The built <see cref="Client"/>.</returns>
        public Client Build()
        {
            if (this.isBuilt)
            {
                throw new InvalidOperationException("Client already built.");
            }

            this.isBuilt = true;
            return this.client;
        }

        /// <summary>
        /// Adds the <paramref name="clientSecret"/> to the list of client secrets for the client and configures the client to
        /// require using the secret when getting tokens from the token endpoint.
        /// </summary>
        /// <param name="clientSecret">The client secret to add.</param>
        /// <returns>The <see cref="ClientBuilder"/>.</returns>
        internal ClientBuilder WithClientSecret(string clientSecret)
        {
            this.client.ClientSecrets.Add(new Secret(clientSecret));
            this.client.RequireClientSecret = true;
            return this;
        }

        internal ClientBuilder WithPkce()
        {
            this.client.RequirePkce = true;
            this.client.AllowPlainTextPkce = false;

            return this;
        }

        internal ClientBuilder FromConfiguration()
        {
            this.client.Properties[ApplicationProfilesPropertyNames.Source] = ApplicationProfilesPropertyValues.Configuration;
            return this;
        }

        internal ClientBuilder WithAllowedGrants(ICollection<string> grants)
        {
            this.client.AllowedGrantTypes = grants;
            return this;
        }

        internal ClientBuilder WithAllowedOrigins(params string[] origins)
        {
            this.client.AllowedCorsOrigins = origins;
            return this;
        }

        internal ClientBuilder AllowAccessTokensViaBrowser()
        {
            this.client.AllowAccessTokensViaBrowser = true;
            return this;
        }

        private static Client CreateClient(string name)
        {
            var client = new Client
            {
                ClientId = name,
                ClientName = name,
                RequireConsent = false,
            };

            return client;
        }
    }
}
