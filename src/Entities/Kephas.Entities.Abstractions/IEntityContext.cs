// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.Entities.Features;

namespace Kephas.Entities;

/// <summary>
/// Service for entity contexts.
/// </summary>
/// <remarks>
/// Typically, in an EntityFrameworkCode environment, entity contexts will specialize the DbContext class.
/// </remarks>
public interface IEntityContext
{
    /// <summary>
    /// Gets the underlying provider name.
    /// </summary>
    public string ProviderName { get; }

    /// <summary>
    /// Gets the supported features.
    /// </summary>
    /// <typeparam name="T">The feature type.</typeparam>
    /// <returns>An enumeration of <see cref="IEntityContextFeature"/>.</returns>
    IEnumerable<T> GetFeatures<T>() where T : IEntityContextFeature;

    /// <summary>
    /// Gets asynchronously a value indicating whether a connection to the underlying database is available.
    /// </summary>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A task yielding a value indicating whether a connection to the underlying database is available.</returns>
    Task<ConnectResult> CanConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a value indicating whether there are entities being changed.
    /// </summary>
    /// <returns><c>true</c> if there are entities being changed, otherwise <c>false</c>.</returns>
    bool HasChanges();
}

/// <summary>
/// Generic service contract for entity contexts.
/// </summary>
/// <remarks>
/// Typically, in an EntityFrameworkCode environment, entity contexts will specialize the DbContext class.
/// </remarks>
/// <typeparam name="TEntityContext">The entity context type.</typeparam>
[ScopedService(ServiceType = typeof(IEntityContext), MetadataType = typeof(EntityContextMetadata))]
public interface IEntityContext<TEntityContext> : IEntityContext
    where TEntityContext : IEntityContext;