// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularDependencyException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the circular dependency exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;

    /// <summary>
    /// Exception for signalling circular dependency errors.
    /// </summary>
    public class CircularDependencyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CircularDependencyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CircularDependencyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularDependencyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public CircularDependencyException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}