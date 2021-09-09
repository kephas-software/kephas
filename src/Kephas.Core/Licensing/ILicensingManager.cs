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
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Interface for licensing manager.
    /// </summary>
    public interface ILicensingManager
    {
        /// <summary>
        /// Checks the license for the provided application identity asynchronously.
        /// </summary>
        /// <param name="appIdentity">Identifier for the application.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the license check result.
        /// </returns>
        Task<ILicenseCheckResult> CheckLicenseAsync(AppIdentity appIdentity, IContext? context = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the license for the provided application identity asynchronously.
        /// </summary>
        /// <param name="appIdentity">Identifier for the application.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the license data.
        /// </returns>
        Task<LicenseData?> GetLicenseAsync(AppIdentity appIdentity, IContext? context = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks the license for the provided application identity.
        /// </summary>
        /// <param name="appIdentity">Identifier for the application.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The license check result.
        /// </returns>
        ILicenseCheckResult CheckLicense(AppIdentity appIdentity, IContext? context = null)
        {
            return this.CheckLicenseAsync(appIdentity, context).GetResultNonLocking();
        }

        /// <summary>
        /// Gets the license for the provided application identity.
        /// </summary>
        /// <param name="appIdentity">Identifier for the application.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The license data.
        /// </returns>
        LicenseData? GetLicense(AppIdentity appIdentity, IContext? context = null)
        {
            return this.GetLicenseAsync(appIdentity, context).GetResultNonLocking();
        }
    }
}
