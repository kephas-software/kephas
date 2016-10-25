// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataCommandResult.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data command result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;

    /// <summary>
    /// Encapsulates the result of a data command.
    /// </summary>
    public class DataCommandResult : IDataCommandResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataCommandResult"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">(Optional) The exception.</param>
        public DataCommandResult(string message, Exception exception = null)
        {
            this.Message = message;
            this.Exception = exception;
        }

        /// <summary>
        /// Gets the result message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; }
    }
}