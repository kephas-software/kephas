// --------------------------------------------------------------------------------------------------------------------
// <copyright file="License.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the license class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Licensing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Data;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Class storing license data.
    /// </summary>
    public sealed class LicenseData : ICloneable, IIdentifiable
    {
        private const int ParseChecksumInvalidCode = 3;
        private const int ChecksumInvalidCode = 4;
        private const string DateTimeFormat = "yyyy-MM-dd";

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseData"/> class.
        /// </summary>
        /// <param name="id">The license identifier.</param>
        /// <param name="appId">The regular expression matching the identifier of the application.</param>
        /// <param name="appVersionRange">The application version range.</param>
        /// <param name="licenseType">The type of the license.</param>
        /// <param name="licensedTo">The entity the application is licensed to.</param>
        /// <param name="licensedBy">The entity that licensed the application.</param>
        /// <param name="validFrom">Optional. The date the license is valid from.</param>
        /// <param name="validTo">Optional. The date the license is valid to.</param>
        /// <param name="data">Optional. The additional data associated with the license.</param>
        public LicenseData(
            string id,
            string appId,
            string appVersionRange,
            string licenseType,
            string licensedTo,
            string licensedBy,
            DateTime? validFrom = null,
            DateTime? validTo = null,
            IDictionary<string, string> data = null)
        {
            Requires.NotNullOrEmpty(id, nameof(id));
            Requires.NotNullOrEmpty(appId, nameof(appId));
            Requires.NotNullOrEmpty(appVersionRange, nameof(appVersionRange));
            Requires.NotNullOrEmpty(licenseType, nameof(licenseType));
            Requires.NotNullOrEmpty(licensedTo, nameof(licensedTo));
            Requires.NotNullOrEmpty(licensedBy, nameof(licensedBy));

            this.Id = id;
            this.AppId = appId;
            this.AppVersionRange = appVersionRange;
            this.LicenseType = licenseType;
            this.LicensedTo = licensedTo;
            this.LicensedBy = licensedBy;
            this.ValidFrom = validFrom;
            this.ValidTo = validTo;
            this.Data = data ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the license identifier.
        /// </summary>
        /// <value>
        /// The license identifier.
        /// </value>
        public string Id { get; }

        /// <summary>
        /// Gets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        object IIdentifiable.Id => this.Id;

        /// <summary>
        /// Gets a regular expression matching the identifier of the application.
        /// </summary>
        /// <value>
        /// The regular expression matching identifier of the application.
        /// </value>
        public string AppId { get; }

        /// <summary>
        /// Gets the application version range.
        /// </summary>
        /// <value>
        /// The application version range.
        /// </value>
        public string AppVersionRange { get; }

        /// <summary>
        /// Gets the date from which the license is valid.
        /// </summary>
        /// <value>
        /// The valid from date.
        /// </value>
        public DateTime? ValidFrom { get; }

        /// <summary>
        /// Gets the date to which this license is valid.
        /// </summary>
        /// <value>
        /// The valid to date.
        /// </value>
        public DateTime? ValidTo { get; }

        /// <summary>
        /// Gets the entity that issued this license.
        /// </summary>
        /// <value>
        /// Describes who issued this object.
        /// </value>
        public string LicensedBy { get; }

        /// <summary>
        /// Gets the entity that licensed the application.
        /// </summary>
        /// <value>
        /// The entity that licensed the application.
        /// </value>
        public string LicensedTo { get; }

        /// <summary>
        /// Gets the type of the license.
        /// </summary>
        /// <value>
        /// The type of the license.
        /// </value>
        public string LicenseType { get; }

        /// <summary>
        /// Gets additional data associated with the license.
        /// </summary>
        /// <value>
        /// The additional data associated with the license.
        /// </value>
        public IDictionary<string, string> Data { get; }

        /// <summary>
        /// Parses the license data from the provided string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A LicenseData.
        /// </returns>
        public static LicenseData Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var splits = value.Split('\n');
            var id = splits[0];
            var appId = splits.Length > 1 ? splits[1] : null;
            var appVersionRange = splits.Length > 2 ? splits[2] : null;
            var licenseType = splits.Length > 3 ? splits[3] : null;
            var licensedTo = splits.Length > 4 ? splits[4] : null;
            var licensedBy = splits.Length > 5 ? splits[5] : null;
            var validFrom = splits.Length > 6 ? DateTimeParse(splits[6]) : null;
            var validTo = splits.Length > 7 ? DateTimeParse(splits[7]) : null;
            var checksumString = splits.Length > 8 ? splits[splits.Length - 1] : null;
            var data = splits.Length > 9 ? DataParse(splits.Skip(8).Take(splits.Length - 9)) : null;

            if (checksumString == null || !int.TryParse(checksumString, out var checksum))
            {
                throw new InvalidLicenseDataException($"The license data for {id} is corrupt, probably was manually changed ({ParseChecksumInvalidCode}).");
            }

            var licenseData = new LicenseData(id, appId, appVersionRange, licenseType, licensedTo, licensedBy, validFrom, validTo, data);
            licenseData.Validate(checksum);

            return licenseData;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new LicenseData(
                this.Id,
                this.AppId,
                this.AppVersionRange,
                this.LicenseType,
                this.LicensedTo,
                this.LicensedBy,
                this.ValidFrom,
                this.ValidTo,
                this.Data.ToDictionary(kv => kv.Key, kv => kv.Value));
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var validFromString = this.ValidFrom?.ToString(DateTimeFormat) ?? string.Empty;
            var validToString = this.ValidTo?.ToString(DateTimeFormat) ?? string.Empty;
            return $"{this.Id}\n{this.AppId}\n{this.AppVersionRange}\n{this.LicenseType}\n{this.LicensedTo}\n{this.LicensedBy}\n{validFromString}\n{validToString}\n{DataToString(this.Data)}\n{this.GetChecksum()}";
        }

        private static DateTime? DateTimeParse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (DateTime.TryParse(value, out var date))
            {
                return date.Date;
            }

            throw new ArgumentException($"The provided date '{value}' has an invalid format.");
        }

        private static IDictionary<string, string> DataParse(IEnumerable<string> values)
        {
            var data = new Dictionary<string, string>();
            foreach (var value in values)
            {
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                var pos = value.IndexOf(':');
                if (pos >= 0)
                {
                    data[value.Substring(0, pos)] = value.Substring(pos + 1);
                }
                else
                {
                    data[value] = null;
                }
            }

            return data;
        }

        private static string DataToString(IDictionary<string, string> data)
        {
            if (data == null || data.Count == 0)
            {
                return string.Empty;
            }

            return string.Join("\n", data.Select(kv => $"{kv.Key}:{kv.Value}"));
        }

        private int GetChecksum()
        {
            var idChecksum = this.GetChecksum(this.Id);
            var appIdChecksum = this.GetChecksum(this.AppId);
            var appVersionRangeChecksum = this.GetChecksum(this.AppVersionRange);
            var licenseTypeChecksum = this.GetChecksum(this.LicenseType);
            var licensedToChecksum = this.GetChecksum(this.LicensedTo);
            var licensedByChecksum = this.GetChecksum(this.LicensedBy);
            var validFromChecksum = this.GetChecksum(this.ValidFrom?.ToString(DateTimeFormat));
            var validToChecksum = this.GetChecksum(this.ValidTo?.ToString(DateTimeFormat));
            var dataChecksum = this.GetChecksum(DataToString(this.Data));

            unchecked
            {
                return idChecksum
                    + (appIdChecksum << 1)
                    + (appVersionRangeChecksum << 2)
                    + (licenseTypeChecksum << 3)
                    + (licensedToChecksum << 4)
                    + (licensedByChecksum << 5)
                    + (validFromChecksum << 6)
                    + (validToChecksum << 7)
                    + (dataChecksum << 8);
            }
        }

        private int GetChecksum(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }

            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                    {
                        break;
                    }

                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        private void Validate(int? checksum)
        {
            if (this.GetChecksum() == checksum)
            {
                return;
            }

            throw new InvalidLicenseDataException($"The license data for {this.AppId} is corrupt, probably was manually changed ({ChecksumInvalidCode}).");
        }
    }
}
