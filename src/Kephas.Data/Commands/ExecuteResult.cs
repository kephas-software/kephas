// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
        /// <param name="result">Optional. The execution result.</param>
        /// <param name="message">Optional. The message.</param>
        /// <param name="exception">Optional. The exception.</param>
        public ExecuteResult(object? result = null, string? message = null, Exception? exception = null)
            : base(message, exception)
        {
            this.Value = result;
        }
    }
}