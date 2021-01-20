// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceCollectionBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Options
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using global::IdentityServer4.Models;

    /// <summary>
    /// Base class for resource collections.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    public abstract class ResourceCollectionBase<T> : Collection<T>
        where T : Resource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceCollectionBase{T}"/> class.
        /// </summary>
        protected ResourceCollectionBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceCollectionBase{T}"/> class with the given
        /// identity resources in <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The initial list of <see cref="T"/>.</param>
        protected ResourceCollectionBase(IList<T> list)
            : base(list)
        {
        }

        /// <summary>
        /// Gets an identity resource given its name.
        /// </summary>
        /// <param name="key">The name of the <see cref="T"/>.</param>
        /// <returns>The <see cref="T"/>.</returns>
        public T this[string key]
        {
            get
            {
                foreach (var candidate in this.Items)
                {
                    if (string.Equals(candidate.Name, key, StringComparison.Ordinal))
                    {
                        return candidate;
                    }
                }

                throw new InvalidOperationException($"{typeof(T).Name} '{key}' not found.");
            }
        }

        /// <summary>
        /// Adds the resources in <paramref name="resources"/> to the collection.
        /// </summary>
        /// <param name="resources">The list of <see cref="T"/> to add.</param>
        public void AddRange(params T[] resources)
        {
            foreach (var resource in resources)
            {
                this.Add(resource);
            }
        }
    }
}