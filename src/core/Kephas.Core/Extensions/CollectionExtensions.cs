// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for collections.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

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
        /// <returns>The provided collection for methods chaining.</returns>
        public static T AddRange<T, TItem>(this T collection, IEnumerable<TItem> items)
            where T : class, ICollection<TItem>
        {
            Contract.Requires(collection != null);

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