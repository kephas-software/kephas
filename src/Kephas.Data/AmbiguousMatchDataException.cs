// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbiguousMatchDataException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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