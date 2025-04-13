// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlEntityContextCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.Entities.Configuration;

namespace Kephas.Entities;

/// <summary>
/// Service providing information related to <see cref="IEntityContext"/>s.
/// </summary>
[ScopedService]
public interface ISqlEntityContextCollection : IEnumerable<IEntityContext>
{
    /// <summary>
    /// Gets the <see cref="IEntityContext"/> serving <paramref name="entityType"/>.
    /// </summary>
    /// <param name="entityType">The served entity type.</param>
    /// <returns>The <see cref="IEntityContext"/>.</returns>
    IEntityContext GetEntityContext(Type entityType);

    /// <summary>
    /// Gets the connection name for the provided <see cref="IEntityContext"/>.
    /// </summary>
    /// <param name="entityContext">The entity context.</param>
    /// <returns>The connection name for <paramref name="entityContext"/>.</returns>
    string GetConnectionName(IEntityContext entityContext);
}