// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityContextSettingsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Entities.Configuration;
using Kephas.Entities.Features;
using Microsoft.EntityFrameworkCore;

namespace Kephas.Entities.Builder;

/// <summary>
/// Service for building <see cref="IEntityContext"/> services based on <see cref="DbContext"/>.
/// </summary>
public interface IDbContextOptionsBuilder : IEntityContextFeature
{
    /// <summary>
    /// Build the DbContext using the provided settings.
    /// </summary>
    /// <param name="optionsBuilder">The builder for the <see cref="DbContextOptions{TContext}"/>.</param>
    /// <param name="entityContextSettings">The entity context settings.</param>
    void Build(DbContextOptionsBuilder optionsBuilder, EntityContextSettings entityContextSettings);
}