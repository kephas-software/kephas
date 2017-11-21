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
        /// <param name="result">The execution result.</param>
        /// <param name="message">The message (optional).</param>
        /// <param name="exception">The exception (optional).</param>
        public ExecuteResult(object result = null, string message = null, Exception exception = null)
            : base(message, exception)
        {
            this.Result = result;
        }

        /// <summary>
        /// Gets the execution result.
        /// </summary>
        /// <value>
        /// The execution result.
        /// </value>
        public object Result { get; }
    }
}