// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginsSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugins settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.NuGet
{
    /// <summary>
    /// The plugins settings.
    /// </summary>
    public class PluginsSettings
    {
        /// <summary>
        /// Gets or sets the full pathname of the NuGet configuration file.
        /// </summary>
        /// <value>
        /// The full pathname of the NuGet configuration file.
        /// </value>
        public string NuGetConfigPath { get; set; }

        /// <summary>
        /// Gets or sets the pathname of the cached packages folder.
        /// </summary>
        /// <value>
        /// The pathname of the cached packages folder.
        /// </value>
        public string PackagesFolder { get; set; }

        /// <summary>
        /// Gets or sets the search term used for identifying plugins.
        /// </summary>
        /// <value>
        /// The search term used for identifying plugins.
        /// </value>
        public string SearchTerm { get; set; }
    }
}
