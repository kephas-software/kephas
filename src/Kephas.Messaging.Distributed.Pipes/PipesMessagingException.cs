// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipesMessagingException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Pipes
{
    using System;

    /// <summary>
    /// Exception for signalling pipe related messagin exceptions.
    /// </summary>
    public class PipesMessagingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipesMessagingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PipesMessagingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipesMessagingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public PipesMessagingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
