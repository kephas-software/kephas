// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UninitializingPluginSignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the uninitializing plugin signal class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Interaction
{
    using Kephas.Application;
    using Kephas.ExceptionHandling;
    using Kephas.Interaction;

    /// <summary>
    /// An uninitializing plugin signal.
    /// </summary>
    public class UninitializingPluginSignal : SignalBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UninitializingPluginSignal"/> class.
        /// </summary>
        /// <param name="pluginId">Identifier for the plugin.</param>
        /// <param name="context">The context.</param>
        /// <param name="message">Optional. The message.</param>
        public UninitializingPluginSignal(AppIdentity pluginId, IPluginContext context, string? message = null)
            : base(message ?? $"Plugin {pluginId} is being uninitialized.")
        {
            this.PluginId = pluginId;
            this.Context = context;
        }

        /// <summary>
        /// Gets the identifier of the plugin.
        /// </summary>
        /// <value>
        /// The identifier of the plugin.
        /// </value>
        public AppIdentity PluginId { get; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public IPluginContext Context { get; }
    }
}
