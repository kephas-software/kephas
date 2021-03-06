// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultLicensingManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default licensing manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Licensing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Cryptography;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;
    using Kephas.Versioning;

    /// <summary>
    /// The default licensing manager.
    /// </summary>
    public class DefaultLicensingManager : ILicensingManager
#if NETSTANDARD2_0
        , ISyncLicensingManager
#endif
    {
        private readonly Func<AppIdentity, IEnumerable<LicenseData>> licenseDataGetter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLicensingManager"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="encryptionService">The encryption service.</param>
        public DefaultLicensingManager(IAppRuntime appRuntime, IEncryptionService encryptionService)
            : this(new LicenseRepository(appRuntime, encryptionService))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLicensingManager"/> class.
        /// </summary>
        /// <param name="licenseRepository">The license repository.</param>
        public DefaultLicensingManager(ILicenseRepository licenseRepository)
            : this(licenseRepository.GetLicenseData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLicensingManager"/> class.
        /// </summary>
        /// <param name="licenseDataGetter">The license data getter.</param>
        public DefaultLicensingManager(Func<AppIdentity, IEnumerable<LicenseData>> licenseDataGetter)
        {
            Requires.NotNull(licenseDataGetter, nameof(licenseDataGetter));

            this.licenseDataGetter = licenseDataGetter;
        }

        /// <summary>
        /// Checks the license for the provided application identity.
        /// </summary>
        /// <param name="appIdentity">Identifier for the application.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The license check result.
        /// </returns>
        public virtual ILicenseCheckResult CheckLicense(AppIdentity appIdentity, IContext? context = null)
        {
            var licenses = this.GetLicenseData(appIdentity);
            var results = licenses.Select(l => this.CheckLicenseData(l, appIdentity, context));
            var successful = results.FirstOrDefault(r => r.IsLicensed);
            if (successful != null)
            {
                return successful;
            }

            return results.Aggregate(
                new LicenseCheckResult(appIdentity, false),
                (acc, r) => acc.MergeMessages(r))
                .Complete();
        }

        /// <summary>
        /// Checks the license for the provided application identity asynchronously.
        /// </summary>
        /// <param name="appIdentity">Identifier for the application.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the check license result.
        /// </returns>
        public virtual async Task<ILicenseCheckResult> CheckLicenseAsync(AppIdentity appIdentity, IContext? context = null, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            return this.CheckLicense(appIdentity, context);
        }

        /// <summary>
        /// Gets the license for the provided application identity.
        /// </summary>
        /// <param name="appIdentity">Identifier for the application.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The license data.
        /// </returns>
        public virtual LicenseData? GetLicense(AppIdentity appIdentity, IContext? context = null)
        {
            return this.GetLicenseData(appIdentity)
                .FirstOrDefault(l => this.CheckLicenseData(l, appIdentity, context).IsLicensed);
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
        public virtual async Task<LicenseData?> GetLicenseAsync(AppIdentity appIdentity, IContext? context = null, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            return this.GetLicense(appIdentity, context);
        }

        /// <summary>
        /// Gets the license data. This is the main extensibility point
        /// when overriding the license retrieval.
        /// </summary>
        /// <param name="appIdentity">Identifier for the application.</param>
        /// <returns>
        /// The license data.
        /// </returns>
        protected virtual IEnumerable<LicenseData> GetLicenseData(AppIdentity appIdentity) => this.licenseDataGetter(appIdentity);

        /// <summary>
        /// Checks the license for the provided application identity.
        /// </summary>
        /// <param name="license">The license data to be checked.</param>
        /// <param name="appIdentity">Identifier for the application.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The license check result.
        /// </returns>
        protected virtual ILicenseCheckResult CheckLicenseData(LicenseData? license, AppIdentity appIdentity, IContext? context = null)
        {
            if (license == null)
            {
                return new LicenseCheckResult(appIdentity, false)
                    .MergeMessage($"Missing license for '{appIdentity}'.")
                    .Complete();
            }

            var result = new LicenseCheckResult(appIdentity, false);
            if (license.ValidFrom.HasValue && DateTime.Now.Date < license.ValidFrom.Value)
            {
                return result
                    .MergeMessage($"The validity of license '{license.Id}' starts only on {license.ValidFrom:d}.")
                    .Complete();
            }

            if (license.ValidTo.HasValue && DateTime.Now.Date > license.ValidTo.Value)
            {
                return result
                    .MergeMessage($"The license '{license.Id}' expired on {license.ValidTo:d}.")
                    .Complete();
            }

            if (!this.IsMatch(license.AppId, appIdentity.Id))
            {
                return result
                    .MergeMessage($"The license '{license.Id}' was issued for app '{license.AppId}' not for the requested '{appIdentity}'.")
                    .Complete();
            }

            if (!this.IsVersionMatch(license.AppVersionRange, appIdentity.Version))
            {
                return result
                    .MergeMessage($"The license '{license.Id}' was issued for version range '{license.AppVersionRange}' not for the requested '{appIdentity}'.")
                    .Complete();
            }

            return result.Value(true)
                .MergeMessage($"Valid license '{license.Id}' for '{appIdentity}'.")
                .Complete();
        }

        private bool IsVersionMatch(string versionRange, SemanticVersion? version)
        {
            var range = VersionRange.Parse(versionRange);
            if (range?.MinVersion != null && (version == null || version < range?.MinVersion))
            {
                return false;
            }

            if (range?.MaxVersion != null && (version == null || version > range?.MaxVersion))
            {
                return false;
            }

            return true;
        }

        private bool IsMatch(string pattern, string value)
        {
            if (pattern.EndsWith(VersionRange.Wildcard))
            {
                var firstPart = pattern.Substring(0, pattern.Length - 1);
                if (value.Length < firstPart.Length)
                {
                    return false;
                }

                return firstPart.Equals(value.Substring(0, firstPart.Length), StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return pattern.Equals(value, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
