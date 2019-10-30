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
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// Interface for data export service.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IDataExportService
    {
        /// <summary>
        /// Exports data asynchronously.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the export result.
        /// </returns>
        Task<IOperationResult> ExportDataAsync(Action<IDataExportContext> optionsConfig = null, CancellationToken cancellationToken = default);
    }
}