// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIdentifiable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Entities;

/// <summary>
/// Provides the <see cref="Id"/> property as an object.
/// Used for generic access to the ID of an entity.
/// </summary>
public interface IIdentifiable
{
    /// <summary>
    /// Gets or sets the untyped ID of this object.
    /// </summary>
    object Id { get; set; }
}

/// <summary>
/// Provides the typed <see cref="Id"/> property.
/// </summary>
public interface IIdentifiable<T> : IIdentifiable
    where T : notnull
{
    /// <summary>
    /// Gets or sets the untyped ID of this object.
    /// </summary>
    object IIdentifiable.Id
    {
        get => Id;
        set => Id = (T) value;
    }

    /// <summary>
    /// Gets or sets the typed ID of this object.
    /// </summary>
    new T Id { get; set; }
}