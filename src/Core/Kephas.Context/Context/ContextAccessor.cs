// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextAccessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Kephas.Threading;

namespace Kephas.Context;

/// <summary>
/// The default implementation of the <see cref="IContextAccessor"/>.
/// </summary>
public class ContextAccessor : AsyncLocalAccessorBase<Dictionary<Type, object>>, IContextAccessor, IContextWriter
{
    private readonly object syncLock = new ();
    
    /// <summary>
    /// Gets the value with the provided type. If a value with the provided type was not registered, an exception is thrown.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>The required value.</returns>
    public T GetValue<T>() where T : notnull =>
        TryGetValue<T>()
        ?? throw new KeyNotFoundException($"No contextual value was set for type {typeof(T)}.");

    /// <summary>
    /// Trier to get the value with the provided type. If a value with the provided type was not registered, a <c>null</c> value is returned.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>The required value.</returns>
    public T? TryGetValue<T>() where T : notnull =>
        (T?)GetData().GetValueOrDefault(typeof(T), default(T?)!);

    /// <summary>
    /// Gets the context values.
    /// </summary>
    /// <returns>An enumeration of context values.</returns>
    public IEnumerable<KeyValuePair<Type, object>> GetContext() =>
        GetData().ToDictionary(kv => kv.Key, kv => kv.Value);

    /// <summary>
    /// Gets the context writer for setting values in the context. 
    /// </summary>
    /// <returns>The context writer.</returns>
    public IContextWriter GetWriter() => this;

    /// <summary>
    /// Sets the value of the provided type.
    /// </summary>
    /// <param name="value">The value to be set.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>This writer.</returns>
    IContextWriter IContextWriter.SetValue<T>([DisallowNull] T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        GetData()[typeof(T)] = value;
        return this;
    }

    /// <summary>
    /// Removes the value of the provided type.
    /// </summary>
    /// <typeparam name="T">The type of the value to be removed.</typeparam>
    /// <returns><c>true</c> is a value was set, otherwise <c>false</c>.</returns>
    bool IContextWriter.RemoveValue<T>() =>
        GetData().Remove(typeof(T));

    /// <summary>
    /// Restores the context from the provided values.
    /// </summary>
    /// <returns>This writer.</returns>
    IContextWriter IContextWriter.RestoreContext(IEnumerable<KeyValuePair<Type, object>> context)
    {
        if (context is null)
        {
            return this;
        }

        var data = GetData();
        foreach ((Type key, object value) in context)
        {
            data[key] = value;
        }

        return this;
    }

    private Dictionary<Type, object> GetData()
    {
        if (Data is null)
        {
            lock (syncLock)
            {
                Data ??= new Dictionary<Type, object>();
            }
        }

        return Data!;
    }
}