// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionFormatter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Versioning
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;

    using Kephas.Diagnostics.Contracts;

    /// <summary>Custom formatter for NuGet versions.</summary>
    public class VersionFormatter : IFormatProvider, ICustomFormatter
    {
        /// <summary>A static instance of the VersionFormatter class.</summary>
        public static readonly VersionFormatter Instance = new VersionFormatter();

        /// <summary>
        /// Format a version string.
        /// </summary>
        /// <param name="format">A format string containing formatting specifications.</param>
        /// <param name="arg">An object to format.</param>
        /// <param name="formatProvider">An object that supplies format information about the current
        ///                              instance.</param>
        /// <returns>
        /// The formatted value.
        /// </returns>
        public string? Format(string format, object arg, IFormatProvider formatProvider)
        {
            Requires.NotNull(arg, nameof(arg));

            string? str1 = null;
            if (arg.GetType() == typeof(IFormattable))
            {
                str1 = ((IFormattable) arg).ToString(format, formatProvider);
            }
            else if (!string.IsNullOrEmpty(format))
            {
                var version = arg as SemanticVersion;
                if (version == null)
                {
                    return str1;
                }

                if (format.Length == 1)
                {
                    str1 = VersionFormatter.Format(format[0], version);
                }
                else
                {
                    var stringBuilder = new StringBuilder(format.Length);
                    foreach (var t in format)
                    {
                        var str2 = Format(t, version);
                        if (str2 == null)
                        {
                            stringBuilder.Append(t);
                        }
                        else
                        {
                            stringBuilder.Append(str2);
                        }
                    }

                    str1 = stringBuilder.ToString();
                }
            }

            return str1;
        }

        /// <summary>
        /// Get version format type.
        /// </summary>
        /// <param name="formatType">An object that specifies the type of format object to return.</param>
        /// <returns>
        /// The version format type.
        /// </returns>
        public object? GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ||
                   formatType == typeof(SemanticVersion)
                ? this
                : null;
        }

        /// <summary>
        /// Create a normalized version string. This string is unique for each version 'identity'
        /// and does not include leading zeros or metadata.
        /// </summary>
        private static string? GetNormalizedString(SemanticVersion version)
        {
            var str = Format('V', version);
            if (version.IsPrerelease)
            {
                str = str + "-" + version.Release;
            }

            return str;
        }

        /// <summary>
        /// Create the full version string including metadata. This is primarily for display purposes.
        /// </summary>
        private static string? GetFullString(SemanticVersion version)
        {
            var str = GetNormalizedString(version);
            if (version.HasMetadata)
            {
                str = str + "+" + version.Metadata;
            }

            return str;
        }

        private static string? Format(char c, SemanticVersion version) =>
            c switch
            {
                'F' => GetFullString(version),
                'M' => version.Metadata,
                'N' => GetNormalizedString(version),
                'R' => version.Release,
                'V' => FormatVersion(version),
                'x' => $"{version.Major}",
                'y' => $"{version.Minor}",
                'z' => $"{version.Patch}",
                't' => $"{version.Hotfix}",
                _ => null
            };

        private static string FormatVersion(SemanticVersion version) =>
            $"{version.Major}.{version.Minor}.{version.Patch}{GetHotfixPart(version)}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetHotfixPart(SemanticVersion version) =>
            !version.Hotfix.HasValue || version.Hotfix == 0 ? string.Empty : $".{version.Hotfix}";
    }
}