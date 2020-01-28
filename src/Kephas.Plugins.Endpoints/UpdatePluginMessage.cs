// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdatePluginMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the update plugin message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;

    /// <summary>
    /// An update plugin message.
    /// </summary>
    [TypeDisplay(Description = "Updates the installed plugin with the indicated version.")]
    public class UpdatePluginMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the package ID.
        /// </summary>
        /// <value>
        /// The package ID.
        /// </value>
        [Display(Description = "The package ID of the plugin.")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [Display(Description = "The package version of the plugin.")]
        public string Version { get; set; }
    }
}
