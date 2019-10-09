namespace SignalRChat.WebApp.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Threading.Tasks;

    using Owin;

    /// <summary>
    /// A SignalR feature manager.
    /// </summary>
    [FeatureInfo(AppFeature.SignalR)]
    public class SignalRFeatureManager : FeatureManagerBase
    {
        /// <summary>Initializes the application asynchronously.</summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        protected override Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            // Any connection or hub wire up and configuration should go here
            var app = (IAppBuilder)appContext["AppBuilder"];
            app.MapSignalR();

            return TaskHelper.CompletedTask;
        }
    }
}