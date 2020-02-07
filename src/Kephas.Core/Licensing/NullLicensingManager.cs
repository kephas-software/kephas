// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullLicensingManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null licensing manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Licensing
{
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
        public Task<ILicenseCheckResult> CheckLicenseAsync(AppIdentity appId, IContext context = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<ILicenseCheckResult>(new LicenseCheckResult(appId, true));
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
        public ILicenseCheckResult CheckLicense(AppIdentity appId, IContext context = null)
        {
            return new LicenseCheckResult(appId, true);
        }
#endif
    }
}
