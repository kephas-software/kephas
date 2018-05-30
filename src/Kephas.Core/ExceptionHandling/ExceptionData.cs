// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionData.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the exception data class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ExceptionHandling
{
    using System;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// An exception data.
    /// </summary>
    public class ExceptionData : ISeverityQualifiedException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionData"/> class.
        /// </summary>
        public ExceptionData()
        {
            this.Severity = SeverityLevel.Error;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionData"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="severity">The severity level (optional).</param>
        public ExceptionData(Exception exception, SeverityLevel severity = SeverityLevel.Error)
            : this()
        {
            Requires.NotNull(exception, nameof(exception));

            if (exception is ISeverityQualifiedException severityQualifiedException)
            {
                severity = severityQualifiedException.Severity;
            }

            this.Message = exception.Message;
            this.Severity = severity;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the severity level.
        /// </summary>
        /// <value>
        /// The severity level.
        /// </value>
        public SeverityLevel Severity { get; set; }
    }
}