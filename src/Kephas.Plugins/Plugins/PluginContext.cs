// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugin context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Plugins
{
    using Kephas.Application;
    using Kephas.Plugins.Transactions;
    using Kephas.Services;

    /// <summary>
    /// A plugin context.
    /// </summary>
    public class PluginContext : Context, IPluginContext
    {
        private PluginData? pluginData;
        private AppIdentity? pluginIdentity;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        public PluginContext(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        /// <summary>
        /// Gets or sets the plugin.
        /// </summary>
        /// <value>
        /// The plugin.
        /// </value>
        public AppIdentity? PluginIdentity
        {
            get => this.pluginIdentity ?? this.Plugin?.Identity ?? this.PluginData?.Identity;
            set => this.pluginIdentity = value;
        }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public PluginOperation? Operation { get; set; }

        /// <summary>
        /// Gets or sets information describing the plugin. Typically this is used upon installation,
        /// when the plugin instance does not exist yet.
        /// </summary>
        /// <value>
        /// Information describing the plugin.
        /// </value>
        public PluginData? PluginData
        {
            get => this.pluginData ?? this.Plugin?.GetPluginData();
            set => this.pluginData = value;
        }

        /// <summary>
        /// Gets or sets the plugin data.
        /// </summary>
        /// <value>
        /// The plugin data.
        /// </value>
        public IPlugin? Plugin { get; set; }

        /// <summary>
        /// Gets or sets the operation transaction.
        /// </summary>
        /// <value>
        /// The operation transaction.
        /// </value>
        public ITransaction? Transaction { get; set; }
    }
}
