// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityContextSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;

namespace Kephas.Entities.Configuration;

/// <summary>
/// Settings for an <see cref="IEntityContext{TEntityContext}"/>.
/// </summary>
public record EntityContextSettings
{
    /// <summary>
    /// Gets or sets the name of the <see cref="IEntityContext"/>.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the connection name.
    /// </summary>
    public string? ConnectionName { get; set; }

    /// <summary>
    /// Gets or sets the default schema.
    /// </summary>
    public string? DefaultSchema { get; set; }

    /// <summary>
    /// Optional builder for the DbContext options.
    /// </summary>
    public Action<DbContextOptionsBuilder>? OptionsAction { get; set; }

    /// <summary>
    /// Gets or sets the connection settings.
    /// </summary>
    public ConnectionSettings? Connection { get; set; }
    
    /// <summary>
    /// Gets the provider, throwing an exception in case the provider could not be identified.
    /// </summary>
    /// <returns>The provider name.</returns>
    public string GetRequiredProvider() => Connection?.Provider ?? throw new InvalidOperationException("The connection provider is not set.");
}
