// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializedPluginSignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the initialized plugin signal class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Interaction
{
    using Kephas.Application;
    using Kephas.ExceptionHandling;
    using Kephas.Interaction;
    using Kephas.Operations;

    /// <summary>
    /// An initialized plugin signal.
    /// </summary>
    public class InitializedPluginSignal : ISignal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializedPluginSignal"/> class.
        /// </summary>
        /// <param name="pluginId">Identifier for the plugin.</param>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="message">Optional. The message.</param>
        public InitializedPluginSignal(AppIdentity pluginId, IPluginContext context, IOperationResult result, string? message = null)
        {
            this.PluginId = pluginId;
            this.Context = context;
            this.Result = result;
            this.Message = message;
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

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public IOperationResult Result { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string? Message { get; }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public SeverityLevel Severity => SeverityLevel.Info;
    }
}
