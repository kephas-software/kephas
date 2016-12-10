using Microsoft.Owin;

using SignalRChat.WebApp;

[assembly: OwinStartup(typeof(Startup))]

namespace SignalRChat.WebApp
{
    using System;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Logging.NLog;
    using Kephas.Platform.Net;
    using Kephas.Threading.Tasks;
    using Kephas.Web.Owin.Application;

    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigurationAsync(app).WaitNonLocking(TimeSpan.FromMinutes(5));
        }

        private async Task ConfigurationAsync(IAppBuilder app)
        {
            var ambientServices = await this.InitializeAmbientServicesAsync().PreserveThreadContext();

            var appContext = new OwinAppContext(app);
            var bootstrapper = ambientServices.CompositionContainer.GetExport<IAppBootstrapper>();
            await bootstrapper.StartAsync(appContext).PreserveThreadContext();
        }

        private async Task<IAmbientServices> InitializeAmbientServicesAsync()
        {
            var ambientServicesBuilder = new AmbientServicesBuilder();
            await ambientServicesBuilder
                    .WithNLogManager()
                    .WithNetAppRuntime()
                    .WithMefCompositionContainerAsync();
            return ambientServicesBuilder.AmbientServices;
        }
    }
}