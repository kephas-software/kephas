// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataImportService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using Kephas.Operations;
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
        Task<IOperationResult> ImportDataAsync(DataStream dataSource, IDataImportContext context, CancellationToken cancellationToken = default);
    }
}