// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterruptSignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Kephas.Interaction
{
    using System;

    using Kephas.ExceptionHandling;

    /// <summary>
    /// Signals that a flow should be interrupted.
    /// Conventions: the call is considered successful if the severity is set to
    /// if the <see cref="Severity"/> is <see cref="LogLevel.Information"/> or <see cref="LogLevel.Warning"/>,
    /// otherwise it is an exception. Exceptions should be returned to the caller through the <see cref="Exception.InnerException"/> property.
    /// </summary>
    public class InterruptSignal : Exception, ISignal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterruptSignal"/> class.
        /// </summary>
        /// <param name="result">Optional. The result to be returned to the caller.</param>
        /// <param name="message">Optional. Message to indicate the reason for flow interruption.</param>
        /// <param name="severity">Optional. Indicates the severity.</param>
        public InterruptSignal(object? result = null, string? message = null, LogLevel? severity = null)
            : base(message ?? "Interrupt flow.")
        {
            this.Result = result;
            this.Severity = severity ?? LogLevel.Information;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterruptSignal"/> class.
        /// </summary>
        /// <param name="exception">The inner exception.</param>
        /// <param name="result">Optional. The result to be returned to the caller.</param>
        /// <param name="message">Optional. Message to indicate the reason for flow interruption.</param>
        /// <param name="severity">Optional. Indicates the severity.</param>
        public InterruptSignal(Exception exception, object? result = null, string? message = null, LogLevel? severity = null)
            : base(message ?? "Interrupt flow.", exception)
        {
            this.Result = result;
            this.Severity = severity ?? LogLevel.Error;
        }

        /// <summary>
        /// Gets the result to be provided back to the flow.
        /// </summary>
        public object? Result { get; }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        public LogLevel Severity { get; }
    }
}
