using Microsoft.Owin;

using SignalRChat.WebApp;

[assembly: OwinStartup(typeof(Startup))]

namespace SignalRChat.WebApp
{
    using Kephas.Threading.Tasks;

    using Microsoft.Owin.BuilderProperties;

    using Owin;

    using SignalRChat.WebApp.Application;

    public class Startup
    {
        private WebApp webApp;

        public void Configuration(IAppBuilder app)
        {
            this.webApp = new WebApp(app);
            this.webApp.BootstrapAsync().WaitNonLocking();
        }
    }
}