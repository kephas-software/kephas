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
    using Kephas.Operations;

    /// <summary>
    /// A process starter.
    /// </summary>
    public class ProcessStarter : Loggable, IProcessStarter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessStarter"/> class.
        /// </summary>
        /// <param name="processStartInfo">Information describing the process start.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        public ProcessStarter(ProcessStartInfo processStartInfo, ILogManager? logManager = null)
            : base(logManager)
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
        /// <exception cref="ProcessStartException">Thrown when the Process Start error condition occurs.</exception>
        /// <param name="config">Optional. Callback to configure the process before start.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public async Task<ProcessStartResult> StartAsync(
            Action<Process>? config = null,
            CancellationToken cancellationToken = default)
        {
            var processStartInfo = this.ProcessStartInfo;

            await Task.Yield();

            var processCommandLine = $"{this.ProcessStartInfo.FileName} {this.ProcessStartInfo.Arguments}";

            this.Logger.Info("Starting '{commandLine}'...", processCommandLine);

            var process = new Process { StartInfo = processStartInfo };

            try
            {
                // allow notifications over Exited & the other events.
                process.EnableRaisingEvents = true;

                config?.Invoke(process);

                var started = process.Start();
                if (!started)
                {
                    this.Logger.Fatal(
                        "There was an error starting process {commandLine}. The runtime indicated that the process did not start.",
                        processCommandLine);
                    throw new ProcessStartException(
                        $"There was an error starting process {processCommandLine}. The runtime indicated that the process did not start.");
                }

                this.Logger.Info("Started '{commandLine}' (#{processId}).", processCommandLine, process.Id);

                return new ProcessStartResult(process);
            }
            catch (Exception ex)
            {
                return new ProcessStartResult(process, ex);
            }
        }
    }
}