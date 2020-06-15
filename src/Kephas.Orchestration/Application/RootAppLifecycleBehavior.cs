// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Application
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Configuration;
    using Kephas.Application.Interaction;
    using Kephas.Application.Reflection;
    using Kephas.Configuration;
    using Kephas.Diagnostics;
    using Kephas.Dynamic;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Messaging.Events;
    using Kephas.Orchestration.Endpoints;
    using Kephas.Orchestration.Interaction;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Lifecycle behavior for the root microservice. It should be processed as the latest of
    /// all the microservices in the system.
    /// </summary>
    [ProcessingPriority(Priority.Lowest)]
    public class RootAppLifecycleBehavior : AppLifecycleBehaviorBase
    {
        private Timer? supervisorTimer;
        private IEventSubscription? restartSubscription;
        private IEventSubscription? setupQuerySubscription;
        private bool enableAppSetup = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="orchestrationManager">The orchestration manager.</param>
        /// <param name="systemConfiguration">The system configuration.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="appSetupService">The application setup service.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public RootAppLifecycleBehavior(
            IAppRuntime appRuntime,
            IOrchestrationManager orchestrationManager,
            IConfiguration<SystemSettings> systemConfiguration,
            IEventHub eventHub,
            IAppSetupService appSetupService,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.AppRuntime = appRuntime;
            this.OrchestrationManager = orchestrationManager;
            this.EventHub = eventHub;
            AppSetupService = appSetupService;
            this.SystemConfiguration = systemConfiguration;
        }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        protected IAppRuntime AppRuntime { get; }

        /// <summary>
        /// Gets the orchestration manager.
        /// </summary>
        protected IOrchestrationManager OrchestrationManager { get; }

        /// <summary>
        /// Gets the event hub.
        /// </summary>
        protected IEventHub EventHub { get; }

        /// <summary>
        /// Gets the application setup service.
        /// </summary>
        protected IAppSetupService AppSetupService { get; }

        /// <summary>
        /// Gets the system configuration.
        /// </summary>
        protected IConfiguration<SystemSettings> SystemConfiguration { get; }

        /// <summary>
        /// Gets or sets the worker processes.
        /// </summary>
        protected IList<ProcessStartResult>? WorkerProcesses { get; set; }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        /// <remarks>
        /// To interrupt the application initialization, simply throw an appropriate exception.
        /// </remarks>
        public override Task BeforeAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            this.setupQuerySubscription = this.EventHub.Subscribe<AppSetupQueryEvent>((e, c) => e.SetupEnabled = e.SetupEnabled && this.enableAppSetup);
            return base.BeforeAppInitializeAsync(appContext, cancellationToken);
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public override async Task AfterAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            await this.StartWorkerProcessesAsync(appContext, cancellationToken).PreserveThreadContext();
            this.restartSubscription = this.EventHub.Subscribe<RestartSignal>((signal, ctx, token) => this.HandleRestartSignalAsync(signal, appContext, token));
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous finalization.
        /// </summary>
        /// <remarks>
        /// To interrupt finalization, simply throw any appropriate exception.
        /// Caution! Interrupting the finalization may cause the application to remain in an undefined state.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public override Task BeforeAppFinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            this.restartSubscription?.Dispose();
            this.restartSubscription = null;

            return this.StopWorkerProcessesAsync(appContext, cancellationToken);
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task AfterAppFinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            this.setupQuerySubscription?.Dispose();
            this.setupQuerySubscription = null;

            return base.AfterAppFinalizeAsync(appContext, cancellationToken);
        }

        /// <summary>
        /// Starts the worker processes asynchronously.
        /// </summary>
        /// <param name="appContext">The application context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An asynchronous result.</returns>
        protected virtual async Task StartWorkerProcessesAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            if (!this.AppRuntime.IsRoot())
            {
                return;
            }

            var appInstanceEntries = this.SystemConfiguration.Settings.Instances;
            if (appInstanceEntries == null || appInstanceEntries.Count == 0)
            {
                return;
            }

            var liveApps =
                (await this.OrchestrationManager.GetLiveAppsAsync(cancellationToken: cancellationToken)
                    .PreserveThreadContext())
                .ToList();
            var startTasks = new List<Task<ProcessStartResult>>();
            this.Logger.Info("Starting worker application instances...");

            foreach (var appInstanceEntry in this.GetWorkerSettings(liveApps))
            {
                var appId = appInstanceEntry.Key;
                var appSettings = appInstanceEntry.Value;

                if (!appSettings.AutoStart)
                {
                    this.Logger.Info("Skipping worker application {app}, configured not to start automatically.", appId);
                    continue;
                }

                this.enableAppSetup = false;

                var appInfo = new AppInfo(appId) { [nameof(AppSettings)] = appSettings };
                startTasks.Add(this.StartWorkerProcessAsync(appInfo, appContext, cancellationToken));
            }

            this.WorkerProcesses = (await Task.WhenAll(startTasks).PreserveThreadContext()).ToList();

            foreach (var processStartResult in this.WorkerProcesses.Where(processStartResult => processStartResult.StartException != null))
            {
                // TODO localization
                this.Logger.Fatal(
                    processStartResult.StartException,
                    "Errors occurred when trying to start a worker application process: {commandLine}.",
                    processStartResult.Process.StartInfo.Arguments);
            }

            this.supervisorTimer = new Timer(
                _ => this.SuperviseWorkerProcessesAsync(appContext, cancellationToken),
                null,
                TimeSpan.FromMinutes(1),
                TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Stops the worker processes asynchronously.
        /// </summary>
        /// <param name="appContext">The application context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An asynchronous result.</returns>
        protected virtual async Task StopWorkerProcessesAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            if (!this.AppRuntime.IsRoot())
            {
                return;
            }

            if (this.supervisorTimer != null)
            {
#if NETSTANDARD2_1
                await this.supervisorTimer.DisposeAsync().PreserveThreadContext();
#else
                this.supervisorTimer.Dispose();
#endif
                this.supervisorTimer = null;
            }

            if (this.WorkerProcesses == null)
            {
                // not properly initialized, possibly abnormal program termination.
                return;
            }

            var logger = appContext.Logger ?? this.Logger;
            var liveApps =
                (await this.OrchestrationManager.GetLiveAppsAsync(ctx => ctx.Impersonate(appContext), cancellationToken)
                    .PreserveThreadContext())
                .ToList();

            this.Logger.Info($"Stopping worker application instances: {string.Join(", ", liveApps.Select(r => r.AppInstanceId))}");

            var rootAppId = this.AppRuntime.GetAppId();
            var stopTasks = new List<Task<StopAppResponseMessage>>();
            foreach (var runtimeAppInfo in liveApps)
            {
                // we are the master now, ignore this live app role
                if (runtimeAppInfo.AppId == rootAppId)
                {
                    continue;
                }

                stopTasks.Add(this.StopWorkerProcessAsync(runtimeAppInfo, appContext, cancellationToken));
            }

            var acknowledgedProcesses = (await Task.WhenAll(stopTasks).PreserveThreadContext())
                                            .Select(m => m?.ProcessId)
                                            .Where(id => id.HasValue)
                                            .Select(id => id.Value)
                                            .ToList();
            foreach (var workerProcessResult in this.WorkerProcesses)
            {
                if (workerProcessResult.StartException != null)
                {
                    workerProcessResult.Dispose();
                    continue;
                }

                var workerProcess = workerProcessResult.Process;
                var workerProcessInfo = $"{workerProcess.StartInfo.FileName} {workerProcess.StartInfo.Arguments}";
                try
                {
                    if (!acknowledgedProcesses.Contains(workerProcess.Id))
                    {
                        logger.Warn($"Worker application process '${workerProcessInfo}' (#{workerProcess.Id}) did not respond to the stop command, trying to kill it.");
                    }

                    var secondsToWait = this.WaitForExit(workerProcess);
                    if (workerProcess.HasExited)
                    {
                        logger.Info($"Worker process '${workerProcessInfo}' (#{workerProcess.Id}) exited at {workerProcess.ExitTime:s}");
                    }
                    else
                    {
                        logger.Warn($"Worker process '${workerProcessInfo}' (#{workerProcess.Id}) did not exit in the first {secondsToWait}s, killing it.");
                        workerProcess.Kill();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"Errors occurred when trying to kill worker application process '${workerProcessInfo}' (#{workerProcess.Id}).");
                }

                workerProcessResult.Dispose();
            }

            this.WorkerProcesses.Clear();

            this.enableAppSetup = true;
        }

        /// <summary>
        /// Gets the settings for all workers.
        /// </summary>
        /// <param name="liveApps">The live apps.</param>
        /// <returns>The settings for all workers.</returns>
        protected virtual IEnumerable<KeyValuePair<string, AppSettings>> GetWorkerSettings(IEnumerable<IRuntimeAppInfo> liveApps)
        {
            // get all the workers which are not already started and are not root.
            var rootAppId = this.AppRuntime.GetAppId()!;
            var appInstanceEntries = this.SystemConfiguration.Settings.Instances ?? new Dictionary<string, AppSettings>();
            return appInstanceEntries
                .Where(appInstanceEntry => !rootAppId.Equals(appInstanceEntry.Key, StringComparison.OrdinalIgnoreCase)
                    && !liveApps.Any(app => app.AppId.Equals(appInstanceEntry.Key, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Supervise the worker processes asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task SuperviseWorkerProcessesAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            foreach (var processStartResult in this.WorkerProcesses.ToList()
                .Where(processStartResult => processStartResult.Process.HasExited))
            {
                if (!(processStartResult[nameof(AppInfo)] is IAppInfo appInfo))
                {
                    continue;
                }

                // TODO localization
                this.Logger.Error("The worker application instance {app} exited prematurely. Trying to restart it...", appInfo.Identity.Id);

                try
                {
                    this.WorkerProcesses.Remove(processStartResult);
                    var newProcessStartResult = await this.StartWorkerProcessAsync(appInfo, appContext, CancellationToken.None).PreserveThreadContext();
                    this.WorkerProcesses.Add(newProcessStartResult);

                    if (newProcessStartResult.StartException != null)
                    {
                        // TODO localization
                        this.Logger.Fatal(
                            processStartResult.StartException,
                            "Errors occurred when trying to re-start a worker process ({app}).",
                            appInfo.Identity.Id);
                    }
                }
                catch (OperationCanceledException)
                {
                    this.Logger.Warn("Cancellation received while trying to restart {app}.", appInfo.Identity.Id);
                    return;
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "Error while trying to restart {app}.", appInfo.Identity.Id);
                }
            }
        }

        /// <summary>
        /// Starts the worker process asynchronously.
        /// </summary>
        /// <param name="appInfo">The application information.</param>
        /// <param name="appContext">The application context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An asynchronous result yielding the <see cref="ProcessStartResult"/>.</returns>
        protected virtual async Task<ProcessStartResult> StartWorkerProcessAsync(IAppInfo appInfo, IAppContext appContext, CancellationToken cancellationToken)
        {
            return (ProcessStartResult) await this.OrchestrationManager
                .StartAppAsync(appInfo, new Expando(), ctx => ctx.Merge(appContext), CancellationToken.None)
                .PreserveThreadContext();
        }

        /// <summary>
        /// Stops deployment asynchronous.
        /// </summary>
        /// <param name="runtimeAppInfo">The deployment.</param>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task<StopAppResponseMessage> StopWorkerProcessAsync(
            IRuntimeAppInfo runtimeAppInfo,
            IAppContext appContext,
            CancellationToken cancellationToken)
        {
            this.Logger.Info($"Stopping worker application instance {runtimeAppInfo}...");

            var workerProcess = this.WorkerProcesses.FirstOrDefault(p => this.TryGetProcessId(p) == runtimeAppInfo.ProcessId);
            if (workerProcess?.Process.HasExited ?? true)
            {
                this.Logger.Warn($"Worker process '${runtimeAppInfo.AppId}/{runtimeAppInfo.AppInstanceId}' (#{runtimeAppInfo.ProcessId}) exited prematurely. A stop message will not be sent anymore.");

                return new StopAppResponseMessage { ProcessId = runtimeAppInfo.ProcessId };
            }

            var result = await this.OrchestrationManager
                .StopAppAsync(runtimeAppInfo, ctx => ctx.Impersonate(appContext), cancellationToken)
                .PreserveThreadContext();

            var message = result[nameof(StopAppResponseMessage)] as StopAppResponseMessage;
            return message!;
        }

        private async Task HandleRestartSignalAsync(RestartSignal restartSignal, IAppContext appContext, CancellationToken cancellationToken)
        {
            this.Logger.Warn("Restarting the worker application instances...");
            try
            {
                await this.StopWorkerProcessesAsync(appContext, cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while stopping the worker application instances.");
            }

            try
            {
                await this.AppSetupService.SetupAsync(appContext, cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while setting up the application during the restart procedure.");
            }

            try
            {
                await this.StartWorkerProcessesAsync(appContext, cancellationToken).PreserveThreadContext();
                this.Logger.Info("Worker application instances restarted successfully.");
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while restarting the worker application instances.");
            }
        }

        private int WaitForExit(Process workerProcess)
        {
            var counter = 0;
            var secondsToWait = 60;
            while (!workerProcess.HasExited && counter < secondsToWait)
            {
                workerProcess.WaitForExit(1000);
                counter++;
            }

            return secondsToWait;
        }

        private int? TryGetProcessId(ProcessStartResult processStartResult)
        {
            try
            {
                return processStartResult.Process.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}