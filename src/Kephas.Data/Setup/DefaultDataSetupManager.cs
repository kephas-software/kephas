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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Data.Setup.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default data setup manager service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataSetupManager : IDataSetupManager
    {
        /// <summary>
        /// The data installer factories.
        /// </summary>
        private readonly ICollection<IExportFactory<IDataInstaller, DataInstallerMetadata>> dataInstallerFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataSetupManager"/> class.
        /// </summary>
        /// <param name="dataInstallerFactories">The data installer factories.</param>
        public DefaultDataSetupManager(ICollection<IExportFactory<IDataInstaller, DataInstallerMetadata>> dataInstallerFactories)
        {
            Requires.NotNull(dataInstallerFactories, nameof(dataInstallerFactories));

            this.dataInstallerFactories = dataInstallerFactories;
        }

        /// <summary>
        /// Installs data asynchronously.
        /// </summary>
        /// <param name="dataSetupContext">Context for the data setup.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result returning the data setup result.
        /// </returns>
        public async Task<object> InstallDataAsync(
            IDataSetupContext dataSetupContext,
            CancellationToken cancellationToken = default)
        {
            var dataInstallers = this.GetOrderedDataInstallers(dataSetupContext);

            var result = new List<object>();
            foreach (var dataInstaller in dataInstallers)
            {
                var handlerResult = await dataInstaller.InstallDataAsync(dataSetupContext, cancellationToken).PreserveThreadContext();
                if (handlerResult != null)
                {
                    result.Add(handlerResult);
                }
            }

            return result;
        }

        /// <summary>
        /// Uninstalls data asynchronously.
        /// </summary>
        /// <param name="dataSetupContext">Context for the data setup.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result returning the data setup result.
        /// </returns>
        public async Task<object> UninstallDataAsync(
            IDataSetupContext dataSetupContext,
            CancellationToken cancellationToken = default)
        {
            var dataInstallers = this.GetOrderedDataInstallers(dataSetupContext);
            dataInstallers.Reverse();

            var result = new List<object>();
            foreach (var dataInstaller in dataInstallers)
            {
                var handlerResult = await dataInstaller.UninstallDataAsync(dataSetupContext, cancellationToken).PreserveThreadContext();
                if (handlerResult != null)
                {
                    result.Add(handlerResult);
                }
            }

            return result;
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
            var dataInstallers = this.dataInstallerFactories.OrderBy(f => f.Metadata.OverridePriority)
                .ThenBy(f => f.Metadata.ProcessingPriority)
                .Where(f => targets == null || targets.Contains(f.Metadata.Target)).Select(f => f.CreateExportedValue())
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