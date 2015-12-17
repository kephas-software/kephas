// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceTransitioningException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The service transitioning exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Transitioning
{
    using System;

    /// <summary>
    /// The service transitioning exception.
    /// </summary>
    public class ServiceTransitioningException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTransitioningException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ServiceTransitioningException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTransitioningException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception.</param>
        public ServiceTransitioningException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}