namespace Metadata.WebApp
{
    using System;

    using Kephas;
    using Kephas.Application;
    using Kephas.Logging.NLog;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SpaServices.AngularCli;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup : Kephas.Application.AspNetCore.StartupBase
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

        protected override void ConfigureAmbientServices(IAmbientServices ambientServices)
        {
            ambientServices
                .WithNLogManager()
                .WithDynamicAppRuntime()
                .WithAutofacCompositionContainer();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request
        /// pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="appLifetime">The application lifetime.</param>
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
    }
}
