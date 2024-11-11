// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqliteEntityContextOptionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Concurrent;
using Kephas.Entities.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Kephas.Entities.Builder;

/// <summary>
/// The SQLite flavor of the <see cref="IEntityContextOptionsBuilder"/>.
/// </summary>
[SqliteProviderName]
public class SqliteEntityContextOptionsBuilder : IEntityContextOptionsBuilder
{
    private static readonly ConcurrentDictionary<string, SqliteConnection> InMemoryConnections =
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

        var connectionString = entityContextSettings.Connection?.ConnectionString ?? "Data Source=:memory:";
        if (IsInMemoryConnection(connectionString))
        {
            var inMemoryConnection = InMemoryConnections.GetOrAdd(connectionString, CreateInMemoryConnection);

            optionsBuilder.UseSqlite(
                inMemoryConnection,
                sqlOptions => ConfigureSqliteOptions(sqlOptions, entityContextSettings));
        }
        else
        {
            optionsBuilder.UseSqlite(
                connectionString,
                sqlOptions => ConfigureSqliteOptions(sqlOptions, entityContextSettings));
        }

        optionsBuilder.ConfigureWarnings(w => w.Ignore(
            SqliteEventId.SequenceConfiguredWarning,
            SqliteEventId.SchemaConfiguredWarning,
            SqliteEventId.SchemasNotSupportedWarning));
    }

    private static bool IsInMemoryConnection(string connectionString)
    {
        return connectionString.Contains(":memory:", StringComparison.InvariantCultureIgnoreCase) || connectionString.Contains("Mode=Memory", StringComparison.InvariantCultureIgnoreCase);
    }

    private static void ConfigureSqliteOptions(SqliteDbContextOptionsBuilder sqlOptions, EntityContextSettings entityContextSettings)
    {
        var commandTimeout = entityContextSettings.Connection?.CommandTimeout;
        if (commandTimeout.HasValue)
        {
            sqlOptions.CommandTimeout((int) commandTimeout.Value.TotalSeconds);
        }

        if (entityContextSettings is SqliteEntityContextSettings { SqliteOptionsAction: not null } sqliteSettings)
        {
            sqliteSettings.SqliteOptionsAction(sqlOptions);
        }
    }

    private static SqliteConnection CreateInMemoryConnection(string connectionString)
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }
}