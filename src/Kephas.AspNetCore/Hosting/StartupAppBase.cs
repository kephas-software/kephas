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
    using Kephas.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using LogLevel = Kephas.Logging.LogLevel;
    using Strings = Kephas.Resources.Strings;

    /// <summary>
    /// Base class for the ASP.NET startup.
    /// </summary>
    /// <remarks>
    /// Typical use: define a Startup class inheriting from this class in the main assembly
    /// (where the Program class is defined). Use the newly defined class in WebHostBuilder.UseStartup&lt;Startup&gt;().
    /// Check https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-3.1 for more options.
    /// Another good read is https://stevetalkscode.co.uk/separating-aspnetcore-startup.
    /// </remarks>
    public abstract class StartupAppBase : AppBase<AmbientServices>
    {
        private Task? bootstrapTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupAppBase"/> class.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="ambientServices">Optional. The ambient services.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        protected StartupAppBase(
            IWebHostEnvironment env,
            IConfiguration config,
            IAmbientServices? ambientServices = null,
            IAppArgs? appArgs = null)
            : base(ambientServices, appArgs: appArgs)
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
        /// The <see cref="ConfigureServices"/> method is called by the host before the <see cref="Configure"/>
        /// method to configure the app's services. Here the configuration options are set by convention.
        /// The host may configure some services before Startup methods are called.
        /// For features that require substantial setup, there are Add{Service} extension methods on IServiceCollection.
        /// For example, AddDbContext, AddDefaultIdentity, AddEntityFrameworkStores, and AddRazorPages.
        /// Adding services to the service container makes them available within the app and in the <see cref="Configure"/> method.
        /// The services are resolved via dependency injection or from ApplicationServices.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            var ambientServices = services.GetAmbientServices() ?? this.AmbientServices;
            this.AmbientServices = ambientServices;
            ambientServices.Add(this.AppArgs);

            try
            {
                var appRuntime = ambientServices.GetAppRuntime();
                foreach (var configurator in this.GetServicesConfigurators(appRuntime))
                {
                    configurator(services, ambientServices);
                }
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Fatal, ex, Strings.App_RunAsync_ErrorDuringConfiguration_Exception);
                throw;
            }

            this.BeforeAppManagerInitialize(this.AppArgs);
        }

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
            var env = this.HostEnvironment;
            var appContext = (IAspNetAppContext)this.AppContext!;
            this.Logger.Info("{app} is running in the {environment} environment.", appContext.AppRuntime.GetAppInstanceId(), env.EnvironmentName);

            this.AmbientServices
                .Add(app)
                .Add(appLifetime);

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
            foreach (var middlewareConfigurator in this.GetMiddlewareConfigurators(appContext))
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
            => this.RunAsync(null, cancellationToken);

        /// <summary>
        /// Avoid disposing of services too soon, in the application stopping event.
        /// Instead, provide a custom <see cref="DisposeServicesContainer"/> called after
        /// the application has been stopped.
        /// </summary>
        protected sealed override void AfterAppManagerFinalize()
        {
        }

        /// <summary>
        /// Gets the middleware configurators.
        /// </summary>
        /// <param name="appContext">The application context.</param>
        /// <returns>An enumeration of middleware configurator callbacks.</returns>
        protected virtual IEnumerable<Action<IAspNetAppContext>> GetMiddlewareConfigurators(IAspNetAppContext appContext)
        {
            var container = appContext.AmbientServices.Injector;
            var middlewareConfigurators = container
                .Resolve<IFactoryEnumerable<IMiddlewareConfigurator, AppServiceMetadata>>()
                .SelectServices()
                .Select(this.GetMiddlewareConfiguratorAction);
            return middlewareConfigurators;
        }

        /// <summary>
        /// Gets the services configurators.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>An enumeration of services configurator callbacks.</returns>
        protected virtual IEnumerable<Action<IServiceCollection, IAmbientServices>> GetServicesConfigurators(IAppRuntime appRuntime)
            => appRuntime.GetServicesConfigurators()
                .Select(this.GetServicesConfiguratorAction);

        /// <summary>
        /// Disposes the services container. Replaces the original <see cref="AfterAppManagerFinalize"/>
        /// which is called too soon in the standard use case.
        /// </summary>
        protected virtual void DisposeServicesContainer()
        {
            base.AfterAppManagerFinalize();
        }

        /// <summary>
        /// Creates the application context.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="appArgs">The application arguments.</param>
        /// <returns>
        /// The new application context.
        /// </returns>
        protected override IAppContext CreateAppContext(IAmbientServices ambientServices, IAppArgs? appArgs)
        {
            var appContext = new AspNetAppContext(
                this.HostEnvironment,
                this.Configuration,
                this.AmbientServices,
                appArgs: appArgs);
            return appContext;
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

        private Action<IServiceCollection, IAmbientServices> GetServicesConfiguratorAction(IServicesConfigurator c)
        {
            return (s, a) =>
            {
                this.Logger.Debug($"Configuring services by {s.GetType()}...");
                c.ConfigureServices(s, a);
                this.Logger.Debug($"Services configured by {s.GetType()}.");
            };
        }

        private Action<IAspNetAppContext> GetMiddlewareConfiguratorAction(IMiddlewareConfigurator s)
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