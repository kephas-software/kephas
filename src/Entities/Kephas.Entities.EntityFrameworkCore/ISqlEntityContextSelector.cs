// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlEntityContextSelector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Entities;

/// <summary>
/// Service selecting an <see cref="IEntityContext"/> based on the entities it serves.
/// </summary>
[ScopedService]
public interface ISqlEntityContextSelector : IEnumerable<IEntityContext>
{
    /// <summary>
    /// Gets the <see cref="IEntityContext"/> serving <paramref name="entityType"/>.
    /// </summary>
    /// <param name="entityType">The served entity type.</param>
    /// <returns>The <see cref="IEntityContext"/>.</returns>
    IEntityContext GetEntityContext(Type entityType);
}