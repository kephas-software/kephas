// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the serialization exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;

    /// <summary>
    /// Exception for signalling serialization errors.
    /// </summary>
    public class SerializationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SerializationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public SerializationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
