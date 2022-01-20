// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisHash.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Redis
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Serialization;
    using StackExchange.Redis;

    /// <summary>
    /// The Redis hash.
    /// </summary>
    public class RedisHash : IRedisHash
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedisHash"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="listName">Name of the list.</param>
        internal RedisHash(IDatabase database, string listName)
        {
            this.Database = database;
            this.ListName = listName;
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        protected IDatabase Database { get; }

        /// <summary>
        /// Gets the list name.
        /// </summary>
        protected string ListName { get; }

        /// <summary>
        /// Adds the serialized entity with the given ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="entityString">[out] The serialized entity.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public bool Add(string id, string entityString)
        {
            this.Database.HashSet(this.ListName, new HashEntry[] { new HashEntry(id, entityString) });
            return true;
        }

        /// <summary>
        /// Query if the hash contains the entity with the given key.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public bool ContainsKey(string id)
        {
            return this.Database.HashExists(this.ListName, id);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public IEnumerator GetEnumerator() => this.GetEnumeratorCore();

        /// <summary>
        /// Removes the entry with the given ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public bool Remove(string id)
        {
            return this.Database.HashDelete(this.ListName, id);
        }

        /// <summary>
        /// Tris to get a value from the hash.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="entityString">[out] The serialized entity.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public bool TryGetValue(string id, out string? entityString)
        {
            var value = this.Database.HashGet(this.ListName, id);
            if (!value.HasValue || value.IsNullOrEmpty)
            {
                entityString = null;
                return false;
            }

            entityString = value.ToString();
            return true;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        protected virtual IEnumerator GetEnumeratorCore()
        {
            var entries = this.Database.HashGetAll(this.ListName);
            return entries.Select(e => e.Value.ToString()).GetEnumerator();
        }
    }

    /// <summary>
    /// The redis hash.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class RedisHash<T> : RedisHash, IRedisHash<T>
        where T : class
    {
        private readonly ISerializationService serializationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisHash{T}"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="serializationService">The serialization service.</param>
        internal RedisHash(IDatabase database, string listName, ISerializationService serializationService)
            : base(database, listName)
        {
            this.serializationService = serializationService;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public new IEnumerator<T> GetEnumerator()
        {
            var entries = this.Database.HashGetAll(this.ListName);
            return entries.Select(e => this.serializationService.JsonDeserialize<T>(e.Value)).GetEnumerator();
        }

        /// <summary>
        /// Attempts to get value from the given data.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="entity">[out] The entity.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public bool TryGetValue(string id, out T? entity)
        {
            if (!base.TryGetValue(id, out var entityString))
            {
                entity = default;
                return false;
            }

            entity = this.serializationService.JsonDeserialize<T>(entityString);
            return true;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        protected override IEnumerator GetEnumeratorCore() => this.GetEnumerator();
    }
}
