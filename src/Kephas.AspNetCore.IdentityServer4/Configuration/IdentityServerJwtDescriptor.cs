// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityServerJwtDescriptor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    using System.Collections.Generic;

    using Kephas.AspNetCore.IdentityServer4.Services;
    using Kephas.Services;

    /// <summary>
    /// Provider of JWT resource settings.
    /// </summary>
    /// <seealso cref="IIdentityServerJwtDescriptor" />
    [OverridePriority(Priority.Low)]
    public class IdentityServerJwtDescriptor : IIdentityServerJwtDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityServerJwtDescriptor"/> class.
        /// </summary>
        /// <param name="appIdProvider">The application identity provider.</param>
        public IdentityServerJwtDescriptor(IIdentityAppIdProvider appIdProvider)
        {
            this.AppIdProvider = appIdProvider;
        }

        /// <summary>
        /// Gets the application identity provider.
        /// </summary>
        /// <value>
        /// The application identity provider.
        /// </value>
        protected IIdentityAppIdProvider AppIdProvider { get; }

        /// <summary>
        /// Gets the resource settings.
        /// </summary>
        /// <returns>
        /// A dictionary containing the resource settings.
        /// </returns>
        public IDictionary<string, ResourceSettings> GetResourceSettings()
        {
            return new Dictionary<string, ResourceSettings>
            {
                [this.AppIdProvider.GetIdentityAppId().Id + "API"] = new ResourceSettings { Profile = ApplicationProfiles.IdentityServerJwt },
            };
        }
    }
}
