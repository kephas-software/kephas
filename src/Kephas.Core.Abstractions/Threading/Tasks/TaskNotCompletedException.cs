// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskNotCompletedException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the task not completed exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Resources;

    /// <summary>
    /// Exception for signalling task not completed errors.
    /// </summary>
    public class TaskNotCompletedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskNotCompletedException" /> class.
        /// </summary>
        /// <param name="task">The task that is not completed.</param>
        public TaskNotCompletedException(Task task)
            : base(Strings.TaskNotCompletedException_Message)
        {
            this.Task = task;
        }

        /// <summary>Initializes a new instance of the <see cref="TaskNotCompletedException" /> class with the specified error message.</summary>
        /// <param name="task">The task that is not completed.</param>
        /// <param name="message">The message that describes the error. </param>
        public TaskNotCompletedException(Task task, string message)
            : base(message)
        {
            this.Task = task;
        }

        /// <summary>Initializes a new instance of the <see cref="TaskNotCompletedException" /> class with the specified error message and inner exception.</summary>
        /// <param name="task">The task that is not completed.</param>
        /// <param name="message">The message that describes the error. </param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not null, the current exception is raised in a catch block that handles the inner exception. </param>
        public TaskNotCompletedException(Task task, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Task = task;
        }

        /// <summary>
        /// Gets the task that is not completed.
        /// </summary>
        /// <value>
        /// The task that is not completed.
        /// </value>
        public Task Task { get; }
    }
}