// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProcessStarter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the process starter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for process starter.
    /// </summary>
    public interface IProcessStarter
    {
        /// <summary>
        /// Gets information describing the process start.
        /// </summary>
        /// <value>
        /// Information describing the process start.
        /// </value>
        ProcessStartInfo ProcessStartInfo { get; }

        /// <summary>
        /// Starts a process asynchronously.
        /// </summary>
        /// <param name="config">Optional. The configuration.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the start.
        /// </returns>
        Task<ProcessStartResult> StartAsync(Action<Process>? config = null, CancellationToken cancellationToken = default);
    }
}