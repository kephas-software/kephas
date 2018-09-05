// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbiguousMatchDataException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ambiguous match data exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;

    /// <summary>
    /// Exception for signalling ambiguous match data errors.
    /// </summary>
    public class AmbiguousMatchDataException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbiguousMatchDataException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public AmbiguousMatchDataException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbiguousMatchDataException"/>
        ///  class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public AmbiguousMatchDataException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}