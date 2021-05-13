// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Permission.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization.Permissions
{
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Stores permission information.
    /// </summary>
    public class Permission : IPermission
    {
        /// <summary>
        /// The separator character for inlining the permissions.
        /// </summary>
        public const char SeparatorChar = ':';

        /// <summary>
        /// Initializes a new instance of the <see cref="Permission"/> class.
        /// </summary>
        /// <param name="permissionString">The inlined permission.</param>
        public Permission(string permissionString)
        {
            Requires.NotNullOrEmpty(permissionString, nameof(permissionString));

            var splits = permissionString.Split(SeparatorChar);
            this.TokenName = EnsureNormalizedString(splits[0]);
            if (splits.Length > 1)
            {
                this.Scope = EnsureNormalizedString(splits[1]);
                if (splits.Length > 2)
                {
#if NETSTANDARD2_0
                    this.Sections = splits.Skip(2).Select(EnsureNormalizedString).ToArray();
#else
                    this.Sections = splits[2..].Select(EnsureNormalizedString).ToArray();
#endif
                }
            }
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