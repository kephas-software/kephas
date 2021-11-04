// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextCache.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data context cache class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Caching
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Resources;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A basic implementation of a data context cache.
    /// </summary>
    public class DataContextCache : IDataContextCache
    {
        /// <summary>
        /// The items.
        /// </summary>
        private readonly IDictionary<object, IEntityEntry> items;

        /// <summary>
        /// The entity entry mappings.
        /// </summary>
        private readonly IDictionary<object, IEntityEntry> entityEntryMappings;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextCache"/> class.
        /// </summary>
        public DataContextCache()
            : this(new Dictionary<object, IEntityEntry>(), new Dictionary<object, IEntityEntry>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextCache"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="entityEntryMappings">The entity entry mappings.</param>
        protected DataContextCache(IDictionary<object, IEntityEntry> items, IDictionary<object, IEntityEntry> entityEntryMappings)
        {
            items = items ?? throw new ArgumentNullException(nameof(items));
            Requires.NotNull(entityEntryMappings, nameof(entityEntryMappings));

            this.items = items;
            this.entityEntryMappings = entityEntryMappings;
        }

        /// <summary>
        /// Gets the number of elements contained in the
        /// <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <value>
        /// The number of elements contained in the
        /// <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </value>
        public int Count => this.items.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" />
        /// is read-only.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise,
        /// <c>false</c>.
        /// </value>
        bool ICollection<KeyValuePair<object, IEntityEntry>>.IsReadOnly => this.items.IsReadOnly;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the
        /// <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the
        /// object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </value>
        public ICollection<object> Keys => this.items.Keys;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in
        /// the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the
        /// object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </value>
        public ICollection<IEntityEntry> Values => this.items.Values;

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        public IEntityEntry this[object key]
        {
            get
            {
                IEntityEntry value;
                if (!this.TryGetValue(key, out value))
                {
                    // TODO localization add a resource
                    throw new KeyNotFoundException();
                }

                return value;
            }

            set
            {
                key = key ?? throw new ArgumentNullException(nameof(key));
                value = value ?? throw new ArgumentNullException(nameof(value));

                if (value.Id != key)
                {
                    throw new ArgumentException(string.Format(Strings.DataContextCache_KeyAndEntityEntryIdDoNotMatch_Exception, key, value.Id), nameof(value));
                }

                IEntityEntry existingItem;
                if (this.TryGetValue(key, out existingItem) && existingItem == value)
                {
                    return;
                }

                if (existingItem != null)
                {
                    this.Remove(existingItem);
                }

                this.Add(value);
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified
        ///                     key, if the key is found; otherwise, the default value for the type of
        ///                     the <paramref name="value" /> parameter. This parameter is passed
        ///                     uninitialized.</param>
        /// <returns>
        /// <c>true</c> if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />
        /// contains an element with the specified key; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool TryGetValue(object key, out IEntityEntry value)
        {
            return this.items.TryGetValue(key, out value);
        }

        /// <summary>
        /// Adds an <see cref="IEntityEntry"/> to the <see cref="IDataContextCache" />.
        /// </summary>
        /// <param name="value">The object to add.</param>
        public virtual void Add(IEntityEntry value)
        {
            value = value ?? throw new ArgumentNullException(nameof(value));

            this.items.Add(value.Id, value);
            this.entityEntryMappings.Add(value.Entity, value);
        }

        /// <summary>
        /// Removes an <see cref="IEntityEntry"/> from the <see cref="IDataContextCache" />.
        /// </summary>
        /// <param name="value">The object to remove.</param>
        /// <returns>
        /// <c>true</c> if the removal succeeds, <c>false</c> otherwise.
        /// </returns>
        public virtual bool Remove(IEntityEntry value)
        {
            value = value ?? throw new ArgumentNullException(nameof(value));

            var result = this.items.Remove(value.Id);
            if (result)
            {
                this.entityEntryMappings.Remove(value.Entity);
            }

            return result;
        }

        /// <summary>
        /// Gets the entity entry associated to the provided entity, or <c>null</c> if none could be found.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity entry or <c>null</c>.
        /// </returns>
        public virtual IEntityEntry GetEntityEntry(object entity)
        {
            var entityEntry = entity.TryGetAttachedEntityEntry();
            if (entityEntry == null)
            {
                this.entityEntryMappings.TryGetValue(entity, out entityEntry);

                // cache the entity info for later use in the entity.
                entityEntry?.TryAttachToEntity(entity);
            }

            return entityEntry;
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an
        /// element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the
        ///                   <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element
        /// with the key; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool ContainsKey(object key) => this.items.ContainsKey(key);

        /// <summary>Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.</summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
        void IDictionary<object, IEntityEntry>.Add(object key, IEntityEntry value)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));
            value = value ?? throw new ArgumentNullException(nameof(value));

            if (value.Id != key)
            {
                throw new ArgumentException(string.Format(Strings.DataContextCache_KeyAndEntityEntryIdDoNotMatch_Exception, key, value.Id), nameof(value));
            }

            this.Add(value);
        }

        /// <summary>
        /// Removes the element with the specified key from the
        /// <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// <c>true</c> if the element is successfully removed; otherwise, <c>false</c>.  This method also returns
        /// <c>false</c> if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </returns>
        public bool Remove(object key)
        {
            IEntityEntry value;
            if (!this.TryGetValue(key, out value))
            {
                return false;
            }

            return this.Remove(value);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        void ICollection<KeyValuePair<object, IEntityEntry>>.Add(KeyValuePair<object, IEntityEntry> item)
        {
            this.Add(item.Value);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public virtual void Clear()
        {
            this.items.Clear();
            this.entityEntryMappings.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a
        /// specific value.
        /// </summary>
        /// <param name="item">The object to locate in the
        ///                    <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="item" /> is found in the
        /// <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <c>false</c>.
        /// </returns>
        bool ICollection<KeyValuePair<object, IEntityEntry>>.Contains(KeyValuePair<object, IEntityEntry> item) => this.items.Contains(item);

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an
        /// <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination
        ///                     of the elements copied from
        ///                     <see cref="T:System.Collections.Generic.ICollection`1" />. The
        ///                     <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(KeyValuePair<object, IEntityEntry>[] array, int arrayIndex) => this.items.CopyTo(array, arrayIndex);

        /// <summary>
        /// Removes the first occurrence of a specific object from the
        /// <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the
        ///                    <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="item" /> was successfully removed from the
        /// <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <c>false</c>. This method also
        /// returns <c>false</c> if <paramref name="item" /> is not found in the original
        /// <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        bool ICollection<KeyValuePair<object, IEntityEntry>>.Remove(KeyValuePair<object, IEntityEntry> item)
        {
            return this.Remove(item.Key);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<object, IEntityEntry>> GetEnumerator() => this.items.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through
        /// the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}