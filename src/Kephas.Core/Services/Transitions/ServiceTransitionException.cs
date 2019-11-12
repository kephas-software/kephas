// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceTransitionException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The service transitioning exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Transitioning
{
    using System;

    /// <summary>
    /// The service transition exception.
    /// </summary>
    public class ServiceTransitionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTransitionException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ServiceTransitionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTransitionException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception.</param>
        public ServiceTransitionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}