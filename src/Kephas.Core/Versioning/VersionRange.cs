// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionRange.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Versioning
{
    using System;
    using System.Linq;

    /// <summary>
    /// Provides operations with version ranges.
    /// </summary>
    public class VersionRange
    {
        /// <summary>
        /// The version range separator.
        /// </summary>
        public static readonly string Separator = ":";

        /// <summary>
        /// The wildcard used for 
        /// </summary>
        public static readonly string Wildcard = "*";

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionRange"/> class.
        /// </summary>
        /// <param name="minVersion">The minimum version.</param>
        /// <param name="maxVersion">The maximum version.</param>
        public VersionRange(SemanticVersion? minVersion, SemanticVersion? maxVersion)
        {
            this.MinVersion = minVersion;
            this.MaxVersion = maxVersion;
        }

        /// <summary>
        /// Gets the minimum version.
        /// </summary>
        public SemanticVersion? MinVersion { get; }

        /// <summary>
        /// Gets the maximum version.
        /// </summary>
        public SemanticVersion? MaxVersion { get; }

        /// <summary>
        /// Parses the version range.
        /// </summary>
        /// <param name="str">The string to be parsed.</param>
        /// <returns>The version range.</returns>
        public static VersionRange? Parse(string? str)
        {
            if (!TryParse(str, out var versionRange))
            {
                throw new ArgumentException($"Version range '{str}' is invalid.", nameof(str));
            }

            return versionRange;
        }

        /// <summary>
        /// Tries to parse the version range.
        /// </summary>
        /// <param name="str">The string to be parsed.</param>
        /// <param name="versionRange">[out] The parsed version range.</param>
        /// <returns>True if the version could be parsed, false otherwise.</returns>
        public static bool TryParse(string? str, out VersionRange? versionRange)
        {
            versionRange = null;
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            var sepPos = str.IndexOf(Separator);
            var minVersionString = sepPos < 0 ? str : str.Substring(0, sepPos);
            var maxVersionString = sepPos < 0 ? str : str.Substring(sepPos + 1);
            if (!TryGetNormalizedVersion(minVersionString, 0, out var minVersion))
            {
                return false;
            }

            if (!TryGetNormalizedVersion(maxVersionString, short.MaxValue, out var maxVersion))
            {
                return false;
            }

            if (minVersion == null && maxVersion == null)
            {
                return false;
            }

            versionRange = new VersionRange(minVersion, maxVersion);
            return true;
        }

        private static bool TryGetNormalizedVersion(
            string versionString,
            short wildCardReplacement,
            out SemanticVersion? version)
        {
            var dashPos = versionString.IndexOf("-");
            var releaseVersionString = dashPos < 0 ? versionString : versionString.Substring(0, dashPos);
            if (releaseVersionString.EndsWith($".{Wildcard}"))
            {
                releaseVersionString = releaseVersionString.Substring(0, releaseVersionString.Length - 2);
            }
            else if (releaseVersionString.EndsWith(Wildcard))
            {
                releaseVersionString = releaseVersionString.Substring(0, releaseVersionString.Length - 1);
            }

            version = null;
            if (string.IsNullOrEmpty(releaseVersionString))
            {
                return true;
            }

            var dotCount = releaseVersionString.Count(c => c == '.');
            for (var i = 0; i < 2 - dotCount; i++)
            {
                releaseVersionString += $".{wildCardReplacement}";
            }

            if (string.IsNullOrEmpty(releaseVersionString))
            {
                return true;
            }

            return SemanticVersion.TryParse(releaseVersionString, out version);
        }
    }
}