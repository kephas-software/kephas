// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserInteractionSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    /// <summary>
    /// Settings for the user interaction.
    /// </summary>
    public class UserInteractionSettings
    {
        /// <summary>
        /// Gets or sets the error URL.
        /// </summary>
        public string? ErrorUrl { get; set; }
    }
}