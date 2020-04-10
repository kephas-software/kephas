// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetAppShutdownAwaiter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ASP net application termination awaiter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// An ASP net application shutdown awaiter.
    /// </summary>
    [OverridePriority(Priority.High)]
    public class AspNetAppShutdownAwaiter : IAppShutdownAwaiter
    {
        /// <summary>
        /// Waits for the application termination asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the termination result.
        /// </returns>
        public Task<(IOperationResult result, AppShutdownInstruction instruction)> WaitForShutdownSignalAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<(IOperationResult result, AppShutdownInstruction instruction)>((new OperationResult { OperationState = OperationState.InProgress }, AppShutdownInstruction.Ignore));
        }
    }
}
