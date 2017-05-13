// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data i/o exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using System;

    /// <summary>
    /// Exception for signalling data I/O errors.
    /// </summary>
    public class DataIOException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataIOException"/> class.
        /// </summary>
        public DataIOException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataIOException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DataIOException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataIOException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public DataIOException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}