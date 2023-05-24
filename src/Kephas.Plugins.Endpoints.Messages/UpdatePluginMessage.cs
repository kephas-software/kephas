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
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Security.Authorization;
    using Kephas.Security.Permissions;
    using Kephas.Security.Permissions.AttributedModel;

    /// <summary>
    /// An update plugin message.
    /// </summary>
    [Display(Description = "Updates the installed plugin with the indicated version. A typical usage is 'updateplugin @all', which updates all installed plugins to the latest version.")]
    [RequiresPermission(typeof(AppAdminPermission))]
    public class UpdatePluginMessage : IMessage<Response>
    {
        /// <summary>
        /// Version identifier indicating the latest version available.
        /// </summary>
        public static readonly string LatestVersion = "@latest";

        /// <summary>
        /// Package identifier indicating all available packages.
        /// </summary>
        public static readonly string All = "@all";

        /// <summary>
        /// Gets or sets the package ID.
        /// </summary>
        /// <value>
        /// The package ID.
        /// </value>
        [Display(Description = "The package ID of the plugin. Use '@all' to update all plugins.")]
        public string Id { get; set; } = All;

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [Display(ShortName = "v", Description = "Optional. The package version of the plugin. Use '@latest' (default) to update to the latest version.")]
        public string Version { get; set; } = LatestVersion;

        /// <summary>
        /// Gets or sets a value indicating whether prerelease versions should be included or not.
        /// </summary>
        /// <value>
        /// True to include prerelease versions, false otherwise.
        /// </value>
        [Display(ShortName = "pre", Description = "Optional. Value indicating whether the prerelease versions should be included or not (default: false).")]
        [DefaultValue(false)]
        public bool IncludePrerelease { get; set; }
    }
}
