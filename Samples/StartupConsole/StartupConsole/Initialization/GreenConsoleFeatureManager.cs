namespace StartupConsole.Initialization
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Services;

    [ProcessingPriority(Priority.High)]
    public class GreenConsoleFeatureManager : FeatureManagerBase
    {
        /// <summary>
        /// Initializes the feature asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
}
