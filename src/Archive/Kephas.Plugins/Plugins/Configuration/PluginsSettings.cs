// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginsSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Configuration
{
    using Kephas.Configuration;
    using Kephas.Dynamic;

    /// <summary>
    /// Settings for plugins.
    /// </summary>
    public class PluginsSettings : Expando, ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the plugins are enabled.
        /// </summary>
        public bool EnablePlugins { get; set; } = true;
    }
}