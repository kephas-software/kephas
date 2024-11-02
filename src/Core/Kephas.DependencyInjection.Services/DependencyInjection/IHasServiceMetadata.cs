// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHasServiceMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection;

/// <summary>
/// Interface providing metadata for a service.
/// </summary>
public interface IHasServiceMetadata
{
    /// <summary>
    /// Gets the service metadata.
    /// </summary>
    /// <returns>The service metadata.</returns>
    object? GetMetadata();
}