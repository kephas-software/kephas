// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpArgsDataContextConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Entities.Builder;
using Kephas.Entities.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Kephas.Entities.Conventions;

/// <summary>
/// Conventions builder for SP args.
/// </summary>
public class SpArgsDataContextConventionsBuilder : IEntityContextConventionsBuilder
{
    /// <summary>
    /// Build the DbContext using the repository options.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="ModelConfigurationBuilder"/>.</param>
    /// <param name="entityContextSettings">The <see cref="EntityContextSettings"/>.</param>
    public void Build(ModelConfigurationBuilder configurationBuilder, EntityContextSettings entityContextSettings)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.Conventions.Add(sp => new SpArgsModelFinalizingConvention());
    }
}