// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConversionException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System;

    /// <summary>
    /// Exception for signalling data conversion errors.
    /// </summary>
    public class DataConversionException : DataException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataConversionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DataConversionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataConversionException"/>
        ///  class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public DataConversionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}