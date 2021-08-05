// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JObjectDictionary.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the object dictionary class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Dynamic;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Dictionary of objects.
    /// </summary>
    public class JObjectDictionary : IDictionary<string, object?>, IDynamic
    {
        private readonly JObject obj;

        /// <summary>
        /// Initializes a new instance of the <see cref="JObjectDictionary"/> class.
        /// </summary>
        /// <param name="obj">The object.</param>
        public JObjectDictionary(JObject obj)
        {
            this.obj = obj;
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the keys of
        /// the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the keys of the
        /// object that implements <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </value>
        public ICollection<string> Keys => this.obj.Properties().Select(prop => prop.Name).ToList();

        /// <summary>
        /// Gets a value indicating whether this object is read only.
        /// </summary>
        /// <value>
        /// True if this object is read only, false if not.
        /// </value>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values
        /// in the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values in the
        /// object that implements <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </value>
        public ICollection<object?> Values => this.obj.Properties().Select(prop => prop.Value.Unwrap()).ToList();

        /// <summary>
        /// Gets the number of items in this dictionary.
        /// </summary>
        /// <value>
        /// The number of items in this dictionary.
        /// </value>
        public int Count => this.obj.Count;

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">.</exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved
        ///                                                                     and
        ///                                                                     <paramref name="key">key</paramref>
        ///                                                                     is not found.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the
        ///                                                  <see cref="T:System.Collections.Generic.IDictionary`2"></see>
        ///                                                  is read-only.</exception>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        public object? this[string key]
        {
            get => this.obj[key].Unwrap();
            set => this.obj[key] = value.Wrap();
        }

        /// <summary>
        /// Adds an element with the provided key and value to the
        /// <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in
        ///                                              the
        ///                                              <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</exception>
        /// <exception cref="T:System.NotSupportedException">The
        ///                                                  <see cref="T:System.Collections.Generic.IDictionary`2"></see>
        ///                                                  is read-only.</exception>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public void Add(string key, object? value) => this.obj.Add(key, value.Wrap());

        /// <summary>
        /// Adds an element with the provided key and value to the
        /// <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void ICollection<KeyValuePair<string, object?>>.Add(KeyValuePair<string, object?> item)
            => this.obj.Add(item.Key, item.Value.Wrap());

        /// <summary>
        /// Clears this object to its blank/initial state.
        /// </summary>
        public void Clear() => this.obj.RemoveAll();

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />.</returns>
        bool ICollection<KeyValuePair<string, object?>>.Contains(KeyValuePair<string, object?> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
        void ICollection<KeyValuePair<string, object?>>.CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        bool ICollection<KeyValuePair<string, object?>>.Remove(KeyValuePair<string, object?> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">.</exception>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">[out] When this method returns, the value associated with the specified
        ///                     key, if the key is found; otherwise, the default value for the type of
        ///                     the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements
        /// <see cref="T:System.Collections.Generic.IDictionary`2"></see> contains an element with the
        /// specified key; otherwise, false.
        /// </returns>
        public bool TryGetValue(string key, out object? value)
        {
            var exists = this.obj.TryGetValue(key, out var token);
            value = token.Unwrap();
            return exists;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            return this.obj.Properties().Select(p => new KeyValuePair<string, object?>(p.Name, p.Value?.Unwrap())).GetEnumerator();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"></see> contains
        /// an element with the specified key.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">.</exception>
        /// <param name="key">The key to locate in the
        ///                   <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"></see> contains an element
        /// with the key; otherwise, false.
        /// </returns>
        public bool ContainsKey(string key) => this.obj.ContainsKey(key);

        /// <summary>
        /// Removes the element with the specified key from the
        /// <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">.</exception>
        /// <exception cref="T:System.NotSupportedException">The
        ///                                                  <see cref="T:System.Collections.Generic.IDictionary`2"></see>
        ///                                                  is read-only.</exception>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns
        /// false if <paramref name="key">key</paramref> was not found in the original
        /// <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </returns>
        public bool Remove(string key) => this.obj.Remove(key);

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
