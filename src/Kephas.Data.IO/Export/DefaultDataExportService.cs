// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataExportService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default data export service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Export
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Client.Queries;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default data export service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataExportService : IDataExportService
    {
        /// <summary>
        /// The client query executor.
        /// </summary>
        private readonly IClientQueryExecutor clientQueryExecutor;

        /// <summary>
        /// The data source write service.
        /// </summary>
        private readonly IDataStreamWriteService dataStreamWriteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataExportService"/> class.
        /// </summary>
        /// <param name="dataStreamWriteService">The data source write service.</param>
        /// <param name="clientQueryExecutor">The client query executor.</param>
        public DefaultDataExportService(
            IDataStreamWriteService dataStreamWriteService,
            IClientQueryExecutor clientQueryExecutor)
        {
            Requires.NotNull(dataStreamWriteService, nameof(dataStreamWriteService));
            Requires.NotNull(clientQueryExecutor, nameof(clientQueryExecutor));

            this.dataStreamWriteService = dataStreamWriteService;
            this.clientQueryExecutor = clientQueryExecutor;
        }

        /// <summary>
        /// Exports data asynchronously.
        /// </summary>
        /// <param name="context">The export context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A data export result.
        /// </returns>
        public async Task<IDataIOResult> ExportDataAsync(IDataExportContext context, CancellationToken cancellationToken = default)
        {
            var result = context.EnsureResult();

            cancellationToken.ThrowIfCancellationRequested();

            var data = await this.clientQueryExecutor.ExecuteQueryAsync(context.Query, cancellationToken)
                           .PreserveThreadContext();

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await this.dataStreamWriteService.WriteAsync(data, context.Output, context, cancellationToken).PreserveThreadContext();
                result.MergeMessage($"Successfully exported date to {context.Output.Name}.");
            }
            catch (Exception ex)
            {
                result.MergeException(ex);
            }

            return result;
        }
    }
}