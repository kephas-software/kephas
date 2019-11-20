// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionData.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
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
        /// <param name="severity">Optional. The severity level. If this value is set, it overwrites the severity level provided by the exception.</param>
        public ExceptionData(Exception exception, SeverityLevel? severity = null)
            : this()
        {
            Requires.NotNull(exception, nameof(exception));

            if (severity == null)
            {
                if (exception is ISeverityQualifiedException severityQualifiedException)
                {
                    severity = severityQualifiedException.Severity;
                }
                else
                {
                    severity = SeverityLevel.Error;
                }
            }

            this.ExceptionType = exception.GetType().FullName;
            this.Message = exception.Message;
            this.Severity = severity.Value;
        }

        /// <summary>
        /// Gets or sets the type of the exception.
        /// </summary>
        /// <value>
        /// The type of the exception.
        /// </value>
        public string ExceptionType { get; set; }

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

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Message} ({this.ExceptionType}: {this.Severity})";
        }
    }
}