// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalculatorShell.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
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
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    public class CalculatorShell : AppBase
    {
        /// <summary>Bootstraps the application asynchronously.</summary>
        /// <param name="appArgs">The application arguments (optional).</param>
        /// <param name="ambientServices">The ambient services (optional). If not provided then <see cref="P:Kephas.AmbientServices.Instance" /> is considered.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result that yields the <see cref="T:Kephas.Application.IAppContext" />.
        /// </returns>
        public override async Task<IAppContext> BootstrapAsync(
            string[] appArgs = null,
            IAmbientServices ambientServices = null,
            CancellationToken cancellationToken = default)
        {
            var appContext = await base.BootstrapAsync(appArgs, ambientServices, cancellationToken).PreserveThreadContext();
            this.Run(appContext.AmbientServices);

            await appContext.SignalShutdown(appContext);
            return appContext;
        }

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
            ambientServicesBuilder
                .WithNLogManager()
                .WithDefaultAppRuntime()
                .WithMefCompositionContainer();
        }

        /// <summary>
        /// Executes the application main functionality asynchronously.
        /// </summary>
        /// <remarks>
        /// This method should be overwritten to provide a meaningful content.
        /// </remarks>
        /// <param name="ambientServices">The configured ambient services.</param>
        protected void Run(IAmbientServices ambientServices)
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
                    var (value, operationName) = calculator.Compute(input);
                    Console.WriteLine($"Result is: {value} (using {operationName}).");
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