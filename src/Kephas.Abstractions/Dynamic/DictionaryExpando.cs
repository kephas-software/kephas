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
}