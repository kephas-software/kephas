// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityContextBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Concurrent;
using Kephas.DependencyInjection;
using Kephas.Entities.Configuration;
using Kephas.Entities.Features;
using Microsoft.EntityFrameworkCore;

namespace Kephas.Entities.Builder;

/// <summary>
/// The default implementation of <see cref="IEntityContextBuilder"/>.
/// </summary>
public class EntityContextBuilder(
    IServiceProvider serviceProvider,
    ILazyEnumerable<IEntityContextFeature, EntityContextFeatureMetadata> lazyFeatures,
    IEntityContextSettingsProvider settingsProvider) : IEntityContextBuilder
{
    private readonly ConcurrentDictionary<Type, Action<ModelConfigurationBuilder>> dbConventionsConfigMap = new();
    private readonly ConcurrentDictionary<Type, Action<DbContextOptionsBuilder>> dbContextConfigMap = new();
    private readonly ConcurrentDictionary<Type, Action<ModelBuilder>> dbModelConfigMap = new();
    private readonly ConcurrentDictionary<Type, IEnumerable<IEntityContextFeature>> dbContextFeaturesMap = new();

    /// <summary>
    /// Gets the features supported by the provided <see cref="DbContext"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    /// <returns>An enumeration of supported features.</returns>
    public IEnumerable<IEntityContextFeature> GetFeatures(DbContext dbContext)
    {
        _ = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        var dbContextType = dbContext.GetType();
        var features = dbContextFeaturesMap.GetOrAdd(
            dbContextType,
            t =>
            {
                var metadata = GetEntityContextMetadata(t);
                var entityContextSettings = GetEntityContextSettings(metadata);
                var provider = entityContextSettings.GetRequiredProvider();
                return lazyFeatures.SelectServices(m =>
                    MatchProvider(m, provider) &&
                    MatchDataContextType(m, t) &&
                    MatchEntityTypes(m, metadata.EntityTypes)).ToList();

            });
        return features;
    }

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
    public void OnConfiguring(DbContext dbContext, DbContextOptionsBuilder optionsBuilder)
    {
        _ = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        var dbContextType = dbContext.GetType();
        var buildDbContextOptions = dbContextConfigMap.GetOrAdd(
            dbContextType,
            t =>
            {
                var metadata = GetEntityContextMetadata(t);
                var entityContextSettings = GetEntityContextSettings(metadata);
                var builders = GetFeatures(dbContext)
                    .OfType<IEntityContextOptionsBuilder>()
                    .ToList();
                return b =>
                {
                    foreach (var builder in builders)
                    {
                        builder.Build(b, entityContextSettings);
                    }
                };
            });

        buildDbContextOptions(optionsBuilder);
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
    public void OnConfigureConventions(DbContext dbContext, ModelConfigurationBuilder configurationBuilder)
    {
        _ = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        var dbContextType = dbContext.GetType();
        var buildDbContextOptions = dbConventionsConfigMap.GetOrAdd(
            dbContextType,
            t =>
            {
                var metadata = GetEntityContextMetadata(t);
                var entityContextSettings = GetEntityContextSettings(metadata);
                var builders = GetFeatures(dbContext)
                    .OfType<IEntityContextConventionsBuilder>()
                    .ToList();
                return b =>
                {
                    foreach (var builder in builders)
                    {
                        builder.Build(b, entityContextSettings);
                    }
                };
            });

        buildDbContextOptions(configurationBuilder);
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
    public void OnModelCreating(DbContext dbContext, ModelBuilder modelBuilder)
    {
        _ = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        var dbContextType = dbContext.GetType();
        var buildModel = dbModelConfigMap.GetOrAdd(
            dbContextType,
            t =>
            {
                var metadata = GetEntityContextMetadata(t);
                var entityContextSettings = GetEntityContextSettings(metadata);
                var builders = GetFeatures(dbContext)
                    .OfType<IEntityContextModelBuilder>()
                    .ToList();
                var entityTypes = metadata.EntityTypes?.ToList() ?? [];
                return b =>
                {
                    foreach (var builder in builders)
                    {
                        builder.Build(b, entityTypes, entityContextSettings);
                    }
                };
            });

        buildModel(modelBuilder);
    }

    /// <summary>
    /// Gets the <see cref="EntityContextMetadata"/> for the <paramref name="dbContextType"/>.
    /// </summary>
    /// <param name="dbContextType">The DbContext type.</param>
    /// <returns>The <see cref="EntityContextMetadata"/>.</returns>
    protected EntityContextMetadata GetEntityContextMetadata(Type dbContextType) =>
        ServiceHelper.GetServiceMetadata<EntityContextMetadata>(dbContextType, typeof(IEntityContext));

    /// <summary>
    /// Gets the entity context settings for the provided metadata.
    /// </summary>
    /// <param name="metadata">The data context metadata.</param>
    /// <returns>The entity context settings.</returns>
    protected EntityContextSettings GetEntityContextSettings(EntityContextMetadata metadata) =>
        settingsProvider.GetEntityContextSettings(metadata.ServiceKey!);

    private static bool MatchEntityTypes(EntityContextFeatureMetadata metadata, IEnumerable<Type>? entityTypes) =>
        metadata.EntityTypes is null ||
        !metadata.EntityTypes.Any() ||
        entityTypes is null ||
        !entityTypes.Any() ||
        entityTypes.Any(t => metadata.EntityTypes.Contains(t));

    private static bool MatchDataContextType(EntityContextFeatureMetadata metadata, Type entityContextType) =>
        metadata.EntityContextType is null || metadata.EntityContextType.IsAssignableFrom(entityContextType);

    private static bool MatchProvider(EntityContextFeatureMetadata metadata, string provider) =>
        metadata.Provider is null || string.Equals(metadata.Provider, provider, StringComparison.InvariantCultureIgnoreCase);
}
