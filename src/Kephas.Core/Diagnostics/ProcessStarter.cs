// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessStarter.cs" company="Kephas Software SRL">
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

    using Kephas.Logging;

    /// <summary>
    /// A process starter.
    /// </summary>
    public class ProcessStarter : Loggable, IProcessStarter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessStarter"/> class.
        /// </summary>
        /// <param name="processStartInfo">Information describing the process start.</param>
        public ProcessStarter(ProcessStartInfo processStartInfo)
        {
            this.ProcessStartInfo = processStartInfo;
        }

        /// <summary>
        /// Gets information describing the process start.
        /// </summary>
        /// <value>
        /// Information describing the process start.
        /// </value>
        public ProcessStartInfo ProcessStartInfo { get; }

        /// <summary>
        /// Starts the process asynchronously.
        /// </summary>
        /// <param name="config">(Optional) Callback to configure the process before start.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task<ProcessStartResult> StartAsync(Action<Process> config = null, CancellationToken cancellationToken = default)
        {
            var processStartInfo = this.ProcessStartInfo;

            var processCommandLine = $"{this.ProcessStartInfo.FileName} {this.ProcessStartInfo.Arguments}";

            this.Logger.Info($"Starting '{processCommandLine}'...");

            var process = new Process { StartInfo = processStartInfo };

            var taskCompletionSource = new TaskCompletionSource<ProcessStartResult>();

            Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        // allow notifications over Exited & the other events.
                        process.EnableRaisingEvents = true;

                        config?.Invoke(process);

                        var started = process.Start();
                        if (!started)
                        {
                            var errorMessage = $"There was an error starting process {processCommandLine}. "
                                               + $"The runtime indicated that the process did not start.";
                            this.Logger.Fatal(errorMessage);
                            throw new ProcessStartException(errorMessage);
                        }

                        this.Logger.Info($"Started '{processCommandLine}' (#{process.Id}).");

                        taskCompletionSource.SetResult(new ProcessStartResult(process));
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.SetResult(new ProcessStartResult(process, ex));
                    }
                }, cancellationToken);

            return taskCompletionSource.Task;
        }
    }
}