// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallPluginMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the install plugin message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;

    /// <summary>
    /// An install plugin message.
    /// </summary>
    [DisplayInfo(Description = "Installs the indicated plugin.")]
    public class InstallPluginMessage : IMessage
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
        [Display(ShortName = "v", Description = "Optional. The package version of the plugin. Use '@latest' (default) to install to the latest version.")]
        public string Version { get; set; } = UpdatePluginMessage.LatestVersion;

        /// <summary>
        /// Gets or sets a value indicating whether prerelease versions should be included or not.
        /// </summary>
        /// <value>
        /// True to include prerelease versions, false otherwise.
        /// </value>
        [Display(ShortName = "pre", Description = "Optional. Value indicating whether to consider prerelease versions if no explicit version is set (default: false).")]
        [DefaultValue(false)]
        public bool IncludePrerelease { get; set; }
    }
}
