// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityFrameworkCoreServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Reflection;
using Kephas.Entities.Configuration;
using Kephas.Extensions;
using Kephas.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Entities;

/// <summary>
/// <see cref="IServiceCollection"/> extensions in the Entity Framework Core area.
/// </summary>
public static class EntityFrameworkCoreServiceCollectionExtensions
{
    /// <summary>
    /// The name of the "Entities" section.
    /// </summary>
    public static readonly string EntitiesSection = "Entities";

    private static readonly MethodInfo AddEntityContextMethod =
        ReflectionHelper.GetGenericMethodOf(_ => AddEntityContext<DbContext>(null!));

    /// <summary>
    /// Add the repository services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The <paramref name="services"/>.</returns>
    public static IServiceCollection AddEntityServices(this IServiceCollection services, IConfiguration configuration)
        => AddEntityServices(services, options => configuration.GetSection(EntitiesSection).Bind(options));

    /// <summary>
    /// Adds the entity related services, including the DbContexts.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="config">The settings for the entities area.</param>
    /// <returns>The <paramref name="services"/>.</returns>
    public static IServiceCollection AddEntityServices(this IServiceCollection services, Action<EntitiesSettings> config)
    {
        services.Configure<EntitiesSettings>(settings => config?.Invoke(settings));
        
        var entitiesSettings = new EntitiesSettings();
        config?.Invoke(entitiesSettings);

        var serviceInfos = services.GetRequiredSingletonService<IServiceInfoCollection>();
        var entityContextInfos = serviceInfos
            .Where(IsEntityContext)
            .Select(s => ServiceHelper.GetServiceMetadata<EntityContextMetadata>(s.Metadata))
            .OrderByDescending(m => m.Priority)
            .ToList();

        foreach (var entityContextInfo in entityContextInfos)
        {
            var entityContextType = entityContextInfo.EntityContextType ?? entityContextInfo.ImplementationType!;
            var addEntityContext = AddEntityContextMethod.MakeGenericMethod(entityContextType);
            addEntityContext.Call(null, services);
        }

        return services;
    }

    /// <summary>
    /// Add the data context services.
    /// </summary>
    /// <typeparam name="TDbContext">The DB Context type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The <paramref name="services"/>.</returns>
    public static IServiceCollection AddEntityContext<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddDbContext<TDbContext>();
        services.AddScoped<DbContext, TDbContext>();

        return services;
    }

    private static bool IsEntityContext(IServiceInfo serviceInfo) =>
        serviceInfo.ServiceType == typeof(IEntityContext) &&
        typeof(DbContext).IsAssignableFrom(serviceInfo.ImplementationType);
}