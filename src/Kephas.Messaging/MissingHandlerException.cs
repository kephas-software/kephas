// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MissingHandlerException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the missing handler messaging exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;

    /// <summary>
    /// Exception for signalling missing handler messaging errors.
    /// </summary>
    public class MissingHandlerException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingHandlerException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MissingHandlerException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingHandlerException"/>
        ///  class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public MissingHandlerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}