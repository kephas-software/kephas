// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqliteIntegrationTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;
using Kephas.Entities.Configuration;
using Kephas.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Entities.Tests;

public abstract class SqliteIntegrationTestBase<TTestClass> : IntegrationTestBase<TTestClass>
    where TTestClass : class
{

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationTestBase{TTestClass}"/> class.
    /// Adds the following services:
    /// <list type="bullet">
    ///     <item>
    ///         <description>Dependency injection with the provided <paramref name="configuration"/>, or <see cref="Test.GetIntegrationConfig"/> if none provided, and using the <typeparamref name="TTestClass"/>'s assembly as an entry assembly.</description>
    ///     </item>
    ///     <item>
    ///         <description>Serilog integration logging.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="includeTestAssembly">If <c>true</c>, includes in the DI container also services from the test assembly, otherwise not.</param>
    /// <param name="configuration">The configuration instance. If not provided, the default integration configuration will be used.</param>
    protected SqliteIntegrationTestBase(bool includeTestAssembly = false, IConfiguration? configuration = null)
        : base(includeTestAssembly, GetSqliteConfiguration(configuration))
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        var lazyEntityContexts = DefaultScope.ServiceProvider
            .GetRequiredService<ILazyEnumerable<IEntityContext, EntityContextMetadata>>();
        foreach (var lazyEntityContext in lazyEntityContexts.Where(l => l.Metadata.Priority == Priority.Low))
        {
            if(lazyEntityContext.Value is DbContext dbContext)
            {
                await dbContext.Database.EnsureCreatedAsync();
            }
        }
    }

    private static IConfiguration? GetSqliteConfiguration(IConfiguration? configuration)
    {
        if (configuration is not null)
        {
            return configuration;
        }

        return Test.GetIntegrationConfig(initialData: new Dictionary<string, string?>
        {
            { $"Entities:{nameof(EntitiesSettings.DefaultConnectionName)}", "Test" },
            { "Entities:Connections:0:Name", "Test" },
            { "Entities:Connections:0:ProviderName", "SQLite" },
            { "Entities:Connections:0:ConnectionString", $"Data Source={typeof(TTestClass).Name}_{Guid.NewGuid():N};Mode=Memory" },
        });
    }
}