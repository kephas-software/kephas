// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiScopeCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Options
{
    using System;
    using System.Collections.Generic;

    using global::IdentityServer4.Models;

    /// <summary>
    /// A collection of <see cref="ApiScope"/>.
    /// </summary>
    public class ApiScopeCollection : ResourceCollectionBase<ApiScope>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiScopeCollection"/> class.
        /// </summary>
        public ApiScopeCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiScopeCollection"/> class with the given
        /// API scopes in <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The initial list of <see cref="ApiScope"/>.</param>
        public ApiScopeCollection(IList<ApiScope> list)
            : base(list)
        {
        }

        /// <summary>
        /// Gets whether a given scope is defined or not.
        /// </summary>
        /// <param name="key">The name of the <see cref="ApiScope"/>.</param>
        /// <returns><c>true</c> when the scope is defined; <c>false</c> otherwise.</returns>
        public bool ContainsScope(string key)
        {
            foreach (var candidate in this.Items)
            {
                if (string.Equals(candidate.Name, key, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
