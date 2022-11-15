// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureApiScopes.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    using global::IdentityServer4.Models;
    using Kephas.AspNetCore.IdentityServer4.Options;
    using Microsoft.Extensions.Options;

    internal class ConfigureApiScopes : IPostConfigureOptions<ApiAuthorizationOptions>
    {
        public void PostConfigure(string name, ApiAuthorizationOptions options)
        {
            this.AddResourceScopesToApiScopes(options);
        }

        private void AddResourceScopesToApiScopes(ApiAuthorizationOptions options)
        {
            foreach (var resource in options.ApiResources)
            {
                foreach (var scope in resource.Scopes)
                {
                    if (!options.ApiScopes.ContainsScope(scope))
                    {
                        options.ApiScopes.Add(new ApiScope(scope));
                    }
                }
            }
        }
    }
}
