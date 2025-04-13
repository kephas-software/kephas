// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContextWriter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Kephas.Context;

/// <summary>
/// Service for writing contextual values.
/// </summary>
public interface IContextWriter
{
    /// <summary>
    /// Sets the value of the provided type.
    /// </summary>
    /// <param name="value">The value to be set.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>This writer.</returns>
    IContextWriter SetValue<T>([DisallowNull] T value) where T : notnull;

    /// <summary>
    /// Removes the value of the provided type.
    /// </summary>
    /// <typeparam name="T">The type of the value to be removed.</typeparam>
    /// <returns><c>true</c> is a value was set, otherwise <c>false</c>.</returns>
    bool RemoveValue<T>() where T : notnull;
    
    /// <summary>
    /// Restores the context from the provided values.
    /// </summary>
    /// <returns>This writer.</returns>
    IContextWriter RestoreContext(IEnumerable<KeyValuePair<Type, object>> context);
}