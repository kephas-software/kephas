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
    using Kephas.ExceptionHandling;
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
            : this(appRuntime, contextFactory, eventHub, appRuntime.GetPluginDataService(), logManager)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManagerBase"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="pluginDataService">The plugin data service.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        protected PluginManagerBase(
            IAppRuntime appRuntime,
            IContextFactory contextFactory,
            IEventHub eventHub,
            IPluginDataService pluginDataService,
            ILogManager logManager = null)
            : base(logManager)
        {
            this.AppRuntime = appRuntime;
            this.ContextFactory = contextFactory;
            this.EventHub = eventHub;
            this.PluginDataService = pluginDataService;
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
        /// Gets the plugin data service.
        /// </summary>
        /// <value>
        /// The plugin data service.
        /// </value>
        protected IPluginDataService PluginDataService { get; }

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
                    .Select(d => new Plugin(new PluginInfo(this.PluginDataService, Path.GetFileName(d), this.PluginDataService.GetPluginData(d).version)) { Location = d });
        }

        /// <summary>
        /// Gets the plugin state.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <returns>
        /// The plugin state.
        /// </returns>
        public virtual PluginState GetPluginState(AppIdentity pluginId)
        {
            var pluginsFolder = this.AppRuntime.GetPluginsLocation();
            var pluginLocation = Directory.EnumerateDirectories(pluginsFolder)
                    .FirstOrDefault(d => Path.GetFileName(d).Equals(pluginId.Id, StringComparison.CurrentCultureIgnoreCase));
            if (pluginLocation == null)
            {
                return PluginState.None;
            }

            return this.PluginDataService.GetPluginData(pluginLocation).state;
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
            var result = new OperationResult<IPlugin>();
            var opResult = await Profiler.WithInfoStopwatchAsync(
                async () =>
                {
                    Action<IPluginContext> installOptions = ctx =>
                        ctx.Merge(options)
                            .Operation(PluginOperation.Install, overwrite: false)
                            .PluginId(pluginId);

                    var (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);
                    var installComplete = false;
                    var initializeComplete = false;
                    if (this.CanInstallPlugin(pid, state, pluginFolder))
                    {
                        var context = this.CreatePluginContext(options)
                            .Merge(installOptions)
                            .Plugin(result.ReturnValue);

                        var instResult = await this.InstallPluginCoreAsync(pluginId, context, cancellationToken)
                                                    .PreserveThreadContext();

                        var pluginData = result.ReturnValue = instResult.ReturnValue;
                        result
                            .MergeResult(instResult)
                            .MergeMessage($"Plugin {pluginId} successfully installed, awaiting initialization.");

                        this.PluginDataService.SetPluginData(pluginData.Location, PluginState.PendingInitialization, pluginData.GetTypeInfo().Version);

                        (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);
                        installComplete = state == PluginState.PendingInitialization;

                        if (installComplete)
                        {
                            this.Logger.Info("Plugin {plugin} successfully installed, awaiting initialization.", pluginId);
                        }
                    }

                    if (this.CanInitializePlugin(pid, state, pluginFolder))
                    {
                        try
                        {
                            var initResult = await this.InitializePluginAsync(pluginId, installOptions, cancellationToken).PreserveThreadContext();
                            result.ReturnValue = initResult.ReturnValue;
                            result.MergeResult(initResult);
                        }
                        catch (Exception ex) when (ex is ISeverityQualifiedException qex && !qex.Severity.IsError())
                        {
                            result.MergeException(ex);
                            if (ex is PluginOperationException pex)
                            {
                                result.Merge(pex.Result);
                            }
                        }

                        (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);
                        initializeComplete = state == PluginState.Enabled;
                    }

                    if (!installComplete && !initializeComplete)
                    {
                        throw new PluginOperationException($"Plugin {pluginId} cannot be installed. State: '{state}', version: '{pid.Version}'.", result);
                    }
                }).PreserveThreadContext();

            return result
                    .MergeResult(opResult)
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
        public virtual async Task<IOperationResult<IPlugin>> UninstallPluginAsync(AppIdentity pluginId, Action<IPluginContext> options = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult<IPlugin>();
            var opResult = await Profiler.WithInfoStopwatchAsync(
                async () =>
                {
                    var (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);

                    pluginId = pid;
                    var pluginInfo = new PluginInfo(this.PluginDataService, pid.Id, pid.Version);
                    var pluginData = new Plugin(pluginInfo) { Location = pluginFolder, State = state };
                    result.ReturnValue = pluginData;

                    Action<IPluginContext> uninstallOptions = ctx =>
                        ctx.Merge(options)
                            .Operation(PluginOperation.Uninstall, overwrite: false)
                            .PluginId(pluginId);
                    var uninstallComplete = false;
                    var uninitializeComplete = false;
                    if (this.CanUninitializePlugin(pluginId, state, pluginFolder))
                    {
                        try
                        {
                            var uninitResult = await this.UninitializePluginAsync(pluginId, uninstallOptions, cancellationToken).PreserveThreadContext();
                            result.ReturnValue = uninitResult.ReturnValue;
                            result.MergeResult(uninitResult);
                        }
                        catch (Exception ex) when (ex is ISeverityQualifiedException qex && !qex.Severity.IsError())
                        {
                            result.MergeException(ex);
                            if (ex is PluginOperationException pex)
                            {
                                result.Merge(pex.Result);
                            }
                        }

                        (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);
                        uninitializeComplete = state == PluginState.PendingUninstallation;
                    }

                    if (this.CanUninstallPlugin(pluginId, state, pluginFolder))
                    {
                        pluginData.State = state;
                        var context = this.ContextFactory.CreateContext<PluginContext>()
                                            .Merge(uninstallOptions)
                                            .Plugin(pluginData);
                        var uninstResult = await this.UninstallPluginCoreAsync(pid, context, cancellationToken).PreserveThreadContext();
                        result.ReturnValue = uninstResult.ReturnValue;
                        result.MergeResult(uninstResult);

                        (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);
                        uninstallComplete = state == PluginState.None;

                        if (uninstallComplete)
                        {
                            this.Logger.Info("Plugin {plugin} successfully uninstalled.", pluginId);
                        }
                    }

                    if (!uninstallComplete && !uninitializeComplete)
                    {
                        throw new PluginOperationException($"Plugin {pluginId} cannot be uninstalled. State: '{state}', version: '{pid.Version}'.", result);
                    }
                }).PreserveThreadContext();

            return result
                .MergeResult(opResult)
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
            IPlugin pluginData = null;

            var result = new OperationResult<IPlugin>();
            var opResult = await Profiler.WithInfoStopwatchAsync(
                async () =>
                {
                    var (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);
                    pluginId = pid;
                    var pluginInfo = new PluginInfo(this.PluginDataService, pid.Id, pid.Version);
                    pluginData = new Plugin(pluginInfo) { Location = pluginFolder };
                    result.ReturnValue = pluginData;

                    Action<IPluginContext> initializeOptions = ctx => ctx
                        .Merge(options)
                        .Operation(PluginOperation.Initialize, overwrite: false)
                        .PluginId(pluginId);
                    var context = this.CreatePluginContext(initializeOptions)
                        .Plugin(pluginData);

                    if (!this.CanInitializePlugin(pid, state, pluginFolder))
                    {
                        throw new PluginOperationException(
                            $"Plugin {pluginId} cannot be initialized. State '{state}', version: '{pid.Version}'.",
                            result,
                            context.Operation == PluginOperation.Initialize ? SeverityLevel.Error : SeverityLevel.Warning);
                    }

                    await this.EventHub.PublishAsync(new InitializingPluginSignal(pluginId, context), context, cancellationToken).PreserveThreadContext();

                    try
                    {
                        var initResult = await this.InitializeDataAsync(pluginId, context, cancellationToken).PreserveThreadContext();
                        result.MergeResult(initResult);

                        this.PluginDataService.SetPluginData(pluginFolder, PluginState.Disabled, pluginId.Version);

                        (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);
                    }
                    catch (Exception ex) when (ex is ISeverityQualifiedException qex && !qex.Severity.IsError())
                    {
                        // treat non-errors as ignorable, meaning that the plugin state is not changed to corrupt.
                        throw;
                    }
                    catch
                    {
                        this.PluginDataService.SetPluginData(pluginFolder, PluginState.Corrupt, pluginId.Version);
                        throw;
                    }

                    await this.EventHub.PublishAsync(new InitializedPluginSignal(pluginId, context, result), context, cancellationToken).PreserveThreadContext();

                    if (this.CanEnablePlugin(pid, state, pluginFolder))
                    {
                        try
                        {
                            var enableResult = await this.EnablePluginAsync(pluginId, initializeOptions, cancellationToken).PreserveThreadContext();
                            result.ReturnValue = enableResult.ReturnValue;
                            result.MergeResult(enableResult);
                        }
                        catch (Exception ex) when (ex is ISeverityQualifiedException qex && !qex.Severity.IsError())
                        {
                            result.MergeException(ex);
                            if (ex is PluginOperationException pex)
                            {
                                result.Merge(pex.Result);
                            }
                        }

                        (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);
                    }
                }).PreserveThreadContext();

            this.Logger.Info("Plugin {plugin} successfully initialized. Elapsed: {elapsed:c}.", pluginId, opResult.Elapsed);

            result.ReturnValue = pluginData;
            result
                .MergeResult(opResult)
                .MergeMessage($"Plugin {pluginId} successfully initialized. Elapsed: {opResult.Elapsed:c}.")
                .Elapsed(opResult.Elapsed);

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
            IPlugin pluginData = null;

            var result = new OperationResult<IPlugin>();
            var opResult = await Profiler.WithInfoStopwatchAsync(
                async () =>
                {
                    var (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);
                    pluginId = pid;

                    Action<IPluginContext> uninitializeOptions = ctx => ctx
                        .Merge(options)
                        .Operation(PluginOperation.Uninitialize, overwrite: false)
                        .PluginId(pluginId);

                    if (this.CanDisablePlugin(pid, state, pluginFolder))
                    {
                        try
                        {
                            var disableResult = await this.DisablePluginAsync(pluginId, uninitializeOptions, cancellationToken).PreserveThreadContext();
                            result.ReturnValue = disableResult.ReturnValue;
                            result.MergeResult(disableResult);
                        }
                        catch (Exception ex) when (ex is ISeverityQualifiedException qex && !qex.Severity.IsError())
                        {
                            result.MergeException(ex);
                            if (ex is PluginOperationException pex)
                            {
                                result.Merge(pex.Result);
                            }
                        }

                        (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);
                    }

                    var pluginInfo = new PluginInfo(this.PluginDataService, pid.Id, pid.Version);
                    pluginData = new Plugin(pluginInfo) { Location = pluginFolder };
                    var context = this.CreatePluginContext(uninitializeOptions)
                        .Plugin(pluginData);

                    result.ReturnValue = pluginData;

                    if (!this.CanUninitializePlugin(pid, state, pluginFolder))
                    {
                        throw new PluginOperationException(
                            $"Plugin {pluginId} cannot be uninitialized. State '{state}', version: '{pid.Version}'.",
                            result,
                            context.Operation == PluginOperation.Uninitialize ? SeverityLevel.Error : SeverityLevel.Warning);
                    }

                    await this.EventHub.PublishAsync(new UninitializingPluginSignal(pluginId, context), context, cancellationToken).PreserveThreadContext();

                    try
                    {
                        var uninitResult = await this.UninitializeDataAsync(pluginId, context, cancellationToken).PreserveThreadContext();
                        result.MergeResult(uninitResult);

                        this.PluginDataService.SetPluginData(pluginFolder, PluginState.PendingUninstallation, pid.Version);
                    }
                    catch (Exception ex) when (ex is ISeverityQualifiedException qex && !qex.Severity.IsError())
                    {
                        // treat non-errors as ignorable, meaning that the plugin state is not changed to corrupt.
                        throw;
                    }
                    catch
                    {
                        this.PluginDataService.SetPluginData(pluginFolder, PluginState.Corrupt, pid.Version);
                        throw;
                    }

                    await this.EventHub.PublishAsync(new UninitializedPluginSignal(pluginId, context, result), context, cancellationToken).PreserveThreadContext();

                }).PreserveThreadContext();

            this.Logger.Info("Plugin {plugin} successfully uninitialized. Elapsed: {elapsed:c}.", pluginId, opResult.Elapsed);

            result.ReturnValue = pluginData;
            result
                .MergeResult(opResult)
                .MergeMessage($"Plugin {pluginId} successfully uninitialized. Elapsed: {opResult.Elapsed:c}.")
                .Elapsed(opResult.Elapsed);

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
            IPlugin pluginData = null;
            var result = new OperationResult<IPlugin>();
            var opResult = Profiler.WithInfoStopwatch(
                () =>
                {
                    var context = this.CreatePluginContext(ctx => ctx.Merge(options).Operation(PluginOperation.Enable, overwrite: false));
                    var (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);
                    if (!this.CanEnablePlugin(pid, state, pluginFolder))
                    {
                        throw new PluginOperationException(
                            $"Plugin {pluginId} cannot be enabled. State '{state}', version: '{pid.Version}'.",
                            result,
                            context.Operation == PluginOperation.Enable ? SeverityLevel.Error : SeverityLevel.Warning);
                    }

                    pluginId = pid;
                    var pluginInfo = new PluginInfo(this.PluginDataService, pid.Id, pid.Version);
                    pluginData = new Plugin(pluginInfo) { Location = pluginFolder };

                    this.PluginDataService.SetPluginData(pluginFolder, PluginState.Enabled, pid.Version);
                });

            this.Logger.Info("Plugin {plugin} successfully enabled. Elapsed: {elapsed:c}.", pluginId, opResult.Elapsed);

            result.ReturnValue = pluginData;
            return result
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
            IPlugin pluginData = null;
            var result = new OperationResult<IPlugin>();
            var opResult = Profiler.WithInfoStopwatch(
                () =>
                {
                    var context = this.CreatePluginContext(ctx => ctx.Merge(options).Operation(PluginOperation.Disable, overwrite: false));
                    var (pluginFolder, state, pid) = this.GetInstalledPluginData(pluginId);
                    if (!this.CanDisablePlugin(pid, state, pluginFolder))
                    {
                        throw new PluginOperationException(
                            $"Plugin {pluginId} cannot be disabled. State '{state}', version: '{pid.Version}'.",
                            result,
                            context.Operation == PluginOperation.Enable ? SeverityLevel.Error : SeverityLevel.Warning);
                    }

                    pluginId = pid;
                    var pluginInfo = new PluginInfo(this.PluginDataService, pid.Id, pid.Version);
                    pluginData = new Plugin(pluginInfo) { Location = pluginFolder };

                    this.PluginDataService.SetPluginData(pluginFolder, PluginState.Disabled, pid.Version);
                });

            this.Logger.Warn("Plugin {plugin} successfully disabled. Elapsed: {elapsed:c}.", pluginId, opResult.Elapsed);

            result.ReturnValue = pluginData;
            return result
                .MergeResult(opResult)
                .MergeMessage($"Plugin {pluginId} successfully disabled. Elapsed: {opResult.Elapsed:c}.")
                .Elapsed(opResult.Elapsed);
        }

        /// <summary>
        /// Asserts that the plugins are enabled.
        /// </summary>
        protected virtual void AssertPluginsEnabled()
        {
            if (!this.AppRuntime.PluginsEnabled())
            {
                throw new PluginOperationException("Cannot proceed with the operation while the plugins are disabled. Please start the application in productive mode to enable them and then rerun the operation.");
            }
        }

        /// <summary>
        /// Asserts that the plugins are disabled.
        /// </summary>
        protected virtual void AssertPluginsDisabled()
        {
            if (this.AppRuntime.PluginsEnabled())
            {
                throw new PluginOperationException("Cannot proceed with the operation while the plugins are enabled. Please start the application in setup mode to disable them and then rerun the operation.");
            }
        }

        /// <summary>
        /// Determines whether the plugin can be installed.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="state">The state.</param>
        /// <param name="pluginLocation">The plugin location.</param>
        /// <returns>
        /// True if the plugin can be installed, false if not.
        /// </returns>
        protected virtual bool CanInstallPlugin(AppIdentity pluginId, PluginState state, string pluginLocation)
        {
            return state == PluginState.None;
        }

        /// <summary>
        /// Determines whether the plugin can be initialized.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="state">The state.</param>
        /// <param name="pluginLocation">The plugin location.</param>
        /// <returns>
        /// True if the plugin can be initialized, false if not.
        /// </returns>
        protected virtual bool CanInitializePlugin(AppIdentity pluginId, PluginState state, string pluginLocation)
        {
            return state == PluginState.PendingInitialization;
        }

        /// <summary>
        /// Determines whether the plugin can be uninitialized.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="state">The state.</param>
        /// <param name="pluginLocation">The plugin location.</param>
        /// <returns>
        /// True if the plugin can be uninitialized, false if not.
        /// </returns>
        protected virtual bool CanUninitializePlugin(AppIdentity pluginId, PluginState state, string pluginLocation)
        {
            return state == PluginState.Disabled;
        }

        /// <summary>
        /// Determines whether the plugin can be uninstalled.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="state">The state.</param>
        /// <param name="pluginLocation">The plugin location.</param>
        /// <returns>
        /// True if the plugin can be uninstalled, false if not.
        /// </returns>
        protected virtual bool CanUninstallPlugin(AppIdentity pluginId, PluginState state, string pluginLocation)
        {
            return state == PluginState.PendingUninstallation || state == PluginState.Corrupt;
        }

        /// <summary>
        /// Determines whether the plugin can be enabled.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="state">The state.</param>
        /// <param name="pluginLocation">The plugin location.</param>
        /// <returns>
        /// True if the plugin can be enabled, false if not.
        /// </returns>
        protected virtual bool CanEnablePlugin(AppIdentity pluginId, PluginState state, string pluginLocation)
        {
            return state == PluginState.Disabled;
        }

        /// <summary>
        /// Determines whether the plugin can be disabled.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <param name="state">The state.</param>
        /// <param name="pluginLocation">The plugin location.</param>
        /// <returns>
        /// True if the plugin can be disabled, false if not.
        /// </returns>
        protected virtual bool CanDisablePlugin(AppIdentity pluginId, PluginState state, string pluginLocation)
        {
            return state == PluginState.Enabled;
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
            var (state, version) = this.PluginDataService.GetPluginData(pluginFolder);
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
            var pluginFolder = context.Plugin.Location;
            if (Directory.Exists(pluginFolder))
            {
                Directory.Delete(pluginFolder, recursive: true);
            }

            return Task.FromResult<IOperationResult<IPlugin>>(new OperationResult<IPlugin>(context.Plugin));
        }
    }
}
