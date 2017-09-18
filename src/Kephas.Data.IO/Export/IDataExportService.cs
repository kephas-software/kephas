// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataExportService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataExportService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Export
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Interface for data export service.
    /// </summary>
    [SharedAppServiceContract]
    public interface IDataExportService
    {
        /// <summary>
        /// Exports data asynchronously.
        /// </summary>
        /// <param name="context">The export context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A data export result.
        /// </returns>
        Task<IDataIOResult> ExportDataAsync(IDataExportContext context, CancellationToken cancellationToken = default);
    }
}