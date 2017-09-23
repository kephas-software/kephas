// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for collections.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Collections
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Resources;

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
        public static T AddRange<T, TItem>(this T collection, IEnumerable<TItem> items)
            where T : class, ICollection<TItem>
        {
            Requires.NotNull(collection, nameof(collection));

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

        /// <summary>
        /// Adds a range of items to the collection.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="items">The items.</param>
        /// <returns>The provided collection for method chaining.</returns>
        public static IProducerConsumerCollection<T> AddRange<T>(this IProducerConsumerCollection<T> collection, IEnumerable<T> items)
        {
            Requires.NotNull(collection, nameof(collection));

            if (items == null)
            {
                return collection;
            }

            foreach (var newItem in items)
            {
                if (!collection.TryAdd(newItem))
                {
                    throw new InvalidOperationException(string.Format(Strings.ConcurrentCollection_CannotAddItem_Exception, newItem));
                }
            }

            return collection;
        }
    }
}