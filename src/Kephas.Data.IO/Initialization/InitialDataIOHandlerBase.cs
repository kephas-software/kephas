// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitialDataIOHandlerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the initial data i/o handler base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Initialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Data.Initialization;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Import;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Net.Mime;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An initial data i/o handler base.
    /// </summary>
    public abstract class InitialDataIOHandlerBase : IInitialDataHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitialDataIOHandlerBase"/> class.
        /// </summary>
        /// <param name="dataImportService">The data import service.</param>
        /// <param name="dataSpaceFactory">The data space factory.</param>
        protected InitialDataIOHandlerBase(
            IDataImportService dataImportService,
            IExportFactory<IDataSpace> dataSpaceFactory)
        {
            Requires.NotNull(dataImportService, nameof(dataImportService));
            Requires.NotNull(dataSpaceFactory, nameof(dataSpaceFactory));

            this.DataImportService = dataImportService;
            this.DataSpaceFactory = dataSpaceFactory;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<IInitialDataHandler> Logger { get; set; }

        /// <summary>
        /// Gets the data import service.
        /// </summary>
        /// <value>
        /// The data import service.
        /// </value>
        public IDataImportService DataImportService { get; }

        /// <summary>
        /// Gets the data space factory.
        /// </summary>
        /// <value>
        /// The data space factory.
        /// </value>
        public IExportFactory<IDataSpace> DataSpaceFactory { get; }

        /// <summary>
        /// Creates the initial data asynchronously.
        /// </summary>
        /// <param name="initialDataContext">Context for the initial data.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// An asynchronous result returning the data creation result.
        /// </returns>
        public virtual async Task<object> CreateDataAsync(
            IInitialDataContext initialDataContext,
            CancellationToken cancellationToken = default)
        {
            var result = new DataIOResult();
            foreach (var dataFilePath in this.GetDataFilePaths())
            {
                var fileResult = await this.ImportDataFileAsync(initialDataContext, dataFilePath, cancellationToken).PreserveThreadContext();
                result.MergeResult(fileResult);
            }

            return result;
        }

        /// <summary>
        /// Gets the data files to be imported.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the data files in this collection.
        /// </returns>
        protected abstract IEnumerable<string> GetDataFilePaths();

        /// <summary>
        /// Import data file asynchronously.
        /// </summary>
        /// <param name="initialDataContext">Context for the initial data.</param>
        /// <param name="dataFilePath">The data file path.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result returning the data import result.
        /// </returns>
        protected virtual async Task<IDataIOResult> ImportDataFileAsync(IInitialDataContext initialDataContext, string dataFilePath, CancellationToken cancellationToken)
        {
            using (var dataSource = this.CreateDataSource(dataFilePath))
            using (var dataSpace = this.DataSpaceFactory.CreateExportedValue())
            {
                dataSpace.Initialize(initialDataContext);
                var importContext = this.CreateDataImportContext(initialDataContext, dataSpace);

                try
                {
                    var result = await this.DataImportService
                                     .ImportDataAsync(dataSource, importContext, cancellationToken)
                                     .PreserveThreadContext();
                    result.Messages?.ForEach(m => this.Logger.Info($"{m.Timestamp}: {m.Message}"));
                    var errorMessage = $"Exception while importing {dataFilePath}.";
                    result.Exceptions?.ForEach(e => this.Logger.Error(e, errorMessage));
                    return result;
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Exception while importing {dataFilePath}.";
                    this.Logger.Error(ex, errorMessage);
                    var result = new DataIOResult();
                    result.MergeException(ex);
                    return result;
                }
            }
        }

        /// <summary>
        /// Creates the data import context.
        /// </summary>
        /// <param name="initialDataContext">Context for the initial data.</param>
        /// <param name="dataSpace">The data space.</param>
        /// <returns>
        /// The new data import context.
        /// </returns>
        protected virtual IDataImportContext CreateDataImportContext(
            IInitialDataContext initialDataContext,
            IDataSpace dataSpace)
        {
            return new DataImportContext(dataSpace, initialDataContext);
        }

        /// <summary>
        /// Creates a data source for the import operation.
        /// </summary>
        /// <param name="dataFilePath">The data file path.</param>
        /// <returns>
        /// The new data source.
        /// </returns>
        protected virtual DataStream CreateDataSource(string dataFilePath)
        {
            if (!File.Exists(dataFilePath))
            {
                // TODO localization
                throw new IOException($"The file {dataFilePath} does not exist.");
            }

            return new DataStream(File.Open(dataFilePath, FileMode.Open, FileAccess.Read), dataFilePath, MediaTypeNames.Application.Json, ownsStream: true);
        }
    }
}