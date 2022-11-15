// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    using Kephas.Dynamic;

    /// <summary>
    /// The service settings.
    /// </summary>
    public class ServiceSettings : Expando
    {
        /// <summary>
        /// Gets or sets the profile.
        /// </summary>
        /// <value>
        /// The profile.
        /// </value>
        public string? Profile { get; set; }
    }
}
