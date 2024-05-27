// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Kephas.Data
{
    using System;

    using Kephas.ExceptionHandling;

    /// <summary>
    /// Exception for signalling generic data errors.
    /// </summary>
    public class DataException : Exception, IErrorNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DataException(string message)
            : base(message)
        {
            this.Severity = LogLevel.Error;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataException"/>
        ///  class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public DataException(string message, Exception inner)
            : base(message, inner)
        {
            this.Severity = LogLevel.Error;
        }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public LogLevel Severity { get; set; }
    }
}
