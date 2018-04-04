// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConversionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        /// <param name="sourceDataContext">The source data context (optional).</param>
        /// <param name="targetDataContext">The target data context (optional).</param>
        /// <param name="rootSourceType">The type of the source root object (optional).</param>
        /// <param name="rootTargetType">The type of the target root object (optional).</param>
        public DataConversionContext(
                IDataConversionService conversionService,
                IDataContext sourceDataContext = null,
                IDataContext targetDataContext = null,
                Type rootSourceType = null,
                Type rootTargetType = null)
            : base(conversionService.AmbientServices)
        {
            Requires.NotNull(conversionService, nameof(conversionService));

            this.DataConversionService = conversionService;
            this.SourceDataContext = sourceDataContext;
            this.TargetDataContext = targetDataContext;
            this.RootSourceType = rootSourceType;
            this.RootTargetType = rootTargetType;
            this.ThrowOnError = true;
        }

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
        /// Gets or sets a <see cref="IDataContext"/> for the source object.
        /// </summary>
        /// <value>
        /// The source data context.
        /// </value>
        public IDataContext SourceDataContext { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="IDataContext"/> for the target object.
        /// </summary>
        /// <value>
        /// The target data context.
        /// </value>
        public IDataContext TargetDataContext { get; set; }

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