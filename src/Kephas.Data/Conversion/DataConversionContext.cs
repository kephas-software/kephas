// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConversionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data conversion context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// A data conversion context.
    /// </summary>
    public class DataConversionContext : Context, IDataConversionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataConversionContext"/> class.
        /// </summary>
        /// <param name="conversionService">The conversion service.</param>
        /// <param name="dataSpace">The data space.</param>
        /// <param name="rootSourceType">Optional. The type of the source root object.</param>
        /// <param name="rootTargetType">Optional. The type of the target root object.</param>
        public DataConversionContext(
            IDataConversionService conversionService,
            IDataSpace dataSpace,
            Type rootSourceType = null,
            Type rootTargetType = null)
            : base(dataSpace)
        {
            Requires.NotNull(conversionService, nameof(conversionService));
            Requires.NotNull(dataSpace, nameof(dataSpace));

            this.DataSpace = dataSpace;
            this.DataConversionService = conversionService;
            this.RootSourceType = rootSourceType;
            this.RootTargetType = rootTargetType;
            this.ThrowOnError = true;
        }

        /// <summary>
        /// Gets the data space.
        /// </summary>
        /// <value>
        /// The data space.
        /// </value>
        public IDataSpace DataSpace { get; }

        /// <summary>
        /// Gets the data conversion service.
        /// </summary>
        /// <value>
        /// The data conversion service.
        /// </value>
        public IDataConversionService DataConversionService { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to throw an exception when an error occurs.
        /// </summary>
        /// <value>
        /// <c>true</c> to throw an error on exceptions, <c>false</c> if not.
        /// </value>
        public bool ThrowOnError { get; set; }

        /// <summary>
        /// Gets or sets the type of the source root object.
        /// </summary>
        /// <value>
        /// The type of the source root object.
        /// </value>
        public Type RootSourceType { get; set; }

        /// <summary>
        /// Gets or sets the type of the target root object.
        /// </summary>
        /// <value>
        /// The type of the target root object.
        /// </value>
        public Type RootTargetType { get; set; }
    }
}