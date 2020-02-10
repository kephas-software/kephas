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
    using System.Linq;
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
        /// <param name="pluginRepository">The plugin data store.</param>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="description">Optional. The description.</param>
        /// <param name="tags">Optional. The tags.</param>
        internal PluginInfo(IAppRuntime appRuntime, IPluginRepository pluginRepository, AppIdentity pluginIdentity, string description = null, string[] tags = null)
            : base(pluginIdentity, description, tags)
        {
            this.AppRuntime = appRuntime;
            this.PluginRepository = pluginRepository;
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
        protected internal IPluginRepository PluginRepository { get; }

        /// <summary>
        /// Creates an instance with the provided arguments (if any).
        /// </summary>
        /// <param name="args">Optional. The arguments.</param>
        /// <returns>
        /// The new instance.
        /// </returns>
        public override object CreateInstance(IEnumerable<object> args = null)
        {
            if (args?.Any() ?? false)
            {
                var pluginData = (PluginData)args.First();
                return new Plugin(this, pluginData);
            }

            return new Plugin(this);
        }
    }
}
