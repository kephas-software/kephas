// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILicensingManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ILicensingManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Licensing
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Services;

    /// <summary>
    /// Interface for licensing manager.
    /// </summary>
    [SingletonAppServiceContract]
    public interface ILicensingManager
    {
        /// <summary>
        /// Gets the app licensing state asynchronously.
        /// </summary>
        /// <param name="appId">Identifier for the application.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the licensing state.
        /// </returns>
        Task<ILicensingState> GetLicensingStateAsync(AppIdentity appId, CancellationToken cancellationToken = default);
    }
}
