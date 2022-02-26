// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisDbSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Redis.Configuration;

using System.ComponentModel.DataAnnotations;

using Kephas.Configuration;

/// <summary>
/// Settings for the Redis database.
/// </summary>
public class RedisDbSettings : ISettings
{
    /// <summary>
    /// Indicates the default database (1).
    /// </summary>
    public const int DefaultDatabase = 1;

    /// <summary>
    /// Gets or sets the connection URI. 
    /// </summary>
    public string? ConnectionUri { get; set; }

    /// <summary>
    /// Gets or sets the database number.
    /// </summary>
    [Display(Description = "Gets or sets the database number.")]
    public int Database { get; set; } = DefaultDatabase;
}