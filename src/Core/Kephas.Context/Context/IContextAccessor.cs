// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContextAccessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Context;

/// <summary>
/// Service for getting contextual values.
/// </summary>
[SingletonService]
public interface IContextAccessor
{
    /// <summary>
    /// Gets the value with the provided type. If a value with the provided type was not registered, an exception is thrown.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>The required value.</returns>
    T GetValue<T>() where T : notnull;

    /// <summary>
    /// Trier to get the value with the provided type. If a value with the provided type was not registered, a <c>null</c> value is returned.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>The required value.</returns>
    T? TryGetValue<T>() where T : notnull;

    /// <summary>
    /// Gets the context writer for setting values in the context. 
    /// </summary>
    /// <returns>The context writer.</returns>
    IContextWriter GetWriter();
}