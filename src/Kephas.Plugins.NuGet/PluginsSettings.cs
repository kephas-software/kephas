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
    using global::NuGet.Resolver;
    using Kephas.Application;

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
        public string? NuGetConfigPath { get; set; }

        /// <summary>
        /// Gets or sets the pathname of the cached packages folder.
        /// </summary>
        /// <value>
        /// The pathname of the cached packages folder.
        /// </value>
        public string? PackagesFolder { get; set; }

        /// <summary>
        /// Gets or sets the relative folder within the package containing the configuration files.
        /// </summary>
        /// <value>
        /// The relative folder within the package containing the configuration files.
        /// </value>
        public string PackageConfigFolder { get; set; } = AppRuntimeBase.DefaultConfigFolder;

        /// <summary>
        /// Gets or sets the search term used for identifying plugins.
        /// </summary>
        /// <value>
        /// The search term used for identifying plugins.
        /// </value>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the resolver dependency behavior.
        /// </summary>
        /// <value>
        /// The resolver dependency behavior.
        /// </value>
        public DependencyBehavior ResolverDependencyBehavior { get; set; } = DependencyBehavior.Lowest;
    }
}
