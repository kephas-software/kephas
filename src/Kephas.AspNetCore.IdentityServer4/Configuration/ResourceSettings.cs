// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    /// <summary>
    /// The resource settings.
    /// </summary>
    /// <seealso cref="ServiceSettings" />
    public class ResourceSettings : ServiceSettings
    {
        /// <summary>
        /// Gets or sets the scopes.
        /// </summary>
        /// <value>
        /// The scopes.
        /// </value>
        public string[]? Scopes { get; set; }
    }
}
