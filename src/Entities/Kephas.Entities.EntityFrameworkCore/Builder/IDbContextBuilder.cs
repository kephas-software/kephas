// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDbContextBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.Entities.Features;
using Microsoft.EntityFrameworkCore;

namespace Kephas.Entities.Builder;

/// <summary>
/// Singleton service for initialization of <see cref="IEntityContext"/>s.
/// </summary>
[SingletonService]
public interface IDbContextBuilder
{
    /// <summary>
    /// Gets the features supported by the provided <see cref="DbContext"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    /// <returns>An enumeration of supported features.</returns>
    IEnumerable<IEntityContextFeature> GetFeatures(DbContext dbContext);

    /// <summary>
    ///     Use this method to configure the database (and other options) to be used for this context.
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
    /// <param name="dbContext">The DbContext.</param>
    /// <param name="optionsBuilder">
    ///     A builder used to create or modify options for this context. Databases (and other extensions)
    ///     typically define extension methods on this object that allow you to configure the context.
    /// </param>
    void OnConfiguring(DbContext dbContext, DbContextOptionsBuilder optionsBuilder)
    {
        // override only if needed.
    }

    /// <summary>
    ///     Use this method to further configure the model that was discovered by convention from the entity types
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
    /// <param name="dbContext">The DbContext.</param>
    /// <param name="modelBuilder">
    ///     The builder being used to construct the model for this context. Databases (and other extensions) typically
    ///     define extension methods on this object that allow you to configure aspects of the model that are specific
    ///     to a given database.
    /// </param>
    void OnModelCreating(DbContext dbContext, ModelBuilder modelBuilder)
    {
        // override only if needed.
    }

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
    /// <param name="dbContext">The DbContext.</param>
    /// <param name="configurationBuilder">
    ///     The builder being used to set defaults and configure conventions that will be used to build the model for this context.
    /// </param>
    void OnConfigureConventions(DbContext dbContext, ModelConfigurationBuilder configurationBuilder)
    {
        // override only if needed.
    }
}