// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticVersion.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Versioning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>A strict SemVer implementation.</summary>
    public class SemanticVersion : IFormattable, IComparable, IComparable<SemanticVersion>, IEquatable<SemanticVersion>
    {
        private static readonly string[] EmptyReleaseLabels = Array.Empty<string>();
        private readonly string[]? releaseLabels;
        private readonly Version version;

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <param name="version">Version to clone.</param>
        public SemanticVersion(SemanticVersion version)
            : this(version.Major, version.Minor, version.Patch, version.ReleaseLabels, version.Metadata)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <remarks>Creates a SemanticVersion X.Y.Z.</remarks>
        /// <param name="major">The X part of the version.</param>
        /// <param name="minor">The Y part of the version.</param>
        /// <param name="patch">The Z part of the version.</param>
        public SemanticVersion(int major, int minor, int patch)
            : this(major, minor, patch, Enumerable.Empty<string>(), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <remarks>Creates a SemanticVersion X.Y.Z-prerelease.</remarks>
        /// <param name="major">The X part of the version.</param>
        /// <param name="minor">The Y part of the version.</param>
        /// <param name="patch">The Z part of the version.</param>
        /// <param name="releaseLabel">The prerelease label.</param>
        public SemanticVersion(int major, int minor, int patch, string? releaseLabel)
            : this(major, minor, patch, ParseReleaseLabels(releaseLabel), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <remarks>Creates a SemanticVersion X.Y.Z-prerelease#build01.</remarks>
        /// <param name="major">The X part of the version.</param>
        /// <param name="minor">The Y part of the version.</param>
        /// <param name="patch">The Z part of the version.</param>
        /// <param name="releaseLabel">The prerelease label.</param>
        /// <param name="metadata">The build metadata.</param>
        public SemanticVersion(int major, int minor, int patch, string? releaseLabel, string? metadata)
            : this(major, minor, patch, ParseReleaseLabels(releaseLabel), metadata)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <remarks>Creates a SemanticVersion X.Y.Z-alpha.1.2#build01.</remarks>
        /// <param name="major">The X part of the version.</param>
        /// <param name="minor">The Y part of the version.</param>
        /// <param name="patch">The Z part of the version.</param>
        /// <param name="releaseLabels">Release labels that have been split by the dot separator.</param>
        /// <param name="metadata">The build metadata.</param>
        public SemanticVersion(
            int major,
            int minor,
            int patch,
            IEnumerable<string>? releaseLabels,
            string? metadata)
            : this(new Version(major, minor, patch, 0), releaseLabels, metadata)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="releaseLabel">Full release label.</param>
        /// <param name="metadata">Build metadata.</param>
        protected SemanticVersion(
            Version version,
            string? releaseLabel = null,
            string? metadata = null)
            : this(version, ParseReleaseLabels(releaseLabel), metadata)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <remarks>Creates a SemanticVersion X.Y.Z.T-alpha.1.2#build01.</remarks>
        /// <param name="major">The X part of the version.</param>
        /// <param name="minor">The Y part of the version.</param>
        /// <param name="patch">The Z part of the version.</param>
        /// <param name="revision">The T part of the version.</param>
        /// <param name="releaseLabel">Prerelease label.</param>
        /// <param name="metadata">Build metadata.</param>
        protected SemanticVersion(
            int major,
            int minor,
            int patch,
            int revision,
            string? releaseLabel,
            string? metadata)
            : this(major, minor, patch, revision, ParseReleaseLabels(releaseLabel), metadata)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <remarks>Creates a SemanticVersion X.Y.Z.T-alpha.1.2#build01.</remarks>
        /// <param name="major">The X part of the version.</param>
        /// <param name="minor">The Y part of the version.</param>
        /// <param name="patch">The Z part of the version.</param>
        /// <param name="revision">The T part of the version.</param>
        /// <param name="releaseLabels">The release labels.</param>
        /// <param name="metadata">The build metadata.</param>
        protected SemanticVersion(
            int major,
            int minor,
            int patch,
            int revision,
            IEnumerable<string>? releaseLabels,
            string? metadata)
            : this(new Version(major, minor, patch, revision), releaseLabels, metadata)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="releaseLabels">The release labels.</param>
        /// <param name="metadata">The build metadata.</param>
        protected SemanticVersion(Version version, IEnumerable<string>? releaseLabels, string? metadata)
        {
            this.version = SemanticVersion.NormalizeVersionValue(version ?? throw new ArgumentNullException(nameof(version)));
            this.Metadata = metadata;
            if (releaseLabels == null)
            {
                return;
            }

            this.releaseLabels = !(releaseLabels is string[] strArray) ? releaseLabels.ToArray<string>() : strArray;
            if (this.releaseLabels.Length >= 1)
            {
                return;
            }

            this.releaseLabels = null;
        }

        /// <summary>Gets the major version X (X.y.z).</summary>
        public int Major => this.version.Major;

        /// <summary>Gets the minor version Y (x.Y.z).</summary>
        public int Minor => this.version.Minor;

        /// <summary>Gets the patch version Z (x.y.Z).</summary>
        public int Patch => this.version.Build;

        /// <summary>Gets the hotfix version T (x.y.z.T).</summary>
        public int? Hotfix => this.version.Revision == 0 ? (int?)null : this.version.Revision;

        /// <summary>
        /// Gets a collection of pre-release labels attached to the version.
        /// </summary>
        public IEnumerable<string> ReleaseLabels =>
            this.releaseLabels ?? EmptyReleaseLabels;

        /// <summary>Gets the full pre-release label for the version.</summary>
        public string Release
        {
            get
            {
                if (this.releaseLabels == null)
                {
                    return string.Empty;
                }

                return this.releaseLabels.Length == 1
                    ? this.releaseLabels[0]
                    : string.Join(".", this.releaseLabels);
            }
        }

        /// <summary>Gets a value indicating whether pre-release labels exist for the version.</summary>
        public virtual bool IsPrerelease =>
            this.releaseLabels?.Any(t => !string.IsNullOrEmpty(t)) ?? false;

        /// <summary>Gets a value indicating whether metadata exists for the version.</summary>
        public virtual bool HasMetadata => !string.IsNullOrEmpty(this.Metadata);

        /// <summary>Gets the build metadata attached to the version.</summary>
        public virtual string? Metadata { get; }

        /// <summary>
        /// Equals operator.
        /// </summary>
        /// <param name="version1">The first instance to compare.</param>
        /// <param name="version2">The second instance to compare.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator ==(SemanticVersion? version1, SemanticVersion? version2)
        {
            return object.Equals(version1, version2);
        }

        /// <summary>
        /// Not equal operator.
        /// </summary>
        /// <param name="version1">The first instance to compare.</param>
        /// <param name="version2">The second instance to compare.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator !=(SemanticVersion? version1, SemanticVersion? version2)
        {
            return !object.Equals(version1, version2);
        }

        /// <summary>
        /// Less than operator.
        /// </summary>
        /// <param name="version1">The first instance to compare.</param>
        /// <param name="version2">The second instance to compare.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator <(SemanticVersion version1, SemanticVersion version2)
        {
            return SemanticVersion.Compare(version1, version2) < 0;
        }

        /// <summary>
        /// Less than or equal operator.
        /// </summary>
        /// <param name="version1">The first instance to compare.</param>
        /// <param name="version2">The second instance to compare.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator <=(SemanticVersion version1, SemanticVersion version2)
        {
            return SemanticVersion.Compare(version1, version2) <= 0;
        }

        /// <summary>
        /// Greater than operator.
        /// </summary>
        /// <param name="version1">The first instance to compare.</param>
        /// <param name="version2">The second instance to compare.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator >(SemanticVersion version1, SemanticVersion version2)
        {
            return SemanticVersion.Compare(version1, version2) > 0;
        }

        /// <summary>
        /// Greater than or equal operator.
        /// </summary>
        /// <param name="version1">The first instance to compare.</param>
        /// <param name="version2">The second instance to compare.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static bool operator >=(SemanticVersion version1, SemanticVersion version2)
        {
            return SemanticVersion.Compare(version1, version2) >= 0;
        }

        /// <summary>
        /// Parses a SemVer string using strict SemVer rules.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
        ///                                     illegal values.</exception>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A <see cref="SemanticVersion"/>.
        /// </returns>
        public static SemanticVersion Parse(string value)
        {
            if (!TryParse(value, out var version))
            {
                throw new ArgumentException(
                    "Invalid version provided: {0}".FormatWith(value),
                    nameof(value));
            }

            return version!;
        }

        /// <summary>
        /// Parse a version string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="semanticVersion">[out] Version to clone.</param>
        /// <returns>
        /// False if the version is not a strict semver, true otherwise.
        /// </returns>
        public static bool TryParse(string? value, out SemanticVersion? semanticVersion)
        {
            semanticVersion = null;
            if (value == null)
            {
                return false;
            }

            var (versionPart, releaseLabelsPart, metadataPart) = ParseSections(value);
            if (versionPart == null || !Version.TryParse(versionPart, out var result))
            {
                return false;
            }

            var strArray = versionPart.Split('.');
            if (strArray.Length > 4)
            {
                return false;
            }

            if (strArray.Any(s => !IsValidPart(s, false)))
            {
                return false;
            }

            if (releaseLabelsPart != null && releaseLabelsPart.Any(t => !IsValidPart(t, false)))
            {
                return false;
            }

            if (metadataPart != null && !IsValid(metadataPart, true))
            {
                return false;
            }

            var version = NormalizeVersionValue(result);
            semanticVersion = new SemanticVersion(version, releaseLabelsPart, metadataPart ?? string.Empty);
            return true;
        }

        /// <summary>
        /// Gives a normalized representation of the version.
        /// This string is unique to the identity of the version and does not contain metadata.
        /// </summary>
        /// <returns>The normalized string.</returns>
        public virtual string ToNormalizedString()
        {
            return this.ToString("N", VersionFormatter.Instance);
        }

        /// <summary>
        /// Gives a full representation of the version include metadata.
        /// This string is not unique to the identity of the version. Other versions
        /// that differ on metadata will have a different full string representation.
        /// </summary>
        /// <returns>The full string representation, including metadata.</returns>
        public virtual string? ToFullString()
        {
            return this.ToString("F", VersionFormatter.Instance);
        }

        /// <summary>Get the normalized string.</summary>
        /// <returns>The normalized string.</returns>
        public override string ToString()
        {
            return this.ToNormalizedString();
        }

        /// <summary>
        /// Custom string format.
        /// </summary>
        /// <param name="format">The format to use.
        ///                      -or-
        ///                      A null reference (<see langword="Nothing" /> in Visual Basic) to use
        ///                      the default format defined for the type of the
        ///                      <see cref="T:System.IFormattable" /> implementation.</param>
        /// <param name="formatProvider">The provider to use to format the value.
        ///                              -or-
        ///                              A null reference (<see langword="Nothing" /> in Visual Basic)
        ///                              to obtain the numeric format information from the current
        ///                              locale setting of the operating system.</param>
        /// <returns>
        /// The formatted string.
        /// </returns>
        public virtual string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (formatProvider == null || !this.TryFormatter(format, formatProvider, out string? formattedString))
            {
                formattedString = this.ToString();
            }

            return formattedString;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return VersionComparer.Default.GetHashCode(this);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer
        /// that indicates whether the current instance precedes, follows, or occurs in the same position
        /// in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has
        /// these meanings:
        /// Value
        ///
        /// Meaning
        ///
        /// Less than zero
        ///
        /// This instance precedes <paramref name="other" /> in the sort order.
        ///
        /// Zero
        ///
        /// This instance occurs in the same position in the sort order as <paramref name="other" />.
        ///
        /// Greater than zero
        ///
        /// This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        public virtual int CompareTo(object? other)
        {
            return this.CompareTo(other as SemanticVersion);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer
        /// that indicates whether the current instance precedes, follows, or occurs in the same position
        /// in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has
        /// these meanings:
        /// Value
        ///
        /// Meaning
        ///
        /// Less than zero
        ///
        /// This instance precedes <paramref name="other" /> in the sort order.
        ///
        /// Zero
        ///
        /// This instance occurs in the same position in the sort order as <paramref name="other" />.
        ///
        /// Greater than zero
        ///
        /// This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        public virtual int CompareTo(SemanticVersion? other)
        {
            return this.CompareTo(other, VersionComparison.Default);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public override bool Equals(object? obj)
        {
            return this.Equals(obj as SemanticVersion);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" />
        /// parameter; otherwise, <see langword="false" />.
        /// </returns>
        public virtual bool Equals(SemanticVersion? other)
        {
            return other != null && VersionComparer.Default.Equals(this, other);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object using the provided comparison.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <param name="versionComparison">Version comparison to be compared.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public virtual bool Equals(SemanticVersion? other, VersionComparison versionComparison)
        {
            return other != null && new VersionComparer(versionComparison).Equals(this, other);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer
        /// that indicates whether the current instance precedes, follows, or occurs in the same position
        /// in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <param name="versionComparison">Version comparison to be compared.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has
        /// these meanings:
        /// Value
        ///
        /// Meaning
        ///
        /// Less than zero
        ///
        /// This instance precedes <paramref name="other" /> in the sort order.
        ///
        /// Zero
        ///
        /// This instance occurs in the same position in the sort order as <paramref name="other" />.
        ///
        /// Greater than zero
        ///
        /// This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        public virtual int CompareTo(SemanticVersion? other, VersionComparison versionComparison)
        {
            return new VersionComparer(versionComparison).Compare(this, other);
        }

        internal static bool IsLetterOrDigitOrDash(char c)
        {
            int num = (int) c;
            return num >= 48 && num <= 57 || num >= 65 && num <= 90 || num >= 97 && num <= 122 || num == 45;
        }

        internal static bool IsDigit(char c)
        {
            int num = (int) c;
            return num >= 48 && num <= 57;
        }

        internal static bool IsValid(string s, bool allowLeadingZeros)
        {
            string str = s;
            char[] chArray = new char[1] {'.'};
            foreach (string s1 in str.Split(chArray))
            {
                if (!IsValidPart(s1, allowLeadingZeros))
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool IsValidPart(string s, bool allowLeadingZeros)
        {
            if (s.Length == 0)
            {
                return false;
            }

            if (!allowLeadingZeros && s.Length > 1 && s[0] == '0')
            {
                var flag = true;
                for (var index = 1; index < s.Length; ++index)
                {
                    if (!IsDigit(s[index]))
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    return false;
                }
            }

            for (var index = 0; index < s.Length; ++index)
            {
                if (!IsLetterOrDigitOrDash(s[index]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Parse the version string into version/release/build The goal of this code is to take the most
        /// direct and optimized path to parsing and validating a semver. Regex would be much cleaner,
        /// but is too slow.
        /// </summary>
        /// <param name="value">The value to be parsed.</param>
        /// <returns>
        /// The version, release labels and metadata.
        /// </returns>
        internal static (string? version, string[]? releaseLabels, string? metadata) ParseSections(string value)
        {
            string? versionPart = null;
            string[]? releasePart = null;
            string? metadataPart = null;
            var num1 = -1;
            var num2 = -1;
            for (var index = 0; index < value.Length; ++index)
            {
                var flag = index == value.Length - 1;
                if (num1 < 0)
                {
                    if (flag || value[index] == '-' || value[index] == '+')
                    {
                        int length = index + (flag ? 1 : 0);
                        versionPart = value.Substring(0, length);
                        num1 = index;
                        if (value[index] == '+')
                        {
                            num2 = index;
                        }
                    }
                }
                else if (num2 < 0)
                {
                    if (flag || value[index] == '+')
                    {
                        var startIndex = num1 + 1;
                        var num3 = index + (flag ? 1 : 0);
                        releasePart = value.Substring(startIndex, num3 - startIndex).Split('.');
                        num2 = index;
                    }
                }
                else if (flag)
                {
                    var startIndex = num2 + 1;
                    var num3 = index + (flag ? 1 : 0);
                    metadataPart = value.Substring(startIndex, num3 - startIndex);
                }
            }

            return (versionPart, releasePart, metadataPart);
        }

        internal static Version NormalizeVersionValue(Version version)
        {
            Version version1 = version;
            if (version.Build < 0 || version.Revision < 0)
            {
                version1 = new Version(
                    version.Major,
                    version.Minor,
                    Math.Max(version.Build, 0),
                    Math.Max(version.Revision, 0));
            }

            return version1;
        }

        /// <summary>
        /// Internal string formatter.
        /// </summary>
        /// <param name="format">The format to use.
        ///                      -or-
        ///                      A null reference (<see langword="Nothing" /> in Visual Basic) to use
        ///                      the default format defined for the type of the
        ///                      <see cref="T:System.IFormattable" /> implementation.</param>
        /// <param name="formatProvider">The provider to use to format the value.
        ///                              -or-
        ///                              A null reference (<see langword="Nothing" /> in Visual Basic)
        ///                              to obtain the numeric format information from the current
        ///                              locale setting of the operating system.</param>
        /// <param name="formattedString">[out] The formatted string.</param>
        /// <returns>
        /// True if the version could be formatted, false otherwise.
        /// </returns>
        protected bool TryFormatter(
            string? format,
            IFormatProvider? formatProvider,
            out string formattedString)
        {
            formattedString = string.Empty;
            if (formatProvider == null ||
                formatProvider.GetFormat(this.GetType()) is not ICustomFormatter customFormatter)
            {
                return false;
            }

            formattedString = customFormatter.Format(format, this, formatProvider);

            return true;
        }

        private static string[]? ParseReleaseLabels(string? releaseLabels) =>
            string.IsNullOrEmpty(releaseLabels) ? null : releaseLabels.Split('.');

        private static int Compare(SemanticVersion version1, SemanticVersion version2)
            => VersionComparer.Default.Compare(version1, version2);
    }
}