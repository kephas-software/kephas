// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.Ontology;

/// <summary>
/// Provides the metadata for a domain.
/// </summary>
public record DomainMetadata : ServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public DomainMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
        if (metadata == null)
        {
            return;
        }

        DomainDependencies = (IEnumerable<Type>?)TryGetValue(metadata, nameof(DomainDependencies));
    }

    /// <summary>
    /// Gets the domain dependencies.
    /// </summary>
    public IEnumerable<Type>? DomainDependencies { get; }
}