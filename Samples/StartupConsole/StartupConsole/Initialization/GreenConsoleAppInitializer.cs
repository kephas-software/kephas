namespace StartupConsole.Initialization
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Services;
    
    [ProcessingPriority(Priority.High)]
    public class GreenConsoleAppInitializer : IAppInitializer
    {
        /// <summary>
        /// Initializes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task InitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
}
