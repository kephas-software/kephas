// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data i/o exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using System;

    using Kephas.Data.IO.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.ExceptionHandling;

    /// <summary>
    /// Exception for signalling data I/O errors.
    /// </summary>
    public class DataIOException : Exception, IDataIOMessage, ISeverityQualifiedException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataIOException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DataIOException(string message)
            : base(message)
        {
            this.Timestamp = DateTimeOffset.Now;
            this.Severity = SeverityLevel.Error;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataIOException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public DataIOException(string message, Exception inner)
            : base(message, inner)
        {
            this.Timestamp = DateTimeOffset.Now;
            this.Severity = SeverityLevel.Error;
        }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public SeverityLevel Severity { get; set; }

        /// <summary>
        /// Wraps, if necessary, the exception into a <see cref="DataIOException"/>.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>A <see cref="DataIOException"/>.</returns>
        public static DataIOException FromException(Exception ex)
        {
            Requires.NotNull(ex, nameof(ex));

            var dex = ex as DataIOException;
            return dex ?? new DataIOException(Strings.DataIOException_FromException_UnexpectedMessage, ex);
        }

        /// <summary>Creates and returns a string representation of the current exception.</summary>
        /// <returns>A string representation of the current exception.</returns>
        public override string ToString()
        {
            return $"{this.Timestamp:s} {base.ToString()}";
        }
    }
}