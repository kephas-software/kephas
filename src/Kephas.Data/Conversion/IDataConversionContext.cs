// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataConversionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataConversionContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Contract interface for data conversion contexts.
    /// </summary>
    public interface IDataConversionContext : IContext
    {
        /// <summary>
        /// Gets the data conversion service.
        /// </summary>
        /// <value>
        /// The data conversion service.
        /// </value>
        IDataConversionService DataConversionService { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to throw an exception when an error occurs.
        /// </summary>
        /// <value>
        /// <c>true</c> to throw an error on exceptions, <c>false</c> if not.
        /// </value>
        bool ThrowOnError { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="IDataContext"/> for the source object.
        /// </summary>
        /// <value>
        /// The source data context.
        /// </value>
        IDataContext SourceDataContext { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="IDataContext"/> for the target object.
        /// </summary>
        /// <value>
        /// The target data context.
        /// </value>
        IDataContext TargetDataContext { get; set; }

        /// <summary>
        /// Gets or sets the type of the root source.
        /// </summary>
        /// <value>
        /// The type of the root source.
        /// </value>
        Type RootSourceType { get; set; }

        /// <summary>
        /// Gets or sets the type of the root target.
        /// </summary>
        /// <value>
        /// The type of the root target.
        /// </value>
        Type RootTargetType { get; set; }
    }
}