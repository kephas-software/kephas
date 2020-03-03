// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullLicensingManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null licensing manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Licensing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Services;

    /// <summary>
    /// A null licensing manager.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullLicensingManager : ILicensingManager
    {
        /// <summary>
        /// Checks the license for the provided application identity asynchronously.
        /// </summary>
        /// <param name="appId">Identifier for the application.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the check license result.
        /// </returns>
        public Task<ILicenseCheckResult> CheckLicenseAsync(AppIdentity appId, IContext? context = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<ILicenseCheckResult>(new LicenseCheckResult(appId, true));
        }

        /// <summary>
        /// Gets the license for the provided application identity asynchronously.
        /// </summary>
        /// <param name="appIdentity">Identifier for the application.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the license data.
        /// </returns>
        public virtual Task<LicenseData?> GetLicenseAsync(AppIdentity appIdentity, IContext? context = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<LicenseData?>(this.GetLicenseData(appIdentity));
        }

#if NETSTANDARD2_1
        /// <summary>
        /// Gets the app licensing state.
        /// </summary>
        /// <param name="appId">Identifier for the application.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The licensing state.
        /// </returns>
        public ILicenseCheckResult CheckLicense(AppIdentity appId, IContext? context = null)
        {
            return new LicenseCheckResult(appId, true);
        }

        /// <summary>
        /// Gets the license for the provided application identity.
        /// </summary>
        /// <param name="appIdentity">Identifier for the application.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The license data.
        /// </returns>
        public LicenseData? GetLicense(AppIdentity appIdentity, IContext? context = null)
        {
            return this.GetLicenseData(appIdentity);
        }
#endif

        private LicenseData? GetLicenseData(AppIdentity appIdentity)
        {
            return new LicenseData(
                Guid.NewGuid().ToString(),
                appIdentity.Id,
                "*",
                "<null>",
                "<null>",
                "<null>");
        }
    }
}
