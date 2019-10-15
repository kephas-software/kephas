// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessStartException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the process start exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Diagnostics
{
    using System;

    /// <summary>
    /// Exception for signalling process start errors.
    /// </summary>
    public class ProcessStartException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessStartException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ProcessStartException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessStartException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public ProcessStartException(string message, Exception inner)
            : base(message, inner) { }
    }
}
