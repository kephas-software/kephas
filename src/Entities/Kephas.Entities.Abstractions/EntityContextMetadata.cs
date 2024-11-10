// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityContextMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.Entities;

/// <summary>
/// Metadata for <see cref="IEntityContext"/> services.
/// </summary>
public record EntityContextMetadata : ServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityContextMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public EntityContextMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
        if (metadata == null)
        {
            return;
        }

        EntityContextType = (Type?)TryGetValue(metadata, nameof(EntityContextType));
        EntityTypes = (IEnumerable<Type>?)TryGetValue(metadata, nameof(EntityTypes));
    }

    /// <summary>
    /// Gets or sets the type of the entity context.
    /// </summary>
    public Type? EntityContextType { get; set; }

    /// <summary>
    /// Gets or sets the entity types served by the data context.
    /// </summary>
    public IEnumerable<Type>? EntityTypes { get; set; }
}
