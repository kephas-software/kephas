// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebStartupBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the web startup class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore
{
    using System;
    using System.ComponentModel;

    using Kephas.AspNetCore.Services.Composition;
    using Kephas.Composition;
    using Kephas.Composition.Hosting;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The startup class.
    /// </summary>
    public abstract class WebStartupBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebStartup"/> class.
        /// </summary>
        /// <param name="env">The environment.</param>
        protected WebStartupBase(IHostingEnvironment env)
        {
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            //    .AddEnvironmentVariables();
            //this.Configuration = builder.Build();
        }

        public IContainer ApplicationContainer { get; private set; }

        public IConfigurationRoot Configuration { get; private set; }

        /// <summary>
        /// ConfigureServices is where you register dependencies. This gets called by the runtime before
        /// the Configure method, below.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>
        /// An IServiceProvider.
        /// </returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // TODO

            var ambientServicesBuilder = new AmbientServicesBuilder();

            var serviceCollectionRegistrar = new ServiceCollectionConventionsRegistrar(services);
            
            //...

            // use it in creating the composition container

            ////// Create the container builder.
            ////var builder = new ContainerBuilder();

            ////// Register dependencies, populate the services from
            ////// the collection, and build the container. If you want
            ////// to dispose of the container at the end of the app,
            ////// be sure to keep a reference to it as a property or field.
            ////builder.RegisterType<MyType>().As<IMyType>();
            ////builder.Populate(services);
            ////this.ApplicationContainer = builder.Build();

            ////// Create the IServiceProvider based on the container.
            ////return new AutofacServiceProvider(this.ApplicationContainer);

            return ambientServicesBuilder.AmbientServices.CompositionContainer.ToServiceProvider();
        }

        // Configure is where you add middleware. This is called after
        // ConfigureServices. You can use IApplicationBuilder.ApplicationServices
        // here if you need to resolve things from the container.
        public void Configure(
          IApplicationBuilder app,
          ILoggerFactory loggerFactory,
          IApplicationLifetime appLifetime)
        {
            ////loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
            ////loggerFactory.AddDebug();

            ////app.UseMvc();

            // If you want to dispose of resources that have been resolved in the
            // application container, register for the "ApplicationStopped" event.
            appLifetime.ApplicationStopped.Register(() => this.ApplicationContainer.Dispose());
        }
    }
}