// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdatePluginMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the update plugin message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Plugins.Endpoints
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;

    /// <summary>
    /// An update plugin message.
    /// </summary>
    [TypeDisplay(Description = "Updates the installed plugin with the indicated version. A typical usage is 'updateplugin all', which updates all installed plugins to the latest version.")]
    public class UpdatePluginMessage : IMessage
    {
        /// <summary>
        /// The latest version identifier.
        /// </summary>
        public static readonly string LatestVersion = "latest";

        /// <summary>
        /// Gets or sets the package ID.
        /// </summary>
        /// <value>
        /// The package ID.
        /// </value>
        [Display(Description = "The package ID of the plugin. Use 'all' to update all plugins.")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [Display(Description = "Optional. The package version of the plugin. Use 'latest' (default) to update to the latest version.")]
        public string Version { get; set; } = LatestVersion;

        /// <summary>
        /// Gets or sets a value indicating whether prerelease versions should be included or not.
        /// </summary>
        /// <value>
        /// True to include prerelease versions, false otherwise.
        /// </value>
        [Display(Description = "Optional. Value indicating whether the prerelease versions should be included or not (default: false).")]
        [DefaultValue(false)]
        public bool IncludePrerelease { get; set; }
    }
}
