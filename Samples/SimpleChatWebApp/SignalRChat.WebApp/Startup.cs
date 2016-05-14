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
    using Kephas.Platform.Net45;
    using Kephas.Threading.Tasks;
    using Kephas.Web.Owin.Application;

    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var ambientServices = this.InitializeAmbientServicesAsync().GetResultNonLocking(TimeSpan.FromMinutes(5));

            var appContext = new OwinAppContext(app);
            var bootstrapper = ambientServices.CompositionContainer.GetExport<IAppBootstrapper>();
            bootstrapper.StartAsync(appContext).WaitNonLocking(TimeSpan.FromMinutes(5));
        }

        private async Task<IAmbientServices> InitializeAmbientServicesAsync()
        {
            var ambientServicesBuilder = new AmbientServicesBuilder();
            await ambientServicesBuilder
                    .WithNLogManager()
                    .WithNet45AppEnvironment()
                    .WithMefCompositionContainerAsync();
            return ambientServicesBuilder.AmbientServices;
        }
    }
}