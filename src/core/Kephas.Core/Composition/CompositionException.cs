// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Exception which occurs on composition errors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition
{
    using System;

    /// <summary>
    /// Exception which occurs on composition errors.
    /// </summary>
    public class CompositionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionException"/> class.
        /// </summary>
        public CompositionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CompositionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public CompositionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}