// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOntologyElement.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Ontology;

/// <summary>
/// Provides primary information about an ontology element.
/// </summary>
public interface IOntologyElement
{
    /// <summary>
    /// Gets the name of the ontology element.
    /// </summary>
    public string Name { get; }
}