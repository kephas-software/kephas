// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    /// <summary>
    /// The client settings.
    /// </summary>
    public class ClientSettings : ServiceSettings
    {
        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        /// <value>
        /// The redirect URI.
        /// </value>
        public string? RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the logout URI.
        /// </summary>
        /// <value>
        /// The logout URI.
        /// </value>
        public string? LogoutUri { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        public string? ClientSecret { get; set; }
    }
}
