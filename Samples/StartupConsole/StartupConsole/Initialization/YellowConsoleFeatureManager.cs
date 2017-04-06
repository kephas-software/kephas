namespace StartupConsole.Initialization
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;

    public class YellowConsoleFeatureManager : FeatureManagerBase
    {
        /// <summary>
        /// Initializes the feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override async Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
    }
}