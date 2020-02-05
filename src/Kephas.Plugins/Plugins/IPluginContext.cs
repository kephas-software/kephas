// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPluginContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IPluginContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Values that represent plugin operations.
    /// </summary>
    /// <seealso/>
    public enum PluginOperation
    {
        /// <summary>
        /// An enum constant representing the install option.
        /// </summary>
        Install,

        /// <summary>
        /// An enum constant representing the initialize option.
        /// </summary>
        Initialize,

        /// <summary>
        /// An enum constant representing the enable option.
        /// </summary>
        Enable,

        /// <summary>
        /// An enum constant representing the disable option.
        /// </summary>
        Disable,

        /// <summary>
        /// An enum constant representing the uninitialize option.
        /// </summary>
        Uninitialize,

        /// <summary>
        /// An enum constant representing the uninstall option.
        /// </summary>
        Uninstall,

        /// <summary>
        /// An enum constant representing the update option.
        /// </summary>
        Update,
    }

    /// <summary>
    /// Interface for plugin context.
    /// </summary>
    public interface IPluginContext : IContext
    {
        /// <summary>
        /// Gets or sets the plugin.
        /// </summary>
        /// <value>
        /// The plugin.
        /// </value>
        AppIdentity PluginId { get; set; }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        PluginOperation? Operation { get; set; }

        /// <summary>
        /// Gets or sets the plugin data.
        /// </summary>
        /// <value>
        /// The plugin data.
        /// </value>
        IPlugin Plugin { get; set; }
    }

    /// <summary>
    /// A plugin context extensions.
    /// </summary>
    public static class PluginContextExtensions
    {
        /// <summary>
        /// Sets the plugin operation.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The context to act on.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="overwrite">Optional. True to overwrite the previously set value, false to preserve it.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Operation<TContext>(this TContext context, PluginOperation operation, bool overwrite = true)
            where TContext : class, IPluginContext
        {
            Requires.NotNull(context, nameof(context));

            if (context.Operation.HasValue && !overwrite)
            {
                return context;
            }

            context.Operation = operation;
            return context;
        }

        /// <summary>
        /// Sets the plugin operation.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The context to act on.</param>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="overwrite">Optional. True to overwrite the previously set value, false to preserve it.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext PluginId<TContext>(this TContext context, AppIdentity pluginId, bool overwrite = true)
            where TContext : class, IPluginContext
        {
            Requires.NotNull(context, nameof(context));

            if (context.PluginId != null && !overwrite)
            {
                return context;
            }

            context.PluginId = pluginId;
            return context;
        }

        /// <summary>
        /// Sets the plugin operation.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The context to act on.</param>
        /// <param name="plugin">The plugin data.</param>
        /// <param name="overwrite">Optional. True to overwrite the previously set value, false to
        ///                         preserve it.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Plugin<TContext>(this TContext context, IPlugin plugin, bool overwrite = true)
            where TContext : class, IPluginContext
        {
            Requires.NotNull(context, nameof(context));

            if (context.Plugin != null && !overwrite)
            {
                return context;
            }

            context.Plugin = plugin;
            return context;
        }
    }
}
