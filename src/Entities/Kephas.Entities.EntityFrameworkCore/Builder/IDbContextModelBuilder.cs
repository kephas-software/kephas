// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDbContextModelBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Entities.Configuration;
using Kephas.Entities.Features;
using Microsoft.EntityFrameworkCore;

namespace Kephas.Entities.Builder;

/// <summary>
/// Feature for building the model.
/// </summary>
public interface IDbContextModelBuilder : IEntityContextFeature
{
    /// <summary>
    /// Builds the model using the provided entity types.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/>.</param>
    /// <param name="entityTypes">The model entity types.</param>
    /// <param name="entityContextSettings">The <see cref="EntityContextSettings"/>.</param>
    void Build(ModelBuilder modelBuilder, IEnumerable<Type> entityTypes, EntityContextSettings entityContextSettings);
}