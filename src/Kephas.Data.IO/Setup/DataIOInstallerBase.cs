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
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An initial data i/o handler base.
    /// </summary>
    public abstract class DataIOInstallerBase : IDataInstaller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataIOInstallerBase"/> class.
        /// </summary>
        /// <param name="dataImportService">The data import service.</param>
        /// <param name="dataSpaceFactory">The data space factory.</param>
        protected DataIOInstallerBase(
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
        public ILogger<IDataInstaller> Logger { get; set; }

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
        /// <param name="dataSetupContext">Context for the data setup.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result returning the data creation result.
        /// </returns>
        public virtual Task<IOperationResult> InstallDataAsync(
            IDataSetupContext dataSetupContext,
            CancellationToken cancellationToken = default)
        {
            return this.ImportDataAsync(dataSetupContext, this.GetInstallDataFilePaths(), cancellationToken);
        }

        /// <summary>
        /// Uninstalls the data asynchronously.
        /// </summary>
        /// <param name="dataSetupContext">Context for the data setup.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result returning the data creation result.
        /// </returns>
        public virtual Task<IOperationResult> UninstallDataAsync(
            IDataSetupContext dataSetupContext,
            CancellationToken cancellationToken = default)
        {
            return this.ImportDataAsync(dataSetupContext, this.GetUninstallDataFilePaths(), cancellationToken);
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
            using (var dataSpace = this.DataSpaceFactory.CreateExportedValue(dataSetupContext))
            {
                var importContext = this.CreateDataImportContext(dataSetupContext, dataSpace);

                var result = new OperationResult();
                try
                {
                    var importResult = await this.DataImportService
                                     .ImportDataAsync(dataSource, importContext, cancellationToken)
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
        /// Creates the data import context.
        /// </summary>
        /// <param name="dataSetupContext">Context for the initial data.</param>
        /// <param name="dataSpace">The data space.</param>
        /// <returns>
        /// The new data import context.
        /// </returns>
        protected virtual IDataImportContext CreateDataImportContext(
            IDataSetupContext dataSetupContext,
            IDataSpace dataSpace)
        {
            return new DataImportContext(dataSpace, dataSetupContext);
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