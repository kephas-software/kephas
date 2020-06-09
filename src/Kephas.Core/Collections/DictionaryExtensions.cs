// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Extension methods for <see cref="IDictionary{TKey,TValue}" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Collections
{
    using System;
    using System.Collections.Generic;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Tries to get the value for the provided key. If the requested item cannot be found, the default value is returned.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The item's key.</param>
        /// <param name="defaultValue">The default value to return if the item could not be found.</param>
        /// <returns>The found value, or the default value.</returns>
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            Requires.NotNull(dictionary, nameof(dictionary));
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
 
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}