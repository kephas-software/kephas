// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDictionaryConvertible.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Collections;

/// <summary>
/// Provides the <see cref="ToDictionary"/> method used to convert this instance to a dictionary.
/// </summary>
public interface IDictionaryConvertible
{
    /// <summary>
    /// Converts this instance to a dictionary.
    /// </summary>
    /// <param name="keyGetter">Optional. The key transformation function.</param>
    /// <param name="valueGetter">Optional. The value transformation function.</param>
    /// <returns>
    /// A dictionary of values with their associated names.
    /// </returns>
    IDictionary<string, object?> ToDictionary(
        Func<string, string>? keyGetter = null,
        Func<object?, object?>? valueGetter = null);
}
