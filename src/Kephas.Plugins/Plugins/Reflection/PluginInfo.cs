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
    using Kephas.Reflection.Dynamic;

    /// <summary>
    /// Information about the plugin.
    /// </summary>
    public class PluginInfo : DynamicTypeInfo, IPluginInfo
    {
        private readonly AppIdentity identity;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginInfo"/> class.
        /// </summary>
        /// <param name="pluginDataService">The plugin data service.</param>
        /// <param name="name">The name.</param>
        /// <param name="version">Optional. the version.</param>
        /// <param name="description">Optional. The description.</param>
        /// <param name="tags">Optional. The tags.</param>
        public PluginInfo(IPluginDataService pluginDataService, string name, string version = null, string description = null, string[] tags = null)
        {
            this.Name = name;
            this.PluginDataSevice = pluginDataService;
            this.Version = version;
            this.Description = description;
            this.Tags = tags;
            this.identity = new AppIdentity(name, version);
        }

        /// <summary>
        /// Gets the plugin data service.
        /// </summary>
        /// <value>
        /// The plugin data service.
        /// </value>
        public IPluginDataService PluginDataSevice { get; }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        public string Version { get; }

        /// <summary>
        /// Gets the plugin description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public string[] Tags { get; }

        /// <summary>
        /// Gets the plugin dependencies.
        /// </summary>
        /// <value>
        /// The dependencies.
        /// </value>
        public IEnumerable<IPluginDependency> Dependencies { get; } = new List<IPluginDependency>();

        /// <summary>
        /// Gets the identity.
        /// </summary>
        /// <returns>
        /// The identity.
        /// </returns>
        public AppIdentity GetIdentity() => this.identity;

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

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.GetIdentity().ToString();
        }
    }
}
