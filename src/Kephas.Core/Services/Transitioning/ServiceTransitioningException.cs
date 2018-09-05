// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceTransitioningException.cs" company="Kephas Software SRL">
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
    /// The service transitioning exception.
    /// </summary>
    public class ServiceTransitioningException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTransitioningException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ServiceTransitioningException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTransitioningException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception.</param>
        public ServiceTransitioningException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}