﻿// --------------------------------------------------------------------------------------------------------------------
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
    using Kephas.Plugins.Transactions;
    using Kephas.Services;

    /// <summary>
    /// Values that represent plugin operations.
    /// </summary>
    /// <seealso/>
    public enum PluginOperation
    {
        /// <summary>
        /// The plugin is being installed.
        /// </summary>
        Install,

        /// <summary>
        /// The plugin is being initialized.
        /// </summary>
        Initialize,

        /// <summary>
        /// The plugin is being enabled.
        /// </summary>
        Enable,

        /// <summary>
        /// The plugin is being disabled.
        /// </summary>
        Disable,

        /// <summary>
        /// The plugin is being prepared for uninitialization.
        /// </summary>
        PrepareUninitialization,

        /// <summary>
        /// The plugin is being uninitialized.
        /// </summary>
        Uninitialize,

        /// <summary>
        /// The plugin is being uninstalled.
        /// </summary>
        Uninstall,

        /// <summary>
        /// The plugin is being updated.
        /// </summary>
        Update,

        /// <summary>
        /// An enum constant representing the download option.
        /// </summary>
        Download,
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
        AppIdentity? PluginIdentity { get; set; }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        PluginOperation? Operation { get; set; }

        /// <summary>
        /// Gets or sets information describing the plugin.
        /// Typically this is used upon installation, when the plugin instance does not exist yet.
        /// </summary>
        /// <value>
        /// Information describing the plugin.
        /// </value>
        PluginData? PluginData { get; set; }

        /// <summary>
        /// Gets or sets the plugin instance.
        /// </summary>
        /// <value>
        /// The plugin instance.
        /// </value>
        IPlugin? Plugin { get; set; }

        /// <summary>
        /// Gets or sets the operation transaction.
        /// </summary>
        /// <value>
        /// The operation transaction.
        /// </value>
        ITransaction? Transaction { get; set; }
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
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="overwrite">Optional. True to overwrite the previously set value, false to preserve it.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext PluginIdentity<TContext>(this TContext context, AppIdentity pluginIdentity, bool overwrite = true)
            where TContext : class, IPluginContext
        {
            Requires.NotNull(context, nameof(context));

            if (context.PluginIdentity != null && !overwrite)
            {
                return context;
            }

            context.PluginIdentity = pluginIdentity;
            return context;
        }

        /// <summary>
        /// Sets the plugin data. Typically this is used upon installation, when the plugin instance does not exist yet.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The context to act on.</param>
        /// <param name="pluginData">The plugin data.</param>
        /// <param name="overwrite">Optional. True to overwrite the previously set value, false to
        ///                         preserve it.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext PluginData<TContext>(this TContext context, PluginData pluginData, bool overwrite = true)
            where TContext : class, IPluginContext
        {
            Requires.NotNull(context, nameof(context));

            if (context.PluginData != null && !overwrite)
            {
                return context;
            }

            context.PluginData = pluginData;
            return context;
        }

        /// <summary>
        /// Sets the plugin instance.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The context to act on.</param>
        /// <param name="plugin">The plugin instance.</param>
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

        /// <summary>
        /// Sets the transaction.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The context to act on.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Transaction<TContext>(this TContext context, ITransaction transaction)
            where TContext : class, IPluginContext
        {
            Requires.NotNull(context, nameof(context));
            Requires.NotNull(transaction, nameof(transaction));

            context.Transaction = transaction;
            return context;
        }
    }
}
