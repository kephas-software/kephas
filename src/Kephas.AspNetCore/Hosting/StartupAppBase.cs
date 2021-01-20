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
    using System.Threading.Tasks;

    using Kephas.Extensions.Configuration;
    using Kephas.Extensions.DependencyInjection;
    using Kephas.Extensions.Hosting.Configuration;
    using Kephas.Extensions.Logging;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;

    using LogLevel = Kephas.Logging.LogLevel;
    using Strings = Kephas.Resources.Strings;

    /// <summary>
    /// Base class for the ASP.NET startup.
    /// </summary>
    /// <remarks>
    /// Check https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-3.1 for more options.
    /// </remarks>
    public abstract class StartupAppBase : AppBase
    {
        private IServiceCollection serviceCollection;
        private Task bootstrapTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupAppBase"/> class.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="ambientServices">Optional. The ambient services.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        /// <param name="containerBuilder">Optional. The container builder.</param>
        protected StartupAppBase(
            IWebHostEnvironment env,
            IConfiguration config,
            IAmbientServices? ambientServices = null,
            IAppArgs? appArgs = null,
            Action<IAmbientServices>? containerBuilder = null)
            : base(ambientServices, containerBuilder: containerBuilder)
        {
            this.HostEnvironment = env;
            this.Configuration = config;
            this.AppArgs = appArgs ?? new AppArgs();
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
        /// Gets the application arguments.
        /// </summary>
        protected IAppArgs AppArgs { get; }

        /// <summary>
        /// The <see cref="ConfigureServices"/> method is called by the host before the <see cref="Configure"/>
        /// method to configure the app's services. Here the configuration options are set by convention.
        /// The host may configure some services before Startup methods are called.
        /// For features that require substantial setup, there are Add{Service} extension methods on IServiceCollection.
        /// For example, AddDbContext, AddDefaultIdentity, AddEntityFrameworkStores, and AddRazorPages.
        /// Adding services to the service container makes them available within the app and in the <see cref="Configure"/> method.
        /// The services are resolved via dependency injection or from ApplicationServices.
        /// </summary>
        /// <param name="serviceCollection">Collection of services.</param>
        public virtual void ConfigureServices(IServiceCollection serviceCollection)
        {
            try
            {
                serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Fatal, ex, Strings.App_BootstrapAsync_ErrorDuringConfiguration_Exception);
                throw;
            }

            this.serviceCollection = serviceCollection;
        }

        /// <summary>
        /// The <see cref="ConfigureContainer"/> method is called by the host after all app configuration is set up
        /// to complete the configuration. This is the place where the container should be built.
        /// </summary>
        /// <param name="ambientServices">Optional. The ambient services.</param>
        public virtual void ConfigureContainer(IAmbientServices ambientServices)
        {
            this.AmbientServices = ambientServices;

            try
            {
                this.AmbientServices
                    .WithServiceCollection(this.serviceCollection)
                    .ConfigureExtensionsLogging()
                    .ConfigureExtensionsOptions()
                    .UseConfiguration(this.Configuration);

                this.BeforeAppManagerInitialize(this.AppArgs);
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Fatal, ex, Strings.App_BootstrapAsync_ErrorDuringConfiguration_Exception);
                throw;
            }
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
            var appContext = (IAspNetAppContext)this.AppContext;
            this.Logger.Info("{app} is running in the {environment} environment.", appContext.AppRuntime.GetAppInstanceId(), env.EnvironmentName);

            this.AmbientServices
                .Register(app)
                .Register(appLifetime);

            // ensure upon request processing that the bootstrapping procedure is done.
            app.Use(async (context, next) =>
                {
                    await this.bootstrapTask.PreserveThreadContext();
                    await next.Invoke();
                });

            // use host configurators to setup the application.
            var container = appContext.CompositionContext;
            var hostConfigurators = container
                .GetExport<IOrderedServiceFactoryCollection<IHostConfigurator, AppServiceMetadata>>()
                .GetServices();
            foreach (var hostConfigurator in hostConfigurators)
            {
                hostConfigurator.Configure(appContext);
            }

            // when the configurators are completed, start the bootstrapping procedure.
            appLifetime.ApplicationStarted.Register(() => this.bootstrapTask = this.BootstrapAsync(this.AppArgs));

            // If you want to dispose of resources that have been resolved in the
            // application container, register for the "ApplicationStopping" event.
            appLifetime.ApplicationStopping.Register(() => this.ShutdownAsync().WaitNonLocking());
            appLifetime.ApplicationStopped.Register(() => this.DisposeServicesContainer());
        }

        /// <summary>
        /// Avoid disposing of services too soon, in the application stopping event.
        /// Instead, provide a custom <see cref="DisposeServicesContainer"/> called after
        /// the application has been stopped.
        /// </summary>
        protected sealed override void AfterAppManagerFinalize()
        {
        }

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
        /// <returns>
        /// The new application context.
        /// </returns>
        protected override IAppContext CreateAppContext(IAmbientServices ambientServices)
        {
            var appContext = new AspNetAppContext(
                this.HostEnvironment,
                this.Configuration,
                this.AmbientServices,
                appArgs: ambientServices.GetService<IAppArgs>());
            return appContext;
        }
    }
}