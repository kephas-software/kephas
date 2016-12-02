// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConversionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data conversion context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System.Diagnostics.Contracts;

    using Kephas.Services;

    /// <summary>
    /// A data conversion context.
    /// </summary>
    public class DataConversionContext : ContextBase, IDataConversionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataConversionContext"/> class.
        /// </summary>
        /// <param name="conversionService">The conversion service.</param>
        /// <param name="sourceDataContext">The source data context (optional).</param>
        /// <param name="targetDataContext">The target data context (optional).</param>
        public DataConversionContext(IDataConversionService conversionService, IDataContext sourceDataContext = null, IDataContext targetDataContext = null)
            : base(conversionService.AmbientServices)
        {
            Contract.Requires(conversionService != null);

            this.SourceDataContext = sourceDataContext;
            this.TargetDataContext = targetDataContext;
            this.ThrowOnError = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to throw an exception when an error occurs.
        /// </summary>
        /// <value>
        /// <c>true</c> to throw an error on exceptions, <c>false</c> if not.
        /// </value>
        public bool ThrowOnError { get; set; }

        /// <summary>
        /// Gets a <see cref="IDataContext"/> for the source object.
        /// </summary>
        /// <value>
        /// The source data context.
        /// </value>
        public IDataContext SourceDataContext { get; }

        /// <summary>
        /// Gets a <see cref="IDataContext"/> for the target object.
        /// </summary>
        /// <value>
        /// The target data context.
        /// </value>
        public IDataContext TargetDataContext { get; }
    }
}