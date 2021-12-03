// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LLBLGenCollectionAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the llbl generate collection adapter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Data.Capabilities;

    using SD.LLBLGen.Pro.ORMSupportClasses;

    /// <summary>
    /// A collection adapter mapping a collection of entity abstractions onto LLBLGen collections of entity implementations.
    /// </summary>
    /// <typeparam name="TEntityImplementation">Type of the entity implementation.</typeparam>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public class LLBLGenCollectionAdapter<TEntityImplementation, TEntity> : ICollection<TEntity>
        where TEntityImplementation : TEntity
    {
        /// <summary>
        /// The entity containing this collection.
        /// </summary>
        private readonly WeakReference<IEntityEntryAware> containerEntity;

        /// <summary>
        /// The collection implementation.
        /// </summary>
        private readonly ICollection<TEntityImplementation> collectionImplementation;

        /// <summary>
        /// The relation.
        /// </summary>
        private readonly IRelationPredicateBucket relation;

        /// <summary>
        /// True if fetched.
        /// </summary>
        private bool fetched = false;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="LLBLGenCollectionAdapter{TEntityImplementation, TEntity}" /> class.
        /// </summary>
        /// <param name="entity">The containing entity.</param>
        /// <param name="collectionImplementation">The collection implementation.</param>
        /// <param name="relation">The relation.</param>
        public LLBLGenCollectionAdapter(IEntityEntryAware entity, ICollection<TEntityImplementation> collectionImplementation, IRelationPredicateBucket relation)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            collectionImplementation = collectionImplementation ?? throw new System.ArgumentNullException(nameof(collectionImplementation));
            relation = relation ?? throw new System.ArgumentNullException(nameof(relation));

            this.containerEntity = new WeakReference<IEntityEntryAware>(entity);
            this.collectionImplementation = collectionImplementation;
            this.relation = relation;
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public int Count
        {
            get
            {
                this.EnsureFetched();
                return this.collectionImplementation.Count;
            }
        }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
        public bool IsReadOnly => this.collectionImplementation.IsReadOnly;

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            this.EnsureFetched();
            return (IEnumerator<TEntity>)this.collectionImplementation.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public void Add(TEntity item)
        {
            this.EnsureFetched();
            this.collectionImplementation.Add((TEntityImplementation)item);
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
        public void Clear()
        {
            this.EnsureFetched();
            this.collectionImplementation.Clear();
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.</summary>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public bool Contains(TEntity item)
        {
            this.EnsureFetched();
            return this.collectionImplementation.Contains((TEntityImplementation)item);
        }

        /// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
        void ICollection<TEntity>.CopyTo(TEntity[] array, int arrayIndex)
        {
            array = array ?? throw new System.ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (array.Rank > 1)
            {
                // TODO localization
                throw new ArgumentException("Array is multidimensional.");
            }

            if (array.Length - arrayIndex < this.Count)
            {
                // TODO localization
                throw new ArgumentException("Not enough elements after index in the destination array.");
            }

            var itemsList = this.collectionImplementation.ToArray();
            for (var i = 0; i < this.Count; ++i)
            {
                array.SetValue(itemsList[i], i + arrayIndex);
            }
        }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public bool Remove(TEntity item)
        {
            this.EnsureFetched();
            return this.collectionImplementation.Remove((TEntityImplementation)item);
        }

        /// <summary>
        /// Ensures that data is fetched.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when a supplied object has been disposed.</exception>
        private void EnsureFetched()
        {
            if (this.fetched)
            {
                return;
            }

            if (!this.containerEntity.TryGetTarget(out var entity))
            {
                throw new ObjectDisposedException("The collection is already disposed.");
            }

            var entityEntry = entity.GetEntityEntry();
            if (entityEntry.ChangeState == ChangeState.Added)
            {
                // do not do anything for newly created entities
                return;
            }

            var dataContext = (ILLBLGenDataContext)entityEntry.DataContext;
            dataContext.DataAccessAdapter.FetchEntityCollection((IEntityCollection2)this.collectionImplementation, this.relation);
            this.fetched = true;
        }
    }
}