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
    public sealed class License : ICloneable, IIdentifiable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="License"/> class.
        /// </summary>
        /// <param name="id">The license identifier.</param>
        /// <param name="appId">The identifier of the application.</param>
        /// <param name="appVersionRange">The application version range.</param>
        /// <param name="licenseType">The type of the license.</param>
        /// <param name="licensedTo">The company the application is licensed to.</param>
        /// <param name="licensedBy">Describes who issued this object.</param>
        /// <param name="validFrom">The valid from.</param>
        /// <param name="validTo">The valid to.</param>
        /// <param name="data">Optional. The additional data associated with the license.</param>
        public License(
            string id,
            string appId,
            string appVersionRange,
            string licenseType,
            string licensedTo,
            string licensedBy,
            DateTime? validFrom,
            DateTime? validTo,
            IDictionary<string, string> data = null)
        {
            Requires.NotNullOrEmpty(id, nameof(id));
            Requires.NotNullOrEmpty(appId, nameof(appId));

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
        /// Gets the identifier of the application.
        /// </summary>
        /// <value>
        /// The identifier of the application.
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
        /// Gets the entity the application is licensed to.
        /// </summary>
        /// <value>
        /// The entity the application is licensed to.
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
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new License(
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
            return $"{this.Id}\n{this.AppId}\n{this.AppVersionRange}\n{this.LicenseType}\n{this.LicensedTo}\n{this.LicensedBy}\n{this.ValidFrom:yyyy-MM-dd}\n{this.ValidTo:yyyy-MM-dd}\n{DataToString(this.Data)}";
        }

        /// <summary>
        /// Parses the license data from the provided string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A LicenseData.
        /// </returns>
        public static License Parse(string value)
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
            var data = DataParse(splits.Skip(8));

            return new License(id, appId, appVersionRange, licenseType, licensedTo, licensedBy, validFrom, validTo, data);
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
    }
}
