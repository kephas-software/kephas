// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityContextFeatureMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.Entities.Features;

/// <summary>
/// Metadata for <see cref="IEntityContextFeature"/>.
/// </summary>
public record EntityContextFeatureMetadata : ServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityContextFeatureMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public EntityContextFeatureMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
        if (metadata == null)
        {
            return;
        }

        Provider = (string?)TryGetValue(metadata, nameof(Provider));
        EntityContextType = (Type?)TryGetValue(metadata, nameof(EntityContextType));
        EntityTypes = (IEnumerable<Type>?)TryGetValue(metadata, nameof(EntityTypes));
    }

    /// <summary>
    /// Gets the provider.
    /// </summary>
    public string? Provider { get; }

    /// <summary>
    /// Gets the target entity context type.
    /// </summary>
    public Type? EntityContextType { get; }

    /// <summary>
    /// Gets or sets the entity types affected by this feature.
    /// </summary>
    public IEnumerable<Type>? EntityTypes { get; set; }
}
