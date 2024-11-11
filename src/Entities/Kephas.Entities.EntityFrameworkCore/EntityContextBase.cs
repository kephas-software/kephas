// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityContextBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Data.Common;
using Kephas.Entities.Builder;
using Kephas.Entities.Features;
using Kephas.ExceptionHandling;
using Microsoft.EntityFrameworkCore;

namespace Kephas.Entities;

/// <summary>
/// Base class for <see cref="DbContext"/>s
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public abstract class EntityContextBase<TDbContext> : DbContext, IEntityContext<TDbContext>
    where TDbContext : DbContext, IEntityContext
{
    private readonly IEntityContextBuilder builder;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityContextBase{TDbContext}"/> class.
    /// </summary>
    /// <param name="options">The context options.</param>
    /// <param name="builder">The entity context builder.</param>
    protected EntityContextBase(
        DbContextOptions<TDbContext> options,
        IEntityContextBuilder builder)
        : base(options)
    {
        this.builder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    /// <summary>
    /// Gets the underlying provider name.
    /// </summary>
    public string ProviderName => Database.ProviderName ?? string.Empty;

    /// <summary>
    /// Gets the supported features.
    /// </summary>
    /// <typeparam name="T">The feature type.</typeparam>
    /// <returns>An enumeration of <see cref="IEntityContextFeature"/>.</returns>
    public IEnumerable<T> GetFeatures<T>() where T : IEntityContextFeature
    {
        return builder?.GetFeatures(this).OfType<T>() ?? Array.Empty<T>();
    }

    /// <summary>
    /// Gets asynchronously a value indicating whether a connection to the underlying database is available.
    /// </summary>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A task yielding a value indicating whether a connection to the underlying database is available.</returns>
    public async Task<ConnectResult> CanConnectAsync(CancellationToken cancellationToken = default)
    {
        var connectionString = Database.GetConnectionString();
        var builder = new DbConnectionStringBuilder { ConnectionString = connectionString ?? string.Empty };
        var serverName = GetKeyValue(builder, "server", "<unknown>");
        var databaseName = GetKeyValue(builder, "database", "<unknown>");

        try
        {
            return new ConnectResult(await Database.CanConnectAsync(cancellationToken), serverName, databaseName);
        }
        catch (Exception ex)
        {
            return new ConnectResult(false, serverName, databaseName, new ExceptionData(ex));
        }
    }

    /// <summary>
    /// Gets a value indicating whether there are entities being changed.
    /// </summary>
    /// <returns><c>true</c> if there are entities being changed, otherwise <c>false</c>.</returns>
    public bool HasChanges() => ChangeTracker.HasChanges();

    /// <summary>
    ///     Override this method to configure the database (and other options) to be used for this context.
    ///     This method is called for each instance of the context that is created.
    ///     The base implementation does nothing.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         In situations where an instance of <see cref="T:Microsoft.EntityFrameworkCore.DbContextOptions" /> may or may not have been passed
    ///         to the constructor, you can use <see cref="P:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.IsConfigured" /> to determine if
    ///         the options have already been set, and skip some or all of the logic in
    ///         <see cref="M:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)" />.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-dbcontext">DbContext lifetime, configuration, and initialization</see>
    ///         for more information and examples.
    ///     </para>
    /// </remarks>
    /// <param name="optionsBuilder">
    ///     A builder used to create or modify options for this context. Databases (and other extensions)
    ///     typically define extension methods on this object that allow you to configure the context.
    /// </param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => 
        builder?.OnConfiguring(this, optionsBuilder);

    /// <summary>
    ///     Override this method to further configure the model that was discovered by convention from the entity types
    ///     exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
    ///     and re-used for subsequent instances of your derived context.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
    ///         then this method will not be run. However, it will still run when creating a compiled model.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see> for more information and
    ///         examples.
    ///     </para>
    /// </remarks>
    /// <param name="modelBuilder">
    ///     The builder being used to construct the model for this context. Databases (and other extensions) typically
    ///     define extension methods on this object that allow you to configure aspects of the model that are specific
    ///     to a given database.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder) => 
        builder?.OnModelCreating(this, modelBuilder);

    /// <summary>
    ///     Override this method to set defaults and configure conventions before they run. This method is invoked before
    ///     <see cref="M:Microsoft.EntityFrameworkCore.DbContext.OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder)" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
    ///         then this method will not be run. However, it will still run when creating a compiled model.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-pre-convention">Pre-convention model building in EF Core</see> for more information and
    ///         examples.
    ///     </para>
    /// </remarks>
    /// <param name="configurationBuilder">
    ///     The builder being used to set defaults and configure conventions that will be used to build the model for this context.
    /// </param>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) => 
        builder?.OnConfigureConventions(this, configurationBuilder);

    private static string GetKeyValue(DbConnectionStringBuilder builder, string key, string defaultValue) =>
        builder.TryGetValue(key, out var value) ? value?.ToString() ?? defaultValue : defaultValue;
}
