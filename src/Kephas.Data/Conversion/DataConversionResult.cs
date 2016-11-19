// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConversionResult.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data conversion result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System;
    using Kephas.Dynamic;

    /// <summary>
    /// Encapsulates the result of a data conversion.
    /// </summary>
    public class DataConversionResult : Expando, IDataConversionResult
    {
        /// <summary>
        /// Gets or sets the exception, if one occurred during conversion.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the target object as the result of the conversion.
        /// </summary>
        /// <value>
        /// The target object.
        /// </value>
        public object Target { get; set; }

        /// <summary>
        /// Initializes a new <see cref="DataConversionResult"/> object from the given exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        /// A <see cref="DataConversionResult"/>.
        /// </returns>
        public static DataConversionResult FromException(Exception exception)
        {
            return new DataConversionResult { Exception = exception };
        }

        /// <summary>
        /// Initializes a new <see cref="DataConversionResult"/> object from the given target.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns>
        /// A <see cref="DataConversionResult"/>.
        /// </returns>
        public static DataConversionResult FromTarget(object target)
        {
            return new DataConversionResult { Target = target };
        }
    }
}