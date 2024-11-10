// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Signature.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the signature class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;

namespace Kephas.Reflection;

/// <summary>
/// A signature.
/// </summary>
public record Signature : IReadOnlyList<Type>
{
    private readonly Type[] value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Signature"/> class.
    /// </summary>
    /// <param name="value">A variable-length parameters list containing value.</param>
    public Signature(params Type[] value)
    {
        this.value = value ?? [];
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
    public int Count => value.Length;

    /// <summary>
    /// Gets the element at the specified index in the read-only list.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>
    /// The element at the specified index in the read-only list.
    /// </returns>
    public Type this[int index] => value[index];

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>
    /// A hash code for the current object.
    /// </returns>
    public override int GetHashCode() =>
        value.Select((t, i) => (i + 1) * (t?.GetHashCode() ?? 0)).Sum();

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

        return $"({string.Join(", ", value.Select(GetTypeName))})";
    }

    /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
    public virtual bool Equals(Signature? other)
    {
        return other switch
        {
            null => false,
            _ when ReferenceEquals(this, other) => true,
            _ => value.SequenceEqual(other.value),
        };
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<Type> GetEnumerator() => ((IList<Type>)value).GetEnumerator();

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>
    /// The enumerator.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => value.GetEnumerator();
}
