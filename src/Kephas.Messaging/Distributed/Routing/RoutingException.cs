// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutingException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the routing exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Routing
{
    using System;

    /// <summary>
    /// Exception for signalling routing errors.
    /// </summary>
    public class RoutingException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public RoutingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public RoutingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
