// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugin information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Reflection
{
    using System.Collections.Generic;

    using Kephas.Application.Reflection;

    /// <summary>
    /// Information about the plugin.
    /// </summary>
    public class PluginInfo : AppInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginInfo"/> class.
        /// </summary>
        /// <param name="pluginDataService">The plugin data service.</param>
        /// <param name="name">The name.</param>
        /// <param name="version">Optional. the version.</param>
        /// <param name="description">Optional. The description.</param>
        /// <param name="tags">Optional. The tags.</param>
        public PluginInfo(IPluginDataService pluginDataService, string name, string version = null, string description = null, string[] tags = null)
            : base(name, version, description, tags)
        {
            this.PluginDataService = pluginDataService;
        }

        /// <summary>
        /// Gets the plugin data service.
        /// </summary>
        /// <value>
        /// The plugin data service.
        /// </value>
        public IPluginDataService PluginDataService { get; }

        /// <summary>
        /// Creates an instance with the provided arguments (if any).
        /// </summary>
        /// <param name="args">Optional. The arguments.</param>
        /// <returns>
        /// The new instance.
        /// </returns>
        public override object CreateInstance(IEnumerable<object> args = null)
        {
            return new Plugin(this);
        }
    }
}
