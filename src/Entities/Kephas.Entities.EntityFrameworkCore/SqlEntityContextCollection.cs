    // --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlEntityContextCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using Kephas.DependencyInjection;
using Kephas.Entities.Configuration;

namespace Kephas.Entities;

/// <summary>
/// The default implementation of the <see cref="ISqlEntityContextCollection"/>.
/// </summary>
public class SqlEntityContextCollection(
    ILazyEnumerable<IEntityContext, EntityContextMetadata> lazyEntityContexts,
    IEntityContextSettingsProvider settingsProvider)
    : ISqlEntityContextCollection
{
    /// <summary>
    /// Gets the <see cref="IEntityContext"/> serving <paramref name="entityType"/>.
    /// </summary>
    /// <param name="entityType">The served entity type.</param>
    /// <returns>The <see cref="IEntityContext"/>.</returns>
    public IEntityContext GetEntityContext(Type entityType) =>
        lazyEntityContexts.GetService(m => m.EntityTypes?.Contains(entityType) is true)
            ?? throw new ServiceResolutionException($"No entity context found for entity type '{entityType}'.");

    /// <summary>
    /// Gets the connection name for the provided <see cref="IEntityContext"/>.
    /// </summary>
    /// <param name="entityContext">The entity context.</param>
    /// <returns>The connection name for <paramref name="entityContext"/>.</returns>
    public string GetConnectionName(IEntityContext entityContext)
    {
        var entityContextType = entityContext.GetType();
        var entityContextName = lazyEntityContexts
            .FirstOrDefault(l => l.Metadata.ImplementationType == entityContextType)
            ?.Metadata.ServiceKey;
        if (entityContextName is null)
        {
            throw new ArgumentException($"No entity context registration found for the provided {entityContextType}.");
        }

        return settingsProvider.GetEntityContextSettings(entityContextName).Connection?.Name
            ?? throw new ArgumentException($"No connection name for the provided {entityContextType}.");
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<IEntityContext> GetEnumerator() =>
        lazyEntityContexts.SelectServices().GetEnumerator();

    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}