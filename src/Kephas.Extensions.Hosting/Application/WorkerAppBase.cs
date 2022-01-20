// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerAppBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the worker application base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Hosting.Application
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// A worker application base.
    /// </summary>
    public abstract class WorkerAppBase : AppBase<AmbientServices>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerAppBase"/> class.
        /// </summary>
        /// <param name="ambientServices">Optional. The ambient services.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        /// <param name="builderOptions">Optional. The host builder options.</param>
        protected WorkerAppBase(
            IAmbientServices? ambientServices = null,
            IAppArgs? appArgs = null,
            Action<IHostBuilder>? builderOptions = null)
            : this(ambientServices, CreateBuilder(builderOptions, appArgs), appArgs)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerAppBase"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="builder">The host builder.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        protected WorkerAppBase(IAmbientServices? ambientServices, IHostBuilder builder, IAppArgs? appArgs = null)
            : base(ambientServices, appArgs)
        {
            this.HostBuilder = builder;
        }

        /// <summary>
        /// Gets the host builder.
        /// </summary>
        /// <value>
        /// The host builder.
        /// </value>
        protected IHostBuilder HostBuilder { get; }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        protected IHost? Host { get; private set; }

        /// <summary>
        /// Runs the application asynchronously.
        /// </summary>
        /// <param name="mainCallback">Optional. The callback for the main function.
        /// If not provided, the service implementing <see cref="T:Kephas.Application.IAppMainLoop" /> will be invoked,
        /// otherwise the application will end.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the <see cref="T:Kephas.Application.IAppContext" />.
        /// </returns>
        public override Task<(IAppContext? appContext, AppShutdownInstruction instruction)> RunAsync(
            Func<IAppArgs, Task<(IOperationResult result, AppShutdownInstruction instruction)>>? mainCallback = null,
            CancellationToken cancellationToken = default)
        {
            if (this.AppArgs.RunAsService)
            {
                this.HostBuilder.UseWindowsService();
                this.HostBuilder.UseSystemd();
            }

            this.HostBuilder.ConfigureServices(this.AddBackgroundWorker);

            this.BeforeAppManagerInitialize(this.AppArgs);

            this.Host = this.HostBuilder.Build();

            var stoppingTokenSource = new CancellationTokenSource();
            var eventHub = this.AmbientServices.Injector.Resolve<IEventHub>();
            eventHub.Subscribe<ShutdownSignal>((_, _) => stoppingTokenSource.Cancel());

            return base.RunAsync(mainCallback ?? (args => RunHostAsync(this.Host, stoppingTokenSource.Token)), cancellationToken);
        }

        /// <summary>
        /// The building of the services container is the responsibility of the host.
        /// </summary>
        /// <remarks>
        /// Override this method to initialize the startup services, like log manager and configuration manager.
        /// </remarks>
        /// <param name="ambientServices">The ambient services.</param>
        protected override void BuildServicesContainer(IAmbientServices ambientServices)
        {
        }

        /// <summary>
        /// Adds the background worker as a hosted service.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void AddBackgroundWorker(IServiceCollection services)
        {
            services.AddHostedService(svc => new BackgroundWorker(this.RunBackgroundTaskAsync));
        }

        /// <summary>
        /// Runs the background task asynchronously.
        /// </summary>
        /// <param name="stoppingToken">The cancellation token used for stopping the process.</param>
        /// <returns>The asynchronous result.</returns>
        protected virtual async Task RunBackgroundTaskAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(50, stoppingToken);
                }
                catch (Exception ex)
                {
                    this.Log(LogLevel.Warning, ex);
                }
            }
        }

        private static IHostBuilder CreateBuilder(Action<IHostBuilder>? builderOptions, IAppArgs? appArgs)
        {
            var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(appArgs?.ToCommandArgs().ToArray() ?? Array.Empty<string>());
            builderOptions?.Invoke(builder);
            return builder;
        }

        private static async Task<(IOperationResult result, AppShutdownInstruction instruction)> RunHostAsync(
            IHost host, CancellationToken stoppingToken)
        {
            var opResult = new OperationResult<bool>(true);
            await host.RunAsync(stoppingToken).PreserveThreadContext();
            return (opResult.Complete(), AppShutdownInstruction.Shutdown);
        }
    }
}