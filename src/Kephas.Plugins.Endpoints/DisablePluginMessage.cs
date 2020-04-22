// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisablePluginMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the disable plugin message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;

    /// <summary>
    /// A disable plugin message.
    /// </summary>
    [DisplayInfo(Description = "Disables the indicated plugin.")]
    public class DisablePluginMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the package ID.
        /// </summary>
        /// <value>
        /// The package ID.
        /// </value>
        [Display(Description = "The package ID of the plugin.")]
        public string Id { get; set; }
    }
}
