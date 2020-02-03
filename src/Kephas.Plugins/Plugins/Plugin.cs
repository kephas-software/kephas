// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugin class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using Kephas;
    using Kephas.Application;
    using Kephas.Data;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Plugins.Reflection;
    using Kephas.Reflection;

    /// <summary>
    /// A plugin instance.
    /// </summary>
    public class Plugin : Expando, IPlugin
    {
        private readonly IPluginInfo pluginInfo;
        private PluginState? state;

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        /// <param name="pluginInfo">Information describing the plugin.</param>
        public Plugin(IPluginInfo pluginInfo)
        {
            Requires.NotNull(pluginInfo, nameof(pluginInfo));

            this.pluginInfo = pluginInfo;
        }

        /// <summary>
        /// Gets or sets the full pathname of the installation folder.
        /// </summary>
        /// <value>
        /// The full pathname of the installation folder.
        /// </value>
        public string FolderPath { get; protected internal set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public PluginState State
        {
            get => this.state ?? PluginHelper.GetPluginState(this.FolderPath);
            protected internal set => this.state = value;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        object IIdentifiable.Id => this.GetIdentity().ToString();

        /// <summary>
        /// Gets the identity.
        /// </summary>
        /// <returns>
        /// The identity.
        /// </returns>
        public AppIdentity GetIdentity() => this.pluginInfo.GetIdentity();

        /// <summary>
        /// Gets the type information for this instance.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        public IPluginInfo GetTypeInfo() => this.pluginInfo;

        /// <summary>
        /// Gets type information.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        ITypeInfo IInstance.GetTypeInfo() => this.GetTypeInfo();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.pluginInfo.GetIdentity()} ({this.State})";
        }
    }
}
