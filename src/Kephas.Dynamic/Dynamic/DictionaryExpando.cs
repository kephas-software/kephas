// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryExpando.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic;

/// <summary>
/// Expando class based on a dictionary.
/// </summary>
/// <typeparam name="T">The dictionary item type.</typeparam>
internal class DictionaryExpando<T> : ExpandoBase<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryExpando{T}"/> class.
    /// </summary>
    /// <param name="dictionary">The dictionary.</param>
    internal DictionaryExpando(IDictionary<string, T> dictionary)
        : base(dictionary ?? throw new ArgumentNullException(nameof(dictionary)))
    {
    }

    /// <summary>
    /// Gets the dynamic member names of this instance.
    /// </summary>
    /// <returns>An enumeration of member names.</returns>
    public override IEnumerable<string> GetDynamicMemberNames() => this.InnerDictionary.Keys;

    /// <summary>
    /// Indicates whether the <paramref name="memberName"/> is defined in the expando.
    /// </summary>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// True if defined, false if not.
    /// </returns>
    public override bool HasDynamicMember(string memberName) => this.InnerDictionary.ContainsKey(memberName);
}