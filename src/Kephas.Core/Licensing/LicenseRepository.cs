// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LicenseRepository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the license repository class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Licensing
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Kephas.Application;
    using Kephas.Cryptography;

    /// <summary>
    /// A license repository.
    /// </summary>
    internal class LicenseRepository : ILicenseRepository
    {
        private const string LicenseFileName = "License";

        private readonly IAppRuntime appRuntime;
        private readonly IEncryptionService encryptionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseRepository"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="encryptionService">The encryption service.</param>
        public LicenseRepository(IAppRuntime appRuntime, IEncryptionService encryptionService)
        {
            this.appRuntime = appRuntime;
            this.encryptionService = encryptionService;
        }

        /// <summary>
        /// Gets the license information from the store.
        /// </summary>
        /// <param name="appIdentity">The app identity requesting the license.</param>
        /// <returns>
        /// The license data or <c>null</c>, if a license could not be found for the requesting
        /// application.
        /// </returns>
        public LicenseData GetLicenseData(AppIdentity appIdentity)
        {
            var probingFileNames = new[] { $"{LicenseFileName}.lic", $"{appIdentity?.Id ?? LicenseFileName}.lic" };
            foreach (var licenseLocation in this.GetLicenseLocations(appIdentity))
            {
                foreach (var probingFileName in probingFileNames)
                {
                    var licenseFilePath = Path.Combine(licenseLocation, probingFileName);
                    if (File.Exists(licenseFilePath))
                    {
                        return this.GetLicenseData(licenseFilePath);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Stores the license data, making it persistable among multiple application runs.
        /// </summary>
        /// <param name="appIdentity">The app identity being associated the license.</param>
        /// <param name="licenseData">Information describing the license.</param>
        public void StoreLicenseData(AppIdentity appIdentity, LicenseData licenseData)
        {
            var fileName = $"{appIdentity?.Id ?? LicenseFileName}.lic";
            var licenseLocation = this.GetLicenseLocations(appIdentity).First();
            if (!Directory.Exists(licenseLocation))
            {
                Directory.CreateDirectory(licenseLocation);
            }

            var licenseFilePath = Path.Combine(licenseLocation, fileName);
            this.StoreLicenseData(licenseFilePath, licenseData);
        }

        private IEnumerable<string> GetLicenseLocations(AppIdentity appIdentity)
        {
            var locations = this.appRuntime.GetAppLicenseLocations();
            return locations.Any()
                ? locations
                : new[] { this.appRuntime.GetFullPath(AppRuntimeBase.DefaultLicenseFolder) };
        }

        private LicenseData GetLicenseData(string licenseFilePath)
        {
            var encryptedLicenseString = File.ReadAllText(licenseFilePath);
            var licenseString = this.encryptionService.Decrypt(encryptedLicenseString);
            return LicenseData.Parse(licenseString);
        }

        private void StoreLicenseData(string licenseFilePath, LicenseData licenseData)
        {
            var encryptedLicenseString = this.encryptionService.Encrypt(licenseData.ToString());
            File.WriteAllText(licenseFilePath, encryptedLicenseString);
        }
    }
}
