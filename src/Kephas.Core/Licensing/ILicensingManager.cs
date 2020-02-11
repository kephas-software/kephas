// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILicensingManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ILicensingManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

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

#if NETSTANDARD2_1
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
#endif
    }

#if NETSTANDARD2_1
#else
    /// <summary>
    /// Interface for licensing manager.
    /// </summary>
    public interface ISyncLicensingManager
    {
        /// <summary>
        /// Checks the license for the provided application identity asynchronously.
        /// </summary>
        /// <param name="appIdentity">Identifier for the application.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The license check result.
        /// </returns>
        ILicenseCheckResult CheckLicense(AppIdentity appIdentity, IContext? context = null);
    }

    /// <summary>
    /// A licensing manager extensions.
    /// </summary>
    public static class LicensingManagerExtensions
    {
        /// <summary>
        /// Checks the license for the provided application identity asynchronously.
        /// </summary>
        /// <param name="licensingManager">The licensing manager.</param>
        /// <param name="appIdentity">Identifier for the application.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The license check result.
        /// </returns>
        public static ILicenseCheckResult CheckLicense(this ILicensingManager licensingManager, AppIdentity appIdentity, IContext? context = null)
        {
            Requires.NotNull(licensingManager, nameof(licensingManager));

            if (licensingManager is ISyncLicensingManager syncLicensingManager)
            {
                return syncLicensingManager.CheckLicense(appIdentity, context);
            }

            return licensingManager.CheckLicenseAsync(appIdentity, context).GetResultNonLocking();
        }
    }
#endif
}
