namespace ConsoleApp
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Composition.Mef;
    using Kephas.Diagnostics;
    using Kephas.Hosting.Net45;
    using Kephas.Logging.NLog;

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

            var elapsed = await Profiler.WithStopwatchAsync(
                async () =>
                    {
                        var ambientServicesBuilder = await new AmbientServicesBuilder()
                                .WithNLogManager()
                                .WithNet45HostingEnvironment()
                                .WithMefCompositionContainerAsync();

                        var bootstrapper = ambientServicesBuilder.AmbientServices.CompositionContainer.GetExport<IAppBootstrapper>();
                        await bootstrapper.StartAsync();
                    });

            Console.WriteLine($"Application started. Elapsed: {elapsed:c}.");
        }
    }
}