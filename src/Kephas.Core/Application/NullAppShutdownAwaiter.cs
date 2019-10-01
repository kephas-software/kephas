// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullAppTerminationAwaiter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null application termination awaiter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// A null application shutdown awaiter.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullAppShutdownAwaiter : IAppShutdownAwaiter
    {
        /// <summary>
        /// Waits for the application termination asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the termination result.
        /// </returns>
        public Task<(object result, AppShutdownInstruction instruction)> WaitForShutdownSignalAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<(object result, AppShutdownInstruction instruction)>((null, AppShutdownInstruction.Shutdown));
        }
    }
}
