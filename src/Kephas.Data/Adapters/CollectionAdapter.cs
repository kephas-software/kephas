// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionAdapter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the entity collection adapter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Adapters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// An entity collection adapter.
    /// </summary>
    /// <remarks>
    /// It assumes that even if exposed through an interface,
    /// all items in the collection are of type <typeparamref name="TEntityImplementation"/>.
    /// </remarks>
    /// <typeparam name="TEntityImplementation">Type of the entity implementation.</typeparam>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public class CollectionAdapter<TEntityImplementation, TEntity> : ICollection<TEntity>
        where TEntityImplementation : TEntity
    {
        /// <summary>
        /// The collection implementation.
        /// </summary>
        private readonly ICollection<TEntityImplementation> collectionImplementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionAdapter{TEntityImplementation,TEntity}"/> class.
        /// </summary>
        /// <param name="collectionImplementation">The collection implementation.</param>
        public CollectionAdapter(ICollection<TEntityImplementation> collectionImplementation)
        {
            Requires.NotNull(collectionImplementation, nameof(collectionImplementation));

            this.collectionImplementation = collectionImplementation;
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public int Count => this.collectionImplementation.Count;

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
        public bool IsReadOnly => this.collectionImplementation.IsReadOnly;

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
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
            this.collectionImplementation.Add((TEntityImplementation)item);
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
        public void Clear()
        {
            this.collectionImplementation.Clear();
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.</summary>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public bool Contains(TEntity item)
        {
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
            throw new NotSupportedException($"CopyTo is not supported in {nameof(CollectionAdapter<TEntityImplementation, TEntity>)}");
        }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public bool Remove(TEntity item)
        {
            return this.collectionImplementation.Remove((TEntityImplementation)item);
        }
    }
}