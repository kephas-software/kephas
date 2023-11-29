// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Exception which occurs on composition errors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    /// <summary>
    /// Exception which occurs on injection errors.
    /// </summary>
    public class AppServiceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceException"/> class.
        /// </summary>
        public AppServiceException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AppServiceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public AppServiceException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
