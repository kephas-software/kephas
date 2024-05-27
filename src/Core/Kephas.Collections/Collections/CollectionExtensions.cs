// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Extension methods for collections.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Collections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods for collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds a range of items to the collection.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="items">The items.</param>
        /// <returns>The provided collection for method chaining.</returns>
        public static T AddRange<T, TItem>(this T collection, IEnumerable<TItem>? items)
            where T : class, ICollection<TItem>
        {
            collection = collection ?? throw new ArgumentNullException(nameof(collection));

            if (items == null)
            {
                return collection;
            }

            foreach (var item in items)
            {
                collection.Add(item);
            }

            return collection;
        }
    }
}