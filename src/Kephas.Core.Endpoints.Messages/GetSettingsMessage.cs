﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSettingsMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;
    using Kephas.Reflection;
    using Kephas.Security.Permissions;
    using Kephas.Security.Permissions.AttributedModel;

    /// <summary>
    /// A get settings message.
    /// </summary>
    [DisplayInfo(Description = "Gets the settings for the provided type.")]
    [RequiresPermission(typeof(AppAdminPermission))]
    public class GetSettingsMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the settings type.
        /// </summary>
        [DisplayInfo(Description = "The name of the settings type to retrieve. The 'Settings' ending may be left out.")]
        public string? SettingsType { get; set; }
    }
}