// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataExportService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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