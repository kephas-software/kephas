// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataExportService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default data export service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Export
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Data.Client.Queries;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.ExceptionHandling;
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
        /// The composition context.
        /// </summary>
        private readonly ICompositionContext compositionContext;

        /// <summary>
        /// The data source write service.
        /// </summary>
        private readonly IDataStreamWriteService dataStreamWriteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataExportService"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataStreamWriteService">The data source write service.</param>
        /// <param name="clientQueryExecutor">The client query executor.</param>
        public DefaultDataExportService(
            ICompositionContext compositionContext,
            IDataStreamWriteService dataStreamWriteService,
            IClientQueryExecutor clientQueryExecutor)
        {
            Requires.NotNull(compositionContext, nameof(compositionContext));
            Requires.NotNull(dataStreamWriteService, nameof(dataStreamWriteService));
            Requires.NotNull(clientQueryExecutor, nameof(clientQueryExecutor));

            this.compositionContext = compositionContext;
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
            Requires.NotNull(context, nameof(context));

            var result = context.EnsureResult();

            cancellationToken.ThrowIfCancellationRequested();

            var data = await this.GetDataAsync(context, cancellationToken).PreserveThreadContext();

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await this.dataStreamWriteService.WriteAsync(data, context.Output, context, cancellationToken).PreserveThreadContext();
                result.MergeMessage(string.Format(Strings.DefaultDataExportService_ExportDataAsync_SuccessMessage, context.Output.Name));
            }
            catch (Exception ex)
            {
                result.MergeException(ex);
            }

            return result;
        }

        /// <summary>
        /// Gets the data to export asynchronously.
        /// </summary>
        /// <param name="context">The export context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// An asynchronous result that yields the data.
        /// </returns>
        protected virtual async Task<IEnumerable<object>> GetDataAsync(IDataExportContext context, CancellationToken cancellationToken)
        {
            IEnumerable<object> data;

            if (context.Query != null)
            {
                var queryExecutionContext = new ClientQueryExecutionContext(context);
                context.ClientQueryExecutionContextConfig?.Invoke(queryExecutionContext);
                data = await this.clientQueryExecutor.ExecuteQueryAsync(context.Query, queryExecutionContext, cancellationToken)
                           .PreserveThreadContext();
            }
            else
            {
                data = context.Data;
            }

            if (context.ThrowOnNotFound && (data == null || !data.Any()))
            {
                throw new NotFoundDataException(Strings.DefaultDataExportService_ExportDataAsync_NoDataException) { Severity = SeverityLevel.Warning };
            }

            return data;
        }
    }
}