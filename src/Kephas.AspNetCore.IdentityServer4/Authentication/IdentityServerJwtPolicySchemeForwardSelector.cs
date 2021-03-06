// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityServerJwtPolicySchemeForwardSelector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Authentication
{
    using System;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;

    internal class IdentityServerJwtPolicySchemeForwardSelector
    {
        private readonly PathString identityPath;
        private readonly string identityServerJwtScheme;

        public IdentityServerJwtPolicySchemeForwardSelector(
            string identityPath,
            string identityServerJwtScheme)
        {
            this.identityPath = identityPath;
            this.identityServerJwtScheme = identityServerJwtScheme;
        }

        public string SelectScheme(HttpContext ctx)
        {
            if (ctx.Request.Path.StartsWithSegments(this.identityPath, StringComparison.OrdinalIgnoreCase))
            {
                return IdentityConstants.ApplicationScheme;
            }

            return this.identityServerJwtScheme;
        }
    }
}
