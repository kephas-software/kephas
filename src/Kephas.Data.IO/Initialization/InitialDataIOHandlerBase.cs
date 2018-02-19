// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitialDataIOHandlerBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the initial data i/o handler base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Initialization
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Data.Initialization;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Import;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Net.Mime;
    using Kephas.Reflection;
    using Kephas.Services;
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
        /// <param name="sourceDataContextProvider">Source data context provider.</param>
        /// <param name="targetDataContextProvider">Target data context provider.</param>
        protected InitialDataIOHandlerBase(
            IDataImportService dataImportService,
            Func<IContext, IDataContext> sourceDataContextProvider,
            Func<IContext, IDataContext> targetDataContextProvider)
        {
            Requires.NotNull(dataImportService, nameof(dataImportService));
            Requires.NotNull(sourceDataContextProvider, nameof(sourceDataContextProvider));
            Requires.NotNull(targetDataContextProvider, nameof(targetDataContextProvider));

            this.DataImportService = dataImportService;
            this.SourceDataContextProvider = sourceDataContextProvider;
            this.TargetDataContextProvider = targetDataContextProvider;
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
        /// Gets the source data context provider.
        /// </summary>
        /// <value>
        /// The source data context provider.
        /// </value>
        protected Func<IContext, IDataContext> SourceDataContextProvider { get; }

        /// <summary>
        /// Gets the target data context provider.
        /// </summary>
        /// <value>
        /// The target data context provider.
        /// </value>
        protected Func<IContext, IDataContext> TargetDataContextProvider { get; }

        /// <summary>
        /// Creates the initial data asynchronously.
        /// </summary>
        /// <param name="initialDataContext">Context for the initial data.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// An asynchronous result returning the data creation result.
        /// </returns>
        public virtual async Task<object> CreateDataAsync(IContext initialDataContext, CancellationToken cancellationToken = default)
        {
            var result = new DataIOResult();
            foreach (var dataFile in this.GetDataFiles())
            {
                var fileResult = await this.ImportDataFileAsync(initialDataContext, dataFile, cancellationToken).PreserveThreadContext();
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
        protected abstract IEnumerable<string> GetDataFiles();

        /// <summary>
        /// Import data file asynchronously.
        /// </summary>
        /// <param name="initialDataContext">Context for the initial data.</param>
        /// <param name="dataFileName">The data file name.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result returning the data import result.
        /// </returns>
        protected virtual async Task<IDataIOResult> ImportDataFileAsync(IContext initialDataContext, string dataFileName, CancellationToken cancellationToken)
        {
            using (var dataSource = this.CreateDataSource(dataFileName))
            using (var sourceDataContext = this.SourceDataContextProvider(initialDataContext))
            using (var targetDataContext = this.TargetDataContextProvider(initialDataContext))
            {
                var importContext = this.CreateDataImportContext(initialDataContext, sourceDataContext, targetDataContext);
                
                try
                {
                    var result = await this.DataImportService
                                     .ImportDataAsync(dataSource, importContext, cancellationToken)
                                     .PreserveThreadContext();
                    result.Messages?.ForEach(m => this.Logger.Info($"{m.Timestamp}: {m.Message}"));
                    result.Exceptions?.ForEach(e => this.Logger.Error(e, ""));
                    return result;
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Exception while importing {dataFileName}.";
                    this.Logger.Error(ex, errorMessage);
                    var result = new DataIOResult();
                    result.Exceptions.TryAdd(ex as DataIOException ?? new DataIOException(errorMessage, ex));
                    return result;
                }
            }
        }

        /// <summary>
        /// Creates the data import context.
        /// </summary>
        /// <param name="initialDataContext">Context for the initial data.</param>
        /// <param name="sourceDataContext">Context for the source data.</param>
        /// <param name="targetDataContext">Context for the target data.</param>
        /// <returns>
        /// The new data import context.
        /// </returns>
        protected virtual IDataImportContext CreateDataImportContext(
            IContext initialDataContext,
            IDataContext sourceDataContext,
            IDataContext targetDataContext)
        {
            return new DataImportContext(sourceDataContext, targetDataContext);
        }

        /// <summary>
        /// Creates an output data source.
        /// </summary>
        /// <param name="dataFileName">The data file name.</param>
        /// <returns>
        /// The new data source.
        /// </returns>
        protected virtual DataStream CreateDataSource(string dataFileName)
        {
            var assembly = this.GetType().AsRuntimeTypeInfo().TypeInfo.Assembly;
            var assemblyNamespace = assembly.GetName().Name;
            var data = assembly.GetManifestResourceStream($"{assemblyNamespace}.Data.{dataFileName}");
            return new DataStream(data, dataFileName, MediaTypeNames.Application.Json, ownsStream: true);
        }
    }
}