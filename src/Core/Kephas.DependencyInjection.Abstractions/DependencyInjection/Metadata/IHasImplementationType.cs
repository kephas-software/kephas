// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHasImplementationType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection.Metadata;

/// <summary>
/// Interface providing the service implementation type.
/// </summary>
public interface IHasImplementationType
{
    /// <summary>
    /// Gets a value indicating whether the service overrides the
    /// service implementation that it specializes.
    /// </summary>
    /// <value>
    /// True if the service overrides the service implemenation that it specializes, false otherwise.
    /// </value>
    public bool OverrideBaseImplementation { get; }
    
    /// <summary>
    /// Gets or sets the type implementing the service type.
    /// </summary>
    public Type? ImplementationType { get; set; }
}