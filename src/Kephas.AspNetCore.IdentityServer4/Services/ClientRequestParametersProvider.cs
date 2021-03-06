// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientRequestParametersProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::IdentityServer4.Extensions;
    using Kephas.AspNetCore.IdentityServer4.Configuration;
    using Kephas.AspNetCore.IdentityServer4.Options;
    using Kephas.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Default implementation of <see cref="IClientRequestParametersProvider"/>.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class ClientRequestParametersProvider : IClientRequestParametersProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientRequestParametersProvider"/> class.
        /// </summary>
        /// <param name="urlResolver">The absolute URL resolver.</param>
        /// <param name="options">The options.</param>
        public ClientRequestParametersProvider(
            IAbsoluteUrlResolver urlResolver,
            IOptions<ApiAuthorizationOptions> options)
        {
            this.UrlResolver = urlResolver;
            this.Options = options;
        }

        /// <summary>
        /// Gets the absolute URL resolver.
        /// </summary>
        protected IAbsoluteUrlResolver UrlResolver { get; }

        /// <summary>
        /// Gets the API authorization options.
        /// </summary>
        protected IOptions<ApiAuthorizationOptions> Options { get; }

        /// <summary>
        /// Gets parameter values for the client with client id<paramref name="clientId"/>.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/>.</param>
        /// <param name="clientId">The client id for the client.</param>
        /// <returns>A <see cref="IDictionary{TKey, TValue}"/> containing the client parameters and their values.</returns>
        public IDictionary<string, string> GetClientParameters(HttpContext context, string clientId)
        {
            var client = this.Options.Value.Clients[clientId];
            var authority = context.GetIdentityServerIssuerUri();
            var responseType = string.Empty;
            if (!client.Properties.TryGetValue(ApplicationProfilesPropertyNames.Profile, out var type))
            {
                throw new InvalidOperationException($"Can't determine the type for the client '{clientId}'");
            }

            switch (type)
            {
                case ApplicationProfiles.IdentityServerSPA:
                case ApplicationProfiles.SPA:
                case ApplicationProfiles.NativeApp:
                    responseType = "code";
                    break;
                default:
                    throw new InvalidOperationException($"Invalid application type '{type}' for '{clientId}'.");
            }

            return new Dictionary<string, string>
            {
                ["authority"] = authority,
                ["client_id"] = client.ClientId,
                ["redirect_uri"] = this.UrlResolver.GetAbsoluteUrl(client.RedirectUris.First(), context)!,
                ["post_logout_redirect_uri"] = this.UrlResolver.GetAbsoluteUrl(client.PostLogoutRedirectUris.First(), context)!,
                ["response_type"] = responseType,
                ["scope"] = string.Join(" ", client.AllowedScopes),
            };
        }
    }
}
