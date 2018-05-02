// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartupBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the startup class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.AspNetCore.Application;
    using Kephas.AspNetCore.Composition;
    using Kephas.Composition;
    using Kephas.Logging;
    using Kephas.Services.Composition;
    using Kephas.Threading.Tasks;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Strings = Kephas.Resources.Strings;

    /// <summary>
    /// Base class for the ASP.NET startup.
    /// </summary>
    public abstract class StartupBase
    {
        /// <summary>
        /// The application arguments.
        /// </summary>
        private readonly string[] appArgs;

        /// <summary>
        /// The ambient services.
        /// </summary>
        private IAmbientServices ambientServices;

        /// <summary>
        /// The bootstrap task.
        /// </summary>
        private Task bootstrapTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupBase"/> class.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="ambientServices">The ambient services (optional).</param>
        /// <param name="appArgs">The application arguments (optional).</param>
        protected StartupBase(IHostingEnvironment env, IConfiguration config, IAmbientServices ambientServices = null, string[] appArgs = null)
        {
            this.HostingEnvironment = env;
            this.Configuration = config;
            this.ambientServices = ambientServices;
            this.appArgs = appArgs;
        }

        /// <summary>
        /// Gets the hosting environment.
        /// </summary>
        /// <value>
        /// The hosting environment.
        /// </value>
        public IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected ILogger Logger { get; set; }

        /// <summary>
        /// Configure services.
        /// </summary>
        /// <param name="serviceCollection">Collection of services.</param>
        /// <returns>
        /// An IServiceProvider.
        /// </returns>
        public virtual IServiceProvider ConfigureServices(IServiceCollection serviceCollection)
        {
            try
            {
                this.Log(LogLevel.Info, null, Strings.App_BootstrapAsync_Bootstrapping_Message);

                this.ambientServices = this.ambientServices ?? AmbientServices.Instance;

                this.Log(LogLevel.Info, null, Strings.App_BootstrapAsync_ConfiguringAmbientServices_Message);
                var ambientServicesBuilder = new AmbientServicesBuilder(this.ambientServices);
                this.ConfigureAmbientServices(this.appArgs, ambientServicesBuilder);

                this.Logger = this.Logger ?? this.ambientServices.GetLogger(this.GetType());
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Fatal, ex, Strings.App_BootstrapAsync_ErrorDuringConfiguration_Exception);
                throw;
            }

            return this.ambientServices.CompositionContainer.ToServiceProvider();
        }

        /// <summary>
        /// Configures the given application.
        /// </summary>
        /// <param name="app">The application builder.</param>
        public virtual void Configure(IApplicationBuilder app)
        {
            var appContext = this.CreateAppContext(app, this.appArgs, this.ambientServices);

            // ensure upon request processing that the bootstrapping procedure is done.
            app.Use(async (context, next) =>
                {
                    await this.bootstrapTask.PreserveThreadContext();
                    await next.Invoke();
                });

            // use host configurators to setup the application.
            var container = appContext.AmbientServices.CompositionContainer;
            var hostConfigurators = container.GetExportFactories<IHostConfigurator, AppServiceMetadata>()
                .OrderBy(f => f.Metadata.OverridePriority)
                .ThenBy(f => f.Metadata.ProcessingPriority)
                .Select(f => f.CreateExportedValue())
                .ToList();
            foreach (var hostConfigurator in hostConfigurators)
            {
                hostConfigurator.Configure(appContext);
            }

            // when the configurators are completed, start the bootstrapping procedure.
            this.bootstrapTask = this.BootstrapAsync(appContext);
        }

        /// <summary>
        /// Bootstraps the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result that yields the <see cref="IAppContext"/>.
        /// </returns>
        public virtual async Task<IAspNetAppContext> BootstrapAsync(
            IAspNetAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            try
            {
                this.Log(LogLevel.Info, null, Strings.App_BootstrapAsync_InitializingAppManager_Message);
                await this.InitializeAppManagerAsync(appContext, cancellationToken);

                this.Log(LogLevel.Info, null, Strings.App_BootstrapAsync_StartComplete_Message);

                return appContext;
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Fatal, ex, Strings.App_BootstrapAsync_ErrorDuringInitialization_Exception);

                try
                {
                    await this.ShutdownAsync(ambientServices, cancellationToken).PreserveThreadContext();
                }
                catch (Exception shutdownEx)
                {
                    this.Log(LogLevel.Fatal, shutdownEx, Strings.App_BootstrapAsync_ErrorDuringForcedShutdown_Exception);
                }

                throw;
            }
        }

        /// <summary>
        /// Shuts down the application asynchronously.
        /// </summary>
        /// <param name="ambientServices">The ambient services (optional). If not provided then <see cref="AmbientServices.Instance"/> is considered.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the <see cref="IAppContext"/>.
        /// </returns>
        public virtual async Task<IAppContext> ShutdownAsync(
            IAmbientServices ambientServices = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                this.Log(LogLevel.Info, null, Strings.App_ShutdownAsync_ShuttingDown_Message);

                ambientServices = ambientServices ?? AmbientServices.Instance;
                var appContext = await this.FinalizeAppManagerAsync(ambientServices, cancellationToken);

                this.Log(LogLevel.Info, null, Strings.App_ShutdownAsync_Complete_Message);

                return appContext;
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Fatal, ex, Strings.App_ShutdownAsync_ErrorDuringFinalization_Exception);
                throw;
            }
        }

        /// <summary>
        /// Configures the ambient services asynchronously.
        /// </summary>
        /// <remarks>
        /// This method should be overwritten to provide a meaningful content.
        /// </remarks>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        protected abstract void ConfigureAmbientServices(
            string[] appArgs,
            AmbientServicesBuilder ambientServicesBuilder);

        /// <summary>
        /// Initializes the application manager asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the <see cref="IAppContext"/>.
        /// </returns>
        protected virtual async Task<IAspNetAppContext> InitializeAppManagerAsync(IAspNetAppContext appContext, CancellationToken cancellationToken)
        {
            var container = appContext.AmbientServices.CompositionContainer;
            var appManager = container.GetExport<IAppManager>();

            await appManager.InitializeAppAsync(appContext, cancellationToken).PreserveThreadContext();
            return appContext;
        }

        /// <summary>
        /// Finalizes the application manager asynchronously.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the <see cref="IAppContext"/>.
        /// </returns>
        protected virtual async Task<IAppContext> FinalizeAppManagerAsync(IAmbientServices ambientServices, CancellationToken cancellationToken)
        {
            var container = ambientServices.CompositionContainer;
            var appContext = container.GetExport<IAppContext>();
            var appManager = container.GetExport<IAppManager>();

            await appManager.FinalizeAppAsync(appContext, cancellationToken).PreserveThreadContext();
            return appContext;
        }

        /// <summary>
        /// Creates the application context.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// The new application context.
        /// </returns>
        protected virtual IAspNetAppContext CreateAppContext(IApplicationBuilder app, string[] appArgs, IAmbientServices ambientServices)
        {
            var appContext = new AspNetAppContext(
                app,
                ambientServices,
                appArgs: appArgs,
                signalShutdown: c => this.ShutdownAsync(ambientServices));
            return appContext;
        }

        /// <summary>
        /// Logs the provided information.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        protected virtual void Log(LogLevel level, Exception exception, string messageFormat, params object[] args)
        {
            this.Logger?.Log(level, exception, messageFormat, args);
        }
    }
}