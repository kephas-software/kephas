// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDomain.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Ontology;

/// <summary>
/// Models a domain of the application.
/// </summary>
[SingletonService(MetadataType = typeof(DomainMetadata))]
public interface IDomain : IOntologyElement
{
    /// <summary>
    /// Gets the other domains that this domain is dependent upon.
    /// </summary>
    public IEnumerable<IDomain> Dependencies { get; }
    
    /// <summary>
    /// Gets the elements a domain consists of.
    /// </summary>
    public IEnumerable<IOntologyElement> Elements { get; }
}