// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Signature.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the signature class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A signature.
    /// </summary>
    public class Signature : IEquatable<Signature>, IEnumerable<Type>, IReadOnlyList<Type>
    {
        private readonly Type[] value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Signature"/> class.
        /// </summary>
        /// <param name="value">A variable-length parameters list containing value.</param>
        public Signature(params Type[] value)
        {
            this.value = value ?? Type.EmptyTypes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Signature"/> class.
        /// </summary>
        /// <param name="value">A variable-length parameters list containing value.</param>
        public Signature(IEnumerable<Type> value)
        {
            this.value = value?.ToArray() ?? Type.EmptyTypes;
        }

        /// <summary>
        /// Gets the number of items in this list.
        /// </summary>
        /// <value>
        /// The number of items.
        /// </value>
        public int Count => this.value.Length;

        /// <summary>
        /// Gets the element at the specified index in the read-only list.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>
        /// The element at the specified index in the read-only list.
        /// </returns>
        public Type this[int index] => this.value[index];

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(Signature? other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.value.Length != other.value.Length)
            {
                return false;
            }

            return !this.value.Where((t, i) => !(t?.Equals(other.value[i]) ?? other.value[i] == null)).Any();
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is Signature signature)
            {
                return this.Equals(signature);
            }

            return false;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode() =>
            this.value.Select((t, i) => (i + 1) * (t?.GetType().GetHashCode() ?? 0)).Sum();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            string GetTypeName(Type? t) =>
                t == null ? string.Empty : t.IsNullableType() ? $"{t.GetNonNullableType().Name}?" : t.Name;

            return $"({string.Join(", ", this.value.Select(GetTypeName))})";
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Type> GetEnumerator() => ((IList<Type>)this.value).GetEnumerator();

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.value.GetEnumerator();
    }
}
