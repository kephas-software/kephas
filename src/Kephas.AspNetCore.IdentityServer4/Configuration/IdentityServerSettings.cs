// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityServerSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    using System.Collections.Generic;

    using Kephas.Configuration;
    using Kephas.Dynamic;

    /// <summary>
    /// Settings for identity server.
    /// </summary>
    /// <seealso cref="Kephas.Dynamic.Expando" />
    public class IdentityServerSettings : Expando, ISettings
    {
        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <value>
        /// The resources.
        /// </value>
        public IDictionary<string, ResourceSettings> Resources { get; set; } = new Dictionary<string, ResourceSettings>();

        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        /// <value>
        /// The clients.
        /// </value>
        public IDictionary<string, ClientSettings> Clients { get; set; } = new Dictionary<string, ClientSettings>();

        /// <summary>
        /// Gets or sets the identity.
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
        public IdentityResourceSettings? Identity { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public KeySettings? Key { get; set; }

        /// <summary>
        /// Gets or sets the user interaction settings.
        /// </summary>
        /// <value>
        /// The user interaction settings.
        /// </value>
        public UserInteractionSettings? UserInteraction { get; set; }
    }
}
