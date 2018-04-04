// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataCommandResult.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data command result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;

    using Kephas.Data.Resources;
    using Kephas.Dynamic;

    /// <summary>
    /// Encapsulates the result of a data command.
    /// </summary>
    public class DataCommandResult : Expando, IDataCommandResult
    {
        /// <summary>
        /// The result representing a successful operation.
        /// </summary>
        public static readonly DataCommandResult Success = new DataCommandResult(Strings.DataCommandResult_Successful_Message);

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCommandResult"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception (optional).</param>
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