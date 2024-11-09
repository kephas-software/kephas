// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivationException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the activation exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Activation
{
    using System;

    /// <summary>
    /// Occurs when activation fails.
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