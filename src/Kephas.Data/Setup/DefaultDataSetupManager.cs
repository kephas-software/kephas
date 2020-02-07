// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataSetupManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default initial data service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Setup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Data.Setup.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default data setup manager service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataSetupManager : IDataSetupManager
    {
        private readonly IContextFactory contextFactory;
        private readonly ICollection<IExportFactory<IDataInstaller, DataInstallerMetadata>> dataInstallerFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataSetupManager"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="dataInstallerFactories">The data installer factories.</param>
        public DefaultDataSetupManager(IContextFactory contextFactory, ICollection<IExportFactory<IDataInstaller, DataInstallerMetadata>> dataInstallerFactories)
        {
            Requires.NotNull(contextFactory, nameof(contextFactory));
            Requires.NotNull(dataInstallerFactories, nameof(dataInstallerFactories));

            this.contextFactory = contextFactory;
            this.dataInstallerFactories = dataInstallerFactories;
        }

        /// <summary>
        /// Installs data asynchronously.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result returning the data setup result.
        /// </returns>
        /// <example>
        /// .
        /// </example>
        public async Task<IOperationResult> InstallDataAsync(
            Action<IDataSetupContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            using (var dataSetupContext = this.CreateDataSetupContext(optionsConfig))
            {
                var dataInstallers = this.GetOrderedDataInstallers(dataSetupContext);

                var result = new OperationResult();
                foreach (var dataInstaller in dataInstallers)
                {
                    var handlerResult = await dataInstaller.InstallDataAsync(optionsConfig, cancellationToken).PreserveThreadContext();
                    if (handlerResult != null)
                    {
                        result.MergeMessages(handlerResult);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Uninstalls data asynchronously.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result returning the data setup result.
        /// </returns>
        public async Task<IOperationResult> UninstallDataAsync(
            Action<IDataSetupContext> optionsConfig = null,
            CancellationToken cancellationToken = default)
        {
            using (var dataSetupContext = this.CreateDataSetupContext(optionsConfig))
            {
                var dataInstallers = this.GetOrderedDataInstallers(dataSetupContext);
                dataInstallers.Reverse();

                var result = new OperationResult();
                foreach (var dataInstaller in dataInstallers)
                {
                    var handlerResult = await dataInstaller.UninstallDataAsync(optionsConfig, cancellationToken).PreserveThreadContext();
                    if (handlerResult != null)
                    {
                        result.MergeMessages(handlerResult);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Creates data setup context.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The new data setup context.
        /// </returns>
        protected virtual IDataSetupContext CreateDataSetupContext(Action<IDataSetupContext> optionsConfig = null)
        {
            return this.contextFactory.CreateContext<DataSetupContext>().Merge(optionsConfig);
        }

        /// <summary>
        /// Gets the ordered data installers.
        /// </summary>
        /// <param name="dataSetupContext">Context for the data setup.</param>
        /// <returns>
        /// The ordered data installers.
        /// </returns>
        protected virtual List<IDataInstaller> GetOrderedDataInstallers(IDataSetupContext dataSetupContext)
        {
            var targets = this.GetTargets(dataSetupContext);
            var dataInstallers = this.dataInstallerFactories
                .Order()
                .GetServices(f => targets == null || targets.Contains(f.Metadata.Target))
                .ToList();
            return dataInstallers;
        }

        /// <summary>
        /// Gets the targets.
        /// </summary>
        /// <param name="dataSetupContext">Context for the data setup.</param>
        /// <returns>
        /// The targets.
        /// </returns>
        protected virtual IList<string> GetTargets(IDataSetupContext dataSetupContext)
        {
            return dataSetupContext?.Targets?.ToList();
        }
    }
}