// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartupBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the startup class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore
{
    using System;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Application.AspNetCore.Configuration;
    using Kephas.Application.AspNetCore.Hosting;
    using Kephas.Composition;
    using Kephas.Extensions.Configuration;
    using Kephas.Extensions.DependencyInjection;
    using Kephas.Extensions.Logging;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;

    using LogLevel = Kephas.Logging.LogLevel;
    using Strings = Kephas.Resources.Strings;

    /// <summary>
    /// Base class for the ASP.NET startup.
    /// </summary>
    /// <remarks>
    /// Check https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-2.1 for more options.
    /// </remarks>
    public abstract class StartupBase : AppBase
    {
        private readonly string[] appArgs;
        private Microsoft.Extensions.DependencyInjection.IServiceCollection serviceCollection;
        private Task bootstrapTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupBase"/> class.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="ambientServices">Optional. The ambient services.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        protected StartupBase(IWebHostEnvironment env, IConfiguration config, IAmbientServices ambientServices = null, string[] appArgs = null)
            : base(ambientServices)
        {
            this.HostEnvironment = env;
            this.Configuration = config;
            this.appArgs = appArgs;
        }

        /// <summary>
        /// Gets the hosting environment.
        /// </summary>
        /// <value>
        /// The hosting environment.
        /// </value>
        public IWebHostEnvironment HostEnvironment { get; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the DI services.
        /// </summary>
        /// <param name="serviceCollection">Collection of services.</param>
        public virtual void ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection serviceCollection)
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
        /// Configures the ambient services container.
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

                this.InitializePrerequisites(this.appArgs);
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Fatal, ex, Strings.App_BootstrapAsync_ErrorDuringConfiguration_Exception);
                throw;
            }
        }

        /// <summary>
        /// Configures the given application.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="appLifetime">The application lifetime.</param>
        public virtual void Configure(
            IApplicationBuilder app,
            IHostApplicationLifetime appLifetime)
        {
            this.AmbientServices
                .Register(app)
                .Register(appLifetime);

            var appContext = (IAspNetAppContext)this.AppContext;

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
            appLifetime.ApplicationStarted.Register(() => this.bootstrapTask = this.BootstrapAsync(this.appArgs));

            // If you want to dispose of resources that have been resolved in the
            // application container, register for the "ApplicationStopping" event.
            appLifetime.ApplicationStopping.Register(() => this.ShutdownAsync().WaitNonLocking());
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