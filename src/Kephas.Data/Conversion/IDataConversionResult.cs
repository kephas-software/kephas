// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataConversionResult.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataConversionResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System;
    using Kephas.Dynamic;

    /// <summary>
    /// Interface for the data conversion result.
    /// </summary>
    public interface IDataConversionResult : IExpando
    {
        /// <summary>
        /// Gets or sets the exception, if one occurred during conversion.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the target object as the result of the conversion.
        /// </summary>
        /// <remarks>
        /// In the case that no target object was provided, 
        /// the converter tries to identify one in the target data context 
        /// based on the type and the ID from the source. The identified target 
        /// is then set in the <see cref="Target"/> property of the <see cref="IDataConversionResult"/>, 
        /// because the input target parameter in the <see cref="IDataConversionService.ConvertAsync"/>
        /// method is an input parameter.
        /// </remarks>
        /// <value>
        /// The target object.
        /// </value>
        object Target { get; set; }
    }
}