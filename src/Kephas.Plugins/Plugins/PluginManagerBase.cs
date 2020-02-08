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
    using Kephas.Application.Reflection;
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
            : this(appRuntime, contextFactory, eventHub, appRuntime.GetPluginRepository(), logManager)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManagerBase"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="pluginRepository">The plugin data store.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        protected PluginManagerBase(
            IAppRuntime appRuntime,
            IContextFactory contextFactory,
            IEventHub eventHub,
            IPluginRepository pluginRepository,
            ILogManager logManager = null)
            : base(logManager)
        {
            this.AppRuntime = appRuntime;
            this.ContextFactory = contextFactory;
            this.EventHub = eventHub;
            this.PluginRepository = pluginRepository;
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
        /// Gets the plugin repository.
        /// </summary>
        /// <value>
        /// The plugin repository.
        /// </value>
        protected IPluginRepository PluginRepository { get; }

        /// <summary>
        /// Gets the available plugins asynchronously.
        /// </summary>
        /// <param name="filter">Optional. Specifies the filter.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the available plugins.
        /// </returns>
        public abstract Task<IOperationResult<IEnumerable<IAppInfo>>> GetAvailablePluginsAsync(Action<ISearchContext> filter = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the installed plugins.
        /// </summary>
        /// <returns>
        /// An enumeration of installed plugins.
        /// </returns>
        public virtual IEnumerable<IPlugin> GetInstalledPlugins()
        {
            return this.AppRuntime.GetInstalledPlugins().Select(this.ToPlugin);
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
            return this.PluginRepository.GetPluginData(pluginId).State;
        }

        /// <summary>
        /// Installs the plugin asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the install operation result.
        /// </returns>
        public virtual async Task<IOperationResult<IPlugin>> InstallPluginAsync(AppIdentity pluginIdentity, Action<IPluginContext> options = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult<IPlugin>();
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    // set the plugin identity in the options with the value provided, which should include
                    // the version, too. Before installation, the version returned from the store is empty.
                    Action<IPluginContext> installOptions = ctx =>
                        ctx.Merge(options)
                            .Operation(PluginOperation.Install, overwrite: false)
                            .PluginIdentity(pluginIdentity);

                    var pluginData = this.GetInstalledPluginData(pluginIdentity);
                    pluginIdentity = pluginData.Identity;

                    var installComplete = false;
                    var initializeComplete = false;
                    if (this.CanInstallPlugin(pluginData))
                    {
                        var context = this.CreatePluginContext(options)
                            .Merge(installOptions)
                            .Plugin(result.ReturnValue);

                        var instWrappedResult = await Profiler.WithStopwatchAsync(
                                    () => this.InstallPluginCoreAsync(pluginIdentity, context, cancellationToken))
                            .PreserveThreadContext();

                        var plugin = result.ReturnValue = instWrappedResult.ReturnValue.ReturnValue;
                        result
                            .MergeMessages(instWrappedResult.ReturnValue)
                            .MergeMessage($"Plugin {pluginIdentity} successfully installed, awaiting initialization. Elapsed: {instWrappedResult.Elapsed:c}.");

                        this.PluginRepository.StorePluginData(new PluginData(pluginData.Identity, PluginState.PendingInitialization));

                        pluginData = this.GetInstalledPluginData(pluginIdentity);
                        installComplete = pluginData.State == PluginState.PendingInitialization;

                        if (installComplete)
                        {
                            this.Logger.Info("Plugin {plugin} successfully installed, awaiting initialization. Elapsed: {elapsed:c}.", pluginIdentity, instWrappedResult.Elapsed);
                        }
                    }

                    if (this.CanInitializePlugin(pluginData))
                    {
                        try
                        {
                            result.MergeAll(
                                await this.InitializePluginAsync(pluginIdentity, installOptions, cancellationToken)
                                    .PreserveThreadContext());
                        }
                        catch (Exception ex) when (ex is ISeverityQualifiedNotification qex && !qex.Severity.IsError())
                        {
                            result.MergeException(ex);
                            if (ex is PluginOperationException pex)
                            {
                                result.MergeMessages(pex.Result);
                            }
                        }

                        pluginData = this.GetInstalledPluginData(pluginIdentity);
                        initializeComplete = pluginData.State == PluginState.Enabled;
                    }

                    if (!installComplete && !initializeComplete)
                    {
                        throw new PluginOperationException($"Plugin {pluginIdentity} cannot be installed. State: '{pluginData.State}'.", result);
                    }
                }).PreserveThreadContext();

            return result
                    .MergeMessages(opResult)
                    .Complete(opResult.Elapsed);
        }

        /// <summary>
        /// Uninstalls the plugin asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the uninstall operation result.
        /// </returns>
        public virtual async Task<IOperationResult<IPlugin>> UninstallPluginAsync(AppIdentity pluginIdentity, Action<IPluginContext> options = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult<IPlugin>();
            var uninstallComplete = false;
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    var pluginData = this.GetInstalledPluginData(pluginIdentity);
                    pluginIdentity = pluginData.Identity;

                    var plugin = this.ToPlugin(pluginData);
                    result.ReturnValue = plugin;

                    Action<IPluginContext> uninstallOptions = ctx =>
                        ctx.Merge(options)
                            .Operation(PluginOperation.Uninstall, overwrite: false)
                            .PluginIdentity(pluginIdentity);

                    var uninitializeComplete = false;
                    if (this.CanUninitializePlugin(pluginData) || this.CanDisablePlugin(pluginData))
                    {
                        try
                        {
                            result.MergeAll(
                                await this.UninitializePluginAsync(pluginIdentity, uninstallOptions, cancellationToken)
                                    .PreserveThreadContext());
                        }
                        catch (Exception ex) when (ex is ISeverityQualifiedNotification qex && !qex.Severity.IsError())
                        {
                            result.MergeException(ex);
                            if (ex is PluginOperationException pex)
                            {
                                result.MergeMessages(pex.Result);
                            }
                        }

                        pluginData = this.GetInstalledPluginData(pluginIdentity);
                        uninitializeComplete = pluginData.State == PluginState.PendingUninstallation;
                    }

                    if (this.CanUninstallPlugin(pluginData))
                    {
                        var context = this.ContextFactory.CreateContext<PluginContext>()
                                            .Merge(uninstallOptions)
                                            .Plugin(plugin);

                        var uninstWrappedResult = await Profiler.WithStopwatchAsync(
                                () => this.UninstallPluginCoreAsync(pluginIdentity, context, cancellationToken))
                            .PreserveThreadContext();
                        result.MergeAll(uninstWrappedResult.ReturnValue);

                        this.PluginRepository.StorePluginData(new PluginData(pluginData.Identity, PluginState.None));

                        pluginData = this.GetInstalledPluginData(pluginIdentity);
                        uninstallComplete = pluginData.State == PluginState.None;

                        if (uninstallComplete)
                        {
                            this.Logger.Info("Plugin {plugin} successfully uninstalled. Elapsed: {elapsed:c}.", pluginIdentity, uninstWrappedResult.Elapsed);
                        }
                    }

                    if (!uninstallComplete && !uninitializeComplete)
                    {
                        throw new PluginOperationException($"Plugin {pluginIdentity} cannot be uninstalled. State: '{pluginData.State}'.", result);
                    }
                }).PreserveThreadContext();

            return result
                .MergeMessages(opResult)
                .Complete(opResult.Elapsed);
        }

        /// <summary>
        /// Initializes the plugin asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the initialize operation result.
        /// </returns>
        public virtual async Task<IOperationResult<IPlugin>> InitializePluginAsync(AppIdentity pluginIdentity, Action<IPluginContext> options = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult<IPlugin>();
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    var pluginData = this.GetInstalledPluginData(pluginIdentity);
                    pluginIdentity = pluginData.Identity;

                    var plugin = this.ToPlugin(pluginData);
                    result.ReturnValue(plugin);

                    Action<IPluginContext> initializeOptions = ctx => ctx
                        .Merge(options)
                        .Operation(PluginOperation.Initialize, overwrite: false)
                        .PluginIdentity(pluginIdentity);
                    var context = this.CreatePluginContext(initializeOptions)
                        .Plugin(plugin);

                    if (!this.CanInitializePlugin(pluginData))
                    {
                        throw new PluginOperationException(
                            $"Plugin {pluginIdentity} cannot be initialized. State '{pluginData.State}'.",
                            result,
                            context.Operation == PluginOperation.Initialize ? SeverityLevel.Error : SeverityLevel.Warning);
                    }

                    await this.EventHub.PublishAsync(new InitializingPluginSignal(pluginIdentity, context), context, cancellationToken).PreserveThreadContext();

                    var initializeComplete = false;
                    try
                    {
                        var initWrappedResult = await Profiler.WithStopwatchAsync(
                                () => this.InitializeDataAsync(pluginIdentity, context, cancellationToken))
                            .PreserveThreadContext();
                        result
                            .MergeAll(initWrappedResult.ReturnValue)
                            .ReturnValue(plugin);

                        this.PluginRepository.StorePluginData(new PluginData(pluginData.Identity, PluginState.Disabled));

                        pluginData = this.GetInstalledPluginData(pluginIdentity);
                        initializeComplete = pluginData.State == PluginState.Disabled;
                        if (initializeComplete)
                        {
                            this.Logger.Info("Plugin {plugin} successfully initialized. Elapsed: {elapsed:c}.", pluginIdentity, initWrappedResult.Elapsed);
                        }
                    }
                    catch (Exception ex) when (ex is ISeverityQualifiedNotification qex && !qex.Severity.IsError())
                    {
                        // treat non-errors as ignorable, meaning that the plugin state is not changed to corrupt.
                        throw;
                    }
                    catch
                    {
                        this.PluginRepository.StorePluginData(new PluginData(pluginData.Identity, PluginState.Corrupt));
                        throw;
                    }

                    await this.EventHub.PublishAsync(new InitializedPluginSignal(pluginIdentity, context, result), context, cancellationToken).PreserveThreadContext();

                    if (this.CanEnablePlugin(pluginData))
                    {
                        try
                        {
                            var enableResult = await this.EnablePluginAsync(pluginIdentity, initializeOptions, cancellationToken).PreserveThreadContext();
                            result.MergeAll(enableResult);
                        }
                        catch (Exception ex) when (ex is ISeverityQualifiedNotification qex && !qex.Severity.IsError())
                        {
                            result.MergeException(ex);
                            if (ex is PluginOperationException pex)
                            {
                                result.MergeMessages(pex.Result);
                            }
                        }
                    }

                    return plugin;
                }).PreserveThreadContext();

            result
                .MergeMessages(opResult)
                .MergeMessage($"Plugin {pluginIdentity} successfully initialized. Elapsed: {opResult.Elapsed:c}.")
                .Complete(opResult.Elapsed);

            return result;
        }

        /// <summary>
        /// Uninitializes the plugin asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the uninitialize operation result.
        /// </returns>
        public virtual async Task<IOperationResult<IPlugin>> UninitializePluginAsync(AppIdentity pluginIdentity, Action<IPluginContext> options = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult<IPlugin>();
            var uninitializeComplete = false;
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    var pluginData = this.GetInstalledPluginData(pluginIdentity);
                    pluginIdentity = pluginData.Identity;

                    Action<IPluginContext> uninitializeOptions = ctx => ctx
                        .Merge(options)
                        .Operation(PluginOperation.Uninitialize, overwrite: false)
                        .PluginIdentity(pluginIdentity);

                    if (this.CanDisablePlugin(pluginData))
                    {
                        try
                        {
                            result.MergeAll(
                                await this.DisablePluginAsync(pluginIdentity, uninitializeOptions, cancellationToken)
                                    .PreserveThreadContext());
                        }
                        catch (Exception ex) when (ex is ISeverityQualifiedNotification qex && !qex.Severity.IsError())
                        {
                            result.MergeException(ex);
                            if (ex is PluginOperationException pex)
                            {
                                result.MergeMessages(pex.Result);
                            }
                        }

                        pluginData = this.GetInstalledPluginData(pluginIdentity);
                    }

                    var plugin = this.ToPlugin(pluginData);
                    result.ReturnValue(plugin);
                    var context = this.CreatePluginContext(uninitializeOptions)
                        .Plugin(plugin);

                    if (!this.CanUninitializePlugin(pluginData))
                    {
                        throw new PluginOperationException(
                            $"Plugin {pluginIdentity} cannot be uninitialized. State '{pluginData.State}'.",
                            result,
                            context.Operation == PluginOperation.Uninitialize ? SeverityLevel.Error : SeverityLevel.Warning);
                    }

                    await this.EventHub.PublishAsync(new UninitializingPluginSignal(pluginIdentity, context), context, cancellationToken).PreserveThreadContext();

                    try
                    {
                        var uninitWrappedResult = await Profiler.WithStopwatchAsync(
                            () => this.UninitializeDataAsync(pluginIdentity, context, cancellationToken)).PreserveThreadContext();
                        result.MergeAll(uninitWrappedResult.ReturnValue);

                        this.PluginRepository.StorePluginData(new PluginData(pluginData.Identity, PluginState.PendingUninstallation));

                        pluginData = this.GetInstalledPluginData(pluginIdentity);
                        uninitializeComplete = pluginData.State == PluginState.PendingUninstallation;
                    }
                    catch (Exception ex) when (ex is ISeverityQualifiedNotification qex && !qex.Severity.IsError())
                    {
                        // treat non-errors as ignorable, meaning that the plugin state is not changed to corrupt.
                        throw;
                    }
                    catch
                    {
                        this.PluginRepository.StorePluginData(new PluginData(pluginData.Identity, PluginState.Corrupt));
                        throw;
                    }

                    await this.EventHub.PublishAsync(new UninitializedPluginSignal(pluginIdentity, context, result), context, cancellationToken).PreserveThreadContext();

                    return plugin;

                }).PreserveThreadContext();

            this.Logger.Info("Plugin {plugin} successfully uninitialized. Elapsed: {elapsed:c}.", pluginIdentity, opResult.Elapsed);

            result
                .MergeAll(opResult)
                .MergeMessage($"Plugin {pluginIdentity} successfully uninitialized. Elapsed: {opResult.Elapsed:c}.")
                .Complete(opResult.Elapsed);

            return result;
        }

        /// <summary>
        /// Enables the plugin asynchronously if the plugin was previously disabled.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the enable operation result.
        /// </returns>
        public virtual async Task<IOperationResult<IPlugin>> EnablePluginAsync(AppIdentity pluginIdentity, Action<IPluginContext> options = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult<IPlugin>();
            var opResult = Profiler.WithStopwatch(
                () =>
                {
                    var context = this.CreatePluginContext(ctx => ctx.Merge(options).Operation(PluginOperation.Enable, overwrite: false));

                    var pluginData = this.GetInstalledPluginData(pluginIdentity);
                    pluginIdentity = pluginData.Identity;

                    if (!this.CanEnablePlugin(pluginData))
                    {
                        throw new PluginOperationException(
                            $"Plugin {pluginIdentity} cannot be enabled. State '{pluginData.State}'.",
                            result,
                            context.Operation == PluginOperation.Enable ? SeverityLevel.Error : SeverityLevel.Warning);
                    }

                    this.PluginRepository.StorePluginData(new PluginData(pluginData.Identity, PluginState.Enabled));

                    pluginData = this.GetInstalledPluginData(pluginIdentity);

                    return this.ToPlugin(pluginData);
                });

            this.Logger.Info("Plugin {plugin} successfully enabled. Elapsed: {elapsed:c}.", pluginIdentity, opResult.Elapsed);

            return result
                .MergeAll(opResult)
                .MergeMessage($"Plugin {pluginIdentity} successfully enabled. Elapsed: {opResult.Elapsed:c}.")
                .Complete(opResult.Elapsed);
        }

        /// <summary>
        /// Disables the plugin asynchronously if the plugin was previously enabled.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the enable operation result.
        /// </returns>
        public virtual async Task<IOperationResult<IPlugin>> DisablePluginAsync(AppIdentity pluginIdentity, Action<IPluginContext> options = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult<IPlugin>();
            var opResult = Profiler.WithStopwatch(
                () =>
                {
                    var context = this.CreatePluginContext(ctx => ctx.Merge(options).Operation(PluginOperation.Disable, overwrite: false));

                    var pluginData = this.GetInstalledPluginData(pluginIdentity);
                    pluginIdentity = pluginData.Identity;

                    if (!this.CanDisablePlugin(pluginData))
                    {
                        throw new PluginOperationException(
                            $"Plugin {pluginIdentity} cannot be disabled. State '{pluginData.State}'.",
                            result,
                            context.Operation == PluginOperation.Disable ? SeverityLevel.Error : SeverityLevel.Warning);
                    }

                    this.PluginRepository.StorePluginData(new PluginData(pluginData.Identity, PluginState.Disabled));

                    pluginData = this.GetInstalledPluginData(pluginIdentity);

                    return this.ToPlugin(pluginData);
                });

            this.Logger.Warn("Plugin {plugin} successfully disabled. Elapsed: {elapsed:c}.", pluginIdentity, opResult.Elapsed);

            return result
                .MergeAll(opResult)
                .MergeMessage($"Plugin {pluginIdentity} successfully disabled. Elapsed: {opResult.Elapsed:c}.")
                .Complete(opResult.Elapsed);
        }

        /// <summary>
        /// Updates the plugin asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the update operation result.
        /// </returns>
        public async Task<IOperationResult<IPlugin>> UpdatePluginAsync(AppIdentity pluginIdentity, Action<IPluginContext> options = null, CancellationToken cancellationToken = default)
        {
            IOperationResult<IPlugin> result = new OperationResult<IPlugin>();
            var context = this.CreatePluginContext(options)
                .Operation(PluginOperation.Update, overwrite: false)
                .PluginIdentity(pluginIdentity);
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    if (string.IsNullOrEmpty(pluginIdentity.Version))
                    {
                        throw new ArgumentNullException(nameof(pluginIdentity.Version), $"Please provide the version to which the plugin {pluginIdentity} should be updated.");
                    }

                    var pluginDataBefore = this.GetInstalledPluginData(pluginIdentity);

                    Action<IPluginContext> updateOptions = ctx =>
                        ctx.Merge(options)
                            .Operation(PluginOperation.Update, overwrite: false)
                            .PluginIdentity(pluginIdentity);

                    var uninstResult = await this.UninstallPluginAsync(pluginDataBefore.Identity, updateOptions, cancellationToken).PreserveThreadContext();
                    result.MergeAll(uninstResult);

                    var instResult = await this.InstallPluginAsync(pluginIdentity, updateOptions, cancellationToken).PreserveThreadContext();
                    result.MergeAll(instResult);

                    return instResult.ReturnValue;
                }).PreserveThreadContext();

            this.Logger.Warn("Plugin {plugin} successfully updated. Elapsed: {elapsed:c}.", pluginIdentity, opResult.Elapsed);

            return result
                .MergeAll(opResult)
                .MergeMessage($"Plugin {pluginIdentity} successfully updated. Elapsed: {opResult.Elapsed:c}.")
                .Complete(opResult.Elapsed);
        }

        /// <summary>
        /// Converts a plugin data to a plugin entity.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        /// <returns>
        /// Plugin data as an <see cref="IPlugin"/>.
        /// </returns>
        protected virtual IPlugin ToPlugin(PluginData pluginData)
        {
            return new Plugin(new PluginInfo(this.AppRuntime, this.PluginRepository, pluginData.Identity));
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
        /// <param name="pluginData">Information describing the plugin.</param>
        /// <returns>
        /// True if the plugin can be installed, false if not.
        /// </returns>
        protected virtual bool CanInstallPlugin(PluginData pluginData)
        {
            return pluginData.State == PluginState.None;
        }

        /// <summary>
        /// Determines whether the plugin can be initialized.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        /// <returns>
        /// True if the plugin can be initialized, false if not.
        /// </returns>
        protected virtual bool CanInitializePlugin(PluginData pluginData)
        {
            return pluginData.State == PluginState.PendingInitialization;
        }

        /// <summary>
        /// Determines whether the plugin can be uninitialized.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        /// <returns>
        /// True if the plugin can be uninitialized, false if not.
        /// </returns>
        protected virtual bool CanUninitializePlugin(PluginData pluginData)
        {
            return pluginData.State == PluginState.Disabled;
        }

        /// <summary>
        /// Determines whether the plugin can be uninstalled.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        /// <returns>
        /// True if the plugin can be uninstalled, false if not.
        /// </returns>
        protected virtual bool CanUninstallPlugin(PluginData pluginData)
        {
            return pluginData.State == PluginState.PendingUninstallation || pluginData.State == PluginState.Corrupt;
        }

        /// <summary>
        /// Determines whether the plugin can be enabled.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        /// <returns>
        /// True if the plugin can be enabled, false if not.
        /// </returns>
        protected virtual bool CanEnablePlugin(PluginData pluginData)
        {
            return pluginData.State == PluginState.Disabled;
        }

        /// <summary>
        /// Determines whether the plugin can be disabled.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        /// <returns>
        /// True if the plugin can be disabled, false if not.
        /// </returns>
        protected virtual bool CanDisablePlugin(PluginData pluginData)
        {
            return pluginData.State == PluginState.Enabled;
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
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <returns>
        /// The installed plugin data.
        /// </returns>
        protected virtual PluginData GetInstalledPluginData(AppIdentity pluginIdentity)
        {
            var pluginData = this.PluginRepository.GetPluginData(pluginIdentity);
            return pluginData;
        }

        /// <summary>
        /// Initializes the plugin data asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task<IOperationResult> InitializeDataAsync(AppIdentity pluginIdentity, IPluginContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<IOperationResult>(new OperationResult());
        }

        /// <summary>
        /// Uninitializes the plugin data asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task<IOperationResult> UninitializeDataAsync(AppIdentity pluginIdentity, IPluginContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<IOperationResult>(new OperationResult());
        }

        /// <summary>
        /// Installs the plugin asynchronously (core implementation).
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the plugin data.
        /// </returns>
        protected abstract Task<IOperationResult<IPlugin>> InstallPluginCoreAsync(AppIdentity pluginIdentity, IPluginContext context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uninstall the plugin asynchronously (core implementation).
        /// </summary>
        /// <param name="pluginIdentity">Identifier for the plugin.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task<IOperationResult<IPlugin>> UninstallPluginCoreAsync(AppIdentity pluginIdentity, IPluginContext context, CancellationToken cancellationToken)
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
