// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityServerJwtDescriptor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    using System.Collections.Generic;

    using Kephas.Services;
    using Microsoft.AspNetCore.Hosting;

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
        /// <param name="environment">The environment.</param>
        public IdentityServerJwtDescriptor(IWebHostEnvironment environment)
        {
            this.Environment = environment;
        }

        /// <summary>
        /// Gets the environment.
        /// </summary>
        /// <value>
        /// The environment.
        /// </value>
        public IWebHostEnvironment Environment { get; }

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
                [this.Environment.ApplicationName + "API"] = new ResourceSettings { Profile = ApplicationProfiles.IdentityServerJwt },
            };
        }
    }
}
