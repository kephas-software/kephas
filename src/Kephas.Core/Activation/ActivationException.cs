// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivationException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the activation exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Activation
{
    using System;

    /// <summary>
    /// Defines an exception thrown when activation fails.
    /// </summary>
    public class ActivationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ActivationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public ActivationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}