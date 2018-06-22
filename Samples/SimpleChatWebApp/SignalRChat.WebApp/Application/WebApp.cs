namespace SignalRChat.WebApp.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging.NLog;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;
    using Kephas.Web.Owin.Application;

    using Microsoft.Owin.BuilderProperties;

    using Owin;

    public class WebApp : AppBase
    {
        private readonly IAppBuilder appBuilder;

        private OwinAppContext appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApp"/> class.
        /// </summary>
        /// <param name="appBuilder">The application builder.</param>
        public WebApp(IAppBuilder appBuilder)
        {
            Requires.NotNull(appBuilder, nameof(appBuilder));

            this.appBuilder = appBuilder;
        }

        /// <summary>Configures the ambient services asynchronously.</summary>
        /// <remarks>
        /// This method should be overwritten to provide a meaningful content.
        /// </remarks>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        protected override async Task ConfigureAmbientServicesAsync(
            string[] appArgs,
            AmbientServicesBuilder ambientServicesBuilder,
            CancellationToken cancellationToken)
        {
            ambientServicesBuilder
                .WithNLogManager()
                .WithDefaultAppRuntime()
                .WithMefCompositionContainer();
        }

        /// <summary>Creates the application context.</summary>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>The new application context.</returns>
        protected override IAppContext CreateAppContext(string[] appArgs, IAmbientServices ambientServices)
        {
            new AppProperties(this.appBuilder.Properties).OnAppDisposing.Register(
                () =>
                    {
                        this.ShutdownAsync().WaitNonLocking();
                    });

            this.appContext = new OwinAppContext(this.appBuilder, ambientServices, appArgs: appArgs, signalShutdown: ctx => Task.FromResult<IAppContext>(this.appContext));
            return this.appContext;
        }
    }
}