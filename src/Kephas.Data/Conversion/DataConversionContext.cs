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
    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// A data conversion context.
    /// </summary>
    public class DataConversionContext : ContextBase, IDataConversionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataConversionContext"/> class.
        /// </summary>
        /// <param name="sourceDataContext">The source data context.</param>
        /// <param name="targetDataContext">The target data context.</param>
        public DataConversionContext(IDataContext sourceDataContext = null, IDataContext targetDataContext = null)
        {
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