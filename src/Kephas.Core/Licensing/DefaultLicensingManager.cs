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
#if NETSTANDARD2_1
#else
        , ISyncLicensingManager
#endif
    {
        private readonly Func<AppIdentity, LicenseData?> licenseDataGetter;

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
        public DefaultLicensingManager(Func<AppIdentity, LicenseData?> licenseDataGetter)
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
            var license = this.GetLicenseData(appIdentity);
            if (license == null)
            {
                return new LicenseCheckResult(appIdentity, false)
                    .MergeMessage("Missing license.");
            }

            var result = new LicenseCheckResult(appIdentity, false);
            if (license.ValidFrom.HasValue && DateTime.Now.Date < license.ValidFrom.Value)
            {
                return result
                    .MergeMessage($"The license validity starts only on {license.ValidFrom:d}.");
            }

            if (license.ValidTo.HasValue && DateTime.Now.Date > license.ValidTo.Value)
            {
                return result
                    .MergeMessage($"The license expired on {license.ValidTo:d}.");
            }

            if (!this.IsMatch(license.AppId, appIdentity.Id))
            {
                return result
                    .MergeMessage($"The license was issued for app '{license.AppId}' not for the requested '{appIdentity}'.");
            }

            if (!this.IsVersionMatch(license.AppVersionRange, appIdentity.Version))
            {
                return result
                    .MergeMessage($"The license was issued for version range '{license.AppVersionRange}' not for the requested '{appIdentity}'.");
            }

            return result.Value(true)
                .MergeMessage("Valid license.");
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
        public virtual Task<ILicenseCheckResult> CheckLicenseAsync(AppIdentity appIdentity, IContext? context = null, CancellationToken cancellationToken = default)
        {
            return ((Func<ILicenseCheckResult>)(() => this.CheckLicense(appIdentity))).AsAsync(cancellationToken: cancellationToken);
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
            return this.GetLicenseData(appIdentity);
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
            return ((Func<LicenseData?>)(() => this.GetLicense(appIdentity))).AsAsync(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Gets the license data. This is the main extensibility point
        /// when overriding the license retrieval.
        /// </summary>
        /// <param name="appIdentity">Identifier for the application.</param>
        /// <returns>
        /// The license data.
        /// </returns>
        protected virtual LicenseData? GetLicenseData(AppIdentity appIdentity) => this.licenseDataGetter(appIdentity);

        private bool IsVersionMatch(string versionRange, SemanticVersion? version)
        {
            var dashPos = versionRange.IndexOf(LicenseData.VersionRangeSeparator);
            var minVersionString = dashPos < 0 ? versionRange : versionRange.Substring(0, dashPos);
            var maxVersionString = dashPos < 0 ? versionRange : versionRange.Substring(dashPos + 1);
            var minVersion = this.GetNormalizedVersion(minVersionString, 0);
            var maxVersion = string.IsNullOrEmpty(maxVersionString) ? null : this.GetNormalizedVersion(maxVersionString, short.MaxValue);

            if (minVersion != null && (version == null || version < minVersion))
            {
                return false;
            }

            if (maxVersion != null && (version == null || version > maxVersion))
            {
                return false;
            }

            return true;
        }

        private bool IsVersionMatch(string versionRange, string? version)
        {
            var semanticVersion = string.IsNullOrEmpty(version) ? null : this.GetNormalizedVersion(version, 0);
            return IsVersionMatch(versionRange, semanticVersion);
        }

        private SemanticVersion? GetNormalizedVersion(string version, short wildCardReplacement)
        {
            var dashPos = version.IndexOf("-");
            var releaseVersionString = dashPos < 0 ? version : version.Substring(0, dashPos);
            if (releaseVersionString.EndsWith(".*"))
            {
                releaseVersionString = releaseVersionString.Substring(0, releaseVersionString.Length - 2);
            }
            else if (releaseVersionString.EndsWith("*"))
            {
                releaseVersionString = releaseVersionString.Substring(0, releaseVersionString.Length - 1);
            }

            var dotCount = releaseVersionString.Count(c => c == '.');
            for (var i = 0; i < 3 - dotCount; i++)
            {
                releaseVersionString += $".{wildCardReplacement}";
            }

            return string.IsNullOrEmpty(releaseVersionString) ? null : SemanticVersion.Parse(releaseVersionString);
        }

        private bool IsMatch(string pattern, string value)
        {
            if (pattern.EndsWith("*"))
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
