// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppIdentity.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the app identity class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;

    using Kephas.Versioning;

    /// <summary>
    /// An app identity.
    /// </summary>
    public sealed record AppIdentity
    {
        private const char ItemSeparatorChar = ':';

        private static readonly char[] InvalidChars = { ItemSeparatorChar, ';', ',', '|', '/', '\\', '<', '>', '?', '\'', '"', '*', '@', '#', '$', '^', '`', '[', ']', '{', '}' };

        /// <summary>
        /// Initializes a new instance of the <see cref="AppIdentity"/> class.
        /// </summary>
        /// <param name="id">The app ID.</param>
        /// <param name="version">Optional. The version.</param>
        public AppIdentity(string id, SemanticVersion? version)
        {
            if (!this.IsValidId(id, out var message))
            {
                throw new ArgumentException(message, nameof(id));
            }

            this.Id = id;
            this.Version = version;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppIdentity"/> class.
        /// </summary>
        /// <param name="id">The app ID.</param>
        /// <param name="version">Optional. The version.</param>
        public AppIdentity(string id, string? version = null)
        {
            if (!this.IsValidId(id, out var message))
            {
                throw new ArgumentException(message, nameof(id));
            }

            if (!this.TryParseVersion(version, out var semanticVersion, out message))
            {
                throw new ArgumentException(message, nameof(version));
            }

            this.Id = id;
            this.Version = semanticVersion;
        }

        /// <summary>
        /// Gets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public SemanticVersion? Version { get; }

        /// <summary>
        /// Parses a string and returns an instance of <see cref="AppIdentity"/>.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>
        /// An AppIdentity.
        /// </returns>
        public static AppIdentity Parse(string value)
        {
            value = value ?? throw new ArgumentNullException(nameof(value));

            var indexOfSeparator = value.IndexOf(ItemSeparatorChar);
            if (indexOfSeparator >= 0)
            {
                return new AppIdentity(value[..indexOfSeparator], value[(indexOfSeparator + 1)..]);
            }

            return new AppIdentity(value);
        }

        /// <summary>
        /// Returns a value indicating whether this identity matches the provided one.
        /// It is considered a match when the IDs are the same and either the versions are
        /// the same or this version is <c>null</c>.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// True if match, false if not.
        /// </returns>
        public bool IsMatch(AppIdentity? other)
        {
            return other != null
                && this.Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase)
                && (this.Version?.Equals(other.Version) ?? true);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.Version == null ? this.Id : $"{this.Id}{ItemSeparatorChar}{this.Version}";
        }

        private bool IsValidId(string? id, out string? message)
        {
            if (string.IsNullOrEmpty(id))
            {
                message = $"The app ID may not be empty.";
                return false;
            }

            if (id.IndexOfAny(new[] { ' ', '\t', '\r', '\n' }) >= 0)
            {
                message = $"The app ID '{id}' may not contain whitespace.";
                return false;
            }

            var invalidCharPos = id.IndexOfAny(InvalidChars);
            if (invalidCharPos >= 0)
            {
                message = $"The app ID '{id}' may not contain '{id[invalidCharPos]}'. Not allowed characters: '{string.Join(string.Empty, InvalidChars)}'.";
                return false;
            }

            message = null;
            return true;
        }

        private bool TryParseVersion(string? version, out SemanticVersion? semanticVersion, out string? message)
        {
            message = null;
            semanticVersion = null;

            if (string.IsNullOrEmpty(version))
            {
                return true;
            }

            if (version.IndexOfAny(new[] { ' ', '\t', '\r', '\n' }) >= 0)
            {
                message = $"The app version '{version}' may not contain whitespace.";
                return false;
            }

            var invalidCharPos = version.IndexOfAny(InvalidChars);
            if (invalidCharPos >= 0)
            {
                message = $"The app version '{version}' may not contain '{version[invalidCharPos]}'. Not allowed characters: '{string.Join(string.Empty, InvalidChars)}'.";
                return false;
            }

            if (!SemanticVersion.TryParse(version, out semanticVersion))
            {
                message = $"The app version '{version}' is invalid.";
                return false;
            }

            message = null;
            return true;
        }
    }
}
