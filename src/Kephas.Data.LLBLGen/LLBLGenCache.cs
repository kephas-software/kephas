// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LLBLGenCache.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the llbl generate cache class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Data;
    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.LLBLGen.Resources;
    using Kephas.Diagnostics.Contracts;

    using SD.LLBLGen.Pro.ORMSupportClasses;

    /// <summary>
    /// The context cache for LLBLGen.
    /// </summary>
    public class LLBLGenCache : Context, IDataContextCache
    {
        /// <summary>
        /// The items.
        /// </summary>
        private readonly IDictionary<object, IEntityEntry> items;

        /// <summary>
        /// The entity information mappings.
        /// </summary>
        private readonly IDictionary<Guid, IEntityEntry> entityEntryMappings;

        /// <summary>
        /// Indicates whether the cache is currently adding an item.
        /// Used to synchronize the dictionaries with the native cache.
        /// </summary>
        private bool isAddingItem;

        /// <summary>
        /// Indicates whether the cache is currently removing an item.
        /// Used to synchronize the dictionaries with the native cache.
        /// </summary>
        private bool isRemovingItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="LLBLGenCache"/> class.
        /// </summary>
        public LLBLGenCache()
            : this(new Dictionary<object, IEntityEntry>(), new Dictionary<Guid, IEntityEntry>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LLBLGenCache"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="entityEntryMappings">The entity information mappings.</param>
        protected LLBLGenCache(IDictionary<object, IEntityEntry> items, IDictionary<Guid, IEntityEntry> entityEntryMappings)
        {
            Requires.NotNull(items, nameof(items));
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
        public bool IsReadOnly => this.items.IsReadOnly;

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
        /// The new entities.
        /// </summary>
        protected internal new Dictionary<Guid, IEntityCore> NewEntities => base.NewEntities;

        /// <summary>
        /// Gets or sets the entity information factory.
        /// </summary>
        protected internal Func<object, IEntityEntry> EntityInfoFactory { get; set; }

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
                if (!this.TryGetValue(key, out var value))
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

                if (!key.Equals(value.Id))
                {
                    throw new ArgumentException(string.Format(Strings.DataContextCache_KeyAndEntityInfoIdDoNotMatch_Exception, key, value.Id), nameof(value));
                }

                if (this.TryGetValue(key, out var existingItem) && existingItem == value)
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
        /// Adds an <see cref="T:Kephas.Data.Capabilities.IEntityEntry" /> to the <see cref="T:Kephas.Data.Caching.IDataContextCache" />.
        /// </summary>
        /// <param name="value">The object to add.</param>
        public void Add(IEntityEntry value)
        {
            Requires.NotNull(value, nameof(value));

            this.AddCore(value);

            try
            {
                this.isAddingItem = true;
                base.Add((IEntityCore)value.Entity);
            }
            finally
            {
                this.isAddingItem = false;
            }
        }

        /// <summary>
        /// Removes an <see cref="T:Kephas.Data.Capabilities.IEntityEntry" /> from the <see cref="T:Kephas.Data.Caching.IDataContextCache" />.
        /// </summary>
        /// <param name="value">The object to remove.</param>
        /// <returns>
        /// <c>true</c> if the removal succeeds, <c>false</c> otherwise.
        /// </returns>
        public bool Remove(IEntityEntry value)
        {
            Requires.NotNull(value, nameof(value));

            var result = this.RemoveCore(value);
            if (result)
            {
                try
                {
                    this.isRemovingItem = true;
                    base.Remove((IEntityCore)value.Entity);
                }
                finally
                {
                    this.isRemovingItem = false;
                }
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
        public virtual IEntityEntry GetEntityEntry(object entity)
        {
            var wellKnownEntity = entity as IEntityEntryAware;
            var entityEntry = wellKnownEntity?.GetEntityEntry();
            if (entityEntry == null)
            {
                entityEntry = this.TryGetMappedEntityEntry(entity);
                if (entityEntry != null)
                {
                    wellKnownEntity?.SetEntityEntry(entityEntry);
                }
                else if (entity is IIdentifiable identifiableEntity && !Id.IsEmpty(identifiableEntity.Id))
                {
                    // try to get from the cache an entity with the same type and ID, and return it.
                    entityEntry = this.Values.FirstOrDefault(
                        ei => object.Equals(ei.EntityId, identifiableEntity.Id) && ei.Entity.GetType() == entity.GetType());
                }
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
        public bool ContainsKey(object key) => this.items.ContainsKey(key);

        /// <summary>Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.</summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
        void IDictionary<object, IEntityEntry>.Add(object key, IEntityEntry value)
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
        public virtual bool Remove(object key)
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
        public virtual void CopyTo(KeyValuePair<object, IEntityEntry>[] array, int arrayIndex) => this.items.CopyTo(array, arrayIndex);

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
            return this.Remove(item.Value);
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

        /// <summary>
        /// Called at the end of the Clear() method.
        /// </summary>
        protected override void OnClearComplete()
        {
            base.OnClearComplete();

            this.items.Clear();
            this.entityEntryMappings.Clear();
        }

        /// <summary>
        /// Called at the end of Add(TEntity).
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="toAdd">To add.</param>
        protected override void OnAddComplete<TEntity>(TEntity toAdd)
        {
            base.OnAddComplete<TEntity>(toAdd);

            if (!this.isAddingItem && !this.ContainsMapping(toAdd))
            {
                var entityEntry = this.CreateEntityInfo(toAdd);
                this.AddCore(entityEntry);
            }
        }

        /// <summary>
        /// Adds an entity info to the internal dictionaries.
        /// </summary>
        /// <param name="value">The object to use as the value of the element to add.</param>
        protected virtual void AddCore(IEntityEntry value)
        {
            this.items.Add(value.Id, value);
            this.AddMapping(value.Entity, value);
        }

        /// <summary>
        /// Called at the end of Remove(IEntityCore), when toRemove has been removed from this context.
        /// </summary>
        /// <param name="toRemove">To remove.</param>
        protected override void OnRemoveComplete(IEntityCore toRemove)
        {
            base.OnRemoveComplete(toRemove);

            if (!this.isRemovingItem)
            {
                var entityEntry = this.TryGetMappedEntityEntry(toRemove);
                if (entityEntry != null)
                {
                    this.RemoveCore(entityEntry);
                }
            }
        }

        /// <summary>
        /// Removes the entity info from the internal dictionaries.
        /// </summary>
        /// <param name="value">The object to remove.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        protected virtual bool RemoveCore(IEntityEntry value)
        {
            var result = this.items.Remove(value.Id);
            if (result)
            {
                this.RemoveMapping(value.Entity);
            }

            return result;
        }

        /// <summary>
        /// Creates entity information.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The new entity information.
        /// </returns>
        protected virtual IEntityEntry CreateEntityInfo(object entity)
        {
            if (this.EntityInfoFactory == null)
            {
                // TODO localization
                throw new InvalidOperationException("Before creating an entity information, the EntityInfoFactory must be set. This is done usually in the DataContext constructor.");
            }

            return this.EntityInfoFactory(entity);
        }

        /// <summary>
        /// Adds a mapping to 'entityEntry'.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="entityEntry">Information describing the entity.</param>
        private void AddMapping(object entity, IEntityEntry entityEntry)
        {
            this.entityEntryMappings.Add(this.GetEntityKey(entity), entityEntry);
        }

        /// <summary>
        /// Removes the mapping described by entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        private bool RemoveMapping(object entity)
        {
            return this.entityEntryMappings.Remove(this.GetEntityKey(entity));
        }

        /// <summary>
        /// Query if 'entity' contains mapping.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        private bool ContainsMapping(object entity)
        {
            return this.entityEntryMappings.ContainsKey(this.GetEntityKey(entity));
        }

        /// <summary>
        /// Try get mapped entity information.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// An IEntityEntry.
        /// </returns>
        private IEntityEntry TryGetMappedEntityEntry(object entity)
        {
            return this.entityEntryMappings.TryGetValue(this.GetEntityKey(entity));
        }

        /// <summary>
        /// Gets entity key.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity key.
        /// </returns>
        private Guid GetEntityKey(object entity)
        {
            if (entity is IEntity2 llblgenEntity)
            {
                if (Guid.Empty == llblgenEntity.ObjectID)
                {
                    throw new DataException($"The entity {entity} does not have an ObjectID properly set (is empty) and cannot be used in the {this.GetType()}.");
                }

                return llblgenEntity.ObjectID;
            }

            // TODO localization
            throw new DataException($"The entity {entity} is not a LLBLGen entity and cannot be used in the {this.GetType()}.");
        }
    }
}