namespace StartupConsole.Application
{
    using System;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Diagnostics;
    using Kephas.Platform.Net;
    using Kephas.Logging.NLog;

    using AppContext = Kephas.Application.AppContext;

    /// <summary>
    /// A console shell.
    /// </summary>
    public class ConsoleShell
    {
        /// <summary>
        /// Starts the application asynchronously.
        /// </summary>
        /// <returns>
        /// A task.
        /// </returns>
        public async Task StartAppAsync()
        {
            Console.WriteLine("Application starting...");

            var ambientServicesBuilder = new AmbientServicesBuilder();
            var elapsed = await Profiler.WithStopwatchAsync(
                async () =>
                    {
                        await ambientServicesBuilder
                                .WithNLogManager()
                                .WithNetAppRuntime()
                                .WithMefCompositionContainerAsync();

                        var appBootstrapper = ambientServicesBuilder.AmbientServices.CompositionContainer.GetExport<IAppBootstrapper>();
                        await appBootstrapper.StartAsync(new AppContext());
                    });

            var appManifest = ambientServicesBuilder.AmbientServices.CompositionContainer.GetExport<IAppManifest>();
            Console.WriteLine();
            Console.WriteLine($"Application '{appManifest.AppId} V{appManifest.AppVersion}' started. Elapsed: {elapsed:c}.");

            Console.WriteLine();
            Console.WriteLine("Press any key to end the program.");
        }
    }
}