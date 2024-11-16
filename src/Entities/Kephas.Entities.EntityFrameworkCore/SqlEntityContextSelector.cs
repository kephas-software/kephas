// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlEntityContextSelector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using Kephas.DependencyInjection;

namespace Kephas.Entities;

/// <summary>
/// The default implementation of the <see cref="ISqlEntityContextSelector"/>.
/// </summary>
/// <param name="lazyEntityContexts"></param>
public class SqlEntityContextSelector(
    ILazyEnumerable<IEntityContext, EntityContextMetadata> lazyEntityContexts)
    : ISqlEntityContextSelector
{
    /// <summary>
    /// Gets the <see cref="IEntityContext"/> serving <paramref name="entityType"/>.
    /// </summary>
    /// <param name="entityType">The served entity type.</param>
    /// <returns>The <see cref="IEntityContext"/>.</returns>
    public IEntityContext GetEntityContext(Type entityType) =>
        lazyEntityContexts.GetService(m => m.EntityTypes?.Contains(entityType) is true)
            ?? throw new ServiceResolutionException($"No entity context found for entity type '{entityType}'.");

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<IEntityContext> GetEnumerator() =>
        lazyEntityContexts.SelectServices().GetEnumerator();

    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}