// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityServerSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    using Kephas.Dynamic;

    /// <summary>
    /// Settings for identity server.
    /// </summary>
    /// <seealso cref="Kephas.Dynamic.Expando" />
    public class IdentityServerSettings : Expando
    {
        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <value>
        /// The resources.
        /// </value>
        public Expando Resources { get; set; }
    }
}
