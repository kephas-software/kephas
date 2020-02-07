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

    using Kephas.Application;
    using Kephas.Application.Reflection;

    /// <summary>
    /// Information about the plugin.
    /// </summary>
    public class PluginInfo : AppInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginInfo"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="pluginDataStore">The plugin data store.</param>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="description">Optional. The description.</param>
        /// <param name="tags">Optional. The tags.</param>
        internal PluginInfo(IAppRuntime appRuntime, IPluginDataStore pluginDataStore, AppIdentity pluginIdentity, string description = null, string[] tags = null)
            : base(pluginIdentity, description, tags)
        {
            this.AppRuntime = appRuntime;
            this.PluginDataStore = pluginDataStore;
        }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        protected internal IAppRuntime AppRuntime { get; }

        /// <summary>
        /// Gets the plugin data store.
        /// </summary>
        /// <value>
        /// The plugin data store.
        /// </value>
        protected internal IPluginDataStore PluginDataStore { get; }

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
