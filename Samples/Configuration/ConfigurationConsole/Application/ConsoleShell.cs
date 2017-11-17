namespace StartupConsole.Application
{
    using System;
    using System.Threading;
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
    public class ConsoleShell : AppBase
    {
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
            await ambientServicesBuilder
                .WithNLogManager()
                .WithNetAppRuntime()
                .WithMefCompositionContainerAsync()
                .PreserveThreadContext();
        }

        /// <summary>
        /// Executes the application main functionality asynchronously.
        /// </summary>
        /// <remarks>
        /// This method should be overwritten to provide a meaningful content.
        /// </remarks>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServices">The configured ambient services.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        protected override Task RunAsync(string[] appArgs, IAmbientServices ambientServices, CancellationToken cancellationToken)
        {
            var appManifest = ambientServices.CompositionContainer.GetExport<IAppManifest>();
            Console.WriteLine();
            Console.WriteLine($"Application '{appManifest.AppId} V{appManifest.AppVersion}' started.");

            Console.WriteLine();
            Console.WriteLine("Press any key to end the program.");

            Console.ReadLine();

            return base.RunAsync(appArgs, ambientServices, cancellationToken);
        }
    }
}