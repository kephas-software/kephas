namespace SignalRChat.WebApp.Application
{
    using Kephas;
    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging.NLog;
    using Kephas.Threading.Tasks;

    using Microsoft.Owin.BuilderProperties;

    using Owin;

    public class WebApp : AppBase
    {
        private readonly IAppBuilder appBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApp"/> class.
        /// </summary>
        /// <param name="appBuilder">The application builder.</param>
        public WebApp(IAppBuilder appBuilder)
        {
            Requires.NotNull(appBuilder, nameof(appBuilder));

            this.appBuilder = appBuilder;
        }

        protected override void ConfigureAmbientServices(IAmbientServices ambientServices)
        {
            ambientServices
                .WithNLogManager()
                .WithDynamicAppRuntime()
                .WithMefCompositionContainer();
        }

        protected override IAppContext CreateAppContext(IAmbientServices ambientServices)
        {
            var appProperties = new AppProperties(this.appBuilder.Properties);
            appProperties.OnAppDisposing.Register(
                () =>
                {
                    this.ShutdownAsync().WaitNonLocking();
                });
            var appContext = base.CreateAppContext(ambientServices);
            appContext["AppProperties"] = appProperties;
            appContext["AppBuilder"] = this.appBuilder;
            return appContext;
        }
    }
}