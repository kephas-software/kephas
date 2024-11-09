// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;

namespace Kephas.Entities.Configuration;

/// <summary>
/// Provides connection settings.
/// </summary>
public record ConnectionSettings
{
    /// <summary>
    /// Gets the connection name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the provider type.
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the command timeout.
    /// </summary>
    public TimeSpan? CommandTimeout { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether how many retries are performed on failure.
    /// </summary>
    public int? MaxRetryCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the maximum retry delay on failure.
    /// </summary>
    public TimeSpan? MaxRetryDelay { get; set; }

    /// <summary>
    /// Gets or sets the query tracking behavior.
    /// </summary>
    public QueryTrackingBehavior? QueryTrackingBehavior { get; set; }
}
