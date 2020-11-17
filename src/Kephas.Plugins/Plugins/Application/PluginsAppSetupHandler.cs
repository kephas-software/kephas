// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginsAppSetupHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Application
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Diagnostics;
    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Application setup handler for installing/uninstalling plugins.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class PluginsAppSetupHandler : IAppSetupHandler
    {
        private readonly IPluginManager pluginManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginsAppSetupHandler"/> class.
        /// </summary>
        /// <param name="pluginManager">The plugin manager.</param>
        public PluginsAppSetupHandler(IPluginManager pluginManager)
        {
            this.pluginManager = pluginManager;
        }

        /// <summary>
        /// Performs one step in the application setup.
        /// </summary>
        /// <param name="appContext">The application context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public virtual async Task<IOperationResult> SetupAsync(
            IContext appContext,
            CancellationToken cancellationToken = default)
        {
            // initializes, uninitializes, or uninstalls pending plugins
            return new OperationResult()
                .MergeMessages(await this.UpdatePendingPluginsAsync(appContext, cancellationToken)
                    .PreserveThreadContext())
                .MergeMessages(await this.UninitializePendingPluginsAsync(appContext, cancellationToken)
                    .PreserveThreadContext())
                .MergeMessages(await this.UninstallPendingPluginsAsync(appContext, cancellationToken)
                    .PreserveThreadContext())
                .MergeMessages(await this.InitializePendingPluginsAsync(appContext, cancellationToken)
                    .PreserveThreadContext())
                .Complete();
        }

        /// <summary>
        /// Initializes the pending plugins asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task<IOperationResult> InitializePendingPluginsAsync(
            IContext context,
            CancellationToken cancellationToken = default)
        {
            return this.ExecutePendingPluginOperationAsync(
                context,
                (p, ctx) => p.State == PluginState.PendingInitialization,
                (p, ctx, token) => this.pluginManager
                    .InitializePluginAsync(p.Identity, ctx => ctx.Merge(context), token),
                cancellationToken);
        }

        /// <summary>
        /// Uninitializes the pending plugins asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task<IOperationResult> UninitializePendingPluginsAsync(
            IContext context,
            CancellationToken cancellationToken = default)
        {
            return this.ExecutePendingPluginOperationAsync(
                context,
                (p, ctx) => p.State == PluginState.PendingUninitialization,
                (p, ctx, token) => this.pluginManager
                    .UninitializePluginAsync(p.Identity, ctx => ctx.Merge(context), token),
                cancellationToken);
        }

        /// <summary>
        /// Uninstalls the pending plugins asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task<IOperationResult> UninstallPendingPluginsAsync(
            IContext context,
            CancellationToken cancellationToken = default)
        {
            return this.ExecutePendingPluginOperationAsync(
                context,
                (p, ctx) => p.State == PluginState.PendingUninstallation,
                (p, ctx, token) => this.pluginManager
                    .UninstallPluginAsync(p.Identity, ctx => ctx.Merge(context), token),
                cancellationToken);
        }

        /// <summary>
        /// Update the pending plugins asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task<IOperationResult> UpdatePendingPluginsAsync(
            IContext context,
            CancellationToken cancellationToken = default)
        {
            return this.ExecutePendingPluginOperationAsync(
                context,
                (p, ctx) => p.GetPluginData().UpdatingToVersion != null,
                (p, ctx, token) => this.pluginManager
                    .UpdatePluginAsync(new AppIdentity(p.Identity.Id, p.GetPluginData().UpdatingToVersion), ctx => ctx.Merge(context), token),
                cancellationToken);
        }

        /// <summary>
        /// Executes the plugin pending operation asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="filter">The plugin filter.</param>
        /// <param name="asyncOperation">The plugin async operation.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected async Task<IOperationResult> ExecutePendingPluginOperationAsync(
            IContext context,
            Func<IPlugin, IContext, bool> filter,
            Func<IPlugin, IContext, CancellationToken, Task<IOperationResult<IPlugin>>> asyncOperation,
            CancellationToken cancellationToken = default)
        {
            var installedPlugins = this.pluginManager.GetInstalledPlugins();

            var result = new OperationResult();
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    foreach (var plugin in installedPlugins.Where(p => filter(p, context)))
                    {
                        try
                        {
                            var initResult = await asyncOperation(plugin, context, cancellationToken)
                                .PreserveThreadContext();
                            result.MergeMessages(initResult);
                        }
                        catch (OperationCanceledException cex)
                        {
                            result.MergeException(cex);
                            break;
                        }
                        catch (Exception ex)
                        {
                            result.MergeException(ex);
                        }
                    }
                }).PreserveThreadContext();

            return result.MergeAll(opResult).Complete(opResult.Elapsed);
        }
    }
}