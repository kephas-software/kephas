// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOInstallerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data I/O installer base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Setup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Import;
    using Kephas.Data.Setup;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Net.Mime;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An initial data i/o handler base.
    /// </summary>
    public abstract class DataIOInstallerBase : Loggable, IDataInstaller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataIOInstallerBase"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="dataImportService">The data import service.</param>
        /// <param name="dataSpaceFactory">The data space factory.</param>
        protected DataIOInstallerBase(
            IContextFactory contextFactory,
            IDataImportService dataImportService,
            IExportFactory<IDataSpace> dataSpaceFactory)
        {
            Requires.NotNull(contextFactory, nameof(contextFactory));
            Requires.NotNull(dataImportService, nameof(dataImportService));
            Requires.NotNull(dataSpaceFactory, nameof(dataSpaceFactory));

            this.ContextFactory = contextFactory;
            this.DataImportService = dataImportService;
            this.DataSpaceFactory = dataSpaceFactory;
        }

        /// <summary>
        /// Gets the context factory.
        /// </summary>
        /// <value>
        /// The context factory.
        /// </value>
        public IContextFactory ContextFactory { get; }

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
        /// Installs the data asynchronously.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result returning the data creation result.
        /// </returns>
        public virtual async Task<IOperationResult> InstallDataAsync(
            Action<IDataSetupContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            using (var dataSetupContext = this.CreateDataSetupContext(optionsConfig))
            {
                return await this.ImportDataAsync(dataSetupContext, this.GetInstallDataFilePaths(), cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Uninstalls the data asynchronously.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result returning the data creation result.
        /// </returns>
        public virtual async Task<IOperationResult> UninstallDataAsync(
            Action<IDataSetupContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            using (var dataSetupContext = this.CreateDataSetupContext(optionsConfig))
            {
                return await this.ImportDataAsync(dataSetupContext, this.GetUninstallDataFilePaths(), cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Creates the data setup context.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The new data setup context.
        /// </returns>
        protected virtual IDataSetupContext CreateDataSetupContext(Action<IDataSetupContext> optionsConfig = null)
        {
            return this.ContextFactory.CreateContext<DataSetupContext>().Merge(optionsConfig);
        }

        /// <summary>
        /// Gets the files containing data to be installed.
        /// </summary>
        /// <returns>
        /// An enumeration of file paths.
        /// </returns>
        protected virtual IEnumerable<string> GetInstallDataFilePaths()
        {
            return new string[0];
        }

        /// <summary>
        /// Gets the files containing data to be uninstalled.
        /// </summary>
        /// <returns>
        /// An enumeration of file paths.
        /// collection.
        /// </returns>
        protected virtual IEnumerable<string> GetUninstallDataFilePaths()
        {
            return new string[0];
        }

        /// <summary>
        /// Imports the data contained in the provided files asynchronously.
        /// </summary>
        /// <param name="dataSetupContext">Context for the initial data.</param>
        /// <param name="dataFilePaths">The data file paths.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result returning the data setup result.
        /// </returns>
        protected virtual async Task<IOperationResult> ImportDataAsync(
            IDataSetupContext dataSetupContext,
            IEnumerable<string> dataFilePaths,
            CancellationToken cancellationToken = default)
        {
            var result = new OperationResult();
            if (dataFilePaths != null)
            {
                foreach (var dataFilePath in dataFilePaths)
                {
                    var fileResult = await this.ImportDataFileAsync(dataSetupContext, dataFilePath, cancellationToken).PreserveThreadContext();
                    result.MergeResult(fileResult);
                }
            }

            return result;
        }

        /// <summary>
        /// Import data file asynchronously.
        /// </summary>
        /// <param name="dataSetupContext">Context for the initial data.</param>
        /// <param name="dataFilePath">The data file path.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result returning the data import result.
        /// </returns>
        protected virtual async Task<IOperationResult> ImportDataFileAsync(IDataSetupContext dataSetupContext, string dataFilePath, CancellationToken cancellationToken)
        {
            using (var dataSource = this.CreateDataSource(dataFilePath))
            using (var dataSpace = this.DataSpaceFactory.CreateInitializedValue(dataSetupContext))
            {
                var result = new OperationResult();
                try
                {
                    var importResult = await this.DataImportService
                                     .ImportDataAsync(dataSource, this.GetDataImportConfig(dataSetupContext, dataSpace), cancellationToken)
                                     .PreserveThreadContext();
                    result.MergeResult(importResult);
                    result.Messages?.ForEach(m => this.Logger.Info($"{m.Timestamp}: {m.Message}"));
                    var errorMessage = $"Exception while importing {dataFilePath}.";
                    result.Exceptions?.ForEach(e => this.Logger.Error(e, errorMessage));
                    return result;
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Exception while importing {dataFilePath}.";
                    this.Logger.Error(ex, errorMessage);
                    result.MergeException(ex);
                    return result;
                }
            }
        }

        /// <summary>
        /// Gets the data import configuration.
        /// </summary>
        /// <param name="dataSetupContext">Context for the data setup.</param>
        /// <param name="dataSpace">The data space.</param>
        /// <returns>
        /// The data import configuration.
        /// </returns>
        protected virtual Action<IDataImportContext> GetDataImportConfig(IDataSetupContext dataSetupContext, IDataSpace dataSpace)
        {
            return ctx => ctx.DataSpace(dataSpace);
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