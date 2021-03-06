﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnablePluginMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the enable plugin message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;
    using Kephas.Security.Authorization;
    using Kephas.Security.Permissions;
    using Kephas.Security.Permissions.AttributedModel;

    /// <summary>
    /// An enable plugin message.
    /// </summary>
    [DisplayInfo(Description = "Enables the indicated plugin.")]
    [RequiresPermission(typeof(AppAdminPermission))]
    public class EnablePluginMessage : IMessage
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
