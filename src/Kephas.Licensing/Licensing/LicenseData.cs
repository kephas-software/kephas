// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LicenseData.cs" company="Kephas Software SRL">
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
    using Kephas.Licensing.Resources;

    /// <summary>
    /// Class storing license data.
    /// </summary>
    /// <param name="Id">Gets the license identifier.</param>
    /// <param name="AppId">Gets the regular expression matching the identifier of the application.</param>
    /// <param name="AppVersionRange">Gets the application version range.</param>
    /// <param name="LicenseType">Gets the type of the license.</param>
    /// <param name="LicensedTo">Gets the entity the license is assigned to.</param>
    /// <param name="LicensedBy">Gets the entity that issued this license.</param>
    public sealed record LicenseData(
        string Id,
        string AppId,
        string AppVersionRange,
        string LicenseType,
        string LicensedTo,
        string LicensedBy,
        DateTime? ValidFrom = null,
        DateTime? ValidTo = null,
        IDictionary<string, string?>? Data = null) : IIdentifiable
    {
        private const int ParseChecksumInvalidCode = 3;
        private const int ChecksumInvalidCode = 100;
        private const string DateTimeFormat = "yyyy-MM-dd";

        /// <summary>
        /// Gets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        object IIdentifiable.Id => this.Id;

        /// <summary>
        /// Gets the license identifier.
        /// </summary>
        /// <value>
        /// The license identifier.
        /// </value>
        public string Id { get; } = string.IsNullOrEmpty(Id) ? throw new ArgumentException(LicensingStrings.LicenseData_ArgumentMustNotBeEmpty, nameof(Id)) : Id;

        /// <summary>
        /// Gets a regular expression matching the identifier of the application.
        /// </summary>
        /// <value>
        /// The regular expression matching identifier of the application.
        /// </value>
        public string AppId { get; } = string.IsNullOrEmpty(AppId) ? throw new ArgumentException(LicensingStrings.LicenseData_ArgumentMustNotBeEmpty, nameof(AppId)) : AppId;

        /// <summary>
        /// Gets the application version range.
        /// </summary>
        /// <value>
        /// The application version range.
        /// </value>
        public string AppVersionRange { get; } = string.IsNullOrEmpty(AppVersionRange) ? throw new ArgumentException(LicensingStrings.LicenseData_ArgumentMustNotBeEmpty, nameof(AppVersionRange)) : AppVersionRange;

        /// <summary>
        /// Gets the date from which the license is valid.
        /// </summary>
        /// <value>
        /// The valid from date.
        /// </value>
        public DateTime? ValidFrom { get; } = ValidFrom;

        /// <summary>
        /// Gets the date to which this license is valid.
        /// </summary>
        /// <value>
        /// The valid to date.
        /// </value>
        public DateTime? ValidTo { get; } = ValidTo;

        /// <summary>
        /// Gets the entity that issued this license.
        /// </summary>
        /// <value>
        /// Describes who issued this object.
        /// </value>
        public string LicensedBy { get; } = string.IsNullOrEmpty(LicensedBy) ? throw new ArgumentException(LicensingStrings.LicenseData_ArgumentMustNotBeEmpty, nameof(LicensedBy)) : LicensedBy;

        /// <summary>
        /// Gets the entity the license is assigned to.
        /// </summary>
        /// <value>
        /// The entity the license is assigned to.
        /// </value>
        public string LicensedTo { get; } = string.IsNullOrEmpty(LicensedTo) ? throw new ArgumentException(LicensingStrings.LicenseData_ArgumentMustNotBeEmpty, nameof(LicensedTo)) : LicensedTo;

        /// <summary>
        /// Gets the type of the license.
        /// </summary>
        /// <value>
        /// The type of the license.
        /// </value>
        public string LicenseType { get; } = string.IsNullOrEmpty(LicenseType) ? throw new ArgumentException(LicensingStrings.LicenseData_ArgumentMustNotBeEmpty, nameof(LicenseType)) : LicenseType;

        /// <summary>
        /// Gets additional data associated with the license.
        /// </summary>
        /// <value>
        /// The additional data associated with the license.
        /// </value>
        public IDictionary<string, string?> Data { get; } = Data?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? new Dictionary<string, string?>();

        /// <summary>
        /// Parses the license data from the provided string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A LicenseData.
        /// </returns>
        public static LicenseData? Parse(string value)
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
            var checksumString = splits.Length > 8 ? splits[^1] : null;
            var data = splits.Length > 9 ? DataParse(splits[8..^1]) : null;

            if (checksumString == null || !int.TryParse(checksumString, out var checksum))
            {
                throw new InvalidLicenseDataException($"The license data for {id} is corrupt, probably was manually changed ({ParseChecksumInvalidCode}).");
            }

            var licenseData = new LicenseData(id, appId!, appVersionRange!, licenseType!, licensedTo!, licensedBy!, validFrom, validTo, data);
            licenseData.Validate(checksum);

            return licenseData;
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

            throw new ArgumentException(string.Format(LicensingStrings.LicenseData_TheProvidedDateHasAnInvalidFormat, value));
        }

        private static IDictionary<string, string?> DataParse(IEnumerable<string> values)
        {
            var data = new Dictionary<string, string?>();
            foreach (var value in values)
            {
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                var pos = value.IndexOf(':');
                if (pos >= 0)
                {
                    data[value[..pos]] = value[(pos + 1)..];
                }
                else
                {
                    data[value] = null;
                }
            }

            return data;
        }

        private static string DataToString(IDictionary<string, string?> data)
        {
            return data is { Count: not 0 }
                ? string.Join("\n", data.Select(kv => $"{kv.Key}:{kv.Value}"))
                : string.Empty;
        }

        private int GetChecksum()
        {
            var hashCodeGenerator = new HashCodeGenerator()
                .CombineStable(this.Id)
                .CombineStable(this.AppId)
                .CombineStable(this.AppVersionRange)
                .CombineStable(this.LicenseType)
                .CombineStable(this.LicensedTo)
                .CombineStable(this.LicensedBy)
                .CombineStable(this.ValidFrom?.ToString(DateTimeFormat))
                .CombineStable(this.ValidTo?.ToString(DateTimeFormat))
                .CombineStable(DataToString(this.Data));
            return hashCodeGenerator.GeneratedHash;
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
