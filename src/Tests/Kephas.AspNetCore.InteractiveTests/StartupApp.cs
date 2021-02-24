namespace Kephas.Application.AspNetCore.InteractiveTests
{
    using Kephas;
    using Kephas.Application.AspNetCore.Hosting;
    using Kephas.Serialization.Json;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.SpaServices.AngularCli;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class StartupApp : StartupAppBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartupApp" /> class.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="config">The configuration.</param>
        public StartupApp(IWebHostEnvironment env, IConfiguration config)
            : base(env, config, containerBuilder: ambientServices => ambientServices.BuildWithAutofac())
        {
        }

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
        public override void ConfigureServices(IServiceCollection services)
        {
            var ambientServices = services.GetAmbientServices();

            services
                .AddRazorPages()
                .AddNewtonsoftJson(
                    options =>
                    {
                        var jsonSettingsProvider = this.AmbientServices.CompositionContainer
                            .GetExport<IJsonSerializerSettingsProvider>();
                        jsonSettingsProvider.ConfigureJsonSerializerSettings(options.SerializerSettings);
                    });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            base.ConfigureServices(services);
        }

        /// <summary>
        /// The Configure method is used to specify how the app responds to HTTP requests.
        /// The request pipeline is configured by adding middleware components to an IApplicationBuilder instance.
        /// IApplicationBuilder is available to the Configure method, but it isn't registered in the service container.
        /// Hosting creates an IApplicationBuilder and passes it directly to Configure.
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
    }
}
