// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqliteEntityContextSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MySql.EntityFrameworkCore.Infrastructure;

namespace Kephas.Entities.Configuration;

/// <summary>
/// Entity context settings for MySQL.
/// </summary>
public record MySqlEntityContextSettings : EntityContextSettings
{
    /// <summary>
    /// Optional builder for the DbContext options.
    /// </summary>
    public Action<MySQLDbContextOptionsBuilder>? MySqlOptionsAction { get; set; }
}