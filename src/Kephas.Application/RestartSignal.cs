// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestartSignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using Kephas.ExceptionHandling;
    using Kephas.Interaction;

    /// <summary>
    /// Signal for restarting the application. This class cannot be inherited.
    /// </summary>
    public sealed class RestartSignal : ISignal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestartSignal"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="severity">Optional. The severity.</param>
        public RestartSignal(string message, SeverityLevel severity = SeverityLevel.Info)
        {
            this.Message = message;
            this.Severity = severity;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public SeverityLevel Severity { get; }
    }
}