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
    using Kephas.Application.Reflection;
    using Kephas.Composition;
    using Kephas.Diagnostics;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Events;
    using Kephas.Operations;
    using Kephas.Orchestration.Application;
    using Kephas.Orchestration.Endpoints;
    using Kephas.Orchestration.Interaction;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The default orchestration manager.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultOrchestrationManager : Loggable, IOrchestrationManager, IAsyncInitializable, IAsyncFinalizable
    {
        private readonly IExportFactory<IProcessStarterFactory> processStarterFactoryFactory;

        private Timer timer;
        private IEventSubscription appStartedSubscription;
        private IEventSubscription appStoppedSubscription;
        private IEventSubscription appHeartbeatSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultOrchestrationManager"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="processStarterFactoryFactory">Factory for the process starter factory.</param>
        public DefaultOrchestrationManager(
            IAppRuntime appRuntime,
            IEventHub eventHub,
            IMessageBroker messageBroker,
            IMessageProcessor messageProcessor,
            IExportFactory<IProcessStarterFactory> processStarterFactoryFactory)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));
            Requires.NotNull(eventHub, nameof(eventHub));
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(messageProcessor, nameof(messageProcessor));
            Requires.NotNull(processStarterFactoryFactory, nameof(processStarterFactoryFactory));

            this.AppRuntime = appRuntime;
            this.EventHub = eventHub;
            this.MessageBroker = messageBroker;
            this.MessageProcessor = messageProcessor;
            this.processStarterFactoryFactory = processStarterFactoryFactory;
        }

        /// <summary>
        /// Gets a context for the application.
        /// </summary>
        /// <value>
        /// The application context.
        /// </value>
        public IContext AppContext { get; private set; }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        public IAppRuntime AppRuntime { get; private set; }

        /// <summary>
        /// Gets the event hub.
        /// </summary>
        /// <value>
        /// The event hub.
        /// </value>
        public IEventHub EventHub { get; }

        /// <summary>
        /// Gets the message broker.
        /// </summary>
        /// <value>
        /// The message broker.
        /// </value>
        public IMessageBroker MessageBroker { get; }

        /// <summary>
        /// Gets the message processor.
        /// </summary>
        /// <value>
        /// The message processor.
        /// </value>
        public IMessageProcessor MessageProcessor { get; }

        /// <summary>
        /// Gets or sets the timer due time.
        /// </summary>
        /// <value>
        /// The timer due time.
        /// </value>
        protected internal TimeSpan TimerDueTime { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Gets or sets the timer period.
        /// </summary>
        /// <value>
        /// The timer period.
        /// </value>
        protected internal TimeSpan TimerPeriod { get; set; } = TimeSpan.FromSeconds(10);

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
        public virtual async Task InitializeAsync(IContext context, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(context, nameof(context));

            this.AppContext = context;

            await this.InitializeLiveAppsAsync(cancellationToken).PreserveThreadContext();

            this.appStartedSubscription = this.EventHub.Subscribe<AppStartedEvent>(this.OnAppStartedAsync);
            this.appStoppedSubscription = this.EventHub.Subscribe<AppStoppedEvent>(this.OnAppStoppedAsync);
            this.appHeartbeatSubscription = this.EventHub.Subscribe<AppHeartbeatEvent>(this.OnAppHeartbeatAsync);
        }

        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">The context for finalization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public virtual Task FinalizeAsync(IContext context, CancellationToken cancellationToken = default)
        {
            if (this.timer == null)
            {
                // not properly initialized, possibly abnormal program termination.
                return TaskHelper.CompletedTask;
            }

            this.timer.Dispose();

            this.appStartedSubscription?.Dispose();
            this.appStoppedSubscription?.Dispose();
            this.appHeartbeatSubscription?.Dispose();

            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// Gets the live apps asynchronously.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the live apps.
        /// </returns>
        public virtual Task<IEnumerable<IRuntimeAppInfo>> GetLiveAppsAsync(Action<IContext> optionsConfig = null, CancellationToken cancellationToken = default)
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
        public virtual async Task<IOperationResult> StartAppAsync(IAppInfo appInfo, IExpando arguments, Action<IContext> optionsConfig = null, CancellationToken cancellationToken = default)
        {
            var processedArguments = this.GetAppExecutableArgs(appInfo);
            var executablePath = this.GetAppExecutablePath(appInfo);

            var processStarter = this.CreateProcessStarterFactory(appInfo, arguments, optionsConfig)
                .WithManagedExecutable(executablePath)
                .WithArguments(processedArguments.ToArray())
                .WithWorkingDirectory(this.AppRuntime.GetAppLocation())
                .CreateProcessStarter();

            var processStartResult = await processStarter.StartAsync(cancellationToken: cancellationToken).PreserveThreadContext();
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
        public virtual async Task<IOperationResult> StopAppAsync(IRuntimeAppInfo runtimeAppInfo, Action<IContext> optionsConfig = null, CancellationToken cancellationToken = default)
        {
            this.Logger.Info($"Stopping application {runtimeAppInfo.AppInstanceId}...");

            var stopMessage = this.CreateStopAppMessage(runtimeAppInfo);

            try
            {
                if (runtimeAppInfo.AppId == this.AppRuntime.GetAppId() && runtimeAppInfo.AppInstanceId == this.AppRuntime.GetAppInstanceId())
                {
                    // terminate this instance
                    var response = await this.MessageProcessor.ProcessAsync(
                        stopMessage,
                        ctx => ctx.Impersonate(this.AppContext),
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
                            ctx => ctx.Impersonate(this.AppContext),
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
        /// Tries to get the OS process.
        /// </summary>
        /// <param name="runtimeAppInfo">Information describing the runtime application.</param>
        /// <returns>
        /// The OS process.
        /// </returns>
        protected virtual Process TryGetProcess(IRuntimeAppInfo runtimeAppInfo)
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
            return TaskHelper.CompletedTask;
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
        protected virtual Task OnAppStartedAsync(AppStartedEvent appEvent, IContext context, CancellationToken cancellationToken)
        {
            this.Logger.Info($"App started: {appEvent?.AppInfo}.");

            var appKey = this.GetAppKey(appEvent?.AppInfo);
            if (appKey == null)
            {
                return TaskHelper.CompletedTask;
            }

            if (appEvent.AppInfo.AppId == this.AppRuntime.GetAppId() && appEvent.AppInfo.AppInstanceId == this.AppRuntime.GetAppInstanceId())
            {
                this.timer = new Timer(this.OnHeartbeat, context, this.TimerDueTime, this.TimerPeriod);
            }

            this.LiveApps.AddOrUpdate(appKey, appEvent, (_, ai) => appEvent);
            return TaskHelper.CompletedTask;
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
        protected virtual Task OnAppStoppedAsync(AppStoppedEvent appEvent, IContext context, CancellationToken cancellationToken)
        {
            this.Logger.Info($"App stopped: {appEvent?.AppInfo}.");

            var appKey = this.GetAppKey(appEvent?.AppInfo);
            if (appKey == null)
            {
                return TaskHelper.CompletedTask;
            }

            this.LiveApps.TryRemove(appKey, out _);
            return TaskHelper.CompletedTask;
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
        protected virtual Task OnAppHeartbeatAsync(AppHeartbeatEvent appEvent, IContext context, CancellationToken cancellationToken)
        {
            var appKey = this.GetAppKey(appEvent?.AppInfo);
            if (appKey == null)
            {
                return TaskHelper.CompletedTask;
            }

            this.LiveApps.AddOrUpdate(appKey, appEvent, (_, ai) => appEvent);
            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// Gets the application key in the live apps collection.
        /// </summary>
        /// <param name="appInfo">Information describing the application.</param>
        /// <returns>
        /// The application key.
        /// </returns>
        protected virtual string GetAppKey(IRuntimeAppInfo appInfo)
        {
            if (appInfo == null || (appInfo.AppId == null && appInfo.AppInstanceId == null))
            {
                return null;
            }

            return $"{appInfo.AppId}/{appInfo.AppInstanceId}";
        }

        /// <summary>
        /// Gets the app executable path.
        /// </summary>
        /// <param name="appInfo">Information describing the application.</param>
        /// <returns>
        /// The app executable path.
        /// </returns>
        protected virtual string GetAppExecutablePath(IAppInfo appInfo)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            return entryAssembly.Location;
        }

        /// <summary>
        /// Gets the app executable arguments.
        /// </summary>
        /// <param name="appInfo">Information describing the application.</param>
        /// <returns>
        /// The app executable arguments.
        /// </returns>
        protected virtual IEnumerable<string> GetAppExecutableArgs(IAppInfo appInfo)
        {
            yield break;
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
        protected virtual IProcessStarterFactory CreateProcessStarterFactory(IAppInfo appInfo, IExpando arguments, Action<IContext> optionsConfig = null)
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
                AppInfo = this.AppRuntime.GetAppInfo(),
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
    }
}