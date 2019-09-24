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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Reflection;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Events;
    using Kephas.Operations;
    using Kephas.Orchestration.Application;
    using Kephas.Orchestration.Endpoints;
    using Kephas.Orchestration.Resources;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The default orchestration manager.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultOrchestrationManager : Loggable, IOrchestrationManager, IAsyncInitializable, IAsyncFinalizable
    {
        /// <summary>
        /// The application manifest.
        /// </summary>
        private readonly IAppManifest appManifest;

        /// <summary>
        /// The application runtime.
        /// </summary>
        private readonly IAppRuntime appRuntime;

        /// <summary>
        /// The event hub.
        /// </summary>
        private readonly IEventHub eventHub;

        /// <summary>
        /// The message broker.
        /// </summary>
        private readonly IMessageBroker messageBroker;

        /// <summary>
        /// The live apps.
        /// </summary>
        private readonly ConcurrentDictionary<string, IAppEvent> liveApps = new ConcurrentDictionary<string, IAppEvent>();

        /// <summary>
        /// The timer.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// The application started subscription.
        /// </summary>
        private IEventSubscription appStartedSubscription;

        /// <summary>
        /// The application stopped subscription.
        /// </summary>
        private IEventSubscription appStoppedSubscription;

        /// <summary>
        /// The application heartbeat subscription.
        /// </summary>
        private IEventSubscription appHeartbeatSubscription;
        private IContext appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultOrchestrationManager"/> class.
        /// </summary>
        /// <param name="appManifest">The application manifest.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="messageBroker">The message broker.</param>
        public DefaultOrchestrationManager(
            IAppManifest appManifest,
            IAppRuntime appRuntime,
            IEventHub eventHub,
            IMessageBroker messageBroker)
        {
            Requires.NotNull(appManifest, nameof(appManifest));
            Requires.NotNull(appRuntime, nameof(appRuntime));
            Requires.NotNull(eventHub, nameof(eventHub));
            Requires.NotNull(messageBroker, nameof(messageBroker));

            this.appManifest = appManifest;
            this.appRuntime = appRuntime;
            this.eventHub = eventHub;
            this.messageBroker = messageBroker;
        }

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
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">A context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        public async Task InitializeAsync(IContext context, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(context, nameof(context));

            this.appContext = context;

            await this.InitializeLiveAppInfosAsync(this.liveApps, cancellationToken).PreserveThreadContext();

            this.appStartedSubscription = this.eventHub.Subscribe<AppStartedEvent>(this.OnAppStartedAsync);
            this.appStoppedSubscription = this.eventHub.Subscribe<AppStoppedEvent>(this.OnAppStoppedAsync);
            this.appHeartbeatSubscription = this.eventHub.Subscribe<AppHeartbeatEvent>(this.OnAppHeartbeatAsync);
        }

        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">The context for finalization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public Task FinalizeAsync(IContext context, CancellationToken cancellationToken = default)
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
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the live apps.
        /// </returns>
        public virtual async Task<IEnumerable<IRuntimeAppInfo>> GetLiveAppsAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
            var apps = this.liveApps.Values.Select(v => v.AppInfo).ToArray();
            return apps;
        }

        /// <summary>
        /// Starts the application asynchronously.
        /// </summary>
        /// <param name="appInfo">Information describing the application.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields an operation result.
        /// </returns>
        public virtual Task<IOperationResult> StartAppAsync(IAppInfo appInfo, IExpando arguments, IContext context = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult { OperationState = OperationState.Failed };
            result.Exceptions.Add(new NotSupportedException(Strings.OrchestrationManager_StartAppAsync_NotSupported));
            return Task.FromResult<IOperationResult>(result);
        }

        /// <summary>
        /// Stops a running application asynchronously.
        /// </summary>
        /// <param name="runtimeAppInfo">Information describing the runtime application.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields an operation result.
        /// </returns>
        public virtual Task<IOperationResult> StopAppAsync(IRuntimeAppInfo runtimeAppInfo, IContext context = null, CancellationToken cancellationToken = default)
        {
            var result = new OperationResult { OperationState = OperationState.Failed };
            result.Exceptions.Add(new NotSupportedException(Strings.OrchestrationManager_StopAppAsync_NotSupported));
            return Task.FromResult<IOperationResult>(result);
        }

        /// <summary>
        /// Gets runtime application infos asynchronously.
        /// </summary>
        /// <param name="liveAppInfos">The live application infos.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the runtime application infos.
        /// </returns>
        protected virtual Task InitializeLiveAppInfosAsync(ConcurrentDictionary<string, IAppEvent> liveAppInfos, CancellationToken cancellationToken)
        {
            return TaskHelper.CompletedTask;
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

            if (appEvent.AppInfo.AppId == this.appManifest.AppId && appEvent.AppInfo.AppInstanceId == this.appManifest.AppInstanceId)
            {
                this.timer = new Timer(this.OnHeartbeat, context, this.TimerDueTime, this.TimerPeriod);
            }

            this.liveApps.AddOrUpdate(appKey, appEvent, (_, ai) => appEvent);
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

            this.liveApps.TryRemove(appKey, out _);
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

            this.liveApps.AddOrUpdate(appKey, appEvent, (_, ai) => appEvent);
            return TaskHelper.CompletedTask;
        }

        private AppHeartbeatEvent CreateAppHeartbeatEvent()
        {
            return new AppHeartbeatEvent
            {
                AppInfo = this.appManifest.GetAppInfo(this.appRuntime),
                Timestamp = DateTimeOffset.Now,
            };
        }

        private string GetAppKey(IRuntimeAppInfo appInfo)
        {
            if (appInfo == null || (appInfo.AppId == null && appInfo.AppInstanceId == null))
            {
                return null;
            }

            return $"{appInfo.AppId}/{appInfo.AppInstanceId}";
        }

        /// <summary>
        /// Executes the heartbeat timer action.
        /// </summary>
        /// <param name="state">The state.</param>
        private async void OnHeartbeat(object state)
        {
            try
            {
                var heartbeatEvent = this.CreateAppHeartbeatEvent();
                await this.messageBroker.PublishAsync(heartbeatEvent, (IContext)state).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Exception on application heartbeat.");
            }
        }
    }
}