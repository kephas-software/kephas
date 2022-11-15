// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperationMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOperationMessage interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations
{
    using System;

    using Kephas.ExceptionHandling;

    /// <summary>
    /// Contract for data operation messages.
    /// </summary>
    public interface IOperationMessage
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string Message { get; }

        /// <summary>
        /// Gets the exception, if any.
        /// </summary>
        Exception? Exception { get; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Gets a value indicating whether this message is an error.
        /// </summary>
        /// <returns><c>true</c> if this message is an error, <c>false</c> otherwise.</returns>
        bool IsError()
            => this is ISeverityQualifiedNotification { Severity: SeverityLevel.Error or SeverityLevel.Fatal }
               || this.Exception is ISeverityQualifiedNotification { Severity: SeverityLevel.Error or SeverityLevel.Fatal } or { } and not ISeverityQualifiedNotification;

        /// <summary>
        /// Gets a value indicating whether this message is a warning.
        /// </summary>
        /// <returns><c>true</c> if this message is a warning, <c>false</c> otherwise.</returns>
        bool IsWarning() =>
            this is ISeverityQualifiedNotification { Severity: SeverityLevel.Warning }
            || this.Exception is ISeverityQualifiedNotification { Severity: SeverityLevel.Warning };
    }
}