// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntitiesSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;

namespace Kephas.Entities.Configuration;

/// <summary>
/// 
/// </summary>
public record EntitiesSettings
{
    /// <summary>
    /// Gets or sets the options for repositories.
    /// </summary>
    public ICollection<EntityContextSettings>? EntityContexts { get; set; }

    /// <summary>
    /// Gets or sets the options for connections.
    /// </summary>
    public ICollection<ConnectionSettings>? Connections { get; set; }

    /// <summary>
    /// Gets or sets the default connection name.
    /// </summary>
    public string? DefaultConnectionName { get; set; }

    /// <summary>
    /// Gets or sets the command timeout.
    /// </summary>
    public TimeSpan? DefaultCommandTimeout { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether how many retries are performed on failure.
    /// </summary>
    public int? DefaultMaxRetryCount { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating the maximum retry delay on failure.
    /// </summary>
    public TimeSpan? DefaultMaxRetryDelay { get; set; }

    /// <summary>
    /// Gets or sets the query tracking behavior.
    /// </summary>
    public QueryTrackingBehavior? DefaultQueryTrackingBehavior { get; set; }
}