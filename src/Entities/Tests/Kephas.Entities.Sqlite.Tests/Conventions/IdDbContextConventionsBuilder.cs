// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdDbContextConvention.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Entities.Builder;
using Kephas.Entities.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Kephas.Entities.Tests.Conventions;

public class IdDbContextConventionsBuilder : IDbContextConventionsBuilder
{
    public void Build(ModelConfigurationBuilder configurationBuilder, EntityContextSettings entityContextSettings)
    {
        configurationBuilder.Conventions.Add(sp => new IdConvention());
    }
    
    private class IdConvention : IModelFinalizingConvention
    {
        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
        {
            foreach (var entityBuilder in modelBuilder.Metadata.GetEntityTypes()
                         .Where(b => typeof(IIdentifiable<Guid>).IsAssignableFrom(b.ClrType))
                         .Select(b => b.Builder))
            {
                if (entityBuilder.CanSetPrimaryKey([nameof(IIdentifiable.Id)]))
                {
                    entityBuilder.PrimaryKey([nameof(IIdentifiable.Id)]);
                }
            }
        }
    }
}