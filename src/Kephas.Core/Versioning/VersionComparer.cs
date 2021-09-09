// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionComparer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Versioning
{
    using System;
    using System.Linq;

    /// <summary>
    /// An IVersionComparer for NuGetVersion and NuGetVersion types.
    /// </summary>
    public sealed class VersionComparer : IVersionComparer
    {
        /// <summary>A default comparer that compares metadata as strings.</summary>
        public static readonly IVersionComparer Default =
            new VersionComparer(VersionComparison.Default);

        /// <summary>A comparer that uses only the version numbers.</summary>
        public static readonly IVersionComparer Version =
            new VersionComparer(VersionComparison.Version);

        /// <summary>Compares versions without comparing the metadata.</summary>
        public static readonly IVersionComparer VersionRelease =
            new VersionComparer(VersionComparison.VersionRelease);

        /// <summary>A version comparer that follows SemVer 2.0.0 rules.</summary>
        public static IVersionComparer VersionReleaseMetadata =
            new VersionComparer(VersionComparison.VersionReleaseMetadata);

        private readonly VersionComparison mode;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionComparer"/> class
        /// in the default mode.
        /// </summary>
        public VersionComparer()
        {
            this.mode = VersionComparison.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionComparer"/> class
        /// respecting the given comparison mode.
        /// </summary>
        /// <param name="versionComparison">The comparison mode.</param>
        public VersionComparer(VersionComparison versionComparison)
        {
            this.mode = versionComparison;
        }

        /// <summary>
        /// Compares the given versions using the <see cref="VersionComparison"/> mode.
        /// </summary>
        /// <param name="version1">First semantic version to be compared.</param>
        /// <param name="version2">Second semantic version to be compared.</param>
        /// <param name="versionComparison">The comparison mode.</param>
        /// <returns>
        /// A negative value if the first version is less than the second version, zero if both versions
        /// are equal, and a positive value if the first version is greater than the second one.
        /// </returns>
        public static int Compare(
            SemanticVersion version1,
            SemanticVersion version2,
            VersionComparison versionComparison)
        {
            return new VersionComparer(versionComparison).Compare(version1, version2);
        }

        /// <summary>
        /// Determines if both versions are equal.
        /// </summary>
        /// <param name="version1">First semantic version to be compared.</param>
        /// <param name="version2">Second semantic version to be compared.</param>
        /// <returns>
        /// True if both versions are equal, false otherwise.
        /// </returns>
        public bool Equals(SemanticVersion? version1, SemanticVersion? version2)
        {
            if (object.Equals(version1, version2))
            {
                return true;
            }

            if (object.Equals(version2, null) || object.Equals(version1, null))
            {
                return false;
            }

            if (this.mode != VersionComparison.Default && this.mode != VersionComparison.VersionRelease)
            {
                return this.Compare(version1, version2) == 0;
            }

            return version1.Major == version2.Major &&
                   version1.Minor == version2.Minor &&
                   version1.Patch == version2.Patch &&
                   version1.Hotfix == version2.Hotfix &&
                   VersionComparer.AreReleaseLabelsEqual(version1, version2);
        }

        /// <summary>
        /// Gives a hash code based on the normalized version string.
        /// </summary>
        /// <param name="version">The semantic version.</param>
        /// <returns>
        /// The hash code of the provided version.
        /// </returns>
        public int GetHashCode(SemanticVersion? version)
        {
            if (version == null)
            {
                return 0;
            }

            var hashCodeGenerator = new HashCodeGenerator();
            hashCodeGenerator.Combine(version.Major);
            hashCodeGenerator.Combine(version.Minor);
            hashCodeGenerator.Combine(version.Patch);
            if (version.Hotfix.HasValue)
            {
                hashCodeGenerator.Combine(version.Hotfix.Value);
            }

            if (this.mode == VersionComparison.Default || this.mode == VersionComparison.VersionRelease ||
                this.mode == VersionComparison.VersionReleaseMetadata)
            {
                var releaseLabelsOrNull = GetReleaseLabelsOrNull(version);
                if (releaseLabelsOrNull != null)
                {
                    var ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
                    foreach (string o in releaseLabelsOrNull)
                    {
                        hashCodeGenerator.Combine(o, ordinalIgnoreCase);
                    }
                }
            }

            if (this.mode == VersionComparison.VersionReleaseMetadata && version.HasMetadata)
            {
                hashCodeGenerator.Combine(version.Metadata, StringComparer.OrdinalIgnoreCase);
            }

            return hashCodeGenerator.GeneratedHash;
        }

        /// <summary>Compare versions.</summary>
        /// <param name="version1">First semantic version to be compared.</param>
        /// <param name="version2">Second semantic version to be compared.</param>
        /// <returns>
        /// A negative value if the first version is less than the second version,
        /// zero if both versions are equal,
        /// and a positive value if the first version is greater than the second one.
        /// </returns>
        public int Compare(SemanticVersion? version1, SemanticVersion? version2)
        {
            if (object.Equals(version1, version2))
            {
                return 0;
            }

            if (version2 == null)
            {
                return 1;
            }

            if (version1 == null)
            {
                return -1;
            }

            var num1 = version1.Major.CompareTo(version2.Major);
            if (num1 != 0)
            {
                return num1;
            }

            var num2 = version1.Minor.CompareTo(version2.Minor);
            if (num2 != 0)
            {
                return num2;
            }

            int num3 = version1.Patch.CompareTo(version2.Patch);
            if (num3 != 0)
            {
                return num3;
            }

            int num4 = !version1.Hotfix.HasValue && !version2.Hotfix.HasValue
                ? 0
                : !version1.Hotfix.HasValue
                    ? -1
                    : !version2.Hotfix.HasValue
                        ? 1
                        : version1.Hotfix.Value.CompareTo(version2.Hotfix.Value);
            if (num4 != 0)
            {
                return num4;
            }

            if (this.mode != VersionComparison.Version)
            {
                var releaseLabelsOrNull1 = GetReleaseLabelsOrNull(version1);
                var releaseLabelsOrNull2 = GetReleaseLabelsOrNull(version2);
                if (releaseLabelsOrNull1 != null && releaseLabelsOrNull2 == null)
                {
                    return -1;
                }

                if (releaseLabelsOrNull1 == null && releaseLabelsOrNull2 != null)
                {
                    return 1;
                }

                if (releaseLabelsOrNull1 != null && releaseLabelsOrNull2 != null)
                {
                    var num5 = CompareReleaseLabels(releaseLabelsOrNull1, releaseLabelsOrNull2);
                    if (num5 != 0)
                    {
                        return num5;
                    }
                }

                if (this.mode == VersionComparison.VersionReleaseMetadata)
                {
                    var num5 = StringComparer.OrdinalIgnoreCase.Compare(
                        version1.Metadata ?? string.Empty,
                        version2.Metadata ?? string.Empty);
                    if (num5 != 0)
                    {
                        return num5;
                    }
                }
            }

            return 0;
        }

        /// <summary>Compares sets of release labels.</summary>
        private static int CompareReleaseLabels(string[] version1, string[] version2)
        {
            var num1 = 0;
            var num2 = Math.Max(version1.Length, version2.Length);
            for (var index = 0; index < num2; ++index)
            {
                var flag1 = index < version1.Length;
                var flag2 = index < version2.Length;
                if (!flag1 & flag2)
                {
                    return -1;
                }

                if (flag1 && !flag2)
                {
                    return 1;
                }

                num1 = VersionComparer.CompareRelease(version1[index], version2[index]);
                if (num1 != 0)
                {
                    return num1;
                }
            }

            return num1;
        }

        /// <summary>
        /// Release labels are compared as numbers if they are numeric, otherwise they will be compared
        /// as strings.
        /// </summary>
        private static int CompareRelease(string version1, string version2)
        {
            var flag1 = int.TryParse(version1, out var result1);
            var flag2 = int.TryParse(version2, out var result2);
            return !(flag1 & flag2)
                ? (!(flag1 | flag2) ? StringComparer.OrdinalIgnoreCase.Compare(version1, version2) : (!flag1 ? 1 : -1))
                : result1.CompareTo(result2);
        }

        /// <summary>
        /// Returns an array of release labels from the version, or null.
        /// </summary>
        private static string[]? GetReleaseLabelsOrNull(SemanticVersion version)
        {
            if (!version.IsPrerelease)
            {
                return null;
            }

            var releaseLabels = version.ReleaseLabels;
            return releaseLabels is string[] strArray
                ? strArray
                : null;
        }

        /// <summary>Compare release labels</summary>
        private static bool AreReleaseLabelsEqual(SemanticVersion version1, SemanticVersion version2)
        {
            var releaseLabelsOrNull1 = GetReleaseLabelsOrNull(version1);
            var releaseLabelsOrNull2 = GetReleaseLabelsOrNull(version2);
            if ((releaseLabelsOrNull1 == null && releaseLabelsOrNull2 != null) ||
                (releaseLabelsOrNull1 != null && releaseLabelsOrNull2 == null))
            {
                return false;
            }

            if (releaseLabelsOrNull1 == null || releaseLabelsOrNull2 == null)
            {
                return true;
            }

            if (releaseLabelsOrNull1.Length != releaseLabelsOrNull2.Length)
            {
                return false;
            }

            return !releaseLabelsOrNull1
                .Where((t, index) => !StringComparer.OrdinalIgnoreCase.Equals(t, releaseLabelsOrNull2[index]))
                .Any();
        }
    }
}