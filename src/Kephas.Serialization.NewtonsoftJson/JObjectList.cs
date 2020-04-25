// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JObjectList.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the object list class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// List of j objects.
    /// </summary>
    public class JObjectList : IList<object>
    {
        private readonly JArray array;

        /// <summary>
        /// Initializes a new instance of the <see cref="JObjectList"/> class.
        /// </summary>
        /// <param name="array">The array.</param>
        public JObjectList(JArray array)
        {
            this.array = array;
        }

        /// <summary>
        /// Gets the number of elements in this list.
        /// </summary>
        /// <value>
        /// The number of elements in this list.
        /// </value>
        public int Count => this.array.Count;

        /// <summary>
        /// Gets a value indicating whether this object is read only.
        /// </summary>
        /// <value>
        /// True if this object is read only, false if not.
        /// </value>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException">.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the
        ///                                                  <see cref="T:System.Collections.Generic.IList`1"></see>
        ///                                                  is read-only.</exception>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        public object this[int index]
        {
            get => this.array[index].Unwrap();
            set => this.array[index] = value.Wrap();
        }

        /// <summary>
        /// Adds a new item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(object item)
        {
            this.array.Add(item.Wrap());
        }

        /// <summary>
        /// Clears this object to its blank/initial state.
        /// </summary>
        public void Clear()
        {
            this.array.Clear();
        }

        /// <summary>
        /// Query if this object contains the given item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>
        /// True if the object is in this collection, false if not.
        /// </returns>
        public bool Contains(object item)
        {
            return this.array.Contains(item.Wrap());
        }

        void ICollection<object>.CopyTo(object[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public IEnumerator<object> GetEnumerator()
        {
            return this.array.Select(e => e.Unwrap()).GetEnumerator();
        }

        /// <summary>
        /// Determines the index of a specific item in the
        /// <see cref="T:System.Collections.Generic.IList`1"></see>.
        /// </summary>
        /// <param name="item">The object to locate in the
        ///                    <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
        /// <returns>
        /// The index of <paramref name="item">item</paramref> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(object item)
        {
            return this.array.IndexOf(item.Wrap());
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"></see> at the
        /// specified index.
        /// </summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException">.</exception>
        /// <exception cref="T:System.NotSupportedException">The
        ///                                                  <see cref="T:System.Collections.Generic.IList`1"></see>
        ///                                                  is read-only.</exception>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the
        ///                    <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
        public void Insert(int index, object item)
        {
            this.array.Insert(index, item.Wrap());
        }

        /// <summary>
        /// Removes the given item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public bool Remove(object item)
        {
            return this.array.Remove(item.Wrap());
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"></see> item at the specified
        /// index.
        /// </summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException">.</exception>
        /// <exception cref="T:System.NotSupportedException">The
        ///                                                  <see cref="T:System.Collections.Generic.IList`1"></see>
        ///                                                  is read-only.</exception>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            this.array.RemoveAt(index);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return this.array.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is JArray jarr
                ? ReferenceEquals(this.array, jarr)
                : obj is JObjectList jlist
                    ? ReferenceEquals(this, jlist) || ReferenceEquals(this.array, jlist.array)
                    : false;
        }
    }
}
