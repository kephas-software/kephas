// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHasServiceKey.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection.Metadata;

/// <summary>
/// Interface providing the <see cref="ServiceKey"/>.
/// </summary>
public interface IHasServiceKey
{
    /// <summary>
    /// Gets or sets the key of the instance implementing the <see cref="IHasServiceType.ServiceType"/>.
    /// </summary>
    /// <remarks>
    /// Typically, this is the name of the service implementation type from which the service type name has been stripped away.
    /// </remarks>
    public string? ServiceKey { get; internal set; }
}