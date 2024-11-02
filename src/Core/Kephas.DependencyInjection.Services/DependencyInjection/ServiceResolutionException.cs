// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceResolutionException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection
{
    /// <summary>
    /// Exception occurring when a service cannot be resolved.
    /// </summary>
    public class ServiceResolutionException : ServiceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResolutionException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ServiceResolutionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResolutionException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public ServiceResolutionException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Gets the service type to resolve.
        /// </summary>
        public Type? ServiceType { get; init; }
    }
}