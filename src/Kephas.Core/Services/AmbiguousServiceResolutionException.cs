// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbiguousServiceResolutionException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    /// <summary>
    /// Occurs when the service resolution is ambiguous.
    /// </summary>
    public class AmbiguousServiceResolutionException : ServiceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbiguousServiceResolutionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public AmbiguousServiceResolutionException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbiguousServiceResolutionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public AmbiguousServiceResolutionException(string message, Exception inner)
            : base(message, inner) { }
    }
}
