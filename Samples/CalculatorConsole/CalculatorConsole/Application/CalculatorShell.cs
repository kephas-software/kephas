// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalculatorShell.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the calculator shell class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CalculatorConsole.Application
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

    public class CalculatorShell : AppBase
    {
        /// <summary>
        /// Starts the application asynchronously.
        /// </summary>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result that yields the provided or the created
        /// <see cref="T:Kephas.IAmbientServices" />.
        /// </returns>
        public override async Task<IAmbientServices> StartApplicationAsync(
            string[] appArgs = null,
            IAmbientServices ambientServices = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine("Application initializing...");

            var elapsed = await Profiler.WithStopwatchAsync(
                              async () =>
                                  {
                                      ambientServices = await base.StartApplicationAsync(appArgs, ambientServices, cancellationToken).PreserveThreadContext();
                                  });

            var appManifest = ambientServices.CompositionContainer.GetExport<IAppManifest>();
            Console.WriteLine();
            Console.WriteLine($"Application '{appManifest.AppId} V{appManifest.AppVersion}' started. Elapsed: {elapsed:c}.");

            this.RunCalculator(appManifest);

            await this.StopApplicationAsync(ambientServices, cancellationToken).PreserveThreadContext();

            return ambientServices;
        }

        /// <summary>
        /// Configures the ambient services asynchronously.
        /// </summary>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        protected override async Task ConfigureAmbientServicesAsync(
            string[] appArgs,
            IAmbientServices ambientServices,
            CancellationToken cancellationToken)
        {
            var ambientServicesBuilder = new AmbientServicesBuilder((AmbientServices)ambientServices);
            await ambientServicesBuilder
                .WithNLogManager()
                .WithNetAppRuntime()
                .WithMefCompositionContainerAsync();
        }

        private void RunCalculator(IAppManifest appManifest)
        {
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
                    Console.WriteLine($"Result is: {result.Value} (using {result.OperationName}).");
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