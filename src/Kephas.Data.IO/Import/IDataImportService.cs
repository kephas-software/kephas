// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataImportService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataImportService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Import
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.IO.DataStreams;
    using Kephas.Services;

    /// <summary>
    /// Interface for data import service.
    /// </summary>
    [SharedAppServiceContract]
    public interface IDataImportService
    {
        /// <summary>
        /// Imports the data asynchronously.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A data import result.
        /// </returns>
        Task<IDataIOResult> ImportDataAsync(DataStream dataSource, IDataImportContext context, CancellationToken cancellationToken = default);
    }
}