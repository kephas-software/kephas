// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MySqlEntityContextOptionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Concurrent;
using Kephas.Entities.Configuration;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Infrastructure;
using MySql.Data.MySqlClient;

namespace Kephas.Entities.Builder;

/// <summary>
/// The SQLite flavor of the <see cref="IEntityContextOptionsBuilder"/>.
/// </summary>
[MySqlProviderName]
public class MySqlEntityContextOptionsBuilder : IEntityContextOptionsBuilder
{
    private static readonly ConcurrentDictionary<string, MySqlConnection> InMemoryConnections =
        new(StringComparer.InvariantCultureIgnoreCase);

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