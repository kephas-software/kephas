// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConversionException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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