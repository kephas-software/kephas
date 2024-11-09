// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRedisHash.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Redis
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for Redis hash.
    /// </summary>
    public interface IRedisHash : IEnumerable
    {
        /// <summary>
        /// Tris to get a value from the hash.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="entityString">[out] The serialized entity.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        bool TryGetValue(string id, out string? entityString);

        /// <summary>
        /// Removes the entry with the given ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        bool Remove(string id);

        /// <summary>
        /// Query if the hash contains the entity with the given key.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        bool ContainsKey(string id);

        /// <summary>
        /// Adds the serialized entity with the given ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="entityString">[out] The serialized entity.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        bool Add(string id, string entityString);
    }

    /// <summary>
    /// Interface for redis hash.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public interface IRedisHash<T> : IRedisHash, IEnumerable<T>
        where T : class
    {
        /// <summary>
        /// Attempts to get value from the given data.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="entity">[out] The entity.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        bool TryGetValue(string id, out T? entity);
    }
}
