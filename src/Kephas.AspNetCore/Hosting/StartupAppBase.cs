// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartupAppBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the startup class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Services.Builder;
    using Kephas.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Base class for the ASP.NET startup.
    /// </summary>
    /// <remarks>
    /// Typical use: define a Startup class inheriting from this class in the main assembly
    /// (where the Program class is defined). Use the newly defined class in WebHostBuilder.UseStartup&lt;Startup&gt;().
    /// Check https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-3.1 for more options.
    /// Another good read is https://stevetalkscode.co.uk/separating-aspnetcore-startup.
    /// </remarks>
    public abstract class StartupAppBase : AppBase
    {
        private Task? bootstrapTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupAppBase"/> class.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        protected StartupAppBase(
            IWebHostEnvironment env,
            IConfiguration config,
            IAppArgs? appArgs = null)
            : base(appArgs: appArgs)
        {
            this.HostEnvironment = env;
            this.Configuration = config;
        }

        /// <summary>
        /// Gets the hosting environment.
        /// </summary>
        /// <value>
        /// The hosting environment.
        /// </value>
        protected IWebHostEnvironment HostEnvironment { get; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        protected IConfiguration Configuration { get; }

        /// <summary>
        /// The Configure method is used to specify how the app responds to HTTP requests.
        /// The request pipeline is configured by adding middleware components to an IApplicationBuilder instance.
        /// IApplicationBuilder is available to the Configure method, but it isn't registered in the service container.
        /// Hosting creates an IApplicationBuilder and passes it directly to Configure.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="appLifetime">The application lifetime.</param>
        public virtual void Configure(
            IApplicationBuilder app,
            IHostApplicationLifetime appLifetime)
        {
            var appContext = (IWebAppContext)this.AppContext!;
            appContext.App = app;
            this.Logger.Info(
                "{app} is running in the {environment} environment.",
                appContext.AppRuntime.GetAppInstanceId(),
                this.HostEnvironment.EnvironmentName);

            // ensure upon request processing that the bootstrapping procedure is done.
            app.Use(async (context, next) =>
            {
                if (this.bootstrapTask == null)
                {
                    throw new ApplicationException("The bootstrap task is not initialized!");
                }

                await this.bootstrapTask.PreserveThreadContext();
                await next.Invoke();
            });

            // use middleware configurators to setup the application.
            foreach (var middlewareConfigurator in this.GetMiddlewareConfigurators())
            {
                middlewareConfigurator(appContext);
            }

            // when the configurators are completed, start the bootstrapping procedure.
            appLifetime.ApplicationStarted.Register(() => this.bootstrapTask = this.RunServiceAsync());

            // If you want to dispose of resources that have been resolved in the
            // application container, register for the "ApplicationStopping" event.
            appLifetime.ApplicationStopping.Register(() => this.ShutdownAsync().WaitNonLocking());
            appLifetime.ApplicationStopped.Register(() => this.DisposeServicesContainer());
        }

        /// <summary>
        /// Runs the application asynchronously in service mode.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the <see cref="IAppContext"/>.
        /// </returns>
        protected virtual Task<AppRunResult> RunServiceAsync(CancellationToken cancellationToken = default)
            => this.RunAsync(cancellationToken);

        /// <summary>
        /// Gets the middleware configurators.
        /// </summary>
        /// <returns>An enumeration of middleware configurator callbacks.</returns>
        protected virtual IEnumerable<Action<IWebAppContext>> GetMiddlewareConfigurators()
        {
            var container = this.ServiceProvider!;
            var middlewareConfigurators = container
                .Resolve<IFactoryEnumerable<IMiddlewareConfigurator, AppServiceMetadata>>()
                .SelectServices()
                .Select(this.GetMiddlewareConfiguratorAction);
            return middlewareConfigurators;
        }

        /// <summary>
        /// Disposes the services container. Replaces the original <see cref="AfterAppManagerFinalize"/>
        /// which is called too soon in the standard use case.
        /// </summary>
        protected virtual void DisposeServicesContainer()
        {
        }

        /// <summary>
        /// Creates the application context.
        /// </summary>
        /// <returns>
        /// The new application context.
        /// </returns>
        protected override IAppContext CreateAppContext()
        {
            var appContext = new WebAppContext(
                this.HostEnvironment,
                this.Configuration,
                this.ServicesBuilder,
                this.AppArgs)
            {
                Logger = this.Logger,
            };
            return appContext;
        }

        private Action<IWebAppContext> GetMiddlewareConfiguratorAction(IMiddlewareConfigurator s)
        {
            return c =>
            {
                this.Logger.Debug($"Configuring middleware by {s.GetType()}...");
                s.Configure(c);
                this.Logger.Debug($"Middleware configured by {s.GetType()}.");
            };
        }
    }
}