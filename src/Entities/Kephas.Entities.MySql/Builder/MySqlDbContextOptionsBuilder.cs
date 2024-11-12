// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MySqlDbContextOptionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Entities.Configuration;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Infrastructure;

namespace Kephas.Entities.Builder;

/// <summary>
/// The SQLite flavor of the <see cref="IDbContextOptionsBuilder"/>.
/// </summary>
[MySqlProviderName]
public class MySqlDbContextOptionsBuilder : IDbContextOptionsBuilder
{
    /// <summary>
    /// Build the DbContext using the provided settings.
    /// </summary>
    /// <param name="optionsBuilder">The builder for the <see cref="DbContextOptions{TContext}"/>.</param>
    /// <param name="entityContextSettings">The entity context settings.</param>
    public void Build(DbContextOptionsBuilder optionsBuilder, EntityContextSettings entityContextSettings)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);
        ArgumentNullException.ThrowIfNull(entityContextSettings);

        var connectionString = entityContextSettings.Connection?.ConnectionString;
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"No connection string provided for '{entityContextSettings.Name}'");
        }

        optionsBuilder.UseMySQL(
            connectionString,
            sqlOptions => ConfigureMySqlOptions(sqlOptions, entityContextSettings));
    }

    private static void ConfigureMySqlOptions(MySQLDbContextOptionsBuilder sqlOptions, EntityContextSettings entityContextSettings)
    {
        var commandTimeout = entityContextSettings.Connection?.CommandTimeout;
        if (commandTimeout.HasValue)
        {
            sqlOptions.CommandTimeout((int) commandTimeout.Value.TotalSeconds);
        }

        if (entityContextSettings is MySqlEntityContextSettings { MySqlOptionsAction: not null } mySqlSettings)
        {
            mySqlSettings.MySqlOptionsAction(sqlOptions);
        }
    }
}