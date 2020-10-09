// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConversionResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data conversion result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System;

    using Kephas.Operations;

    /// <summary>
    /// Encapsulates the result of a data conversion.
    /// </summary>
    public class DataConversionResult : OperationResult, IDataConversionResult
    {
        /// <summary>
        /// Gets or sets the target object as the result of the conversion.
        /// </summary>
        /// <value>
        /// The target object.
        /// </value>
        public object? Target
        {
            get => this.Value;
            set => this.Value = value;
        }

        /// <summary>
        /// Initializes a new <see cref="DataConversionResult"/> object from the given exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        /// A <see cref="DataConversionResult"/>.
        /// </returns>
        public static DataConversionResult FromException(Exception exception)
        {
            return new DataConversionResult().Fail(exception);
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
            return new DataConversionResult().Value(target).Complete();
        }
    }
}