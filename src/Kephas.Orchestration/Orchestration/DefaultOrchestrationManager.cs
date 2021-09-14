// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultOrchestrationManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default orchestration manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Configuration;
    using Kephas.Application.Reflection;
    using Kephas.Collections;
    using Kephas.Commands;
    using Kephas.Composition;
    using Kephas.Configuration;
    using Kephas.Diagnostics;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Operations;
    using Kephas.Orchestration.Application;
    using Kephas.Orchestration.Configuration;
    using Kephas.Orchestration.Diagnostics;
    using Kephas.Orchestration.Endpoints;
    using Kephas.Orchestration.Interaction;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The default orchestration manager.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultOrchestrationManager : Loggable, IOrchestrationManager, IAsyncInitializable, IAsyncFinalizable
    {
        private readonly IExportFactory<IProcessStarterFactory> processStarterFactoryFactory;
        private readonly IHostInfoProvider hostInfoProvider;
        private Timer heartbeatTimer;
        private IEventSubscription appStartedSubscription;
        private IEventSubscription appStoppedSubscription;
        private IEventSubscription appHeartbeatSubscription;
        private readonly Lazy<string> lazyRootAppInstanceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultOrchestrationManager"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="processStarterFactoryFactory">Factory for the process starter factory.</param>
        /// <param name="configuration">The orchestration configuration.</param>
        /// <param name="hostInfoProvider">The host information provider.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        public DefaultOrchestrationManager(
            IAppRuntime appRuntime,
            IEventHub eventHub,
            IMessageBroker messageBroker,
            IMessageProcessor messageProcessor,
            IExportFactory<IProcessStarterFactory> processStarterFactoryFactory,
            IConfiguration<OrchestrationSettings> configuration,
            IHostInfoProvider hostInfoProvider,
            ILogManager? logManager = null)
            : base(logManager)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));
            Requires.NotNull(eventHub, nameof(eventHub));
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(messageProcessor, nameof(messageProcessor));
            Requires.NotNull(processStarterFactoryFactory, nameof(processStarterFactoryFactory));
            Requires.NotNull(configuration, nameof(configuration));
            Requires.NotNull(hostInfoProvider, nameof(hostInfoProvider));

            this.AppRuntime = appRuntime;
            this.EventHub = eventHub;
            this.MessageBroker = messageBroker;
            this.MessageProcessor = messageProcessor;
            this.Configuration = configuration;
            this.hostInfoProvider = hostInfoProvider;
            this.processStarterFactoryFactory = processStarterFactoryFactory;

            this.HeartbeatDueTime = this.HeartbeatInterval = this.Configuration.GetSettings().HeartbeatInterval;

            this.lazyRootAppInstanceId = new Lazy<string>(this.ComputeRootAppInstanceId);
        }

        /// <summary>
        /// Gets or sets the heartbeat timer due time.
        /// </summary>
        /// <value>
        /// The timer heartbeat  due time.
        /// </value>
        protected internal TimeSpan HeartbeatDueTime { get; set; }

        /// <summary>
        /// Gets or sets the heartbeat timer period.
        /// </summary>
        /// <value>
        /// The heartbeat timer period.
        /// </value>
        protected internal TimeSpan HeartbeatInterval { get; set; }

        /// <summary>
        /// Gets a context for the application.
        /// </summary>
        /// <value>
        /// The application context.
        /// </value>
        protected IAppContext AppContext { get; private set; }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        protected IAppRuntime AppRuntime { get; private set; }

        /// <summary>
        /// Gets the event hub.
        /// </summary>
        /// <value>
        /// The event hub.
        /// </value>
        protected IEventHub EventHub { get; }

        /// <summary>
        /// Gets the message broker.
        /// </summary>
        /// <value>
        /// The message broker.
        /// </value>
        protected IMessageBroker MessageBroker { get; }

        /// <summary>
        /// Gets the message processor.
        /// </summary>
        /// <value>
        /// The message processor.
        /// </value>
        protected IMessageProcessor MessageProcessor { get; }

        /// <summary>
        /// Gets the orchestration configuration.
        /// </summary>
        protected IConfiguration<OrchestrationSettings> Configuration { get; }

        /// <summary>
        /// Gets the live apps cache.
        /// </summary>
        /// <value>
        /// The live apps cache.
        /// </value>
        protected ConcurrentDictionary<string, IAppEvent> LiveApps { get; } = new ConcurrentDictionary<string, IAppEvent>();

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">A context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        public virtual async Task InitializeAsync(IContext? context, CancellationToken cancellationToken = default)
        {
            if (context is IAppContext appContext)
            {
                this.AppContext = appContext;
            }
            else
            {
                throw new ArgumentException($"Expected an instance of {nameof(IAppContext)}.", nameof(context));
            }

            await this.InitializeLiveAppsAsync(cancellationToken).PreserveThreadContext();

            this.appStartedSubscription = this.EventHub.Subscribe<AppStartedEvent>(this.HandleAppStartedAsync);
            this.appStoppedSubscription = this.EventHub.Subscribe<AppStoppedEvent>(this.HandleAppStoppedAsync);
            this.appHeartbeatSubscription = this.EventHub.Subscribe<AppHeartbeatEvent>(this.HandleAppHeartbeatAsync);

            this.Logger.Debug("The orchestration manager is initialized.");
        }

        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">The context for finalization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public virtual Task FinalizeAsync(IContext? context, CancellationToken cancellationToken = default)
        {
            if (this.heartbeatTimer == null)
            {
                // not properly initialized, possibly abnormal program termination.
                return Task.CompletedTask;
            }

            this.heartbeatTimer.Dispose();

            this.appStartedSubscription?.Dispose();
            this.appStoppedSubscription?.Dispose();
            this.appHeartbeatSubscription?.Dispose();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the root application instance ID.
        /// </summary>
        /// <returns>The root application instance ID.</returns>
        public virtual string GetRootAppInstanceId() => this.lazyRootAppInstanceId.Value;

        /// <summary>
        /// Gets the application settings for the provided application.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <returns>The application settings.</returns>
        public AppSettings? GetAppSettings(string appId)
        {
            var systemSettings = this.Configuration.GetSettings();
            return systemSettings.Instances.TryGetValue(appId);
        }

        /// <summary>
        /// Gets the live apps asynchronously.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the live apps.
        /// </returns>
        public virtual Task<IEnumerable<IRuntimeAppInfo>> GetLiveAppsAsync(Action<IContext>? optionsConfig = null, CancellationToken cancellationToken = default)
        {
            var apps = this.LiveApps.Values.Select(v => v.AppInfo).ToArray();
            return Task.FromResult<IEnumerable<IRuntimeAppInfo>>(apps);
        }

        /// <summary>
        /// Starts the application asynchronously.
        /// </summary>
        /// <param name="appInfo">Information describing the application.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields an operation result.
        /// </returns>
        public virtual async Task<IOperationResult> StartAppAsync(IAppInfo appInfo, IDynamic arguments, Action<IContext>? optionsConfig = null, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            var appSettings = appInfo[nameof(AppSettings)] as AppSettings;
            var processedArguments = this.GetAppExecutableArgs(appInfo, arguments).ToCommandArgs();
            var (executableFile, runtime) = this.GetAppExecutableInfo(appInfo);

            var processStarterFactory = this.CreateProcessStarterFactory(appInfo, arguments, optionsConfig)
                .WithManagedExecutable(executableFile, runtime)
                .WithArguments(processedArguments.ToArray())
                .WithWorkingDirectory(this.AppRuntime.GetAppLocation());

            var envVariables = appSettings?.EnvironmentVariables?.ToDictionary();
            envVariables?.ForEach(envVar => processStarterFactory.WithEnvironmentVariable(envVar.Key, envVar.Value?.ToString() ?? string.Empty));

            var processStarter = processStarterFactory.CreateProcessStarter();

            await this.EventHub.PublishAsync(new AppStartingEvent { AppInfo = appInfo.GetRuntimeAppInfo() }, this.AppContext, cancellationToken).PreserveThreadContext();

            var processStartResult = await processStarter.StartAsync(cancellationToken: cancellationToken).PreserveThreadContext();
            processStartResult[nameof(AppInfo)] = appInfo;
            return processStartResult;
        }

        /// <summary>
        /// Stops a running application asynchronously.
        /// </summary>
        /// <param name="runtimeAppInfo">Information describing the runtime application.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields an operation result.
        /// </returns>
        public virtual async Task<IOperationResult> StopAppAsync(IRuntimeAppInfo runtimeAppInfo, Action<IContext>? optionsConfig = null, CancellationToken cancellationToken = default)
        {
            this.Logger.Info("Stopping application {appInstanceId}...", runtimeAppInfo.AppInstanceId);

            var stopMessage = this.CreateStopAppMessage(runtimeAppInfo);

            try
            {
                if (runtimeAppInfo.AppId == this.AppRuntime.GetAppId() && runtimeAppInfo.AppInstanceId == this.AppRuntime.GetAppInstanceId())
                {
                    // terminate this instance
                    var response = await this.MessageProcessor.ProcessAsync(
                        stopMessage,
                        ctx => ctx.Merge(optionsConfig),
                        cancellationToken).PreserveThreadContext();
                    return new OperationResult
                    {
                        OperationState = OperationState.Completed,
                        [nameof(StopAppResponseMessage)] = response,
                    };
                }
                else
                {
                    // terminate a remote application
                    var stopped = false;
                    using (var sub = this.EventHub.Subscribe<AppStoppedEvent>((e, ctx) => { if (e.AppInfo.AppInstanceId == runtimeAppInfo.AppInstanceId) stopped = true; }))
                    {
                        // initiate stop
                        await this.MessageBroker.ProcessOneWayAsync(
                            stopMessage,
                            new Endpoint(appId: runtimeAppInfo.AppId, appInstanceId: runtimeAppInfo.AppInstanceId),
                            ctx => ctx.Merge(optionsConfig),
                            cancellationToken).PreserveThreadContext();

                        // wait for the notification or process termination
                        const int delayms = 100;
                        var elapsed = TimeSpan.Zero;
                        var timeout = TaskHelper.DefaultTimeout;
                        var process = this.TryGetProcess(runtimeAppInfo);
                        while (!stopped && elapsed < timeout && !(process?.HasExited ?? false))
                        {
                            await Task.Delay(delayms).PreserveThreadContext();
                        }
                    }

                    return new OperationResult
                    {
                        OperationState = stopped ? OperationState.Completed : OperationState.InProgress,
                        [nameof(StopAppResponseMessage)] = stopped ? new StopAppResponseMessage { ProcessId = runtimeAppInfo.ProcessId } : null,
                    };
                }
            }
            catch (Exception ex)
            {
                // TODO localization
                this.Logger.Error(ex, $"Error while trying to stop application {runtimeAppInfo}.");
                return new OperationResult
                {
                    OperationState = OperationState.Failed,
                }.MergeException(ex);
            }
        }

        /// <summary>
        /// Gets the environment in which the application is running.
        /// </summary>
        /// <returns>The environment name.</returns>
        protected virtual string? GetEnvironment()
        {
            this.EnsureInitialized();

            return this.AppContext.AppArgs.Environment;
        }

        /// <summary>
        /// Tries to get the OS process.
        /// </summary>
        /// <param name="runtimeAppInfo">Information describing the runtime application.</param>
        /// <returns>
        /// The OS process.
        /// </returns>
        protected virtual Process? TryGetProcess(IRuntimeAppInfo runtimeAppInfo)
        {
            try
            {
                return runtimeAppInfo.ProcessId != 0 ? Process.GetProcessById(runtimeAppInfo.ProcessId) : null;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, $"Could not get the process with ID: {runtimeAppInfo.ProcessId}.");
                return null;
            }
        }

        /// <summary>
        /// Initializes the live apps cache asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task InitializeLiveAppsAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes the heartbeat timer action.
        /// </summary>
        /// <param name="state">The state.</param>
        protected virtual async void OnHeartbeat(object state)
        {
            try
            {
                var heartbeatEvent = this.CreateAppHeartbeatEvent();
                await this.MessageBroker.PublishAsync(heartbeatEvent, ctx => ctx.Impersonate((IContext)state)).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Exception on application heartbeat.");
            }
        }

        /// <summary>
        /// Callback invoked when an application was started.
        /// </summary>
        /// <param name="appEvent">The application event.</param>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task HandleAppStartedAsync(AppStartedEvent appEvent, IContext? context, CancellationToken cancellationToken)
        {
            this.Logger.Info("App started: {appInstance}.", appEvent.AppInfo);

            var appKey = this.GetAppKey(appEvent.AppInfo);
            if (appKey == null)
            {
                return Task.CompletedTask;
            }

            if (appEvent.AppInfo.AppId == this.AppRuntime.GetAppId() && appEvent.AppInfo.AppInstanceId == this.AppRuntime.GetAppInstanceId())
            {
                if (this.heartbeatTimer == null)
                {
                    this.heartbeatTimer = new Timer(this.OnHeartbeat, context, this.HeartbeatDueTime, this.HeartbeatInterval);
                }
            }

            this.LiveApps.AddOrUpdate(appKey, appEvent, (_, ai) => appEvent);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Callback invoked when an application was stopped.
        /// </summary>
        /// <param name="appEvent">The application event.</param>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task HandleAppStoppedAsync(AppStoppedEvent appEvent, IContext? context, CancellationToken cancellationToken)
        {
            this.Logger.Info("App stopped: {appInstance}.", appEvent?.AppInfo);

            var appKey = this.GetAppKey(appEvent?.AppInfo);
            if (appKey == null)
            {
                return Task.CompletedTask;
            }

            this.LiveApps.TryRemove(appKey, out _);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Callback invoked when an application notified its heartbeat.
        /// </summary>
        /// <param name="appEvent">The application event.</param>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task HandleAppHeartbeatAsync(AppHeartbeatEvent appEvent, IContext? context, CancellationToken cancellationToken)
        {
            var appKey = this.GetAppKey(appEvent?.AppInfo);
            if (appKey == null)
            {
                return Task.CompletedTask;
            }

            this.LiveApps.AddOrUpdate(appKey, appEvent, (_, ai) => appEvent);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the application key in the live apps collection.
        /// </summary>
        /// <param name="appInfo">Information describing the application.</param>
        /// <returns>
        /// The application key.
        /// </returns>
        protected virtual string? GetAppKey(IRuntimeAppInfo? appInfo)
        {
            if (appInfo == null || (appInfo.AppId == null && appInfo.AppInstanceId == null))
            {
                return null;
            }

            return $"{appInfo.AppId}/{appInfo.AppInstanceId}";
        }

        /// <summary>
        /// Gets the app executable runtime and entry module path.
        /// </summary>
        /// <param name="appInfo">Information describing the application.</param>
        /// <returns>
        /// The app executable runtime and entry module path.
        /// </returns>
        protected virtual (string executablePath, string? runtime) GetAppExecutableInfo(IAppInfo appInfo)
        {
            var entryAssemblyLocation = Assembly.GetEntryAssembly().Location;
            var currentProcess = Process.GetCurrentProcess();
            return RuntimeEnvironment.FrameworkName switch
            {
                RuntimeEnvironment.NetRuntime or RuntimeEnvironment.NetCoreRuntime
                    => currentProcess.ProcessName == "dotnet"
                        ? (entryAssemblyLocation, currentProcess.ProcessName)
                        : (currentProcess.MainModule.FileName, null),
                RuntimeEnvironment.MonoRuntime
                    => (entryAssemblyLocation, currentProcess.ProcessName),
                _ => (entryAssemblyLocation, null),
            };
        }

        /// <summary>
        /// Gets the app executable arguments.
        /// </summary>
        /// <param name="appInfo">Information describing the application.</param>
        /// <param name="arguments">The executable arguments.</param>
        /// <returns>
        /// The app executable arguments.
        /// </returns>
        protected virtual IArgs GetAppExecutableArgs(IAppInfo appInfo, IDynamic arguments)
        {
            var appArgs = new Args
            {
                [AppArgs.AppIdArgName] = appInfo.Identity.Id,
                [AppArgs.AppInstanceIdArgName] = appInfo[AppRuntimeBase.AppInstanceIdKey],
                [AppArgs.RootArgName] = this.GetRootAppInstanceId(),
                [AppArgs.EnvArgName] = this.GetEnvironment(),
            }.Merge(arguments);

            return appArgs;
        }

        /// <summary>
        /// Creates a new process starter factory.
        /// </summary>
        /// <param name="appInfo">Information describing the application.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The new process starter factory.
        /// </returns>
        protected virtual IProcessStarterFactory CreateProcessStarterFactory(IAppInfo appInfo, IDynamic arguments, Action<IContext>? optionsConfig = null)
            => this.processStarterFactoryFactory.CreateExportedValue();

        /// <summary>
        /// Creates application heartbeat event.
        /// </summary>
        /// <returns>
        /// The new application heartbeat event.
        /// </returns>
        protected virtual AppHeartbeatEvent CreateAppHeartbeatEvent()
        {
            return new AppHeartbeatEvent
            {
                AppInfo = this.hostInfoProvider.GetRuntimeAppInfo(),
                Timestamp = DateTimeOffset.Now,
            };
        }

        /// <summary>
        /// Creates stop application message.
        /// </summary>
        /// <param name="runtimeAppInfo">Information describing the runtime application.</param>
        /// <returns>
        /// The new stop application message.
        /// </returns>
        protected virtual StopAppMessage CreateStopAppMessage(IRuntimeAppInfo runtimeAppInfo)
        {
            return new StopAppMessage
            {
                AppId = runtimeAppInfo.AppId,
                AppInstanceId = runtimeAppInfo.AppInstanceId,
            };
        }

        /// <summary>
        /// Computes the ID of the root application instance.
        /// </summary>
        /// <returns>The root application instance ID.</returns>
        protected virtual string ComputeRootAppInstanceId()
        {
            this.EnsureInitialized();

            if (this.AppRuntime.IsRoot())
            {
                return this.AppRuntime.GetAppInstanceId()!;
            }

            var rootId = this.AppContext.AppArgs.RootAppInstanceId;
            if (!string.IsNullOrEmpty(rootId))
            {
                return rootId!;
            }

            throw new OrchestrationException($"Cannot identify the ID of the root application instance. Possible resolution: add '-{AppArgs.RootArgName} <root-instance-id>' to the application startup arguments.");
        }

        /// <summary>
        /// Checks whether the service is initialized and, if not, throws an <see cref="ServiceNotInitializedException"/>.
        /// </summary>
        /// <exception cref="ServiceNotInitializedException">The service is not initialized. Consider invoking the <see cref="InitializeAsync"/> method.</exception>
        protected void EnsureInitialized()
        {
            if (this.AppContext == null)
            {
                throw new ServiceNotInitializedException($"The {this.GetType().Name} service is not initialized.");
            }
        }
    }
}