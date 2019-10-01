// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppTerminationAwaiter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppTerminationAwaiter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// Singleton application service contract for the service awaiting for the application shutdown signal.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IAppShutdownAwaiter
    {
        /// <summary>
        /// Waits for the shutdown signal asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the shutdown result.
        /// </returns>
        Task<(IOperationResult result, AppShutdownInstruction instruction)> WaitForShutdownSignalAsync(CancellationToken cancellationToken = default);
    }
}
