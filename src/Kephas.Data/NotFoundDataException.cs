// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotFoundDataException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the not found data exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;

    /// <summary>
    /// Exception for signalling not found data errors.
    /// </summary>
    public class NotFoundDataException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundDataException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NotFoundDataException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundDataException"/>
        ///  class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public NotFoundDataException(string message, Exception inner) 
            : base(message, inner)
        {
        }
    }
}