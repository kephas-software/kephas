namespace StartupConsole.Application
{
    using System;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Diagnostics;
    using Kephas.Logging.NLog;
    using Kephas.Platform.Net;
    using Kephas.Threading.Tasks;

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
            IAppManager appManager = null;
            var appContext = new AppContext();
            var elapsed = await Profiler.WithStopwatchAsync(
                async () =>
                    {
                        await ambientServicesBuilder
                                .WithNLogManager()
                                .WithNetAppRuntime()
                                .WithMefCompositionContainerAsync();

                        appManager = ambientServicesBuilder.AmbientServices.CompositionContainer.GetExport<IAppManager>();
                        await appManager.InitializeAppAsync(appContext);
                    });

            var appManifest = ambientServicesBuilder.AmbientServices.CompositionContainer.GetExport<IAppManifest>();
            Console.WriteLine();
            Console.WriteLine($"Application '{appManifest.AppId} V{appManifest.AppVersion}' started. Elapsed: {elapsed:c}.");

            Console.WriteLine();
            Console.WriteLine("Press any key to end the program.");

            await appManager.FinalizeAppAsync(appContext).PreserveThreadContext();
        }
    }
}