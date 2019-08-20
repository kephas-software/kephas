// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataSetupManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataSetupManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Setup
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// Application service contract for setting up and uninstalling data.
    /// </summary>
    /// <remarks>
    /// The typical implementation aggregates multiple <see cref="IDataInstaller"/> services
    /// and calls them in their priority order.
    /// </remarks>
    [SingletonAppServiceContract]
    public interface IDataSetupManager
    {
        /// <summary>
        /// Installs data asynchronously.
        /// </summary>
        /// <param name="dataSetupContext">Context for the data setup.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result returning the data setup result.
        /// </returns>
        Task<IOperationResult> InstallDataAsync(
            IDataSetupContext dataSetupContext,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Uninstalls data asynchronously.
        /// </summary>
        /// <param name="dataSetupContext">Context for the data setup.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result returning the data setup result.
        /// </returns>
        Task<IOperationResult> UninstallDataAsync(
            IDataSetupContext dataSetupContext,
            CancellationToken cancellationToken = default);
    }
}