// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataImportContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data import context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Import
{
    using System;

    using Kephas.Data;
    using Kephas.Data.Commands;
    using Kephas.Data.Conversion;
    using Kephas.Data.IO;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A data import context.
    /// </summary>
    public class DataImportContext : DataIOContext, IDataImportContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataImportContext"/> class.
        /// </summary>
        /// <param name="sourceDataContext">The source data context.</param>
        /// <param name="targetDataContext">The target data context.</param>
        public DataImportContext(IDataContext sourceDataContext, IDataContext targetDataContext)
        {
            Requires.NotNull(sourceDataContext, nameof(sourceDataContext));
            Requires.NotNull(targetDataContext, nameof(targetDataContext));

            this.SourceDataContext = sourceDataContext;
            this.TargetDataContext = targetDataContext;
        }

        /// <summary>
        /// Gets or sets the source data context.
        /// </summary>
        /// <value>
        /// The source data context.
        /// </value>
        public IDataContext SourceDataContext { get; set; }

        /// <summary>
        /// Gets or sets the target data context.
        /// </summary>
        /// <value>
        /// The target data context.
        /// </value>
        public IDataContext TargetDataContext { get; set; }

        /// <summary>
        /// Gets or sets the data conversion context configuration.
        /// </summary>
        /// <value>
        /// The data conversion context configuration.
        /// </value>
        public Action<object, IDataConversionContext> DataConversionContextConfig { get; set; }

        /// <summary>
        /// Gets or sets the persist changes context configuration.
        /// </summary>
        /// <value>
        /// The persist changes context configuration.
        /// </value>
        public Action<IPersistChangesContext> PersistChangesContextConfig { get; set; }
    }
}