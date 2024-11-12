// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityContextSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Options;

namespace Kephas.Entities.Configuration;

/// <summary>
/// The default implementation of <see cref="IEntityContextSettingsProvider"/>.
/// </summary>
public class EntityContextSettingsProvider(IOptions<EntitiesSettings> entitiesOptions)
    : IEntityContextSettingsProvider
{
    /// <summary>
    /// Gets the settings for the <see cref="IEntityContext"/> with the provided name.
    /// </summary>
    /// <param name="entityContextName">The name of the <see cref="IEntityContext"/>.</param>
    /// <returns>The <see cref="EntityContextSettings"/>.</returns>
    public EntityContextSettings GetEntityContextSettings(string entityContextName)
    {
        ArgumentNullException.ThrowIfNull(entityContextName);

        var entitiesSettings = entitiesOptions.Value;
        var entityContextSettings = (entitiesSettings.EntityContexts?.FirstOrDefault(
                                         c => entityContextName.Equals(c.Name, StringComparison.OrdinalIgnoreCase))
                                     ?? new()) with { Name = entityContextName };

        var connectionSettings = entityContextSettings.Connection;
        if (connectionSettings is null)
        {
            var connectionName = entityContextSettings.ConnectionName ?? entitiesSettings.DefaultConnectionName ?? entityContextName;
            connectionSettings = entitiesSettings.Connections?.FirstOrDefault(
                c => connectionName.Equals(c.Name, StringComparison.OrdinalIgnoreCase));
            if (connectionSettings is not null)
            {
                entityContextSettings.Connection = connectionSettings;
            }
        }

        if (connectionSettings is not null)
        {
            connectionSettings.CommandTimeout ??= entitiesSettings.DefaultCommandTimeout;
            connectionSettings.MaxRetryCount ??= entitiesSettings.DefaultMaxRetryCount;
            connectionSettings.MaxRetryDelay ??= entitiesSettings.DefaultMaxRetryDelay;
            connectionSettings.QueryTrackingBehavior ??= entitiesSettings.DefaultQueryTrackingBehavior;
        }
        
        return entityContextSettings;
    }
}