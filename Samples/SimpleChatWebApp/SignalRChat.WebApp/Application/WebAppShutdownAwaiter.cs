using System.Threading;
using System.Threading.Tasks;
using Kephas.Application;
using Kephas.Operations;

namespace SignalRChat.WebApp.Application
{
    public class WebAppShutdownAwaiter : IAppShutdownAwaiter
    {
        /// <summary>
        /// Waits for the shutdown signal asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. A token that allows processing to be
        ///                                 cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the shutdown result.
        /// </returns>
        public async Task<(IOperationResult result, AppShutdownInstruction instruction)> WaitForShutdownSignalAsync(CancellationToken cancellationToken = default)
        {
            return (new OperationResult(), AppShutdownInstruction.Ignore);
        }
    }
}