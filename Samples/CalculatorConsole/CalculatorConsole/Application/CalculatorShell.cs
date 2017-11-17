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
    using Kephas.Logging.NLog;
    using Kephas.Platform.Net;

    public class CalculatorShell : AppBase
    {
        /// <summary>
        /// Configures the ambient services asynchronously.
        /// </summary>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        protected override async Task ConfigureAmbientServicesAsync(
            string[] appArgs,
            AmbientServicesBuilder ambientServicesBuilder,
            CancellationToken cancellationToken)
        {
            await ambientServicesBuilder
                .WithNLogManager()
                .WithNetAppRuntime()
                .WithMefCompositionContainerAsync();
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

            return base.RunAsync(appArgs, ambientServices, cancellationToken);
        }
    }
}