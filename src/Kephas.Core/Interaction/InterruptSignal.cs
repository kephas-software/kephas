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
    /// </summary>
    public class InterruptSignal : Exception, ISignal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterruptSignal"/> class.
        /// </summary>
        /// <param name="message">Optional. Message to indicate the reason for flow interruption.</param>
        /// <param name="severity">Optional. Indicates the severity.</param>
        public InterruptSignal(string? message = null, SeverityLevel severity = SeverityLevel.Info)
            : base(message ?? "Interrupt flow.")
        {
        }

        /// <summary>
        /// Gets or sets the result to be provided to the flow.
        /// </summary>
        public object? Result { get; set; }

        /// <summary>
        /// Gets or sets the exception to be provided to the flow.
        /// </summary>
        public Exception? Exception { get; set; }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public SeverityLevel Severity { get; } = SeverityLevel.Info;
    }
}