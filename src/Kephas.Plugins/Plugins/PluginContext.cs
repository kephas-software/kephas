// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugin context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Services;

    /// <summary>
    /// A plugin context.
    /// </summary>
    public class PluginContext : Context, IPluginContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginContext"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        public PluginContext(ICompositionContext compositionContext)
            : base(compositionContext)
        {
        }

        /// <summary>
        /// Gets or sets the plugin.
        /// </summary>
        /// <value>
        /// The plugin.
        /// </value>
        public AppIdentity PluginIdentity { get; set; }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public PluginOperation? Operation { get; set; }

        /// <summary>
        /// Gets or sets the plugin data.
        /// </summary>
        /// <value>
        /// The plugin data.
        /// </value>
        public IPlugin Plugin { get; set; }
    }
}
