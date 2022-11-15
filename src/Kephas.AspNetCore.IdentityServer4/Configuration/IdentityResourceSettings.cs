// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityResourceSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    /// <summary>
    /// The identity resource settings.
    /// </summary>
    /// <seealso cref="ResourceSettings" />
    public class IdentityResourceSettings : ResourceSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityResourceSettings"/> class.
        /// </summary>
        public IdentityResourceSettings()
        {
            this.Profile = ApplicationProfiles.API;
        }
    }
}
