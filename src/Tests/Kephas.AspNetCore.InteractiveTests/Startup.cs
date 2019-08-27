namespace Kephas.AspNetCore.InteractiveTests
{
    using System;

    using Kephas;
    using Kephas.Application;
    using Kephas.Logging.Serilog;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SpaServices.AngularCli;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Serilog;

    public class Startup : Kephas.AspNetCore.StartupBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="config">The configuration.</param>
        public Startup(IHostingEnvironment env, IConfiguration config)
            : base(env, config)
        {
        }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            var serviceProvider = base.ConfigureServices(services);
            return serviceProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IApplicationLifetime appLifetime)
        {
            var env = this.HostingEnvironment;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            base.Configure(app, appLifetime);
        }

        /// <summary>Configures the ambient services asynchronously.</summary>
        /// <remarks>
        /// Override this method to initialize the startup services, like log manager and configuration manager.
        /// </remarks>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        protected override void ConfigureAmbientServices(string[] appArgs, AmbientServicesBuilder ambientServicesBuilder)
        {
            var serilogConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(this.Configuration);

            ambientServicesBuilder
                .WithSerilogManager(serilogConfig)
                .WithDefaultAppRuntime()
                .WithAutofacCompositionContainer();
        }
    }
}
