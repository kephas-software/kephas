// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessStartResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the process start result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Text;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Operations;

    /// <summary>
    /// Encapsulates the result of the process start.
    /// </summary>
    public class ProcessStartResult : OperationResult, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessStartResult"/> class.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="startException">The start exception (optional).</param>
        public ProcessStartResult(Process process, Exception? startException = null)
        {
            Requires.NotNull(process, nameof(process));

            this.ErrorData = new StringBuilder();
            this.OutputData = new StringBuilder();

            this.Process = process;
            this.Process.OutputDataReceived += (s, a) => this.OutputData.Append(a.Data);
            this.Process.ErrorDataReceived += (s, a) => this.ErrorData.Append(a.Data);
            this.StartException = startException;
        }

        /// <summary>
        /// Gets the process.
        /// </summary>
        /// <value>
        /// The process.
        /// </value>
        public Process Process { get; }

        /// <summary>
        /// Gets the start exception.
        /// </summary>
        /// <value>
        /// The start exception.
        /// </value>
        public Exception? StartException { get; }

        /// <summary>
        /// Gets information describing the error.
        /// </summary>
        /// <value>
        /// Information describing the error.
        /// </value>
        public StringBuilder ErrorData { get; }

        /// <summary>
        /// Gets or sets information describing the output.
        /// </summary>
        /// <value>
        /// Information describing the output.
        /// </value>
        public StringBuilder OutputData { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the
        /// Tnsa.Foundation.Runtime.OperatingSystem.ProcessStartResult and optionally releases the
        /// managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.Process.Dispose();

            if (disposing)
            {
            }
        }
    }
}