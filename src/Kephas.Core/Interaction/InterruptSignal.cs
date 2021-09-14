// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterruptSignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Interaction
{
    using System;

    using Kephas.ExceptionHandling;

    /// <summary>
    /// Signals that a flow should be interrupted.
    /// Conventions: the call is considered successful if the severity is set to
    /// if the <see cref="Severity"/> is <see cref="SeverityLevel.Info"/> or <see cref="SeverityLevel.Warning"/>,
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
        public InterruptSignal(object? result = null, string? message = null, SeverityLevel? severity = null)
            : base(message ?? "Interrupt flow.")
        {
            this.Result = result;
            this.Severity = severity ?? SeverityLevel.Info;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterruptSignal"/> class.
        /// </summary>
        /// <param name="exception">The inner exception.</param>
        /// <param name="result">Optional. The result to be returned to the caller.</param>
        /// <param name="message">Optional. Message to indicate the reason for flow interruption.</param>
        /// <param name="severity">Optional. Indicates the severity.</param>
        public InterruptSignal(Exception exception, object? result = null, string? message = null, SeverityLevel? severity = null)
            : base(message ?? "Interrupt flow.", exception)
        {
            this.Result = result;
            this.Severity = severity ?? SeverityLevel.Error;
        }

        /// <summary>
        /// Gets the result to be provided back to the flow.
        /// </summary>
        public object? Result { get; }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        public SeverityLevel Severity { get; }
    }
}