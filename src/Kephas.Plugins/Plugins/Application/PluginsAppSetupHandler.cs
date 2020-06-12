// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginsAppSetupHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Application
{
    using System;
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
        public virtual async Task<IOperationResult> SetupAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return new OperationResult()
                .MergeMessages(await this.InitializePendingPluginsAsync(appContext, cancellationToken)
                    .PreserveThreadContext())
                .MergeMessages(await this.UninstallPendingPluginsAsync(appContext, cancellationToken)
                    .PreserveThreadContext())
                .Complete();
        }

        /// <summary>
        /// Installs the pending plugins asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        private async Task<IOperationResult> InitializePendingPluginsAsync(
            IContext context,
            CancellationToken cancellationToken = default)
        {
            var installedPlugins = this.pluginManager.GetInstalledPlugins();

            var result = new OperationResult();
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    foreach (var plugin in installedPlugins)
                    {
                        if (plugin.State == PluginState.PendingInitialization)
                        {
                            try
                            {
                                var initResult = await this.pluginManager.InitializePluginAsync(plugin.Identity, ctx => ctx.Merge(context), cancellationToken).PreserveThreadContext();
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
                    }
                }).PreserveThreadContext();

            return result.MergeAll(opResult).Complete(opResult.Elapsed);
        }

        /// <summary>
        /// Uninstalls the pending plugins asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        private async Task<IOperationResult> UninstallPendingPluginsAsync(
            IContext context,
            CancellationToken cancellationToken = default)
        {
            var installedPlugins = this.pluginManager.GetInstalledPlugins();

            var result = new OperationResult();
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    foreach (var plugin in installedPlugins)
                    {
                        if (plugin.State == PluginState.PendingUninstallation)
                        {
                            try
                            {
                                var initResult = await this.pluginManager.UninstallPluginAsync(plugin.Identity, ctx => ctx.Merge(context), cancellationToken).PreserveThreadContext();
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
                    }
                }).PreserveThreadContext();

            return result.MergeAll(opResult).Complete(opResult.Elapsed);
        }
    }
}