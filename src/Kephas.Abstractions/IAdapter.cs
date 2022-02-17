// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas;

/// <summary>
/// Provides the <see cref="Of"/>property returning the adapted object.
/// </summary>
public interface IAdapter
{
    /// <summary>
    /// Gets the object the current instance adapts.
    /// </summary>
    /// <value>
    /// The object the current instance adapts.
    /// </value>
    public object Of { get; }
}

/// <summary>
/// Provides the <see cref="Of"/>property returning the adapted object.
/// </summary>
/// <typeparam name="T">The type of the adapted object.</typeparam>
public interface IAdapter<T> : IAdapter
    where T : notnull
{
    /// <summary>
    /// Gets the object the current instance adapts.
    /// </summary>
    /// <value>
    /// The object the current instance adapts.
    /// </value>
    public new T Of { get; }

    /// <summary>
    /// Gets the object the current instance adapts.
    /// </summary>
    /// <value>
    /// The object the current instance adapts.
    /// </value>
    object IAdapter.Of => this.Of;
}
