// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskTimeoutException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the task timeout exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Exception for signalling task timeout errors.
    /// </summary>
    public class TaskTimeoutException : TimeoutException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskTimeoutException" /> class.
        /// </summary>
        /// <param name="task">The task that timeout.</param>
        public TaskTimeoutException(Task task)
        {
            this.Task = task;
        }

        /// <summary>Initializes a new instance of the <see cref="TaskTimeoutException" /> class with the specified error message.</summary>
        /// <param name="task">The task that timeout.</param>
        /// <param name="message">The message that describes the error. </param>
        public TaskTimeoutException(Task task, string message)
            : base(message)
        {
            this.Task = task;
        }

        /// <summary>Initializes a new instance of the <see cref="TaskTimeoutException" /> class with the specified error message and inner exception.</summary>
        /// <param name="task">The task that timeout.</param>
        /// <param name="message">The message that describes the error. </param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not null, the current exception is raised in a catch block that handles the inner exception. </param>
        public TaskTimeoutException(Task task, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Task = task;
        }

        /// <summary>
        /// Gets the task that timed out.
        /// </summary>
        /// <value>
        /// The task that timed out.
        /// </value>
        public Task Task { get; }
    }
}