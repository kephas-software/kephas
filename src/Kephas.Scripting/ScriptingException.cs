// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptingException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scripting exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using System;

    using Kephas.ExceptionHandling;

    /// <summary>
    /// Exception for signalling scripting errors.
    /// </summary>
    public class ScriptingException : Exception, ISeverityQualifiedNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">Optional. The error code.</param>
        /// <param name="lineNumber">Optional. The line number.</param>
        public ScriptingException(string message, int errorCode = 0, int lineNumber = 0)
            : base(message)
        {
            this.Severity = SeverityLevel.Error;
            this.ErrorCode = errorCode;
            this.LineNumber = lineNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptingException"/>
        ///  class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        /// <param name="errorCode">Optional. The error code.</param>
        /// <param name="lineNumber">Optional. The line number.</param>
        public ScriptingException(string message, Exception inner, int errorCode = 0, int lineNumber = 0)
            : base(message, inner)
        {
            this.Severity = SeverityLevel.Error;
            this.ErrorCode = errorCode;
            this.LineNumber = lineNumber;
        }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public SeverityLevel Severity { get; set; }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public int ErrorCode { get; }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        /// <value>
        /// The line number.
        /// </value>
        public int LineNumber { get; }
    }
}