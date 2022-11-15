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
    using System;
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
        /// An enumeration of possibly matching license data.
        /// </returns>
        public IEnumerable<LicenseData> GetLicenseData(AppIdentity appIdentity)
        {
            var probingFileNames = this.GetProbingFileNames(appIdentity);
            return (from licenseLocation in this.GetLicenseLocations(appIdentity)
                from probingFileName in probingFileNames
                select Path.Combine(licenseLocation, probingFileName)
                into licenseFilePath
                where File.Exists(licenseFilePath)
                select this.GetLicenseData(licenseFilePath)
                into license
                where license != null
                select license)!;
        }

        /// <summary>
        /// Stores the license data, making it persistable among multiple application runs.
        /// </summary>
        /// <param name="appIdentity">The app identity being associated the license.</param>
        /// <param name="rawLicenseData">Raw information describing the license.</param>
        public void StoreRawLicenseData(AppIdentity appIdentity, string rawLicenseData)
        {
            appIdentity = appIdentity ?? throw new ArgumentNullException(nameof(appIdentity));

            var fileName = $"{appIdentity.Id ?? LicenseFileName}.lic";
            var licenseLocation = this.GetLicenseLocations(appIdentity).First();
            if (!Directory.Exists(licenseLocation))
            {
                Directory.CreateDirectory(licenseLocation);
            }

            var licenseFilePath = Path.Combine(licenseLocation, fileName);
            File.WriteAllText(licenseFilePath, rawLicenseData);
        }

        private IEnumerable<string> GetProbingFileNames(AppIdentity appIdentity)
        {
            if (appIdentity?.Id != null)
            {
                var fileName = appIdentity.Id;
                var itemSeparators = new[] { '.', '-' };
                while (!string.IsNullOrEmpty(fileName))
                {
                    yield return $"{fileName}.lic";
                    var indexofSep = fileName!.LastIndexOfAny(itemSeparators);
                    fileName = indexofSep > 0 ? fileName.Substring(0, indexofSep - 1) : null;
                }
            }

            yield return $"{LicenseFileName}.lic";
        }

        private IEnumerable<string> GetLicenseLocations(AppIdentity appIdentity)
        {
            var locations = this.appRuntime.GetAppLicenseLocations();
            return locations.Any()
                ? locations
                : new[] { this.appRuntime.GetFullPath(IAppRuntime.DefaultLicenseFolder) };
        }

        private LicenseData? GetLicenseData(string licenseFilePath)
        {
            if (!File.Exists(licenseFilePath))
            {
                return null;
            }

            var encryptedLicenseString = File.ReadAllText(licenseFilePath);
            var licenseString = this.encryptionService.Decrypt(encryptedLicenseString);
            return LicenseData.Parse(licenseString);
        }
    }
}
