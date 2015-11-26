namespace ConsoleApp
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Composition.Mef;
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
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Console.WriteLine("Application starting...");

            var ambientServicesBuilder = await new AmbientServicesBuilder()
                    .WithNLogManager()
                    .WithNet45HostingEnvironment()
                    .WithMefCompositionContainerAsync();

            var bootstrapper = ambientServicesBuilder.AmbientServices.CompositionContainer.GetExport<IAppBootstrapper>();
            await bootstrapper.StartAsync();

            stopwatch.Stop();
            Console.WriteLine($"Application started. Elapsed: {stopwatch.Elapsed:c}.");
        }
    }
}