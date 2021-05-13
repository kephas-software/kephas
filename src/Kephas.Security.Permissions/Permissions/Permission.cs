// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Permission.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Permissions
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Stores permission information.
    /// </summary>
    public class Permission : IPermission, IEquatable<Permission>
    {
        /// <summary>
        /// The separator character for inlining the permissions.
        /// </summary>
        public const char SeparatorChar = ':';

        /// <summary>
        /// Initializes a new instance of the <see cref="Permission"/> class.
        /// </summary>
        /// <param name="tokenName">The token name.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="sections">The sections.</param>
        public Permission(string tokenName, string? scope, string[]? sections)
        {
            Requires.NotNull(tokenName, nameof(tokenName));

            this.TokenName = tokenName;
            this.Scope = scope;
            this.Sections = sections;
        }

        /// <summary>
        /// Gets the token name.
        /// </summary>
        public string TokenName { get; }

        /// <summary>
        /// Gets the permission scope.
        /// </summary>
        public string? Scope { get; }

        /// <summary>
        /// Gets the sections within the scope.
        /// </summary>
        public string[]? Sections { get; }

        /// <summary>
        /// Returns true if both permissions are equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if both permissions are equal, false otherwise.</returns>
        public static bool operator ==(Permission? left, Permission? right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Returns true if permissions are not equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if permissions are not equal, false otherwise.</returns>
        public static bool operator !=(Permission? left, Permission? right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Parses the provided string and returns a constructed <see cref="Permission"/> instance.
        /// </summary>
        /// <param name="permissionString">The inlined permission.</param>
        /// <returns>A new permission created by parsing the provided string.</returns>
        public static Permission Parse(string permissionString)
        {
            Requires.NotNullOrEmpty(permissionString, nameof(permissionString));

            var splits = permissionString.Split(SeparatorChar);
            var tokenName = EnsureNormalizedString(splits[0]);
            string? scope = null;
            string[]? sections = null;
            if (splits.Length > 1)
            {
                scope = EnsureNormalizedString(splits[1]);
                if (splits.Length > 2)
                {
#if NETSTANDARD2_0
                    sections = splits.Skip(2).Select(EnsureNormalizedString).ToArray();
#else
                    sections = splits[2..].Select(EnsureNormalizedString).ToArray();
#endif
                }
            }

            return new Permission(tokenName, scope, sections);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (obj is not Permission perm)
            {
                return false;
            }

            return this.Equals(perm);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(Permission? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.TokenName == this.TokenName &&
                   other.Scope == this.Scope &&
                   other.Sections?.Length == this.Sections?.Length &&
                   (other.Sections == null || other.Sections.SequenceEqual(this.Sections!));
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => this.ToString().GetHashCode();

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder(this.TokenName);
            if (this.Scope != null)
            {
                sb.Append(SeparatorChar).Append(this.Scope);
                this.Sections?.ForEach(s => sb.Append(SeparatorChar).Append(s));
            }

            return sb.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string EnsureNormalizedString(string? str)
        {
            Requires.NotNullOrEmpty(str, nameof(str));
            return str!;
        }
    }
}