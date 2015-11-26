// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppShell.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A console shell.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WindowsPlatformApp
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Composition.Mef;
    using Kephas.Hosting.WindowsPlatform;

    /// <summary>
    /// A console shell.
    /// </summary>
    public class AppShell
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

            // Console.WriteLine("Application starting...");

            var ambientServicesBuilder = await new AmbientServicesBuilder()
                    //.WithNLogManager()
                    .WithWindowsPlatformHostingEnvironment()
                    .WithMefCompositionContainerAsync();

            var bootstrapper = ambientServicesBuilder.AmbientServices.CompositionContainer.GetExport<IAppBootstrapper>();
            await bootstrapper.StartAsync();

            stopwatch.Stop();
            // Console.WriteLine($"Application started. Elapsed: {stopwatch.Elapsed:c}.");
        }
    }
}