﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializingPluginSignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the initializing plugin signal class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Interaction
{
    using Kephas.Application;
    using Kephas.ExceptionHandling;
    using Kephas.Interaction;

    /// <summary>
    /// An initializing plugin signal.
    /// </summary>
    public class InitializingPluginSignal : SignalBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializingPluginSignal"/> class.
        /// </summary>
        /// <param name="pluginId">Identifier for the plugin.</param>
        /// <param name="context">The context.</param>
        /// <param name="message">Optional. The message.</param>
        public InitializingPluginSignal(AppIdentity pluginId, IPluginContext context, string? message = null)
            : base(message ?? $"Plugin {pluginId} is being initialized.")
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
