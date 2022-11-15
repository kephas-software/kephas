// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Exception which occurs on composition errors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection
{
    using System;

    /// <summary>
    /// Exception which occurs on injection errors.
    /// </summary>
    public class InjectionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InjectionException"/> class.
        /// </summary>
        public InjectionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectionException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InjectionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public InjectionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}