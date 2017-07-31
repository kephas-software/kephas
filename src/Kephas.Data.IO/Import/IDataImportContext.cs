// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataImportContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataImportContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Import
{
    using Kephas.Data;
    using Kephas.Data.IO;

    /// <summary>
    /// Contract for data import contexts.
    /// </summary>
    public interface IDataImportContext : IDataIOContext
    {
        /// <summary>
        /// Gets or sets the source data context.
        /// </summary>
        /// <value>
        /// The source data context.
        /// </value>
        IDataContext SourceDataContext { get; set; }

        /// <summary>
        /// Gets or sets the target data context.
        /// </summary>
        /// <value>
        /// The target data context.
        /// </value>
        IDataContext TargetDataContext { get; set; }
    }
}