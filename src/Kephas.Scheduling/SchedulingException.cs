// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulingException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling
{
    using System;

    /// <summary>
    /// Exception for signalling scheduling errors.
    /// </summary>
    public class SchedulingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SchedulingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public SchedulingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}