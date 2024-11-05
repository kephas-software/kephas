// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAttributeProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAttributeProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime;

/// <summary>
/// Contract for reflective objects providing runtime attributes.
/// </summary>
public interface IAttributeProvider
{
    /// <summary>
    /// Gets the attribute of the provided type.
    /// </summary>
    /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
    /// <returns>
    /// The attribute of the provided type.
    /// </returns>
    IEnumerable<TAttribute> GetAttributes<TAttribute>()
        where TAttribute : class;

    /// <summary>
    /// Gets the single attribute of the provided type.
    /// </summary>
    /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
    /// <returns>
    /// The attribute of the provided type.
    /// </returns>
    public TAttribute? GetAttribute<TAttribute>()
        where TAttribute : class
    {
        return GetAttributes<TAttribute>().SingleOrDefault();
    }
}