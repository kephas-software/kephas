// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LicensingManagerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the licensing manager base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Licensing
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Cryptography;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// A licensing manager base.
    /// </summary>
    public abstract class LicensingManagerBase : ILicensingManager
    {
        /// <summary>
        /// Filename of the license file.
        /// </summary>
        private const string LicenseFileName = "License.lic";

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensingManagerBase"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="encryptionService">The encryption service.</param>
        protected LicensingManagerBase(IAppRuntime appRuntime, IEncryptionService encryptionService)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));
            Requires.NotNull(encryptionService, nameof(encryptionService));

            this.AppRuntime = appRuntime;
            this.EncryptionService = encryptionService;
        }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        protected IAppRuntime AppRuntime { get; }

        /// <summary>
        /// Gets the encryption service.
        /// </summary>
        /// <value>
        /// The encryption service.
        /// </value>
        protected IEncryptionService EncryptionService { get; }

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
            var license = this.GetLicense(appId);
            if (license == null)
            {
                return Task.FromResult<ILicenseCheckResult>(new LicenseCheckResult(appId, false));
            }

            var result = new LicenseCheckResult(appId, false);
            if (license.ValidFrom.HasValue && DateTime.Now.Date < license.ValidFrom.Value)
            {
                return Task.FromResult<ILicenseCheckResult>(
                    result.MergeMessage($"The license validity starts only on {license.ValidFrom}."));
            }

            if (license.ValidTo.HasValue && DateTime.Now.Date > license.ValidTo.Value)
            {
                return Task.FromResult<ILicenseCheckResult>(
                    result.MergeMessage($"The license expired on {license.ValidTo}."));
            }

            if (!string.IsNullOrEmpty(license.AppId) && !license.AppId.Equals(appId.Id, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult<ILicenseCheckResult>(
                    result.MergeMessage($"The license was issued for app '{license.AppId}' not for the requested {appId}."));
            }

            // TODO check version, too

            result.ReturnValue = true;
            return Task.FromResult<ILicenseCheckResult>(result);
        }

        /// <summary>
        /// Gets the license data.
        /// </summary>
        /// <param name="appId">Identifier for the application.</param>
        /// <returns>
        /// The license data.
        /// </returns>
        protected virtual License GetLicense(AppIdentity appId)
        {
            var location = this.AppRuntime.GetAppLocation(appId);
            var licenseFilePath = Path.Combine(location, LicenseFileName);
            if (!File.Exists(licenseFilePath))
            {
                return null;
            }

            var encryptedLicenseString = File.ReadAllText(licenseFilePath);
            var licenseString = this.EncryptionService.Decrypt(encryptedLicenseString);
            return License.Parse(licenseString);
        }
    }
}
