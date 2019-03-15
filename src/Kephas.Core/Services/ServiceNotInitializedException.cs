// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceNotInitializedException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service not initialized exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    /// <summary>
    /// Exception for signalling service not initialized errors.
    /// </summary>
    public class ServiceNotInitializedException : ServiceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotInitializedException"/> class.
        /// </summary>
        public ServiceNotInitializedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotInitializedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ServiceNotInitializedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotInitializedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public ServiceNotInitializedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}