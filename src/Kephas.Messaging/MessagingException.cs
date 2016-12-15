// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the messaging exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using System;

    /// <summary>
    /// Exception for signalling messaging errors.
    /// </summary>
    public class MessagingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MessagingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingException"/>
        ///  class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public MessagingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}