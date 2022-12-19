﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISeverityQualifiedNotification.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISeverityQualifiedException interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ExceptionHandling
{
    using Kephas.Logging;

    /// <summary>
    /// Interface for severity qualified notification.
    /// </summary>
    public interface ISeverityQualifiedNotification : ILoggable
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string? Message { get; }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        SeverityLevel Severity { get; }

        /// <summary>
        /// Gets the log level of the loggable instance.
        /// </summary>
        /// <returns>The log level.</returns>
        LogLevel ILoggable.GetLogLevel() => (LogLevel)this.Severity;
    }
}