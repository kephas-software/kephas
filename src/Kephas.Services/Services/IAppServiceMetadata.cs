// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

/// <summary>
/// Abstraction over the application service metadata.
/// </summary>
public interface IAppServiceMetadata : IHasProcessingPriority, IHasOverridePriority
{
    /// <summary>
    /// Gets a value indicating whether the service overrides the
    /// service it specializes.
    /// </summary>
    /// <value>
    /// True if the service overrides the service it specializes, false otherwise.
    /// </value>
    public bool IsOverride { get; }

    /// <summary>
    /// Gets or sets the concrete service type implementing the service contract.
    /// </summary>
    /// <value>
    /// The type of the service.
    /// </value>
    public Type? ServiceType { get; set; }
}