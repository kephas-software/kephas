// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the workflow exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow
{
    using System;

    /// <summary>
    /// Exception for signalling workflow errors.
    /// </summary>
    public class WorkflowException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public WorkflowException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public WorkflowException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}