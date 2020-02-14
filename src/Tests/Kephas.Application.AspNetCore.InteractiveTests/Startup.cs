namespace Kephas.Application.AspNetCore.InteractiveTests
{
    using System;

    using Kephas;
    using Kephas.Application;
    using Kephas.Logging.Serilog;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.SpaServices.AngularCli;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public class Startup : Kephas.Application.AspNetCore.StartupBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="config">The configuration.</param>
        public Startup(IWebHostEnvironment env, IConfiguration config)
            : base(env, config)
        {
        }

        /// <summary>
        /// Configures the DI services.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <returns>
        /// An IServiceProvider.
        /// </returns>
        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            var serviceProvider = base.ConfigureServices(services);
            return serviceProvider;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request
        /// pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="appLifetime">The application lifetime.</param>
        public override void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifetime)
        {
            var env = this.HostEnvironment;

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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints
                    .MapControllerRoute("default", "{controller}/{action=Index}/{id?}");

                endpoints
                    .MapRazorPages();
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
        /// <param name="ambientServices">The ambient services.</param>
        protected override void ConfigureAmbientServices(IAmbientServices ambientServices)
        {
            var serilogConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(this.Configuration);

            ambientServices
                .WithSerilogManager(serilogConfig)
                .WithDynamicAppRuntime()
                .WithAutofacCompositionContainer();
        }
    }
}
