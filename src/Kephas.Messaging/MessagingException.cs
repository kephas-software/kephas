// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;

    using Kephas.ExceptionHandling;

    /// <summary>
    /// Exception for signalling messaging errors.
    /// </summary>
    public class MessagingException : Exception, ISeverityQualifiedException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MessagingException(string message)
            : base(message)
        {
            this.Severity = SeverityLevel.Error;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingException"/> class.
        /// </summary>
        /// <param name="exceptionData">Information describing the exception.</param>
        public MessagingException(ExceptionData exceptionData)
            : base(exceptionData.Message)
        {
            this.Severity = exceptionData.Severity;
            this.OriginalExceptionType = exceptionData.ExceptionType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingException"/>
        ///  class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public MessagingException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public SeverityLevel Severity { get; }

        /// <summary>
        /// Gets the type of the original exception.
        /// </summary>
        /// <value>
        /// The type of the original exception.
        /// </value>
        public string OriginalExceptionType { get; }
    }
}