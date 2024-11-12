// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDbContextConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Entities.Configuration;
using Kephas.Entities.Features;
using Microsoft.EntityFrameworkCore;

namespace Kephas.Entities.Builder;

/// <summary>
/// Service for building <see cref="DbContext"/> conventions.
/// </summary>
public interface IDbContextConventionsBuilder : IEntityContextFeature
{
    /// <summary>
    /// Build the DbContext using the repository options.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/>.</param>
    /// <param name="entityContextSettings">The <see cref="EntityContextSettings"/>.</param>
    void Build(ModelConfigurationBuilder configurationBuilder, EntityContextSettings entityContextSettings);
}