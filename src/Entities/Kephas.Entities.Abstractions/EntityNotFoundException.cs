// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityNotFoundException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Data.Common;
using Kephas.ExceptionHandling;

namespace Kephas.Entities;

/// <summary>
/// Exception occurring when an entity is not found.
/// </summary>
public class EntityNotFoundException : DbException, IErrorNotification
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="entityId">The entity ID.</param>
    public EntityNotFoundException(Type entityType, object? entityId)
        : base($"Entity '{entityType?.Name}'{GetCondition(entityId)} not found.")
    {
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        EntityId = entityId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="entityId">The entity ID.</param>
    /// <param name="inner">The inner exception.</param>
    public EntityNotFoundException(Type entityType, object? entityId, Exception inner)
        : base($"Entity '{entityType?.Name}'{GetCondition(entityId)} not found.", inner)
    {
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        EntityId = entityId;
    }
    
    /// <summary>
    /// Gets the entity type.
    /// </summary>
    public Type EntityType { get; }
    
    /// <summary>
    /// Gets then entity ID.
    /// </summary>
    public object? EntityId { get; }

    private static string GetCondition(object? id)
    {
        return id is null ? string.Empty : $" with ID '{id}'";
    }
}
