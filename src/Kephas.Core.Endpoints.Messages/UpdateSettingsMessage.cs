// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSettingsMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Security.Permissions;
    using Kephas.Security.Permissions.AttributedModel;

    /// <summary>
    /// Message for updating the settings.
    /// </summary>
    [Display(Description = "Gets the provided settings.")]
    [RequiresPermission(typeof(AppAdminPermission))]
    public class UpdateSettingsMessage
    {
        /// <summary>
        /// Gets or sets the settings type.
        /// </summary>
        [Display(Description = "The name of the settings type to update. The 'Settings' ending may be left out. If not provided, the Settings must contain an already formed settings instance.")]
        public string? SettingsType { get; set; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        [Display(Description = "The settings value to update. If it contains an already formed settings instance, the settings type may not be provided. If it is a string value, it contains the serialized value of the settings.")]
        public object? Settings { get; set; }
    }
}