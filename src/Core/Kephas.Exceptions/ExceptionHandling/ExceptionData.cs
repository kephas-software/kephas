// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionData.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the exception data class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Kephas.ExceptionHandling
{
    using System;

    /// <summary>
    /// An exception data.
    /// </summary>
    public record ExceptionData() : IErrorNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionData"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="severity">Optional. The severity level. If this value is set, it overwrites the severity level provided by the exception.</param>
        public ExceptionData(Exception exception, LogLevel? severity = null)
            : this()
        {
            exception = exception ?? throw new ArgumentNullException(nameof(exception));

            severity ??= exception is IErrorNotification notification ? notification.Severity : LogLevel.Error;

            ExceptionType = exception.GetType().FullName!;
            Message = exception.Message;
            Severity = severity.Value;
        }

        /// <summary>
        /// Gets or sets the type of the exception.
        /// </summary>
        /// <value>
        /// The type of the exception.
        /// </value>
        public string? ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the severity level.
        /// </summary>
        /// <value>
        /// The severity level.
        /// </value>
        public LogLevel Severity { get; set; } = LogLevel.Error;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() =>
            $"{Message} ({ExceptionType}: {Severity})";
    }
}
