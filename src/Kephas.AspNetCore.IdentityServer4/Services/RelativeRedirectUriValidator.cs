// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelativeRedirectUriValidator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::IdentityServer4.Models;
    using global::IdentityServer4.Validation;
    using Kephas.AspNetCore.IdentityServer4.Configuration;
    using Kephas.Diagnostics.Contracts;

    internal class RelativeRedirectUriValidator : StrictRedirectUriValidator
    {
        public RelativeRedirectUriValidator(IAbsoluteUrlResolver absoluteUrlResolver)
        {
            absoluteUrlResolver = absoluteUrlResolver ?? throw new System.ArgumentNullException(nameof(absoluteUrlResolver));

            AbsoluteUrlResolver = absoluteUrlResolver;
        }

        protected IAbsoluteUrlResolver AbsoluteUrlResolver { get; }

        public override Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (IsLocalSPA(client))
            {
                return ValidateRelativeUris(requestedUri, client.RedirectUris);
            }
            else
            {
                return base.IsRedirectUriValidAsync(requestedUri, client);
            }
        }

        public override Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (IsLocalSPA(client))
            {
                return ValidateRelativeUris(requestedUri, client.PostLogoutRedirectUris);
            }
            else
            {
                return base.IsPostLogoutRedirectUriValidAsync(requestedUri, client);
            }
        }

        private static bool IsLocalSPA(Client client) =>
            client.Properties.TryGetValue(ApplicationProfilesPropertyNames.Profile, out var clientType) &&
            ApplicationProfiles.IdentityServerSPA == clientType;

        private Task<bool> ValidateRelativeUris(string requestedUri, IEnumerable<string> clientUris)
        {
            foreach (var url in clientUris)
            {
                if (Uri.IsWellFormedUriString(url, UriKind.Relative))
                {
                    var newUri = AbsoluteUrlResolver.GetAbsoluteUrl(url);
                    if (string.Equals(newUri, requestedUri, StringComparison.Ordinal))
                    {
                        return Task.FromResult(true);
                    }
                }
            }

            return Task.FromResult(false);
        }
    }
}
