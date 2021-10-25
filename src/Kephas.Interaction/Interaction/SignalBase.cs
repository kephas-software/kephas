// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignalBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Interaction
{
    using Kephas.ExceptionHandling;

    /// <summary>
    /// Base class for signals.
    /// </summary>
    public abstract class SignalBase : ISignal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalBase"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="severity">The severity.</param>
        protected SignalBase(string message, SeverityLevel severity = SeverityLevel.Info)
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