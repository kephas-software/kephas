// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the model exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System;

    /// <summary>
    /// Exception for signalling model errors.
    /// </summary>
    public class ModelException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelException"/> class.
        /// </summary>
        public ModelException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ModelException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public ModelException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}