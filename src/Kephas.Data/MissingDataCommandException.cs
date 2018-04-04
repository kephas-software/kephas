// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MissingDataCommandException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the missing data command exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;

    /// <summary>
    /// Exception for signalling missing data command errors.
    /// </summary>
    public class MissingDataCommandException : DataException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingDataCommandException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MissingDataCommandException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingDataCommandException"/>
        ///  class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public MissingDataCommandException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}