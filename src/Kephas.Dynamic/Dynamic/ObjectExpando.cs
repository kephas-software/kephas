// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExpando.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic;

/// <summary>
/// Expando class wrapping an object.
/// </summary>
internal class ObjectExpando : ExpandoBase<object?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectExpando"/> class.
    /// </summary>
    /// <param name="obj">The wrapped object.</param>
    internal ObjectExpando(object obj)
        : base(obj ?? throw new ArgumentNullException(nameof(obj)), null)
    {
    }
}