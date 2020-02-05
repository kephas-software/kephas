// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginManagerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugin manager base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas;
    using Kephas.Application;
    using Kephas.Diagnostics;
    using Kephas.Dynamic;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Plugins.Interaction;
    using Kephas.Plugins.Reflection;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A plugin manager base.
    /// </summary>
    public abstract class PluginManagerBase : Loggable, IPluginManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManagerBase"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        protected PluginManagerBase(
            IAppRuntime appRuntime,
            IContextFactory contextFactory,
            IEventHub eventHub,
            ILogManager logManager = null)
            : base(logManager)
        {
            this.AppRuntime = appRuntime;
            this.ContextFactory = contextFactory;
            this.EventHub = eventHub;
        }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        protected IAppRuntime AppRuntime { get; }

        /// <summary>
        /// Gets the context factory.
        /// </summary>
        /// <value>
        /// The context factory.
        /// </value>
        protected IContextFactory ContextFactory { get; }

        /// <summary>
        /// Gets the event hub.
        /// </summary>
        /// <value>
        /// The event hub.
        /// </value>
        protected IEventHub EventHub { get; }

        /// <summary>
        /// Gets the available plugins asynchronously.
        /// </summary>
        /// <param name="filter">Optional. Specifies the filter.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the available plugins.
        /// </returns>
        public abstract Task<IOperationResult<IEnumerable<IPluginInfo>>> GetAvailablePluginsAsync(Action<ISearchContext> filter = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the installed plugins.
        /// </summary>
        /// <returns>
        /// An enumeration of installed plugins.
        /// </returns>
        public virtual IEnumerable<IPlugin> GetInstalledPlugins()
        {
            var pluginsFolder = this.AppRuntime.GetPluginsLocation();
            return Directory.EnumerateDirectories(pluginsFolder)
                    .Select(d => new Plugin(new PluginInfo(Path.GetFileName(d), PluginHelper.GetPluginVersion(d))) { FolderPath = d });
        }

        /// <summary>
        /// Installs the plugin asynchronously.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the install operation result.
        /// </returns>
        public virtual async Task<IOperationResult<IPlugin>> InstallPluginAsync(AppIdentity pluginId, Action<IPluginContext> options = null, CancellationToken cancellationToken = default)
        {
            var (_, state, pid) = this.GetInstalledPluginData(pluginId);
            if (state != PluginState.None)
            {
                throw new InvalidOperationException($"Plugin {pluginId} is already installed. State: '{state}', version: '{pid.Version}'.");
            }

            var result = new OperationResult<IPlugin>();
            IPlugin pluginData = null;
            var context = this.CreatePluginContext(options)
                .Operation(PluginOperation.Install, overwrite: false)
                .PluginId(pluginId);
            var opResult = await Profiler.WithInfoStopwatchAsync(
                async () =>
                {
                    var pluginResult = await this.InstallPluginCoreAsync(pluginId, context, cancellationToken)
                                                .PreserveThreadContext();
                    result.MergeResult(pluginResult);
                    pluginData = pluginResult.ReturnValue;

                    PluginHelper.SetPluginData(pluginData.FolderPath, PluginState.PendingInitialization, pluginData.GetTypeInfo().Version);
                }).PreserveThreadContext();

            this.Logger.Info("Plugin {plugin} successfully installed, awaiting initialization. Elapsed: {elapsed:c}.", pluginId, opResult.Elapsed);

            result.ReturnValue = pluginData;
            return result
                    .MergeResult(opResult)
                    .MergeMessage($"Plugin {pluginId} successfully installed, awaiting initialization. Elapsed: {opResult.Elapsed:c}.")
                    .Elapsed(opResult.Elapsed);
        }

        /// <summary>
        /// Uninstalls the plugin asynchronously.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the uninstall operation result.
        /// </returns>
        public virtual async Task<IOperationResult> UninstallPluginAsync(AppIdentity pluginId, Action<IPluginContext> options = null, CancellationToken cancellationToken = default)
        {
            this.AssertPluginsDisabled();

            var result = new OperationResult();
            var opResult = await Profiler.WithInfoStopwatchAsync(
                async () =>
                {
                    var (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);

                    pluginId = pid;
                    Action<IPluginContext> uninstallOptions = ctx =>
                        ctx.Merge(options)
                            .Operation(PluginOperation.Uninstall, overwrite: false)
                            .PluginId(pluginId);
                    if (state == PluginState.Enabled || state == PluginState.Disabled)
                    {
                        var uninitResult = await this.UninitializePluginAsync(pluginId, uninstallOptions, cancellationToken).PreserveThreadContext();
                        if (uninitResult.HasErrors())
                        {
                            throw new AggregateException(uninitResult.Exceptions);
                        }

                        result.MergeResult(uninitResult);
                    }

                    state = this.GetInstalledPluginData(pluginId).state;
                    if (state == PluginState.None)
                    {
                        this.Logger.Warn($"Plugin {pluginId} is not installed.");
                        result.MergeException(new InvalidOperationException($"Plugin {pluginId} is not installed."));
                    }

                    var pluginInfo = new PluginInfo(pid.Id, pid.Version);
                    var pluginData = new Plugin(pluginInfo) { FolderPath = pluginFolder, State = state };
                    var context = this.ContextFactory.CreateContext<PluginContext>()
                                        .Merge(uninstallOptions)
                                        .Plugin(pluginData);
                    await this.UninstallPluginCoreAsync(pid, context, cancellationToken).PreserveThreadContext();
                }).PreserveThreadContext();

            this.Logger.Info("Plugin {plugin} successfully uninstalled. Elapsed: {elapsed:c}.", pluginId, result.Elapsed);

            return result
                .MergeResult(opResult)
                .MergeMessage($"Plugin {pluginId} successfully uninstalled. Elapsed: {result.Elapsed:c}.")
                .Elapsed(opResult.Elapsed);
        }

        /// <summary>
        /// Initializes the plugin asynchronously.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the initialize operation result.
        /// </returns>
        public virtual async Task<IOperationResult<IPlugin>> InitializePluginAsync(AppIdentity pluginId, Action<IPluginContext> options = null, CancellationToken cancellationToken = default)
        {
            IPluginInfo pluginInfo = null;
            IPlugin pluginData = null;
            var context = this.CreatePluginContext(options)
                .Operation(PluginOperation.Initialize, overwrite: false)
                .PluginId(pluginId);

            await this.EventHub.PublishAsync(new InitializingPluginSignal(pluginId, context), context, cancellationToken).PreserveThreadContext();

            var result = new OperationResult<IPlugin>();
            var opResult = await Profiler.WithInfoStopwatchAsync(
                async () =>
                {
                    var (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);
                    if (state != PluginState.PendingInitialization)
                    {
                        throw new InvalidOperationException($"Cannot initialize plugin {pluginId} while in state '{state}'.");
                    }

                    context.PluginId(pluginId = pid);
                    pluginInfo = new PluginInfo(pid.Id, pid.Version);
                    pluginData = new Plugin(pluginInfo) { FolderPath = pluginFolder };
                    try
                    {
                        var initResult = await this.InitializeDataAsync(pluginId, context, cancellationToken).PreserveThreadContext();
                        result.MergeResult(initResult);

                        PluginHelper.SetPluginData(pluginFolder, PluginState.Enabled, pluginId.Version);
                    }
                    catch
                    {
                        PluginHelper.SetPluginData(pluginFolder, PluginState.Corrupt, pluginId.Version);
                        throw;
                    }
                }).PreserveThreadContext();

            this.Logger.Info("Plugin {plugin} successfully initialized. Elapsed: {elapsed:c}.", pluginId, opResult.Elapsed);

            result.ReturnValue = pluginData;
            result
                .MergeResult(opResult)
                .MergeMessage($"Plugin {pluginId} successfully initialized. Elapsed: {opResult.Elapsed:c}.")
                .Elapsed(opResult.Elapsed);

            await this.EventHub.PublishAsync(new InitializedPluginSignal(pluginId, context, result), context, cancellationToken).PreserveThreadContext();

            return result;
        }

        /// <summary>
        /// Uninitializes the plugin asynchronously.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the uninitialize operation result.
        /// </returns>
        public virtual async Task<IOperationResult<IPlugin>> UninitializePluginAsync(AppIdentity pluginId, Action<IPluginContext> options = null, CancellationToken cancellationToken = default)
        {
            IPluginInfo pluginInfo = null;
            IPlugin pluginData = null;
            var context = this.CreatePluginContext(options)
                .Operation(PluginOperation.Uninitialize, overwrite: false)
                .PluginId(pluginId);

            await this.EventHub.PublishAsync(new UninitializingPluginSignal(pluginId, context), context, cancellationToken).PreserveThreadContext();

            var result = new OperationResult<IPlugin>();
            var opResult = await Profiler.WithInfoStopwatchAsync(
                async () =>
                {
                    var (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);
                    if (state != PluginState.Enabled && state != PluginState.Disabled)
                    {
                        throw new InvalidOperationException($"Cannot uninitialize plugin {pluginId} while in state '{state}'.");
                    }

                    context.PluginId(pluginId = pid);
                    pluginInfo = new PluginInfo(pid.Id, pid.Version);
                    pluginData = new Plugin(pluginInfo) { FolderPath = pluginFolder };

                    try
                    {
                        var uninitResult = await this.UninitializeDataAsync(pluginId, context, cancellationToken).PreserveThreadContext();
                        result.MergeResult(uninitResult);

                        PluginHelper.SetPluginData(pluginFolder, PluginState.PendingInitialization, pid.Version);
                    }
                    catch
                    {
                        PluginHelper.SetPluginData(pluginFolder, PluginState.Corrupt, pid.Version);
                        throw;
                    }
                }).PreserveThreadContext();

            this.Logger.Info("Plugin {plugin} successfully uninitialized. Elapsed: {elapsed:c}.", pluginId, opResult.Elapsed);

            result.ReturnValue = pluginData;
            result
                .MergeResult(opResult)
                .MergeMessage($"Plugin {pluginId} successfully uninitialized. Elapsed: {opResult.Elapsed:c}.")
                .Elapsed(opResult.Elapsed);

            await this.EventHub.PublishAsync(new UninitializedPluginSignal(pluginId, context, result), context, cancellationToken).PreserveThreadContext();

            return result;
        }

        /// <summary>
        /// Updates the plugin asynchronously.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the update operation result.
        /// </returns>
        public async Task<IOperationResult<IPlugin>> UpdatePluginAsync(AppIdentity pluginId, Action<IPluginContext> options = null, CancellationToken cancellationToken = default)
        {
            this.AssertPluginsDisabled();

            IOperationResult<IPlugin> result = new OperationResult<IPlugin>();
            var context = this.CreatePluginContext(options)
                .Operation(PluginOperation.Update, overwrite: false)
                .PluginId(pluginId);
            var opResult = await Profiler.WithInfoStopwatchAsync(
                async () =>
                {
                    if (string.IsNullOrEmpty(pluginId.Version))
                    {
                        throw new ArgumentNullException(nameof(pluginId.Version), $"Please provide the version to which the plugin {pluginId} should be updated.");
                    }

                    var (pluginFolder, state, pluginOld) = this.GetInstalledPluginData(pluginId);

                    Action<IPluginContext> updateOptions = ctx =>
                        ctx.Merge(options)
                            .Operation(PluginOperation.Update, overwrite: false)
                            .PluginId(pluginId);

                    var uninstResult = await this.UninstallPluginAsync(pluginOld, updateOptions, cancellationToken).PreserveThreadContext();
                    result.MergeResult(uninstResult);

                    var instResult = await this.InstallPluginAsync(pluginId, updateOptions, cancellationToken).PreserveThreadContext();
                    result.MergeResult(instResult).ReturnValue = instResult.ReturnValue;
                }).PreserveThreadContext();

            return result
                .MergeResult(opResult)
                .MergeMessage($"Plugin {pluginId} successfully updated. Elapsed: {opResult.Elapsed:c}.")
                .Elapsed(opResult.Elapsed);
        }

        /// <summary>
        /// Enables the plugin asynchronously if the plugin was previously disabled.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the enable operation result.
        /// </returns>
        public virtual async Task<IOperationResult<IPlugin>> EnablePluginAsync(AppIdentity pluginId, Action<IPluginContext> options = null, CancellationToken cancellationToken = default)
        {
            this.AssertPluginsDisabled();

            IPluginInfo pluginInfo = null;
            IPlugin pluginData = null;
            var opResult = Profiler.WithInfoStopwatch(
                () =>
                {
                    var (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);
                    if (state != PluginState.Disabled)
                    {
                        throw new InvalidOperationException($"Cannot enable plugin {pid} while in state '{state}'.");
                    }

                    pluginId = pid;
                    pluginInfo = new PluginInfo(pid.Id, pid.Version);
                    pluginData = new Plugin(pluginInfo) { FolderPath = pluginFolder };

                    PluginHelper.SetPluginData(pluginFolder, PluginState.Enabled, pid.Version);
                });

            this.Logger.Info("Plugin {plugin} successfully enabled. Elapsed: {elapsed:c}.", pluginId, opResult.Elapsed);

            return new OperationResult<IPlugin>(pluginData)
                .MergeResult(opResult)
                .MergeMessage($"Plugin {pluginId} successfully enabled. Elapsed: {opResult.Elapsed:c}.")
                .Elapsed(opResult.Elapsed);
        }

        /// <summary>
        /// Disables the plugin asynchronously if the plugin was previously enabled.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the enable operation result.
        /// </returns>
        public virtual async Task<IOperationResult<IPlugin>> DisablePluginAsync(AppIdentity pluginId, Action<IPluginContext> options = null, CancellationToken cancellationToken = default)
        {
            this.AssertPluginsDisabled();

            IPluginInfo pluginInfo = null;
            IPlugin pluginData = null;
            var opResult = Profiler.WithInfoStopwatch(
                () =>
                {
                    var (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);
                    if (state != PluginState.Enabled)
                    {
                        throw new InvalidOperationException($"Cannot disable plugin {pid} while in state '{state}'.");
                    }

                    pluginId = pid;
                    pluginInfo = new PluginInfo(pid.Id, pid.Version);
                    pluginData = new Plugin(pluginInfo) { FolderPath = pluginFolder };

                    PluginHelper.SetPluginData(pluginFolder, PluginState.Disabled, pid.Version);
                });

            this.Logger.Warn("Plugin {plugin} successfully disabled. Elapsed: {elapsed:c}.", pluginId, opResult.Elapsed);

            return new OperationResult<IPlugin>(pluginData)
                .MergeResult(opResult)
                .MergeMessage($"Plugin {pluginId} successfully disabled. Elapsed: {opResult.Elapsed:c}.")
                .Elapsed(opResult.Elapsed);
        }

        /// <summary>
        /// Asserts that the plugins are disabled.
        /// </summary>
        protected virtual void AssertPluginsDisabled()
        {
            if (this.AppRuntime.PluginsEnabled())
            {
                throw new InvalidOperationException("Cannot proceed with the operation while the plugins are enabled. Please start the application in setup mode to disable them and then rerun the operation.");
            }
        }

        /// <summary>
        /// Creates the plugin context.
        /// </summary>
        /// <param name="options">Options for controlling the operation.</param>
        /// <returns>
        /// The new plugin context.
        /// </returns>
        protected virtual IPluginContext CreatePluginContext(Action<IPluginContext> options)
        {
            return this.ContextFactory.CreateContext<PluginContext>().Merge(options);
        }

        /// <summary>
        /// Creates the search context.
        /// </summary>
        /// <param name="filter">Specifies the filter.</param>
        /// <returns>
        /// The new search context.
        /// </returns>
        protected virtual ISearchContext CreateSearchContext(Action<ISearchContext> filter)
        {
            return this.ContextFactory.CreateContext<SearchContext>().Merge(filter);
        }

        /// <summary>
        /// Gets the installed plugin data.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <returns>
        /// The installed plugin data.
        /// </returns>
        protected virtual (string pluginFolder, PluginState state, AppIdentity identity) GetInstalledPluginData(AppIdentity pluginId)
        {
            var pluginFolder = Path.Combine(this.AppRuntime.GetPluginsLocation(), pluginId.Id);
            var (state, version) = PluginHelper.GetPluginData(pluginFolder);
            return (pluginFolder, state, new AppIdentity(pluginId.Id, version));
        }

        /// <summary>
        /// Initializes the plugin data asynchronously.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task<IOperationResult> InitializeDataAsync(AppIdentity pluginId, IPluginContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<IOperationResult>(new OperationResult());
        }

        /// <summary>
        /// Uninitializes the plugin data asynchronously.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task<IOperationResult> UninitializeDataAsync(AppIdentity pluginId, IPluginContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<IOperationResult>(new OperationResult());
        }

        /// <summary>
        /// Installs the plugin asynchronously (core implementation).
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the plugin data.
        /// </returns>
        protected abstract Task<IOperationResult<IPlugin>> InstallPluginCoreAsync(AppIdentity pluginId, IPluginContext context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uninstall the plugin asynchronously (core implementation).
        /// </summary>
        /// <param name="pluginId">Identifier for the plugin.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task<IOperationResult<IPlugin>> UninstallPluginCoreAsync(AppIdentity pluginId, IPluginContext context, CancellationToken cancellationToken)
        {
            var pluginFolder = context.Plugin.FolderPath;
            if (Directory.Exists(pluginFolder))
            {
                Directory.Delete(pluginFolder, recursive: true);
            }

            return Task.FromResult<IOperationResult<IPlugin>>(new OperationResult<IPlugin>(context.Plugin));
        }
    }
}
