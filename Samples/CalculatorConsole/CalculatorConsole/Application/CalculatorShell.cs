﻿namespace CalculatorConsole.Application
{
    using System;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Diagnostics;
    using Kephas.Platform.Net;
    using Kephas.Logging.NLog;

    using AppContext = Kephas.Application.AppContext;

    public class CalculatorShell
    {
        /// <summary>
        /// Starts the application asynchronously.
        /// </summary>
        /// <returns>
        /// A task.
        /// </returns>
        public async Task StartAppAsync()
        {
            Console.WriteLine("Application initializing...");

            var ambientServicesBuilder = new AmbientServicesBuilder();
            var elapsed = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    await ambientServicesBuilder
                            .WithNLogManager()
                            .WithNetAppRuntime()
                            .WithMefCompositionContainerAsync();

                    var compositionContainer = ambientServicesBuilder.AmbientServices.CompositionContainer;
                    var appBootstrapper = compositionContainer.GetExport<IAppManager>();
                    await appBootstrapper.InitializeAppAsync(new AppContext());
                });

            var appManifest = ambientServicesBuilder.AmbientServices.CompositionContainer.GetExport<IAppManifest>();
            Console.WriteLine();
            Console.WriteLine($"Application '{appManifest.AppId} V{appManifest.AppVersion}' started. Elapsed: {elapsed:c}.");

            Console.WriteLine();
            Console.WriteLine("Provide an operation in form of: term1 op term2. End the program with q instead of an operation.");

            var calculator = ((CalculatorAppManifest)appManifest).Calculator;
            while (true)
            {
                var input = Console.ReadLine();
                if (input.ToLower().StartsWith("q"))
                {
                    break;
                }

                try
                {
                    var result = calculator.Compute(input);
                    Console.WriteLine($"Result is: {result}.");
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine(ex.Message);

                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}