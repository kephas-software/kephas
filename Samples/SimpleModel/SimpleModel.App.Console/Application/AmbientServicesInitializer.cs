namespace SimpleModel.App.Console.Application
{
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Diagnostics;
    using Kephas.Hosting.Net45;
    using Kephas.Logging.NLog;

    public static class AmbientServicesInitializer
    {
        public static async Task<AmbientServices> InitializeAsync()
        {
            var ambientServicesBuilder = new AmbientServicesBuilder();
            var elapsed = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    await
                        ambientServicesBuilder.WithNLogManager()
                            .WithNet45HostingEnvironment()
                            .WithMefCompositionContainerAsync();

                    var compositionContainer = ambientServicesBuilder.AmbientServices.CompositionContainer;
                    var appBootstrapper = compositionContainer.GetExport<IAppBootstrapper>();
                    await appBootstrapper.StartAsync(new AppContext());
                });

            var ambientServices = ambientServicesBuilder.AmbientServices;
            (ambientServices as dynamic).Elapsed = elapsed;
            return ambientServices;
        }
    }
}