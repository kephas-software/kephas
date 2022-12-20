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
    using Kephas.Plugins.Transactions;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using Kephas.Versioning;

    /// <summary>
    /// A plugin manager base.
    /// </summary>
    public abstract class PluginManagerBase : Loggable, IPluginManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManagerBase"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="injectableFactory">The injectable factory.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        protected PluginManagerBase(
            IAppRuntime appRuntime,
            IInjectableFactory injectableFactory,
            IEventHub eventHub,
            ILogManager? logManager = null)
            : this(appRuntime, injectableFactory, eventHub, appRuntime.GetPluginStore(), logManager)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManagerBase"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="injectableFactory">The injectable factory.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="pluginStore">The plugin data store.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        protected PluginManagerBase(
            IAppRuntime appRuntime,
            IInjectableFactory injectableFactory,
            IEventHub eventHub,
            IPluginStore pluginStore,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.AppRuntime = appRuntime;
            this.InjectableFactory = injectableFactory;
            this.EventHub = eventHub;
            this.PluginStore = pluginStore;
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
        protected IInjectableFactory InjectableFactory { get; }

        /// <summary>
        /// Gets the event hub.
        /// </summary>
        /// <value>
        /// The event hub.
        /// </value>
        protected IEventHub EventHub { get; }

        /// <summary>
        /// Gets the plugin store.
        /// </summary>
        /// <value>
        /// The plugin store.
        /// </value>
        protected IPluginStore PluginStore { get; }

        /// <summary>
        /// Gets the available plugins asynchronously.
        /// </summary>
        /// <param name="filter">Optional. Specifies the filter.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the available plugins.
        /// </returns>
        public abstract Task<IOperationResult<IEnumerable<IAppInfo>>> GetAvailablePluginsAsync(Action<ISearchContext>? filter = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the installed plugins.
        /// </summary>
        /// <returns>
        /// An enumeration of installed plugins.
        /// </returns>
        public virtual IEnumerable<IPlugin> GetInstalledPlugins()
        {
            return this.AppRuntime.GetInstalledPlugins().Select(p => this.ToPlugin(p));
        }

        /// <summary>
        /// Gets the plugin state.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <returns>
        /// The plugin state.
        /// </returns>
        public virtual PluginState GetPluginState(AppIdentity pluginIdentity)
        {
            return this.PluginStore.GetPluginData(pluginIdentity, throwOnInvalid: false).State;
        }

        /// <summary>
        /// Downloads the plugin asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the download operation result.
        /// </returns>
        public virtual async Task<IOperationResult> DownloadPluginAsync(AppIdentity pluginIdentity, Action<IPluginContext>? options = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult();
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    using var downloadContext = this.CreatePluginContext(ctx => ctx.Merge(options)
                        .Operation(PluginOperation.Download, overwrite: false))
                        .PluginIdentity(pluginIdentity);

                    var downloadWrappedResult = await Profiler.WithStopwatchAsync(
                        () => this.DownloadPluginCoreAsync(pluginIdentity, downloadContext, cancellationToken)).PreserveThreadContext();
                    result.MergeAll(downloadWrappedResult.Value);

                    return downloadWrappedResult.Value.Value;
                }).PreserveThreadContext();

            this.Logger.Info("Plugin {plugin} successfully downloaded. Elapsed: {elapsed:c}.\n{messages}", pluginIdentity, opResult.Elapsed, result.Messages);

            return result
                .MergeAll(opResult)
                .MergeMessage($"Plugin {pluginIdentity} successfully downloaded. Elapsed: {opResult.Elapsed:c}.")
                .Complete(opResult.Elapsed);
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
        public virtual async Task<IOperationResult<IPlugin>> InstallPluginAsync(AppIdentity pluginIdentity, Action<IPluginContext>? options = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult<IPlugin>();
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    // set the plugin identity in the options with the value provided, which should include
                    // the version, too. Before installation, the version returned from the store is empty.
                    void InstallOptions(IPluginContext ctx) =>
                        ctx.Merge(options)
                            .Operation(PluginOperation.Install, overwrite: false)
                            .PluginIdentity(pluginIdentity);

                    var pluginData = this.GetInstalledPluginData(pluginIdentity);
                    pluginIdentity = pluginData.Identity;

                    var installComplete = false;
                    var initializeComplete = false;
                    using (var installContext = this.CreatePluginContext(options)
                            .Merge((Action<IPluginContext>)InstallOptions)
                            .PluginData(pluginData)
                            .Transaction(new InstallTransaction(pluginData)))
                    {
                        if (this.CanInstallPlugin(pluginData, installContext))
                        {
                            var installWrappedResult = await Profiler.WithStopwatchAsync(
                                        () => this.InstallPluginCoreAsync(pluginIdentity, installContext, cancellationToken))
                                .PreserveThreadContext();

                            var plugin = installWrappedResult.Value.Value;
                            pluginData = plugin.GetPluginData();
                            result
                                .Value(plugin)
                                .MergeMessages(installWrappedResult.Value)
                                .MergeMessage($"Plugin {pluginIdentity} successfully installed, awaiting initialization. Elapsed: {installWrappedResult.Elapsed:c}.");

                            this.SetPluginState(pluginData, PluginState.PendingInitialization);

                            pluginData = this.GetInstalledPluginData(pluginIdentity);
                            installComplete = pluginData.State == PluginState.PendingInitialization;

                            if (installComplete)
                            {
                                this.Logger.Info("Plugin {plugin} successfully installed, awaiting initialization. Elapsed: {elapsed:c}.\n{messages}", pluginIdentity, installWrappedResult.Elapsed, result.Messages);
                            }
                        }

                        installContext
                            .Plugin(result.Value)
                            .PluginData(pluginData);
                        if (this.CanInitializePlugin(pluginData, installContext))
                        {
                            try
                            {
                                result.MergeAll(
                                    await this.InitializePluginAsync(pluginIdentity, InstallOptions, cancellationToken)
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
        public virtual async Task<IOperationResult<IPlugin>> UninstallPluginAsync(AppIdentity pluginIdentity, Action<IPluginContext>? options = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult<IPlugin>();
            var uninstallComplete = false;
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    var pluginData = this.GetInstalledPluginData(pluginIdentity);
                    pluginIdentity = pluginData.Identity;

                    var plugin = this.ToPlugin(pluginData);
                    result.Value = plugin;

                    Action<IPluginContext> uninstallOptions = ctx =>
                        ctx.Merge(options)
                            .Operation(PluginOperation.Uninstall, overwrite: false)
                            .PluginIdentity(pluginIdentity)
                            .PluginData(pluginData);

                    var uninitializeComplete = false;
                    using var uninstallContext = this.InjectableFactory.Create<PluginContext>()
                        .Merge(uninstallOptions)
                        .Plugin(plugin)
                        .PluginData(pluginData)
                        .Transaction(new InstallTransaction(pluginData));
                    if (this.CanUninitializePlugin(pluginData, uninstallContext)
                        || this.CanPrepareUninitializationPlugin(pluginData, uninstallContext)
                        || this.CanDisablePlugin(pluginData, uninstallContext))
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

                    uninstallContext
                        .PluginData(pluginData)
                        .Plugin(result.Value);
                    if (this.CanUninstallPlugin(pluginData, uninstallContext))
                    {
                        var uninstWrappedResult = await Profiler.WithStopwatchAsync(
                                () => this.UninstallPluginCoreAsync(pluginIdentity, uninstallContext, cancellationToken))
                            .PreserveThreadContext();
                        result.MergeAll(uninstWrappedResult.Value);

                        try
                        {
                            this.SetPluginState(pluginData, PluginState.None);
                        }
                        catch (DirectoryNotFoundException)
                        {
                            // OK, plugin directory removed
                        }
                        catch (Exception ex)
                        {
                            result.MergeException(ex);
                        }

                        pluginData = this.GetInstalledPluginData(pluginIdentity);
                        uninstallComplete = pluginData.State == PluginState.None;

                        if (uninstallComplete)
                        {
                            this.Logger.Info("Plugin {plugin} successfully uninstalled. Elapsed: {elapsed:c}.\n{messages}", pluginIdentity, uninstWrappedResult.Elapsed, result.Messages);
                        }
                    }

                    if (!uninstallComplete && !uninitializeComplete)
                    {
                        throw new PluginOperationException($"Plugin {pluginIdentity} cannot be uninstalled. State: '{pluginData.State}'. Under certain circumstances it may be necessary to perform the uninstallation in multiple successive steps, for example when plugin binaries need to be unloaded first.", result);
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
        public virtual async Task<IOperationResult<IPlugin>> InitializePluginAsync(AppIdentity pluginIdentity, Action<IPluginContext>? options = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult<IPlugin>();
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    var pluginData = this.GetInstalledPluginData(pluginIdentity);
                    pluginIdentity = pluginData.Identity;

                    var plugin = this.ToPlugin(pluginData);
                    result.Value(plugin);

                    Action<IPluginContext> initializeOptions = ctx => ctx
                        .Merge(options)
                        .Operation(PluginOperation.Initialize, overwrite: false)
                        .PluginIdentity(pluginIdentity)
                        .Plugin(plugin);

                    using var initializeContext = this.CreatePluginContext(initializeOptions)
                        .PluginData(pluginData)
                        .Plugin(plugin);
                    if (!this.CanInitializePlugin(pluginData, initializeContext))
                    {
                        throw new PluginOperationException(
                            $"Plugin {pluginIdentity} cannot be initialized. State '{pluginData.State}'.",
                            result,
                            initializeContext.Operation == PluginOperation.Initialize ? SeverityLevel.Error : SeverityLevel.Warning);
                    }

                    var initResult = await this.EventHub.PublishAsync(new InitializingPluginSignal(pluginIdentity, initializeContext), initializeContext, cancellationToken).PreserveThreadContext();
                    if (initResult.HasErrors())
                    {
                        throw new PluginOperationException(initResult);
                    }

                    var initializeComplete = false;
                    try
                    {
                        var initWrappedResult = await Profiler.WithStopwatchAsync(
                                () => this.InitializePluginCoreAsync(pluginIdentity, initializeContext, cancellationToken))
                            .PreserveThreadContext();
                        result
                            .MergeAll(initWrappedResult.Value)
                            .Value(plugin);

                        this.SetPluginState(pluginData, PluginState.Disabled);

                        pluginData = this.GetInstalledPluginData(pluginIdentity);
                        initializeComplete = pluginData.State == PluginState.Disabled;
                        if (initializeComplete)
                        {
                            this.Logger.Info("Plugin {plugin} successfully initialized. Elapsed: {elapsed:c}.\n{messages}", pluginIdentity, initWrappedResult.Elapsed, result.Messages);
                        }
                    }
                    catch (Exception ex) when (ex is ISeverityQualifiedNotification qex && !qex.Severity.IsError())
                    {
                        // treat non-errors as ignorable, meaning that the plugin state is not changed to corrupt.
                        throw;
                    }
                    catch
                    {
                        this.SetPluginState(pluginData, PluginState.Corrupt);
                        throw;
                    }

                    initResult = await this.EventHub.PublishAsync(new InitializedPluginSignal(pluginIdentity, initializeContext, result), initializeContext, cancellationToken).PreserveThreadContext();
                    if (initResult.HasErrors())
                    {
                        throw new PluginOperationException(initResult);
                    }

                    initializeContext
                        .PluginData(pluginData)
                        .Plugin(result.Value);
                    if (this.CanEnablePlugin(pluginData, initializeContext))
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
        /// Prepares the uninitialization procedure of the plugin asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the prepare uninitialize operation result.
        /// </returns>
        public virtual async Task<IOperationResult<IPlugin>> PrepareUninitializePluginAsync(AppIdentity pluginIdentity, Action<IPluginContext>? options = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult<IPlugin>();
            var prepareUninitializeComplete = false;
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    var pluginData = this.GetInstalledPluginData(pluginIdentity);
                    pluginIdentity = pluginData.Identity;

                    void PrepareUninitializeOptions(IPluginContext ctx) =>
                        ctx.Merge(options)
                            .Operation(PluginOperation.PrepareUninitialization, overwrite: false)
                            .PluginIdentity(pluginIdentity);

                    using var prepareUninitializationContext = this.CreatePluginContext(PrepareUninitializeOptions)
                        .PluginData(pluginData);

                    if (this.CanDisablePlugin(pluginData, prepareUninitializationContext))
                    {
                        try
                        {
                            result.MergeAll(
                                await this.DisablePluginAsync(pluginIdentity, PrepareUninitializeOptions, cancellationToken)
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
                    result.Value(plugin);

                    prepareUninitializationContext
                        .PluginData(pluginData)
                        .Plugin(plugin);
                    if (!this.CanPrepareUninitializationPlugin(pluginData, prepareUninitializationContext))
                    {
                        throw new PluginOperationException(
                            $"Plugin {pluginIdentity} cannot be prepared for uninitialization. State '{pluginData.State}'.",
                            result,
                            prepareUninitializationContext.Operation == PluginOperation.PrepareUninitialization ? SeverityLevel.Error : SeverityLevel.Warning);
                    }

                    try
                    {
                        var uninitWrappedResult = await Profiler.WithStopwatchAsync(
                            () => this.PrepareUninitializePluginCoreAsync(pluginIdentity, prepareUninitializationContext, cancellationToken)).PreserveThreadContext();
                        result.MergeAll(uninitWrappedResult.Value);

                        this.SetPluginState(pluginData, PluginState.PendingUninitialization);

                        pluginData = this.GetInstalledPluginData(pluginIdentity);
                        prepareUninitializeComplete = pluginData.State == PluginState.PendingUninitialization;
                    }
                    catch (Exception ex) when (ex is ISeverityQualifiedNotification qex && !qex.Severity.IsError())
                    {
                        // treat non-errors as ignorable, meaning that the plugin state is not changed to corrupt.
                        throw;
                    }
                    catch
                    {
                        this.SetPluginState(pluginData, PluginState.Corrupt);
                        throw;
                    }

                    return plugin;
                }).PreserveThreadContext();

            this.Logger.Info("Plugin {plugin} successfully prepared for uninitialization, awaiting uninitialization. Elapsed: {elapsed:c}.\n{messages}", pluginIdentity, opResult.Elapsed, result.Messages);

            result
                .MergeAll(opResult)
                .MergeMessage($"Plugin {pluginIdentity} successfully prepared for uninitialization. Elapsed: {opResult.Elapsed:c}.")
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
        public virtual async Task<IOperationResult<IPlugin>> UninitializePluginAsync(AppIdentity pluginIdentity, Action<IPluginContext>? options = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult<IPlugin>();
            var uninitializeComplete = false;
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    var pluginData = this.GetInstalledPluginData(pluginIdentity);
                    pluginIdentity = pluginData.Identity;

                    void UninitializeOptions(IPluginContext ctx) =>
                        ctx.Merge(options)
                            .Operation(PluginOperation.Uninitialize, overwrite: false)
                            .PluginIdentity(pluginIdentity);

                    using var uninitializeContext = this.CreatePluginContext(UninitializeOptions)
                        .PluginData(pluginData);

                    if (this.CanPrepareUninitializationPlugin(pluginData, uninitializeContext)
                        || this.CanDisablePlugin(pluginData, uninitializeContext))
                    {
                        try
                        {
                            result.MergeAll(
                                await this.PrepareUninitializePluginAsync(pluginIdentity, UninitializeOptions, cancellationToken)
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
                    result.Value(plugin);

                    uninitializeContext
                        .PluginData(pluginData)
                        .Plugin(plugin);
                    if (!this.CanUninitializePlugin(pluginData, uninitializeContext))
                    {
                        throw new PluginOperationException(
                            $"Plugin {pluginIdentity} cannot be uninitialized. State '{pluginData.State}'.",
                            result,
                            uninitializeContext.Operation == PluginOperation.Uninitialize ? SeverityLevel.Error : SeverityLevel.Warning);
                    }

                    var uninitResult = await this.EventHub.PublishAsync(
                        new UninitializingPluginSignal(pluginIdentity, uninitializeContext),
                        uninitializeContext,
                        cancellationToken).PreserveThreadContext();
                    if (uninitResult.HasErrors())
                    {
                        throw new PluginOperationException(uninitResult);
                    }

                    try
                    {
                        var uninitWrappedResult = await Profiler.WithStopwatchAsync(
                            () => this.UninitializePluginCoreAsync(pluginIdentity, uninitializeContext, cancellationToken)).PreserveThreadContext();
                        result.MergeAll(uninitWrappedResult.Value);

                        this.SetPluginState(pluginData, PluginState.PendingUninstallation);

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
                        this.SetPluginState(pluginData, PluginState.Corrupt);
                        throw;
                    }

                    uninitResult = await this.EventHub.PublishAsync(
                        new UninitializedPluginSignal(pluginIdentity, uninitializeContext, result),
                        uninitializeContext,
                        cancellationToken).PreserveThreadContext();
                    if (uninitResult.HasErrors())
                    {
                        throw new PluginOperationException(uninitResult);
                    }

                    return plugin;
                }).PreserveThreadContext();

            this.Logger.Info("Plugin {plugin} successfully uninitialized, awaiting uninstallation. Elapsed: {elapsed:c}.\n{messages}", pluginIdentity, opResult.Elapsed, result.Messages);

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
        public virtual async Task<IOperationResult<IPlugin>> EnablePluginAsync(AppIdentity pluginIdentity, Action<IPluginContext>? options = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult<IPlugin>();
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    using var enableContext = this.CreatePluginContext(ctx => ctx.Merge(options)
                        .Operation(PluginOperation.Enable, overwrite: false));

                    var pluginData = this.GetInstalledPluginData(pluginIdentity);
                    pluginIdentity = pluginData.Identity;

                    enableContext.PluginData(pluginData);

                    if (!this.CanEnablePlugin(pluginData, enableContext))
                    {
                        throw new PluginOperationException(
                            $"Plugin {pluginIdentity} cannot be enabled. State '{pluginData.State}'.",
                            result,
                            enableContext.Operation == PluginOperation.Enable ? SeverityLevel.Error : SeverityLevel.Warning);
                    }

                    var enableWrappedResult = await Profiler.WithStopwatchAsync(
                        () => this.EnablePluginCoreAsync(pluginIdentity, enableContext, cancellationToken)).PreserveThreadContext();
                    result.MergeAll(enableWrappedResult.Value);

                    this.SetPluginState(pluginData, PluginState.Enabled);

                    pluginData = this.GetInstalledPluginData(pluginIdentity);

                    return this.ToPlugin(pluginData);
                }).PreserveThreadContext();

            this.Logger.Info("Plugin {plugin} successfully enabled. Elapsed: {elapsed:c}.\n{messages}", pluginIdentity, opResult.Elapsed, result.Messages);

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
        public virtual async Task<IOperationResult<IPlugin>> DisablePluginAsync(AppIdentity pluginIdentity, Action<IPluginContext>? options = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult<IPlugin>();
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    using var disableContext = this.CreatePluginContext(ctx => ctx.Merge(options)
                        .Operation(PluginOperation.Disable, overwrite: false));

                    var pluginData = this.GetInstalledPluginData(pluginIdentity);
                    pluginIdentity = pluginData.Identity;

                    if (!this.CanDisablePlugin(pluginData, disableContext))
                    {
                        throw new PluginOperationException(
                            $"Plugin {pluginIdentity} cannot be disabled. State '{pluginData.State}'.",
                            result,
                            disableContext.Operation == PluginOperation.Disable ? SeverityLevel.Error : SeverityLevel.Warning);
                    }

                    var disableWrappedResult = await Profiler.WithStopwatchAsync(
                        () => this.DisablePluginCoreAsync(pluginIdentity, disableContext, cancellationToken)).PreserveThreadContext();
                    result.MergeAll(disableWrappedResult.Value);

                    this.SetPluginState(pluginData, PluginState.Disabled);

                    pluginData = this.GetInstalledPluginData(pluginIdentity);

                    return this.ToPlugin(pluginData);
                }).PreserveThreadContext();

            this.Logger.Warn("Plugin {plugin} successfully disabled. Elapsed: {elapsed:c}.\n{messages}", pluginIdentity, opResult.Elapsed, result.Messages);

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
        public virtual async Task<IOperationResult<IPlugin>> UpdatePluginAsync(AppIdentity pluginIdentity, Action<IPluginContext>? options = null, CancellationToken cancellationToken = default)
        {
            IOperationResult<IPlugin> result = new OperationResult<IPlugin>();
            var context = this.CreatePluginContext(options)
                .Operation(PluginOperation.Update, overwrite: false)
                .PluginIdentity(pluginIdentity);
            var opResult = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    if (pluginIdentity.Version == null)
                    {
                        throw new ArgumentNullException(nameof(pluginIdentity.Version), $"Please provide the version to which the plugin {pluginIdentity} should be updated.");
                    }

                    // first try to download the plugin, and only if it succeeds then go on with uninstallation and then installation
                    var dlResult = await this.DownloadPluginAsync(
                        pluginIdentity,
                        ctx => ctx.Merge(options)
                                  .Operation(PluginOperation.Update)
                                  .PluginIdentity(pluginIdentity),
                        cancellationToken).PreserveThreadContext();
                    result.MergeAll(dlResult);

                    var pluginDataBefore = this.GetInstalledPluginData(new AppIdentity(pluginIdentity.Id));
                    this.SetUpdatingToVersion(pluginDataBefore, pluginIdentity.Version);

                    void UpdateOptions(IPluginContext ctx) =>
                        ctx.Merge(options)
                            .Operation(PluginOperation.Update, overwrite: false);

                    var uninstResult = await this.UninstallPluginAsync(
                        pluginDataBefore.Identity,
                        ctx => ctx.Merge((Action<IPluginContext>)UpdateOptions).PluginIdentity(pluginDataBefore.Identity),
                        cancellationToken).PreserveThreadContext();
                    result.MergeAll(uninstResult);

                    var instResult = await this.InstallPluginAsync(
                        pluginIdentity,
                        ctx => ctx.Merge((Action<IPluginContext>)UpdateOptions).PluginIdentity(pluginIdentity),
                        cancellationToken).PreserveThreadContext();
                    result.MergeAll(instResult);

                    return instResult.Value;
                }).PreserveThreadContext();

            this.Logger.Warn("Plugin {plugin} successfully updated. Elapsed: {elapsed:c}.\n{messages}", pluginIdentity, opResult.Elapsed, result.Messages);

            return result
                .MergeAll(opResult)
                .MergeMessage($"Plugin {pluginIdentity} successfully updated. Elapsed: {opResult.Elapsed:c}.")
                .Complete(opResult.Elapsed);
        }

        /// <summary>
        /// Converts a plugin data to a plugin entity.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        /// <param name="offline">Optional. True to indicate that the offline data is used by the plugin.</param>
        /// <returns>
        /// Plugin data as an <see cref="IPlugin"/>.
        /// </returns>
        protected virtual IPlugin ToPlugin(PluginData pluginData, bool offline = false)
        {
            var pluginInfo = new PluginInfo(this.AppRuntime, this.PluginStore, pluginData.Identity);
            return (Plugin)pluginInfo.CreateInstance(new object?[] { offline ? pluginData : null });
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
        /// <param name="context">The context.</param>
        /// <returns>
        /// True if the plugin can be installed, false if not.
        /// </returns>
        protected virtual bool CanInstallPlugin(PluginData pluginData, IPluginContext context)
        {
            return pluginData.State == PluginState.None;
        }

        /// <summary>
        /// Determines whether the plugin can be initialized.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// True if the plugin can be initialized, false if not.
        /// </returns>
        protected virtual bool CanInitializePlugin(PluginData pluginData, IPluginContext context)
        {
            return pluginData.State == PluginState.PendingInitialization;
        }

        /// <summary>
        /// Determines whether the plugin can be prepared for uninitialization.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// True if the plugin can be prepared for uninitialization, false if not.
        /// </returns>
        protected virtual bool CanPrepareUninitializationPlugin(PluginData pluginData, IPluginContext context)
        {
            return pluginData.State == PluginState.Disabled;
        }

        /// <summary>
        /// Determines whether the plugin can be uninitialized.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// True if the plugin can be uninitialized, false if not.
        /// </returns>
        protected virtual bool CanUninitializePlugin(PluginData pluginData, IPluginContext context)
        {
            return pluginData.State == PluginState.PendingUninitialization;
        }

        /// <summary>
        /// Determines whether the plugin can be uninstalled.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// True if the plugin can be uninstalled, false if not.
        /// </returns>
        protected virtual bool CanUninstallPlugin(PluginData pluginData, IPluginContext context)
        {
            return pluginData.State == PluginState.PendingUninstallation
                || pluginData.State == PluginState.PendingInitialization
                || pluginData.State == PluginState.Corrupt;
        }

        /// <summary>
        /// Determines whether the plugin can be enabled.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// True if the plugin can be enabled, false if not.
        /// </returns>
        protected virtual bool CanEnablePlugin(PluginData pluginData, IPluginContext context)
        {
            return pluginData.State == PluginState.Disabled;
        }

        /// <summary>
        /// Determines whether the plugin can be disabled.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// True if the plugin can be disabled, false if not.
        /// </returns>
        protected virtual bool CanDisablePlugin(PluginData pluginData, IPluginContext context)
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
        protected virtual IPluginContext CreatePluginContext(Action<IPluginContext>? options)
        {
            return this.InjectableFactory.Create<PluginContext>().Merge(options);
        }

        /// <summary>
        /// Creates the search context.
        /// </summary>
        /// <param name="filter">Specifies the filter.</param>
        /// <returns>
        /// The new search context.
        /// </returns>
        protected virtual ISearchContext CreateSearchContext(Action<ISearchContext>? filter)
        {
            return this.InjectableFactory.Create<SearchContext>().Merge(filter);
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
            var pluginData = this.PluginStore.GetPluginData(pluginIdentity);
            return pluginData;
        }

        /// <summary>
        /// Downloads the plugin asynchronously (core implementation).
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="context">Plugin context for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the download operation result.
        /// </returns>
        protected abstract Task<IOperationResult> DownloadPluginCoreAsync(AppIdentity pluginIdentity, IPluginContext context, CancellationToken cancellationToken);

        /// <summary>
        /// Enables the plugin asynchronously (core implementation).
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task<IOperationResult> EnablePluginCoreAsync(AppIdentity pluginIdentity, IPluginContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<IOperationResult>(new OperationResult().Complete());
        }

        /// <summary>
        /// Disables the plugin asynchronously (core implementation).
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task<IOperationResult> DisablePluginCoreAsync(AppIdentity pluginIdentity, IPluginContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<IOperationResult>(new OperationResult().Complete());
        }

        /// <summary>
        /// Initializes the plugin asynchronously (core implementation).
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task<IOperationResult> InitializePluginCoreAsync(AppIdentity pluginIdentity, IPluginContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<IOperationResult>(new OperationResult().Complete());
        }

        /// <summary>
        /// Prepares the plugin uninitialization asynchronously (core implementation).
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task<IOperationResult> PrepareUninitializePluginCoreAsync(AppIdentity pluginIdentity, IPluginContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<IOperationResult>(new OperationResult().Complete());
        }

        /// <summary>
        /// Uninitializes the plugin asynchronously (core implementation).
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task<IOperationResult> UninitializePluginCoreAsync(AppIdentity pluginIdentity, IPluginContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<IOperationResult>(new OperationResult().Complete());
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
        protected abstract Task<IOperationResult<IPlugin>> InstallPluginCoreAsync(AppIdentity pluginIdentity, IPluginContext context, CancellationToken cancellationToken);

        /// <summary>
        /// Uninstall the plugin asynchronously (core implementation).
        /// </summary>
        /// <param name="pluginIdentity">Identifier for the plugin.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task<IOperationResult<IPlugin>> UninstallPluginCoreAsync(AppIdentity pluginIdentity, IPluginContext context, CancellationToken cancellationToken)
        {
            var rollbackResult = await context.Transaction!.RollbackAsync(context, cancellationToken).PreserveThreadContext();

            var pluginFolder = context.Plugin!.Location;
            if (Directory.Exists(pluginFolder))
            {
                Directory.Delete(pluginFolder, recursive: true);
            }

            return new OperationResult<IPlugin>(context.Plugin)
                .MergeMessages(rollbackResult);
        }

        /// <summary>
        /// Ensures that the assemblies probing path for the provided plugin is added.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        protected virtual void EnsurePluginAssembliesProbingPathAdded(AppIdentity pluginIdentity)
        {
        }

        /// <summary>
        /// Ensures that the assemblies probing path for the provided plugin is removed.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        protected virtual void EnsurePluginAssembliesProbingPathRemoved(AppIdentity pluginIdentity)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the plugin state requires that binaries are loaded.
        /// </summary>
        /// <param name="state">The plugin state.</param>
        /// <returns>A value indicating whether the plugin state requires that binaries are loaded.</returns>
        private bool RequiresLoadedBinaries(PluginState state)
        {
            return state == PluginState.Enabled
                   || state == PluginState.PendingInitialization
                   || state == PluginState.PendingUninitialization;
        }

        private void SetPluginState(PluginData pluginData, PluginState state)
        {
            if (this.RequiresLoadedBinaries(state))
            {
                this.EnsurePluginAssembliesProbingPathAdded(pluginData.Identity);
            }
            else
            {
                this.EnsurePluginAssembliesProbingPathRemoved(pluginData.Identity);
            }

            if (state == PluginState.None)
            {
                this.PluginStore.RemovePluginData(pluginData.ChangeState(state));
            }
            else
            {
                this.PluginStore.StorePluginData(pluginData.ChangeState(state));
            }
        }

        private void SetUpdatingToVersion(PluginData pluginData, SemanticVersion version)
        {
            this.PluginStore.StorePluginData(pluginData.ChangeUpdatingToVersion(version));
        }
    }
}
