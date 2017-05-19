// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextCache.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        private readonly IDictionary<Id, IEntityInfo> items;

        /// <summary>
        /// The entity information mappings.
        /// </summary>
        private readonly IDictionary<object, IEntityInfo> entityInfoMappings;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextCache"/> class.
        /// </summary>
        public DataContextCache()
            : this(new Dictionary<Id, IEntityInfo>(), new Dictionary<object, IEntityInfo>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextCache"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="entityInfoMappings">The entity information mappings.</param>
        protected DataContextCache(IDictionary<Id, IEntityInfo> items, IDictionary<object, IEntityInfo> entityInfoMappings)
        {
            Requires.NotNull(items, nameof(items));
            Requires.NotNull(entityInfoMappings, nameof(entityInfoMappings));

            this.items = items;
            this.entityInfoMappings = entityInfoMappings;
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
        bool ICollection<KeyValuePair<Id, IEntityInfo>>.IsReadOnly => this.items.IsReadOnly;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the
        /// <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the
        /// object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </value>
        public ICollection<Id> Keys => this.items.Keys;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in
        /// the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the
        /// object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </value>
        public ICollection<IEntityInfo> Values => this.items.Values;

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        public IEntityInfo this[Id key]
        {
            get
            {
                IEntityInfo value;
                if (!this.TryGetValue(key, out value))
                {
                    // TODO add a resource
                    throw new KeyNotFoundException();
                }

                return value;
            }

            set
            {
                Requires.NotNull(key, nameof(key));
                Requires.NotNull(value, nameof(value));

                if (value.Id != key)
                {
                    throw new ArgumentException(string.Format(Strings.DataContextCache_KeyAndEntityInfoIdDoNotMatch_Exception, key, value.Id), nameof(value));
                }

                IEntityInfo existingItem;
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
        public virtual bool TryGetValue(Id key, out IEntityInfo value)
        {
            return this.items.TryGetValue(key, out value);
        }

        /// <summary>
        /// Adds an <see cref="IEntityInfo"/> to the <see cref="IDataContextCache" />.
        /// </summary>
        /// <param name="value">The object to add.</param>
        public virtual void Add(IEntityInfo value)
        {
            Requires.NotNull(value, nameof(value));

            this.items.Add(value.Id, value);
            this.entityInfoMappings.Add(value.Entity, value);
        }

        /// <summary>
        /// Removes an <see cref="IEntityInfo"/> from the <see cref="IDataContextCache" />.
        /// </summary>
        /// <param name="value">The object to remove.</param>
        /// <returns>
        /// <c>true</c> if the removal succeeds, <c>false</c> otherwise.
        /// </returns>
        public virtual bool Remove(IEntityInfo value)
        {
            Requires.NotNull(value, nameof(value));

            var result = this.items.Remove(value.Id);
            if (result)
            {
                this.entityInfoMappings.Remove(value.Entity);
            }

            return result;
        }

        /// <summary>
        /// Gets the entity information associated to the provided entity, or <c>null</c> if none could be found.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity information or <c>null</c>.
        /// </returns>
        public virtual IEntityInfo GetEntityInfo(object entity)
        {
            var entityInfoAware = entity as IEntityInfoAware;
            var entityInfo = entityInfoAware?.GetEntityInfo();
            if (entityInfo == null)
            {
                this.entityInfoMappings.TryGetValue(entity, out entityInfo);
            }

            return entityInfo;
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
        public virtual bool ContainsKey(Id key) => this.items.ContainsKey(key);

        /// <summary>Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.</summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
        void IDictionary<Id, IEntityInfo>.Add(Id key, IEntityInfo value)
        {
            Requires.NotNull(key, nameof(key));
            Requires.NotNull(value, nameof(value));

            if (value.Id != key)
            {
                throw new ArgumentException(string.Format(Strings.DataContextCache_KeyAndEntityInfoIdDoNotMatch_Exception, key, value.Id), nameof(value));
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
        public bool Remove(Id key)
        {
            IEntityInfo value;
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
        void ICollection<KeyValuePair<Id, IEntityInfo>>.Add(KeyValuePair<Id, IEntityInfo> item)
        {
            this.Add(item.Value);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public virtual void Clear()
        {
            this.items.Clear();
            this.entityInfoMappings.Clear();
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
        bool ICollection<KeyValuePair<Id, IEntityInfo>>.Contains(KeyValuePair<Id, IEntityInfo> item) => this.items.Contains(item);

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an
        /// <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination
        ///                     of the elements copied from
        ///                     <see cref="T:System.Collections.Generic.ICollection`1" />. The
        ///                     <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(KeyValuePair<Id, IEntityInfo>[] array, int arrayIndex) => this.items.CopyTo(array, arrayIndex);

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
        bool ICollection<KeyValuePair<Id, IEntityInfo>>.Remove(KeyValuePair<Id, IEntityInfo> item)
        {
            return this.Remove(item.Key);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<Id, IEntityInfo>> GetEnumerator() => this.items.GetEnumerator();

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