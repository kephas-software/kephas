// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataInstaller.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataInstaller interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Setup
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Setup.AttributedModel;
    using Kephas.Services;

    /// <summary>
    /// Application service contract for handling initial data.
    /// </summary>
    [SharedAppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(DataKindAttribute) })]
    public interface IDataInstaller
    {
        /// <summary>
        /// Installs data asynchronously.
        /// </summary>
        /// <param name="dataSetupContext">Context for the data setup.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result returning the data setup result.
        /// </returns>
        Task<IDataSetupResult> InstallDataAsync(
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
        Task<IDataSetupResult> UninstallDataAsync(
            IDataSetupContext dataSetupContext,
            CancellationToken cancellationToken = default);
    }
}