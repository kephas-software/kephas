// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteResult.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the execute result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;

    /// <summary>
    /// Encapsulates the result of an execute.
    /// </summary>
    public class ExecuteResult : DataCommandResult, IExecuteResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteResult"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception (optional).</param>
        public ExecuteResult(string message, Exception exception = null)
            : base(message, exception)
        {
        }

        /// <summary>
        /// Gets or sets the execution result.
        /// </summary>
        /// <value>
        /// The execution result.
        /// </value>
        public object Result { get; set; }
    }
}