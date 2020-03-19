// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidTransitionException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the InvalidTransitionException class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow
{
    using System;

    /// <summary>
    /// Exception for signalling invalid transition errors.
    /// </summary>
    public class InvalidTransitionException : WorkflowException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTransitionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidTransitionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTransitionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public InvalidTransitionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}