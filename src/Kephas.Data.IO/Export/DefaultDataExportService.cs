// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataExportService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default data export service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using Kephas.Data.IO.Internal;

namespace Kephas.Data.IO.Export
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Client.Queries;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.ExceptionHandling;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default data export service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataExportService : IDataExportService
    {
        private readonly IClientQueryProcessor clientQueryExecutor;
        private readonly IContextFactory contextFactory;
        private readonly IDataStreamWriteService dataStreamWriteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataExportService"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="dataStreamWriteService">The data source write service.</param>
        /// <param name="clientQueryExecutor">The client query executor.</param>
        public DefaultDataExportService(
            IContextFactory contextFactory,
            IDataStreamWriteService dataStreamWriteService,
            IClientQueryProcessor clientQueryExecutor)
        {
            Requires.NotNull(contextFactory, nameof(contextFactory));
            Requires.NotNull(dataStreamWriteService, nameof(dataStreamWriteService));
            Requires.NotNull(clientQueryExecutor, nameof(clientQueryExecutor));

            this.contextFactory = contextFactory;
            this.dataStreamWriteService = dataStreamWriteService;
            this.clientQueryExecutor = clientQueryExecutor;
        }

        /// <summary>
        /// Exports data asynchronously.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the export result.
        /// </returns>
        public IOperationResult ExportDataAsync(Action<IDataExportContext>? optionsConfig = null, CancellationToken cancellationToken = default)
        {
            var context = this.CreateDataExportContext(optionsConfig);

            var job = new DataSourceExportJob(context, this.dataStreamWriteService, this.clientQueryExecutor);
            return job.ExecuteAsync(cancellationToken);
        }

        /// <summary>
        /// Creates the data export context.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The new data export context.
        /// </returns>
        protected virtual IDataExportContext CreateDataExportContext(Action<IDataExportContext>? optionsConfig = null)
        {
            var context = this.contextFactory.CreateContext<DataExportContext>();
            optionsConfig?.Invoke(context);
            return context;
        }

        /// <summary>
        /// The export job.
        /// </summary>
        internal class DataSourceExportJob : DataIOJobBase<IDataExportContext>
        {
            private readonly IDataStreamWriteService dataStreamWriteService;
            private readonly IClientQueryProcessor clientQueryExecutor;

            /// <summary>
            /// Initializes a new instance of the <see cref="DataSourceExportJob"/> class.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="dataStreamWriteService">The writer service.</param>
            /// <param name="clientQueryExecutor">The query executor service.</param>
            public DataSourceExportJob(
                IDataExportContext context,
                IDataStreamWriteService dataStreamWriteService,
                IClientQueryProcessor clientQueryExecutor)
                : base(context)
            {
                this.dataStreamWriteService = dataStreamWriteService;
                this.clientQueryExecutor = clientQueryExecutor;
            }

            /// <summary>
            /// Executes the I/O job (core implementation).
            /// </summary>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>
            /// An operation result.
            /// </returns>
            protected override async Task<IOperationResult> ExecuteCoreAsync(CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var data = await this.GetDataAsync(this.Context, cancellationToken).PreserveThreadContext();
                this.Result.PercentCompleted += 0.5f;

                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await this.dataStreamWriteService.WriteAsync(data, this.Context.Output, this.Context, cancellationToken).PreserveThreadContext();
                    this.Result.MergeMessage(string.Format(Strings.DefaultDataExportService_ExportDataAsync_SuccessMessage, this.Context.Output.Name));
                    this.Result.PercentCompleted = 1f;
                }
                catch (Exception ex)
                {
                    this.Result.MergeException(ex);
                }

                return this.Result;
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
                if (context.Query != null)
                {
                    context.Data = await this.clientQueryExecutor.ExecuteQueryAsync(
                        context.Query,
                        ctx =>
                        {
                            ctx.Merge(context);
                            context.QueryExecutionConfig?.Invoke(ctx);
                        },
                        cancellationToken).PreserveThreadContext();
                }

                var data = context.Data;
                if (context.ThrowOnNotFound && (data == null || !data.Any()))
                {
                    throw new NotFoundDataException(Strings.DefaultDataExportService_ExportDataAsync_NoDataException) { Severity = SeverityLevel.Warning };
                }

                return data;
            }
        }
    }
}